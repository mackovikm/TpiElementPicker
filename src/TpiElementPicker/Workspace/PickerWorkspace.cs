using TpiElementPicker.Models;
using TpiElementPicker.Services;
using TpiGto.Mapping;
using TpiGto.Matching;
using TpiGto.Model;
using TpiGto.Naming;
using TpiGto.Paths;
using TpiGto.Registry;
using TpiGto.Scripting;

namespace TpiElementPicker.Workspace;

/// <summary>
/// Sdílený stav aplikace. Všechna dokovatelná okna pracují nad touto instancí a
/// komunikují spolu jen přes její události – proto je lze libovolně odpojit, zavřít
/// nebo vytáhnout jako plovoucí okno.
/// </summary>
public sealed class PickerWorkspace
{
    private readonly Dictionary<string, int> _domIndexByPath = new(StringComparer.OrdinalIgnoreCase);

    public PickerWorkspace()
    {
        Settings = AppSettings.Load();
        Profiles = ProfileStore.Load();
        Registry = TpiTypeRegistry.CreateDefault(AppSettings.CustomTypesPath);
        Matcher = new ElementTypeMatcher(Registry);
        PathResolver = new ElementPathResolver(Settings.ElementPath);
        Generator = new GtoScriptGenerator(Registry, Settings.Script);
        Document = new ElementMappingDocument { PfName = Settings.LastPfName };
    }

    // ---------------------------------------------------------------- stav

    public AppSettings Settings { get; }
    public ProfileStore Profiles { get; }
    public TpiTypeRegistry Registry { get; }
    public ElementTypeMatcher Matcher { get; private set; }
    public ElementPathResolver PathResolver { get; }
    public GtoScriptGenerator Generator { get; }
    public ElementMappingDocument Document { get; private set; }

    /// <summary>Okno s prohlížečem – registruje se při jeho vytvoření.</summary>
    public IBrowserHost? Browser { get; set; }

    /// <summary>Naposledy kliknutý prvek stránky.</summary>
    public DomNode? CurrentNode { get; private set; }

    /// <summary>Aktuálně rozpracovaný in_ref_element_path (okno Prvek).</summary>
    public string CurrentElementPath { get; set; } = string.Empty;

    /// <summary>Sloupec, položka menu, záložka nebo event doplňovaný za element_path.</summary>
    public string CurrentSuffix { get; set; } = string.Empty;

    /// <summary>
    /// Vyplněno, když se má element tímto GTO teprve vytvořit – 'layout' nebo 'popup'
    /// (parametr in_element_typ).
    /// </summary>
    public string? CurrentCreateElementTyp { get; set; }

    public string PfName
    {
        get => Document.PfName;
        set => SetPfName(value, autoDetectScreenKind: false);
    }

    /// <summary>
    /// Nastaví název page flow. Volitelně z něj odvodí typ obrazovky
    /// (prefix LIST = seznam) – to se hodí u názvu zachyceného ze síťové komunikace.
    /// </summary>
    public void SetPfName(string? value, bool autoDetectScreenKind)
    {
        var pf = value?.Trim() ?? string.Empty;
        if (string.Equals(Document.PfName, pf, StringComparison.Ordinal)) return;

        Document.PfName = pf;
        PfNameChanged?.Invoke(pf);

        if (!autoDetectScreenKind) return;

        var kind = TpiNaming.ScreenKindFromPageFlow(pf);
        if (kind != TpiScreenKind.Unknown)
            ScreenKind = kind;

        var table = TpiNaming.TableNameFromPageFlow(pf);
        if (table is not null)
            Status($"PageFlow {pf} – fyzická tabulka v DB: {table}");
    }

    /// <summary>Fyzický název tabulky odvozený z názvu page flow (vše za prefixem LIST).</summary>
    public string? PhysicalTableName => TpiNaming.TableNameFromPageFlow(Document.PfName);

    public bool PickMode { get; private set; }

    /// <summary>
    /// Typ obrazovky – seznam (tabulka = ObjectList) nebo detail (formulář).
    /// Ovlivňuje návrh in_ref_element_path a kontroly.
    /// </summary>
    public TpiScreenKind ScreenKind
    {
        get => Document.ScreenKind;
        set
        {
            if (Document.ScreenKind == value) return;
            Document.ScreenKind = value;
            ScreenKindChanged?.Invoke(value);
        }
    }

    // -------------------------------------------------------------- události

    public event Action<string>? StatusChanged;
    public event Action<DomNode>? ElementPicked;
    public event Action<DomNode>? TreeLoaded;
    public event Action? MappingChanged;
    public event Action<MappedElement>? EditRequested;
    public event Action<string>? ScriptGenerated;
    public event Action<IReadOnlyList<GtoValidationIssue>>? IssuesChanged;
    public event Action<bool>? PickModeChanged;
    public event Action? CatalogReloaded;
    public event Action<TpiScreenKind>? ScreenKindChanged;

