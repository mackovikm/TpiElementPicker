using System.ComponentModel;
using TpiElementPicker.Services;

namespace TpiElementPicker.Forms;

/// <summary>
/// Nastavení aplikace – odvozování element_path a hlavička generovaných skriptů.
/// </summary>
public sealed class SettingsForm : Form
{
    private readonly AppSettings _settings;
    private readonly PropertyGrid _grid = new() { Dock = DockStyle.Fill };

    public bool ReloadCatalog { get; private set; }

    public SettingsForm(AppSettings settings)
    {
        _settings = settings;

        Text = "Nastavení";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(640, 520);
        MinimizeBox = false;
        MaximizeBox = false;

        var proxy = new SettingsProxy(settings);
        _grid.SelectedObject = proxy;

        var panel = new Panel { Dock = DockStyle.Bottom, Height = 44 };

        var btnReload = new Button
        {
            Text = "Znovu načíst číselník typů",
            Bounds = new Rectangle(8, 8, 200, 28)
        };
        btnReload.Click += (_, _) =>
        {
            ReloadCatalog = true;
            MessageBox.Show(this,
                "Číselník se znovu načte po zavření nastavení.\r\n\r\nVlastní typy: " +
                AppSettings.CustomTypesPath,
                "Číselník typů", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        var btnOk = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Bounds = new Rectangle(ClientSize.Width - 190, 8, 85, 28)
        };

        var btnCancel = new Button
        {
            Text = "Zrušit",
            DialogResult = DialogResult.Cancel,
            Bounds = new Rectangle(ClientSize.Width - 97, 8, 85, 28)
        };

        panel.Controls.AddRange(new Control[] { btnReload, btnOk, btnCancel });
        Controls.Add(_grid);
        Controls.Add(panel);

        AcceptButton = btnOk;
        CancelButton = btnCancel;
    }

    /// <summary>Plochý pohled na nastavení pro PropertyGrid.</summary>
    private sealed class SettingsProxy
    {
        private readonly AppSettings _s;

        public SettingsProxy(AppSettings settings) => _s = settings;

        [Category("element_path"), DisplayName("Zdrojové atributy (v pořadí)")]
        [Description("Atributy prohledávané při odvozování in_ref_element_path. První nalezený vyhrává.")]
        public List<string> SourceAttributes
        {
            get => _s.ElementPath.SourceAttributes;
            set => _s.ElementPath.SourceAttributes = value ?? new List<string>();
        }

        [Category("element_path"), DisplayName("Hledat i u rodičů")]
        public bool SearchAncestors
        {
            get => _s.ElementPath.SearchAncestors;
            set => _s.ElementPath.SearchAncestors = value;
        }

        [Category("element_path"), DisplayName("Maximální hloubka rodičů")]
        public int MaxAncestorDepth
        {
            get => _s.ElementPath.MaxAncestorDepth;
            set => _s.ElementPath.MaxAncestorDepth = value;
        }

        [Category("element_path"), DisplayName("Odstranit předpony")]
        public List<string> TrimPrefixes
        {
            get => _s.ElementPath.TrimPrefixes;
            set => _s.ElementPath.TrimPrefixes = value ?? new List<string>();
        }

        [Category("element_path"), DisplayName("Odstranit přípony")]
        public List<string> TrimSuffixes
        {
            get => _s.ElementPath.TrimSuffixes;
            set => _s.ElementPath.TrimSuffixes = value ?? new List<string>();
        }

        [Category("element_path"), DisplayName("Regex se skupinou (?<path>…)")]
        public string? ExtractRegex
        {
            get => _s.ElementPath.ExtractRegex;
            set => _s.ElementPath.ExtractRegex = value;
        }

        [Category("element_path"), DisplayName("Oddělovač elementu a sloupce/eventu")]
        public string Separator
        {
            get => _s.ElementPath.Separator;
            set => _s.ElementPath.Separator = string.IsNullOrEmpty(value) ? "." : value;
        }

        [Category("PageFlow"), DisplayName("Hledat PageFlow v síťové komunikaci")]
        [Description("Sleduje požadavky stránky a hledá v nich název page flow – nahrazuje ruční hledání v konzoli prohlížeče.")]
        public bool DetectPageFlow
        {
            get => _s.DetectPageFlow;
            set => _s.DetectPageFlow = value;
        }

        [Category("PageFlow"), DisplayName("Vzory pro hledání (regex)")]
        [Description("Skupina (?<pf>…) je nalezený název page flow. Projeví se po restartu aplikace.")]
        public List<string> PageFlowPatterns
        {
            get => _s.PageFlowPatterns;
            set => _s.PageFlowPatterns = value ?? new List<string>();
        }

        [Category("PageFlow"), DisplayName("Limit prohledávané odpovědi (B)")]
        public int PageFlowScanLimitBytes
        {
            get => _s.PageFlowScanLimitBytes;
            set => _s.PageFlowScanLimitBytes = value;
        }

        [Category("Skript"), DisplayName("Připomínka zapnout DBMS Output")]
        [Description("Do hlavičky skriptu vloží komentář – bez DBMS Output nepoznáš, že skript skončil OK.")]
        public bool AddOutputReminder
        {
            get => _s.Script.AddOutputReminder;
            set => _s.Script.AddOutputReminder = value;
        }

        [Category("Skript"), DisplayName("in_user (INIT_MD_EDIT)")]
        public string User
        {
            get => _s.Script.User;
            set => _s.Script.User = value;
        }

        [Category("Skript"), DisplayName("in_termin (INIT_MD_EDIT)")]
        public string Termin
        {
            get => _s.Script.Termin;
            set => _s.Script.Termin = value;
        }

        [Category("Skript"), DisplayName("in_gattrib_overload_typ")]
        public string OverloadType
        {
            get => _s.Script.OverloadType;
            set => _s.Script.OverloadType = value;
        }

        [Category("Skript"), DisplayName("Obalit statické GTO tělem skriptu")]
        public bool IncludeScriptBody
        {
            get => _s.Script.IncludeScriptBody;
            set => _s.Script.IncludeScriptBody = value;
        }

        [Category("Skript"), DisplayName("Přidat reset cache page flow")]
        [Description("Bez resetu cache se změna na obrazovce nemusí projevit. Cenou je pomalejší první načtení.")]
        public bool AppendCacheReset
        {
            get => _s.Script.AppendCacheReset;
            set => _s.Script.AppendCacheReset = value;
        }

        [Category("Skript"), DisplayName("Volání pro reset cache page flow")]
        [Description("Např. SRV.NEJAKY_PACKAGE.RESET_PAGE_FLOW('{PF}');  – {PF} se nahradí názvem page flow. " +
                     "Prázdné = do skriptu se vloží jen připomínka v komentáři.")]
        public string CacheResetStatement
        {
            get => _s.Script.CacheResetStatement;
            set => _s.Script.CacheResetStatement = value ?? string.Empty;
        }

        [Category("Skript – MWF"), DisplayName("Obalit dynamické GTO kostrou MWF")]
        public bool WrapDynamicInMwf
        {
            get => _s.Script.WrapDynamicInMwf;
            set => _s.Script.WrapDynamicInMwf = value;
        }

        [Category("Skript – MWF"), DisplayName("xMwfName")]
        public string MwfName
        {
            get => _s.Script.MwfName;
            set => _s.Script.MwfName = value;
        }

        [Category("Skript – MWF"), DisplayName("xMwfNotice")]
        public string MwfNotice
        {
            get => _s.Script.MwfNotice;
            set => _s.Script.MwfNotice = value;
        }

        [Category("Skript – MWF"), DisplayName("xMwfFirstStep")]
        public string MwfFirstStep
        {
            get => _s.Script.MwfFirstStep;
            set => _s.Script.MwfFirstStep = value;
        }

        [Category("Skript – MWF"), DisplayName("xMwfType")]
        public string MwfType
        {
            get => _s.Script.MwfType;
            set => _s.Script.MwfType = value;
        }
    }
}
