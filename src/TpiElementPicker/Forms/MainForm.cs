using TpiElementPicker.Forms.Docking;
using TpiElementPicker.Services;
using TpiElementPicker.Workspace;
using TpiGto.Mapping;
using TpiGto.Model;
using WeifenLuo.WinFormsUI.Docking;

namespace TpiElementPicker.Forms;

/// <summary>
/// Hlavní okno ve stylu Visual Studia – menu, panel nástrojů, stavový řádek a
/// dokovací plocha. Všechna ostatní okna jsou dokovatelná: lze je přesunout na
/// jinou stranu, sloučit do záložek, schovat do auto-hide pruhu nebo vytáhnout
/// jako samostatné plovoucí okno mimo hlavní okno.
/// </summary>
public sealed class MainForm : Form
{
    private readonly PickerWorkspace _workspace = new();
    private readonly DockPanel _dock = new();

    private readonly BrowserWindow _browserWindow;
    private readonly DomTreeWindow _treeWindow;
    private readonly ElementWindow _elementWindow;
    private readonly TypeWindow _typeWindow;
    private readonly MappingWindow _mappingWindow;
    private readonly ScriptWindow _scriptWindow;
    private readonly IssuesWindow _issuesWindow;
    private readonly PageFlowWindow _pageFlowWindow;

    private readonly MenuStrip _menu = new();
    private readonly ToolStrip _toolbar = new();
    private readonly StatusStrip _statusStrip = new();
    private readonly ToolStripStatusLabel _status = new("Připraveno") { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
    private readonly ToolStripStatusLabel _pickState = new("Výběr vypnut");
    private readonly ToolStripStatusLabel _envState = new("Prostředí: neurčeno");

    private readonly ToolStripComboBox _profileBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 170 };
    private readonly ToolStripTextBox _urlBox = new() { Width = 380 };
    private readonly ToolStripTextBox _pfBox = new() { Width = 230 };
    private readonly ToolStripComboBox _screenKindBox = new()
    {
        DropDownStyle = ComboBoxStyle.DropDownList,
        Width = 120,
        ToolTipText = "Typ obrazovky – na seznamu se tabulka jmenuje ObjectList"
    };
    private readonly ToolStripButton _pickButton = new("Vybrat prvek (F2)")
    {
        DisplayStyle = ToolStripItemDisplayStyle.Text,
        CheckOnClick = true
    };

    private bool _suppress;

    public MainForm()
    {
        Text = "TPI Element Picker";
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        ClientSize = new Size(1400, 820);
        KeyPreview = true;

        _dock.Dock = DockStyle.Fill;
        _dock.DocumentStyle = DocumentStyle.DockingWindow;
        _dock.ShowDocumentIcon = true;
        ApplyTheme(_workspace.Settings.Theme, silent: true);

        _browserWindow = new BrowserWindow(_workspace);
        _treeWindow = new DomTreeWindow(_workspace);
        _elementWindow = new ElementWindow(_workspace);
        _typeWindow = new TypeWindow(_workspace);
        _mappingWindow = new MappingWindow(_workspace);
        _scriptWindow = new ScriptWindow(_workspace);
        _issuesWindow = new IssuesWindow(_workspace);
        _pageFlowWindow = new PageFlowWindow(_workspace);

        BuildMenu();
        BuildToolbar();
        BuildStatusBar();

        Controls.Add(_dock);
        Controls.Add(_statusStrip);
        Controls.Add(_toolbar);
        Controls.Add(_menu);
        MainMenuStrip = _menu;

        _workspace.StatusChanged += text => OnUi(() => _status.Text = text);
        _workspace.PickModeChanged += on => OnUi(() =>
        {
            _suppress = true;
            _pickButton.Checked = on;
            _suppress = false;
            _pickState.Text = on ? "Výběr zapnut" : "Výběr vypnut";
        });
        _workspace.MappingChanged += () => OnUi(UpdateTitle);
        _workspace.PfNameChanged += pf => OnUi(() =>
        {
            _suppress = true;
            _pfBox.Text = pf;
            _suppress = false;
            UpdateTitle();
        });
        _workspace.ScreenKindChanged += kind => OnUi(() => SelectScreenKind(kind));

        FillProfiles();
        _urlBox.Text = _workspace.Settings.LastUrl;
        _pfBox.Text = _workspace.Settings.LastPfName;
    }