    /// <summary>Návrh element_path odvozený z typu prvku (např. ObjectList na seznamu).</summary>
    public event Action<string>? ElementPathSuggested;

    public event Action<string>? PfNameChanged;

    /// <summary>Název page flow zachycený v síťové komunikaci (hodnota, zdroj).</summary>
    public event Action<string, string>? PageFlowCandidateFound;

    public void Status(string text) => StatusChanged?.Invoke(text);

    // ------------------------------------------------------- zprávy ze stránky

    public void HandleBrowserMessage(PickerMessage message)
    {
        switch (message.Type)
        {
            case "tree":
                if (message.Root is null) return;
                message.Root.LinkParents();
                Document.Url = message.Url;
                Document.Title = message.Title;
                TreeLoaded?.Invoke(message.Root);
                Status($"DOM načten – {message.Count} prvků.");
                break;

            case "pick":
                if (message.Node is null) return;
                message.LinkPickChain();
                CurrentNode = message.Node;
                if (string.IsNullOrWhiteSpace(Document.Url))
                    Document.Url = message.Url;
                ElementPicked?.Invoke(message.Node);
                Status("Vybrán prvek " + message.Node.Caption());
                break;

            case "pickmode":
                PickMode = message.On;
                PickModeChanged?.Invoke(message.On);
                break;

            case "ready":
                Status("Stránka připravena: " + message.Title);
                break;
        }
    }

    public async Task SetPickModeAsync(bool on)
    {
        PickMode = on;
        if (Browser is not null)
            await Browser.SetPickModeAsync(on);
        PickModeChanged?.Invoke(on);
        Status(on
            ? "Režim výběru prvku – klikni na prvek ve stránce (Esc ukončí)."
            : "Režim výběru vypnut.");
    }

    public Task RequestTreeAsync() => Browser?.RequestTreeAsync() ?? Task.CompletedTask;

    private readonly HashSet<string> _pageFlowCandidates = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Nahlásí název page flow nalezený v síťové komunikaci.</summary>
    public void ReportPageFlowCandidate(string value, string source)
    {
        if (string.IsNullOrWhiteSpace(value)) return;

        var candidate = value.Trim();
        if (!_pageFlowCandidates.Add(candidate)) return;

        PageFlowCandidateFound?.Invoke(candidate, source);
        Status($"Nalezen možný PageFlow: {candidate}");
    }

    public async Task HighlightAsync(DomNode node)
    {
        if (Browser is null || node.Index < 0) return;
        await Browser.HighlightAsync(node.Index);
    }

    /// <summary>Najde na stránce prvek uložený v mapování.</summary>
    public async Task<bool> LocateAsync(MappedElement element)
    {
        if (Browser is null) return false;

        if (_domIndexByPath.TryGetValue(element.EffectivePath(), out var index) ||
            _domIndexByPath.TryGetValue(element.ElementPath, out index))
        {
            await Browser.HighlightAsync(index);
            return true;
        }

        if (!string.IsNullOrWhiteSpace(element.CssSelector))
            return await Browser.ScrollToSelectorAsync(element.CssSelector!);

        return false;
    }

    // ------------------------------------------------------------ element_path

    /// <summary>Kandidáti na element_path pro daný prvek.</summary>
    public IReadOnlyList<ElementPathCandidate> GetPathCandidates(DomNode node)
        => PathResolver.GetCandidates(node);

    /// <summary>
    /// Návrh element_path podle typu prvku a typu obrazovky. Na seznamu se tabulka
    /// (i její sloupce) adresuje jako <c>ObjectList</c>, proto se z DOM nic odvozovat
    /// nemusí. Vrací null, když typ žádný pevný název nemá.
    /// </summary>
    public string? SuggestPathForType(TpiElementType type)
    {
        if (type is null) return null;
        if (ScreenKind != TpiScreenKind.List) return null;
        if (string.IsNullOrWhiteSpace(type.ListScreenElementName)) return null;

        var suggestion = type.ListScreenElementName!;
        if (string.Equals(CurrentElementPath, suggestion, StringComparison.OrdinalIgnoreCase))
            return suggestion;

        CurrentElementPath = suggestion;
        ElementPathSuggested?.Invoke(suggestion);
        Status($"Obrazovka typu seznam – element_path nastaven na '{suggestion}'." +
               (type.RequiresSubElement ? $" Doplň ještě {type.SubElementLabel}." : string.Empty));
        return suggestion;
    }

