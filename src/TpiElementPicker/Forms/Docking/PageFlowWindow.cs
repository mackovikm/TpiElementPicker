using TpiElementPicker.Workspace;
using TpiGto.Naming;
using WeifenLuo.WinFormsUI.Docking;

namespace TpiElementPicker.Forms.Docking;

/// <summary>
/// Názvy page flow zachycené v síťové komunikaci. Nahrazuje ruční hledání
/// v konzoli prohlížeče – stačí na stránce provést akci a název se objeví tady.
/// </summary>
public sealed class PageFlowWindow : ToolWindowBase
{
    private readonly ListView _list = new()
    {
        Dock = DockStyle.Fill,
        View = View.Details,
        FullRowSelect = true,
        GridLines = true,
        MultiSelect = false
    };

    private readonly Label _hint = new()
    {
        Dock = DockStyle.Top,
        Height = 34,
        Padding = new Padding(6, 6, 6, 0),
        Text = "Proveď na stránce akci (otevření záložky, filtr…) – název page flow se objeví v seznamu."
    };

    public PageFlowWindow(PickerWorkspace workspace) : base(workspace)
    {
        Text = "PageFlow";
        TabText = "PageFlow";
        Initialize();
    }

    protected override DockState DefaultDockState => DockState.DockBottom;

    protected override void BuildUi()
    {
        _list.Columns.Add("Název page flow", 280);
        _list.Columns.Add("Typ obrazovky", 110);
        _list.Columns.Add("Tabulka v DB", 220);
        _list.Columns.Add("Zdroj", 300);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 38, Padding = new Padding(6) };

        var btnUse = new Button { Text = "Použít jako PageFlow", Width = 170, Height = 25 };
        btnUse.Click += (_, _) => UseSelected();

        var btnCopy = new Button { Text = "Kopírovat", Width = 110, Height = 25 };
        btnCopy.Click += (_, _) =>
        {
            if (_list.SelectedItems.Count > 0)
                Clipboard.SetText(_list.SelectedItems[0].Text);
        };

        var btnClear = new Button { Text = "Vyčistit seznam", Width = 130, Height = 25 };
        btnClear.Click += (_, _) => _list.Items.Clear();

        buttons.Controls.AddRange(new Control[] { btnUse, btnCopy, btnClear });

        Controls.Add(_list);
        Controls.Add(buttons);
        Controls.Add(_hint);

        _list.DoubleClick += (_, _) => UseSelected();
    }

    protected override void Subscribe()
    {
        Workspace.PageFlowCandidateFound += (value, source) => OnUi(() => Add(value, source));
    }

    private void Add(string value, string source)
    {
        var kind = TpiNaming.ScreenKindFromPageFlow(value) switch
        {
            TpiGto.Model.TpiScreenKind.List => "seznam",
            TpiGto.Model.TpiScreenKind.Detail => "detail",
            _ => string.Empty
        };

        var item = new ListViewItem(value);
        item.SubItems.Add(kind);
        item.SubItems.Add(TpiNaming.TableNameFromPageFlow(value) ?? string.Empty);
        item.SubItems.Add(source);
        _list.Items.Add(item);

        TabText = $"PageFlow ({_list.Items.Count})";

        if (_list.Items.Count == 1)
        {
            item.Selected = true;
            Activate();
        }
    }

    private void UseSelected()
    {
        if (_list.SelectedItems.Count == 0)
        {
            Workspace.Status("Vyber v seznamu název page flow.");
            return;
        }

        var value = _list.SelectedItems[0].Text;
        Workspace.SetPfName(value, autoDetectScreenKind: true);
        Workspace.Status($"PageFlow nastaven na {value}.");
    }
}