    // ------------------------------------------------------------------ menu

    private void BuildMenu()
    {
        var file = new ToolStripMenuItem("&Soubor");

        var openItem = new ToolStripMenuItem("&Otevřít mapování…", null, (_, _) => OpenMapping())
        {
            ShortcutKeys = Keys.Control | Keys.O
        };
        var saveItem = new ToolStripMenuItem("&Uložit mapování…", null, (_, _) => SaveMapping())
        {
            ShortcutKeys = Keys.Control | Keys.S
        };

        file.DropDownItems.Add(openItem);
        file.DropDownItems.Add(saveItem);
        file.DropDownItems.Add(new ToolStripSeparator());
        file.DropDownItems.Add("Uložit &skript…", null, (_, _) => _scriptWindow.Save());
        file.DropDownItems.Add(new ToolStripSeparator());
        file.DropDownItems.Add("&Konec", null, (_, _) => Close());

        var view = new ToolStripMenuItem("&Zobrazit");
        view.DropDownItems.Add(WindowItem("Webová aplikace", () => _browserWindow));
        view.DropDownItems.Add(WindowItem("DOM strom", () => _treeWindow));
        view.DropDownItems.Add(WindowItem("Prvek", () => _elementWindow));
        view.DropDownItems.Add(WindowItem("Typ a vlastnosti", () => _typeWindow));
        view.DropDownItems.Add(WindowItem("Mapování", () => _mappingWindow));
        view.DropDownItems.Add(WindowItem("Skript GTO", () => _scriptWindow));
        view.DropDownItems.Add(WindowItem("Kontroly", () => _issuesWindow));
        view.DropDownItems.Add(WindowItem("PageFlow", () => _pageFlowWindow));
        view.DropDownItems.Add(new ToolStripSeparator());
        view.DropDownItems.Add("Uložit rozložení oken", null, (_, _) => SaveLayout());
        view.DropDownItems.Add("Obnovit výchozí rozložení", null, (_, _) => ResetLayout());
        view.DropDownItems.Add(new ToolStripSeparator());

        var theme = new ToolStripMenuItem("Motiv");
        theme.DropDownItems.Add("Visual Studio 2015 – modrý", null, (_, _) => ApplyTheme("VS2015Blue"));
        theme.DropDownItems.Add("Visual Studio 2015 – světlý", null, (_, _) => ApplyTheme("VS2015Light"));
        theme.DropDownItems.Add("Visual Studio 2015 – tmavý", null, (_, _) => ApplyTheme("VS2015Dark"));
        view.DropDownItems.Add(theme);

        var generate = new ToolStripMenuItem("&Generovat");
        generate.DropDownItems.Add("&Statické GTO", null, (_, _) => _scriptWindow.Generate(GtoMode.Static));
        generate.DropDownItems.Add("&Runtime GTO", null, (_, _) => _scriptWindow.Generate(GtoMode.Dynamic));
        generate.DropDownItems.Add("Runtime GTO nad &daty", null, (_, _) => _scriptWindow.Generate(GtoMode.DynamicData));
        var all = new ToolStripMenuItem("&Vše", null, (_, _) => _scriptWindow.Generate(null))
        {
            ShortcutKeys = Keys.Control | Keys.G
        };
        generate.DropDownItems.Add(all);

        var tools = new ToolStripMenuItem("&Nástroje");
        tools.DropDownItems.Add("&Profily přihlášení…", null, (_, _) => ShowLoginDialog());
        tools.DropDownItems.Add("&Nastavení…", null, (_, _) => ShowSettingsDialog());
        tools.DropDownItems.Add(new ToolStripSeparator());
        tools.DropDownItems.Add("Znovu načíst &číselník typů", null, (_, _) => _workspace.ReloadCatalog());

        var help = new ToolStripMenuItem("&Nápověda");
        help.DropDownItems.Add("O aplikaci", null, (_, _) => ShowAbout());

        _menu.Items.AddRange(new ToolStripItem[] { file, view, generate, tools, help });
    }