    /// <summary>
    /// Zapamatuje atribut jako primární zdroj element_path (uloží do nastavení).
    /// </summary>
    public void RememberPathAttribute(string attribute)
    {
        var list = Settings.ElementPath.SourceAttributes;
        list.RemoveAll(a => string.Equals(a, attribute, StringComparison.OrdinalIgnoreCase));
        list.Insert(0, attribute);
        Settings.Save();
        Status($"Atribut '{attribute}' se nyní používá jako primární zdroj element_path.");
    }

    // --------------------------------------------------------------- mapování

    public MappedElement? FindMapping(string elementPath)
        => Document.Elements.FirstOrDefault(x =>
            string.Equals(x.ElementPath, elementPath, StringComparison.OrdinalIgnoreCase));

    public MappedElement ApplyMapping(TpiElementType type, string? eventCode, IEnumerable<GtoAssignment> assignments)
    {
        var element = new MappedElement
        {
            ElementPath = CurrentElementPath.Trim(),
            TypeCode = type.Code,
            TypeName = type.Name,
            Label = CurrentNode?.Text,
            Tag = CurrentNode?.Tag,
            DomId = CurrentNode?.Id,
            DomName = CurrentNode?.Name,
            InputType = CurrentNode?.InputType,
            CssSelector = CurrentNode?.Css,
            XPath = CurrentNode?.XPath,
            Attributes = CurrentNode?.Attrs ?? new Dictionary<string, string>(),
            Assignments = assignments.ToList(),
            CreateElementTyp = string.IsNullOrWhiteSpace(CurrentCreateElementTyp)
                ? null
                : CurrentCreateElementTyp
        };

        if (type.AppendsEvent)
            element.EventCode = eventCode;
        else if (!string.IsNullOrWhiteSpace(CurrentSuffix))
            element.Column = CurrentSuffix.Trim();

        var existing = Document.Elements.FindIndex(x =>
            string.Equals(x.EffectivePath(), element.EffectivePath(), StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.ElementPath, element.ElementPath, StringComparison.OrdinalIgnoreCase));

        if (existing >= 0)
            Document.Elements[existing] = element;
        else
            Document.Elements.Add(element);

        if (CurrentNode is { Index: >= 0 })
            _domIndexByPath[element.EffectivePath()] = CurrentNode.Index;

        MappingChanged?.Invoke();
        Status($"Uloženo do mapování: {element.EffectivePath()} [{type.Code}]");
        return element;
    }

    public void RemoveMapping(MappedElement element)
    {
        Document.Elements.Remove(element);
        MappingChanged?.Invoke();
        Status("Prvek odebrán z mapování.");
    }

    public void RequestEdit(MappedElement element) => EditRequested?.Invoke(element);

    public void RememberDomIndex(string path, int index)
    {
        if (index >= 0 && !string.IsNullOrWhiteSpace(path))
            _domIndexByPath[path] = index;
    }

    // ---------------------------------------------------------------- skripty

    /// <summary>Vygeneruje skript. mode = null znamená statické i dynamické GTO.</summary>
    public string Generate(GtoMode? mode)
    {
        Generator.Options = Settings.Script;

        var issues = Generator.Validate(Document);
        IssuesChanged?.Invoke(issues);

        if (issues.Any(i => i.Severity == GtoValidationIssue.Error))
        {
            Status("Skript nelze vygenerovat – oprav chyby v okně Kontroly.");
            return string.Empty;
        }

        var script = mode switch
        {
            GtoMode.Static => Generator.Generate(Document, GtoMode.Static),
            GtoMode.Dynamic => Generator.Generate(Document, GtoMode.Dynamic),
            _ => Generator.GenerateAll(Document)
        };

        if (string.IsNullOrWhiteSpace(script))
            script = "-- Není co generovat: žádná zaškrtnutá vlastnost pro zvolený režim.";

        ScriptGenerated?.Invoke(script);
        Status("Skript vygenerován.");
        return script;
    }

    // ------------------------------------------------------ soubory a číselník

    public void LoadDocument(string path)
    {
        Document = ElementMappingDocument.Load(path);
        Settings.LastMappingFile = path;
        _domIndexByPath.Clear();
        MappingChanged?.Invoke();
        Status("Mapování načteno: " + Path.GetFileName(path));
    }

    public void SaveDocument(string path)
    {
        Document.Save(path);
        Settings.LastMappingFile = path;
        Status("Mapování uloženo: " + Path.GetFileName(path));
    }

    public void ReloadCatalog()
    {
        Registry.Reload();
        Matcher = new ElementTypeMatcher(Registry);
        CatalogReloaded?.Invoke();
        Status("Číselník typů znovu načten.");
    }

    public void SaveSettings()
    {
        Settings.LastPfName = Document.PfName;
        Settings.Save();
    }
}
