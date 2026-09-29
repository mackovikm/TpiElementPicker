using System.Text;
using TpiElementPicker.Workspace;
using TpiGto.Mapping;
using WeifenLuo.WinFormsUI.Docking;

namespace TpiElementPicker.Forms.Docking;

/// <summary>Vygenerovaný skript GTO – dokument vedle webové aplikace.</summary>
public sealed class ScriptWindow : ToolWindowBase
{
    private readonly TextBox _script = new()
    {
        Dock = DockStyle.Fill,
        Multiline = true,
        ScrollBars = ScrollBars.Both,
        WordWrap = false,
        Font = new Font("Consolas", 9.75F)
    };

    private readonly CheckBox _wrapMwf = new()
    {
        Text = "Obalit dynamické GTO kostrou MWF",
        AutoSize = true
    };

    public ScriptWindow(PickerWorkspace workspace) : base(workspace)
    {
        Text = "Skript GTO";
        TabText = "Skript GTO";
        Initialize();
    }

    protected override DockState DefaultDockState => DockState.Document;

    protected override void BuildUi()
    {
        var toolbar = new ToolStrip { GripStyle = ToolStripGripStyle.Hidden };

        var btnStatic = new ToolStripButton("Statické GTO") { DisplayStyle = ToolStripItemDisplayStyle.Text };
        btnStatic.Click += (_, _) => Generate(GtoMode.Static);

        var btnDynamic = new ToolStripButton("Dynamické GTO") { DisplayStyle = ToolStripItemDisplayStyle.Text };
        btnDynamic.Click += (_, _) => Generate(GtoMode.Dynamic);

        var btnAll = new ToolStripButton("Vše") { DisplayStyle = ToolStripItemDisplayStyle.Text };
        btnAll.Click += (_, _) => Generate(null);

        var btnCopy = new ToolStripButton("Kopírovat") { DisplayStyle = ToolStripItemDisplayStyle.Text };
        btnCopy.Click += (_, _) => Copy();

        var btnSave = new ToolStripButton("Uložit .sql") { DisplayStyle = ToolStripItemDisplayStyle.Text };
        btnSave.Click += (_, _) => Save();

        var host = new ToolStripControlHost(_wrapMwf);

        toolbar.Items.AddRange(new ToolStripItem[]
        {
            btnStatic, btnDynamic, btnAll, new ToolStripSeparator(),
            btnCopy, btnSave, new ToolStripSeparator(), host
        });

        Controls.Add(_script);
        Controls.Add(toolbar);

        _wrapMwf.Checked = Workspace.Settings.Script.WrapDynamicInMwf;
        _wrapMwf.CheckedChanged += (_, _) =>
            Workspace.Settings.Script.WrapDynamicInMwf = _wrapMwf.Checked;
    }

    protected override void Subscribe()
    {
        Workspace.ScriptGenerated += script => OnUi(() =>
        {
            _script.Text = script;
            Activate();
        });
    }

    /// <summary>Vygeneruje skript; mode = null znamená statické i dynamické GTO.</summary>
    public void Generate(GtoMode? mode) => Workspace.Generate(mode);

    public void Copy()
    {
        if (string.IsNullOrWhiteSpace(_script.Text)) return;
        Clipboard.SetText(_script.Text);
        Workspace.Status("Skript zkopírován do schránky.");
    }

    public void Save()
    {
        if (string.IsNullOrWhiteSpace(_script.Text))
        {
            Workspace.Status("Není co ukládat – nejdřív vygeneruj skript.");
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Filter = "SQL skript (*.sql)|*.sql|Textový soubor (*.txt)|*.txt",
            FileName = (string.IsNullOrWhiteSpace(Workspace.PfName) ? "gto" : Workspace.PfName) + ".sql"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        File.WriteAllText(dialog.FileName, _script.Text, new UTF8Encoding(false));
        Workspace.Status("Skript uložen: " + Path.GetFileName(dialog.FileName));
    }
}