    private ToolStripMenuItem WindowItem(string title, Func<ToolWindowBase> window)
    {
        var item = new ToolStripMenuItem(title);
        item.Click += (_, _) =>
        {
            var content = window();
            if (content.IsHidden || content.DockState == DockState.Unknown)
                content.Show(_dock);
            content.Activate();
        };
        return item;
    }

    // -------------------------------------------------------------- toolbar

    private void BuildToolbar()
    {
        _toolbar.GripStyle = ToolStripGripStyle.Hidden;

        var go = new ToolStripButton("Načíst") { DisplayStyle = ToolStripItemDisplayStyle.Text };
        go.Click += async (_, _) => await NavigateAsync();

        var login = new ToolStripButton("Přihlášení…") { DisplayStyle = ToolStripItemDisplayStyle.Text };
        login.Click += (_, _) => ShowLoginDialog();

        var tree = new ToolStripButton("Načíst DOM (F4)") { DisplayStyle = ToolStripItemDisplayStyle.Text };
        tree.Click += async (_, _) => await _workspace.RequestTreeAsync();

        var apply = new ToolStripButton("Uložit prvek (F5)") { DisplayStyle = ToolStripItemDisplayStyle.Text };
        apply.Click += (_, _) => _typeWindow.Apply();

        var generate = new ToolStripButton("Generovat GTO") { DisplayStyle = ToolStripItemDisplayStyle.Text };
        generate.Click += (_, _) => _scriptWindow.Generate(null);

        _pickButton.CheckedChanged += async (_, _) =>
        {
            if (_suppress) return;
            await _workspace.SetPickModeAsync(_pickButton.Checked);
        };

        _urlBox.KeyDown += async (_, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            await NavigateAsync();
        };

        _pfBox.TextChanged += (_, _) =>
        {
            if (_suppress) return;
            _workspace.PfName = _pfBox.Text;
            UpdateTitle();
        };

        _profileBox.SelectedIndexChanged += (_, _) =>
        {
            if (_suppress) return;
            if (_profileBox.SelectedItem is not ConnectionProfile profile) return;
            if (!string.IsNullOrWhiteSpace(profile.Url))
                _urlBox.Text = profile.Url;
            ShowEnvironment(profile.Environment);
        };

        _screenKindBox.Items.AddRange(new object[] { "Obrazovka: ?", "Seznam", "Detail" });
        _screenKindBox.SelectedIndex = 0;
        _screenKindBox.SelectedIndexChanged += (_, _) =>
        {
            if (_suppress) return;
            _workspace.ScreenKind = (TpiScreenKind)_screenKindBox.SelectedIndex;
        };

        _toolbar.Items.AddRange(new ToolStripItem[]
        {
            _profileBox, _urlBox, go, login, new ToolStripSeparator(),
            _pickButton, tree, new ToolStripSeparator(),
            new ToolStripLabel("PageFlow:"), _pfBox, _screenKindBox, new ToolStripSeparator(),
            apply, generate
        });
    }

    private void BuildStatusBar()
    {
        _envState.AutoSize = true;
        _envState.BorderSides = ToolStripStatusLabelBorderSides.Left;

        _statusStrip.Items.Add(_status);
        _statusStrip.Items.Add(new ToolStripStatusLabel("|"));
        _statusStrip.Items.Add(_pickState);
        _statusStrip.Items.Add(new ToolStripStatusLabel("|"));
        _statusStrip.Items.Add(_envState);
    }

    // ------------------------------------------------------------- rozložení

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (File.Exists(AppSettings.LayoutPath))
        {
            try
            {
                _dock.LoadFromXml(AppSettings.LayoutPath, DeserializeContent);
            }
            catch
            {
                DefaultLayout();
            }
        }
        else
        {
            DefaultLayout();
        }

