using System.Text;
using TpiGto.Mapping;
using TpiGto.Model;
using TpiGto.Naming;
using TpiGto.Registry;

namespace TpiGto.Scripting;

/// <summary>Výsledek kontroly mapování před generováním.</summary>
public sealed record GtoValidationIssue(string Severity, string ElementPath, string Message)
{
    public const string Error = "CHYBA";
    public const string Warning = "VAROVÁNÍ";
    public const string Info = "INFO";

    public override string ToString() => $"[{Severity}] {ElementPath}: {Message}";
}

/// <summary>
/// Generátor skriptů GTO nad namapovanými prvky. Formáty výstupu dodávají
/// zapisovače <see cref="IGtoScriptWriter"/>, takže lze přidat další režim
/// bez zásahu do generátoru.
/// </summary>
public sealed class GtoScriptGenerator
{
    private readonly Dictionary<GtoMode, IGtoScriptWriter> _writers = new();
    private readonly TpiTypeRegistry _registry;

    public GtoScriptGenerator(TpiTypeRegistry registry, GtoScriptOptions? options = null)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        Options = options ?? new GtoScriptOptions();

        RegisterWriter(new StaticGtoScriptWriter());
        RegisterWriter(new DynamicGtoScriptWriter(GtoMode.Dynamic));
        RegisterWriter(new DynamicGtoScriptWriter(GtoMode.DynamicData));
    }

    public GtoScriptOptions Options { get; set; }

    public void RegisterWriter(IGtoScriptWriter writer)
    {
        if (writer is null) throw new ArgumentNullException(nameof(writer));
        _writers[writer.Mode] = writer;
    }

    /// <summary>Vygeneruje skript pro daný režim. Vrátí prázdný řetězec, pokud není co generovat.</summary>
    public string Generate(ElementMappingDocument document, GtoMode mode)
    {
        if (document is null) throw new ArgumentNullException(nameof(document));
        if (!_writers.TryGetValue(mode, out var writer))
            throw new InvalidOperationException($"Pro režim {mode} není zaregistrován žádný zapisovač.");

        var blocks = new StringBuilder();
        var count = 0;

        foreach (var element in document.Elements)
        {
            foreach (var assignment in element.Assignments)
            {
                if (!assignment.Enabled || assignment.Mode != mode)
                    continue;
                if (string.IsNullOrWhiteSpace(assignment.PropertyName))
                    continue;

                writer.WriteBlock(blocks, new GtoBlockContext(
                    document.PfName, element, assignment, _registry, Options));
                count++;
            }
        }

        return count == 0 ? string.Empty : writer.Wrap(blocks.ToString(), Options, document.PfName);
    }

    /// <summary>Statické i dynamické GTO v jednom výstupu (oddělené komentářem).</summary>
    public string GenerateAll(ElementMappingDocument document)
    {
        var sb = new StringBuilder();

        var stat = Generate(document, GtoMode.Static);
        if (!string.IsNullOrWhiteSpace(stat))
        {
            sb.AppendLine("-- ============================================================");
            sb.AppendLine($"-- STATICKÉ GTO – PageFlow {document.PfName}");
            sb.AppendLine("-- ============================================================");
            sb.AppendLine(stat);
        }

        var dyn = Generate(document, GtoMode.Dynamic);
        if (!string.IsNullOrWhiteSpace(dyn))
        {
            if (sb.Length > 0) sb.AppendLine();
            sb.AppendLine("-- ============================================================");
            sb.AppendLine("-- RUNTIME GTO – vlož do theSql plain bloku MWF");
            sb.AppendLine("-- ============================================================");
            sb.AppendLine(dyn);
        }

        var dynData = Generate(document, GtoMode.DynamicData);
        if (!string.IsNullOrWhiteSpace(dynData))
        {
            if (sb.Length > 0) sb.AppendLine();
            sb.AppendLine("-- ============================================================");
            sb.AppendLine("-- RUNTIME GTO NAD DATY – @GATTRIB_OVERLOAD_DATA");
            sb.AppendLine("-- (provede se až po namapování business dat do GMSG)");
            sb.AppendLine("-- ============================================================");
            sb.AppendLine(dynData);
        }

        return sb.ToString();
    }

    /// <summary>Kontrola mapování – prázdné cesty, neznámé vlastnosti, chybějící hodnoty.</summary>
    public IReadOnlyList<GtoValidationIssue> Validate(ElementMappingDocument document)
    {
        var issues = new List<GtoValidationIssue>();

        if (string.IsNullOrWhiteSpace(document.PfName))
        {
            issues.Add(new GtoValidationIssue(GtoValidationIssue.Error, "-",
                "Není vyplněn název PageFlow (in_pf_name)."));
        }
        else
        {
            // Chybný název page flow je častá příčina toho, že se změna vůbec neprojeví.
            if (document.PfName.Any(char.IsWhiteSpace))
                issues.Add(new GtoValidationIssue(GtoValidationIssue.Error, "-",
                    "Název PageFlow obsahuje mezeru."));
            else if (document.PfName.Any(char.IsLower))
                issues.Add(new GtoValidationIssue(GtoValidationIssue.Warning, "-",
                    "Název PageFlow bývá velkými písmeny (např. EDIT_PZSV_NECERTIF_ZAZNAM_HLAV) – zkontroluj ho, " +
                    "při překlepu se změna tiše neprojeví."));
        }

        // Duplicitní nastavení téže vlastnosti na tentýž element_path.
        var duplicates = document.Elements
            .SelectMany(e => e.Assignments.Where(a => a.Enabled)
                .Select(a => new { Path = e.EffectivePath(), a.PropertyName, a.Mode }))
            .GroupBy(x => (x.Path, x.PropertyName.ToUpperInvariant(), x.Mode))
            .Where(g => g.Count() > 1);

        foreach (var duplicate in duplicates)
            issues.Add(new GtoValidationIssue(GtoValidationIssue.Warning, duplicate.Key.Path,
                $"Vlastnost '{duplicate.First().PropertyName}' je pro tento element nastavena {duplicate.Count()}×."));

        // Statické GTO se spouští první, dynamické (v MWF) až po něm a hodnotu přepíše.
        var modeConflicts = document.Elements
            .SelectMany(e => e.Assignments.Where(a => a.Enabled)
                .Select(a => new { Path = e.EffectivePath(), Property = a.PropertyName.ToUpperInvariant(), a.Mode }))
            .GroupBy(x => (x.Path, x.Property))
            .Where(g => g.Select(x => x.Mode).Distinct().Count() > 1);

        foreach (var conflict in modeConflicts)
            issues.Add(new GtoValidationIssue(GtoValidationIssue.Warning, conflict.Key.Path,
                $"Vlastnost '{conflict.First().Property}' je nastavena staticky i dynamicky. " +
                "Statické GTO se spouští první, dynamické z MWF až po něm a hodnotu přepíše."));

        // Nápověda, kde hledat technické názvy sloupců.
        var tableName = TpiNaming.TableNameFromPageFlow(document.PfName);
        if (tableName is not null &&
            document.Elements.Any(e => _registry.Find(e.TypeCode)?.RequiresSubElement == true))
            issues.Add(new GtoValidationIssue(GtoValidationIssue.Info, "-",
                $"Fyzický název tabulky odvozený z PageFlow: {tableName}. " +
                "Technické názvy sloupců hledej v této tabulce – popisek v hlavičce jim odpovídat nemusí."));

        foreach (var element in document.Elements)
        {
            var path = element.EffectivePath();

            if (string.IsNullOrWhiteSpace(element.ElementPath))
                issues.Add(new GtoValidationIssue(GtoValidationIssue.Error, "-",
                    "Prvek nemá vyplněný in_ref_element_path."));

            var type = _registry.Find(element.TypeCode);
            if (type is null)
            {
                issues.Add(new GtoValidationIssue(GtoValidationIssue.Error, path,
                    $"Neznámý typ prvku '{element.TypeCode}'."));
                continue;
            }

            if (type.AppendsEvent && string.IsNullOrWhiteSpace(element.EventCode))
                issues.Add(new GtoValidationIssue(GtoValidationIssue.Warning, path,
                    "Typ CONNECTION obvykle vyžaduje event (blur, click, right_click…)."));

            // Podřízený prvek musí být součástí cesty, jinak změna zasáhne celý element.
            if (type.RequiresSubElement && string.IsNullOrWhiteSpace(element.Column))
                issues.Add(new GtoValidationIssue(GtoValidationIssue.Error, path,
                    $"Typ {type.Code} vyžaduje v cestě {type.SubElementLabel}, " +
                    "jinak se změna vztahuje na celý element."));

            // Nový element lze podle dokumentace vytvořit jen jako layout nebo popup.
            if (!string.IsNullOrWhiteSpace(element.CreateElementTyp))
            {
                var typ = element.CreateElementTyp!.Trim().ToLowerInvariant();
                if (typ is not ("layout" or "popup"))
                    issues.Add(new GtoValidationIssue(GtoValidationIssue.Error, path,
                        $"in_element_typ může být jen 'layout' nebo 'popup', zadáno '{element.CreateElementTyp}'."));

                var placed = element.Assignments.Any(x => x.Enabled &&
                    string.Equals(x.PropertyName, "M_Pf_Element.M_Pf_Element_Name", StringComparison.OrdinalIgnoreCase));

                if (!placed)
                    issues.Add(new GtoValidationIssue(GtoValidationIssue.Warning, path,
                        "Nově vytvořený element je potřeba zařadit pod existující element – " +
                        "doplň vlastnost M_Pf_Element.M_Pf_Element_Name."));
            }

            // Na obrazovce typu seznam se tabulka ve frameworku jmenuje vždy ObjectList.
            if (document.ScreenKind == TpiScreenKind.List &&
                !string.IsNullOrWhiteSpace(type.ListScreenElementName) &&
                !string.Equals(element.ElementPath, type.ListScreenElementName, StringComparison.OrdinalIgnoreCase))
                issues.Add(new GtoValidationIssue(GtoValidationIssue.Warning, path,
                    $"Na obrazovce typu seznam se tabulka jmenuje '{type.ListScreenElementName}' – " +
                    $"zadáno '{element.ElementPath}'."));

            if (element.ElementPath.Any(char.IsWhiteSpace))
                issues.Add(new GtoValidationIssue(GtoValidationIssue.Error, path,
                    "in_ref_element_path obsahuje mezeru."));

            var allowed = _registry.GetProperties(type);

            foreach (var a in element.Assignments.Where(x => x.Enabled))
            {
                var def = allowed.FirstOrDefault(p =>
                    string.Equals(p.Name, a.PropertyName, StringComparison.OrdinalIgnoreCase));

                if (def is null)
                {
                    issues.Add(new GtoValidationIssue(GtoValidationIssue.Warning, path,
                        $"Vlastnost '{a.PropertyName}' není v číselníku pro typ {type.Code}. " +
                        "Špatně zvolený gattrib nehlásí chybu – na obrazovce se prostě nic nestane."));
                    continue;
                }

                if (a.Cancelled != 1 && string.IsNullOrWhiteSpace(a.Value))
                    issues.Add(new GtoValidationIssue(GtoValidationIssue.Warning, path,
                        $"Vlastnost '{def.Name}' nemá vyplněnou hodnotu."));

                if (def.ValueKind == GtoValueKind.Bool01 &&
                    !string.IsNullOrWhiteSpace(a.Value) &&
                    a.Value.Trim() is not ("0" or "1"))
                    issues.Add(new GtoValidationIssue(GtoValidationIssue.Warning, path,
                        $"Vlastnost '{def.Name}' očekává 0/1, zadáno '{a.Value}'."));

                if (def.ValueKind == GtoValueKind.Number &&
                    !string.IsNullOrWhiteSpace(a.Value) &&
                    !decimal.TryParse(a.Value.Trim(), out _))
                    issues.Add(new GtoValidationIssue(GtoValidationIssue.Warning, path,
                        $"Vlastnost '{def.Name}' očekává číslo, zadáno '{a.Value}'."));

                if (a.Mode == GtoMode.Static && !def.AllowStatic)
                    issues.Add(new GtoValidationIssue(GtoValidationIssue.Warning, path,
                        $"Vlastnost '{def.Name}' není určena pro statické GTO."));

                if (a.Mode is GtoMode.Dynamic or GtoMode.DynamicData && !def.AllowDynamic)
                    issues.Add(new GtoValidationIssue(GtoValidationIssue.Warning, path,
                        $"Vlastnost '{def.Name}' nelze konfigurovat v runtime."));

                if (def.Source == GtoPropertySource.CisGto)
                    issues.Add(new GtoValidationIssue(GtoValidationIssue.Info, path,
                        $"Vlastnost '{def.Name}' je jen v cis_gto.docx, ne v seznamu GMSG vlastností – " +
                        "ověř, že v tomto prostředí existuje."));

                // Typ overloadu se uplatní jen u iniciálního (statického) GTO.
                if (a.Mode != GtoMode.Static && a.OverloadType != GtoOverloadType.Default)
                    issues.Add(new GtoValidationIssue(GtoValidationIssue.Warning, path,
                        $"Typ overloadu {a.OverloadTypeName} má význam jen u statického GTO."));

                if (a.OverloadType == GtoOverloadType.Role && string.IsNullOrWhiteSpace(a.RefRole))
                    issues.Add(new GtoValidationIssue(GtoValidationIssue.Error, path,
                        "ROLE_OVERLOAD vyžaduje in_ref_role (m_role.id)."));

                if (a.OverloadType == GtoOverloadType.User && string.IsNullOrWhiteSpace(a.RefLogin))
                    issues.Add(new GtoValidationIssue(GtoValidationIssue.Error, path,
                        "USER_OVERLOAD vyžaduje in_ref_login (tpi_uzivatel.id)."));

                if (a.OverloadType == GtoOverloadType.Default &&
                    (!string.IsNullOrWhiteSpace(a.RefRole) || !string.IsNullOrWhiteSpace(a.RefLogin)))
                    issues.Add(new GtoValidationIssue(GtoValidationIssue.Warning, path,
                        "DEFAULT_OVERLOAD se nevztahuje na uživatele ani roli – vyplněné in_ref_login/in_ref_role se neuplatní."));
            }

            if (type.Code == "CONNECTION")
            {
                var enabledNames = element.Assignments.Where(x => x.Enabled)
                    .Select(x => x.PropertyName).ToList();

                var hasMwf = enabledNames.Any(n =>
                    string.Equals(n, "M_Pf_Connection.M_Wf_Name", StringComparison.OrdinalIgnoreCase));
                var hasSwitch = enabledNames.Any(n =>
                    n.StartsWith("M_Pf_Connection.Supported", StringComparison.OrdinalIgnoreCase) ||
                    n.StartsWith("M_Pf_Connection.Enabled", StringComparison.OrdinalIgnoreCase));

                if (hasMwf && !hasSwitch)
                    issues.Add(new GtoValidationIssue(GtoValidationIssue.Warning, path,
                        "K M_Pf_Connection.M_Wf_Name chybí M_Pf_Connection.Supported (nebo .Enabled) = 1."));
            }
        }

        return issues;
    }
}
