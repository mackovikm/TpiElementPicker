using TpiElementPicker.Workspace;
using TpiGto.Mapping;
using WeifenLuo.WinFormsUI.Docking;

namespace TpiElementPicker.Forms.Docking;

/// <summary>Seznam namapovaných prvků – obdoba okna Solution / Task List.</summary>
public sealed class MappingWindow : ToolWindowBase
{
    private readonly ListView _list = new()
    {
        Dock = DockStyle.Fill,
        View = View.Details,
        FullRowSelect = true,
        GridLines = true,
        MultiSelect = false
    };

    public MappingWindow(PickerWorkspace workspace) : base(workspace)
    {
        Text = "Mapování";
        TabText = "Mapování";
        Initialize();
    }

    protected override DockState DefaultDockState => DockState.DockBottom;

    protected override void BuildUi()
    {
        _list.Columns.Add("element_path", 300);
        _list.Columns.Add("Typ", 130);
        _list.Columns.Add("Vlastností", 80, HorizontalAlignment.Right);
        _list.Columns.Add("Popis", 260);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 38, Padding = new Padding(6) };

        var btnEdit = new Button { Text = "Upravit", Width = 110, Height = 25 };
        btnEdit.Click += (_, _) => EditSelected();

        var btnLocate = new Button { Text = "Najít na stránce", Width = 140, Height = 25 };
        btnLocate.Click += async (_, _) => await LocateSelectedAsync();

        var btnDelete = new Button { Text = "Odebrat", Width = 110, Height = 25 };
        btnDelete.Click += (_, _) => DeleteSelected();

        buttons.Controls.AddRange(new Control[] { btnEdit, btnLocate, btnDelete });

        Controls.Add(_list);
        Controls.Add(buttons);

        _list.DoubleClick += (_, _) => EditSelected();

        var menu = new ContextMenuStrip();
        menu.Items.Add("Upravit", null, (_, _) => EditSelected());
        menu.Items.Add("Najít na stránce", null, async (_, _) => await LocateSelectedAsync());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Odebrat", null, (_, _) => DeleteSelected());
        _list.ContextMenuStrip = menu;
    }

    protected override void Subscribe()
    {
        Workspace.MappingChanged += () => OnUi(ReloadList);
    }

    private MappedElement? Selected
        => _list.SelectedItems.Count > 0 ? _list.SelectedItems[0].Tag as MappedElement : null;

    /// <summary>Načte seznam podle aktuálního mapování.</summary>
    public void ReloadList()
    {
        _list.BeginUpdate();
        try
        {
            _list.Items.Clear();
            foreach (var element in Workspace.Document.Elements)
            {
                var item = new ListViewItem(element.EffectivePath()) { Tag = element };
                item.SubItems.Add(element.TypeCode);
                item.SubItems.Add(element.Assignments.Count(a => a.Enabled).ToString());
                item.SubItems.Add(element.Label ?? string.Empty);
                _list.Items.Add(item);
            }
        }
        finally
        {
            _list.EndUpdate();
        }

        TabText = Workspace.Document.Elements.Count > 0
            ? $"Mapování ({Workspace.Document.Elements.Count})"
            : "Mapování";
    }

    private void EditSelected()
    {
        var element = Selected;
        if (element is null) return;
        Workspace.RequestEdit(element);
    }

    private void DeleteSelected()
    {
        var element = Selected;
        if (element is null) return;
        Workspace.RemoveMapping(element);
    }

    private async Task LocateSelectedAsync()
    {
        var element = Selected;
        if (element is null) return;

        var found = await Workspace.LocateAsync(element);
        Workspace.Status(found
            ? "Prvek nalezen na stránce."
            : "Prvek se na aktuální stránce nenašel – načti DOM strom znovu.");
    }
}