        EnsureVisible();
        UpdateTitle();
    }

    private IDockContent? DeserializeContent(string persistString)
    {
        if (persistString == typeof(BrowserWindow).FullName) return _browserWindow;
        if (persistString == typeof(DomTreeWindow).FullName) return _treeWindow;
        if (persistString == typeof(ElementWindow).FullName) return _elementWindow;
        if (persistString == typeof(TypeWindow).FullName) return _typeWindow;
        if (persistString == typeof(MappingWindow).FullName) return _mappingWindow;
        if (persistString == typeof(ScriptWindow).FullName) return _scriptWindow;
        if (persistString == typeof(IssuesWindow).FullName) return _issuesWindow;
        if (persistString == typeof(PageFlowWindow).FullName) return _pageFlowWindow;
        return null;
    }

    /// <summary>Rozložení jako ve Visual Studiu: dokument uprostřed, nástroje vpravo a dole.</summary>
    private void DefaultLayout()
    {
        _browserWindow.Show(_dock, DockState.Document);
        _scriptWindow.Show(_dock, DockState.Document);

        _treeWindow.Show(_dock, DockState.DockRight);
        _elementWindow.Show(_treeWindow.Pane, DockAlignment.Bottom, 0.55);
        _typeWindow.Show(_elementWindow.Pane, _elementWindow);

        _mappingWindow.Show(_dock, DockState.DockBottom);
        _issuesWindow.Show(_mappingWindow.Pane, _mappingWindow);
        _pageFlowWindow.Show(_mappingWindow.Pane, _mappingWindow);

        _browserWindow.Activate();
    }

    private void EnsureVisible()
    {
        foreach (var window in AllWindows())
            if (window.DockState == DockState.Unknown)
                window.Show(_dock, window is BrowserWindow or ScriptWindow
                    ? DockState.Document
                    : DockState.DockRight);
    }

    private IEnumerable<ToolWindowBase> AllWindows()
    {
        yield return _browserWindow;
        yield return _scriptWindow;
        yield return _treeWindow;
        yield return _elementWindow;
        yield return _typeWindow;
        yield return _mappingWindow;
        yield return _issuesWindow;
        yield return _pageFlowWindow;
    }

    private void SaveLayout()
    {
        try
        {
            Directory.CreateDirectory(AppSettings.AppDataDir);
            _dock.SaveAsXml(AppSettings.LayoutPath);
            _workspace.Status("Rozložení oken uloženo.");
        }
        catch (Exception ex)
        {
            _workspace.Status("Rozložení se nepodařilo uložit: " + ex.Message);
        }
    }

    private void ResetLayout()
    {
        foreach (var window in AllWindows().ToList())
            window.DockPanel = null;

        try
        {
            if (File.Exists(AppSettings.LayoutPath))
                File.Delete(AppSettings.LayoutPath);
        }
        catch
        {
            // soubor smazat nejde – nevadí, přepíše se při zavření
        }

        DefaultLayout();
        _workspace.Status("Obnoveno výchozí rozložení oken.");
    }

    private void ApplyTheme(string themeName, bool silent = false)
    {
        // Motivy VS2015 jsou v balíčku DockPanelSuite.ThemeVS2015; výchozí motiv
        // DockPanel Suite se nastavuje sám, proto se tu nevytváří.
        ThemeBase theme = themeName switch
        {
            "VS2015Light" => new VS2015LightTheme(),
            "VS2015Dark" => new VS2015DarkTheme(),
            _ => new VS2015BlueTheme()
        };

        _workspace.Settings.Theme = themeName;

        try
        {
            _dock.Theme = theme;
        }
        catch (Exception ex)
        {
            if (!silent)
                MessageBox.Show(this,
                    "Motiv se projeví po restartu aplikace.\r\n\r\n" + ex.Message,
                    "Motiv", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    // ---------------------------------------------------------------- akce

    private async Task NavigateAsync()
    {
        if (_workspace.Browser is null) return;
        await _workspace.Browser.NavigateAsync(_urlBox.Text);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.F2:
                _pickButton.Checked = !_pickButton.Checked;
                return true;

            case Keys.F4:
                _ = _workspace.RequestTreeAsync();
                return true;

            case Keys.F5:
                _typeWindow.Apply();
                return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// Barevné odlišení prostředí jako v aplikaci TPI – dev hnědý, preprod růžový,
    /// produkce modrá. Ať je na první pohled vidět, kam skripty míří.
    /// </summary>
    private void ShowEnvironment(TpiEnvironment environment)
    {
        (string text, Color back, Color fore) = environment switch
        {
            TpiEnvironment.Dev => ("DEV", Color.Sienna, Color.White),
            TpiEnvironment.Preprod => ("PREPROD", Color.Orchid, Color.Black),
            TpiEnvironment.Prod => ("PRODUKCE", Color.SteelBlue, Color.White),
            _ => ("Prostředí: neurčeno", SystemColors.Control, SystemColors.ControlText)
        };

        _envState.Text = text;
        _envState.BackColor = back;
        _envState.ForeColor = fore;
        _envState.Font = new Font(_envState.Font,
            environment == TpiEnvironment.Prod ? FontStyle.Bold : FontStyle.Regular);
    }

    /// <summary>Nastaví typ obrazovky v panelu nástrojů bez vyvolání zpětné vazby.</summary>
    private void SelectScreenKind(TpiScreenKind kind)
    {
        var index = (int)kind;
        if (index < 0 || index >= _screenKindBox.Items.Count) return;

        _suppress = true;
        try { _screenKindBox.SelectedIndex = index; }
        finally { _suppress = false; }
    }

    private void FillProfiles()
    {
        _suppress = true;
        try
        {
            _profileBox.Items.Clear();
            foreach (var profile in _workspace.Profiles.Profiles)
                _profileBox.Items.Add(profile);

            if (_profileBox.Items.Count > 0)
                _profileBox.SelectedIndex = 0;

            ShowEnvironment(_profileBox.SelectedItem is ConnectionProfile selected
                ? selected.Environment
                : TpiEnvironment.Unknown);
        }
        finally
        {
            _suppress = false;
        }
    }

    private void OpenMapping()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Mapování TPI (*.json)|*.json|Všechny soubory (*.*)|*.*",
            Title = "Otevřít mapování"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            _workspace.LoadDocument(dialog.FileName);
            _pfBox.Text = _workspace.PfName;

            SelectScreenKind(_workspace.ScreenKind);
            UpdateTitle();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Načtení mapování",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SaveMapping()
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "Mapování TPI (*.json)|*.json",
            Title = "Uložit mapování",
            FileName = string.IsNullOrWhiteSpace(_pfBox.Text) ? "mapovani.json" : _pfBox.Text + ".json"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        _workspace.PfName = _pfBox.Text;
        _workspace.Document.Url = _urlBox.Text;

        try
        {
            _workspace.SaveDocument(dialog.FileName);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Uložení mapování",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ShowLoginDialog()
    {
        var current = _profileBox.SelectedItem as ConnectionProfile;
        using var form = new LoginForm(_workspace.Profiles, current);
        if (form.ShowDialog(this) != DialogResult.OK) return;

        _workspace.Profiles.Save();
        FillProfiles();

        if (form.SelectedProfile is null) return;

        _suppress = true;
        _profileBox.SelectedItem = form.SelectedProfile;
        _suppress = false;

        if (!string.IsNullOrWhiteSpace(form.SelectedProfile.Url))
        {
            _urlBox.Text = form.SelectedProfile.Url;
            if (form.NavigateAfterSave)
                _ = NavigateAsync();
        }
    }

    private void ShowSettingsDialog()
    {
        using var form = new SettingsForm(_workspace.Settings);
        if (form.ShowDialog(this) != DialogResult.OK) return;

        _workspace.Settings.Save();
        _workspace.PathResolver.Options = _workspace.Settings.ElementPath;
        _workspace.Generator.Options = _workspace.Settings.Script;

        if (form.ReloadCatalog)
            _workspace.ReloadCatalog();
    }

    private void ShowAbout()
        => MessageBox.Show(this,
            "TPI Element Picker\r\n\r\n" +
            "Mapování prvků webové aplikace TPI a generování skriptů GTO.\r\n" +
            "Číselník typů a generátor jsou v knihovně TpiGto.\r\n\r\n" +
            "Číselník: " + AppSettings.CustomTypesPath + "\r\n" +
            "Rozložení oken: " + AppSettings.LayoutPath,
            "O aplikaci", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void UpdateTitle()
    {
        var count = _workspace.Document.Elements.Count;
        var pf = string.IsNullOrWhiteSpace(_workspace.PfName) ? "(bez PageFlow)" : _workspace.PfName;
        Text = $"TPI Element Picker – {pf} – {count} prvků";
    }

    private void OnUi(Action action)
    {
        if (IsDisposed) return;
        if (InvokeRequired) BeginInvoke(action);
        else action();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _workspace.Settings.LastUrl = _urlBox.Text;
        _workspace.SaveSettings();
        SaveLayout();
        base.OnFormClosing(e);
    }
}
