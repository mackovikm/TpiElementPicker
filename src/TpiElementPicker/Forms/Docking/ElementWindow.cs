using TpiElementPicker.Models;
using TpiElementPicker.Workspace;
using TpiGto.Paths;
using WeifenLuo.WinFormsUI.Docking;

namespace TpiElementPicker.Forms.Docking;

/// <summary>
/// Detail kliknutého prvku – atributy, návrhy in_ref_element_path a nastavení
/// zdrojového atributu.
/// </summary>
public sealed class ElementWindow : ToolWindowBase
{
    private readonly TextBox _path = new();
    private readonly TextBox _suffix = new();
    private readonly ComboBox _candidates = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ListView _attributes = new()
    {
        Dock = DockStyle.Fill,
        View = View.Details,
        FullRowSelect = true,
        GridLines = true,
        MultiSelect = false
    };

    private bool _suppress;

    public ElementWindow(PickerWorkspace workspace) : base(workspace)
    {
        Text = "Prvek";
        TabText = "Prvek";
        Initialize();
    }

    protected override DockState DefaultDockState => DockState.DockRight;

    protected override void BuildUi()
    {
        var top = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            RowCount = 3,
            Height = 90,
            Padding = new Padding(6, 6, 6, 0)
        };
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        for (var i = 0; i < 3; i++)
            top.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));

        _path.Dock = DockStyle.Fill;
        _suffix.Dock = DockStyle.Fill;
        _candidates.Dock = DockStyle.Fill;

        top.Controls.Add(new Label { Text = "in_ref_element_path:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        top.Controls.Add(_path, 1, 0);
        top.Controls.Add(new Label { Text = "Sloupec / event:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        top.Controls.Add(_suffix, 1, 1);
        top.Controls.Add(new Label { Text = "Návrhy:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
        top.Controls.Add(_candidates, 1, 2);

        _attributes.Columns.Add("Atribut", 170);
        _attributes.Columns.Add("Hodnota", 420);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 38,
            Padding = new Padding(6, 6, 6, 6)
        };

        var btnUseValue = new Button { Text = "Vložit hodnotu do element_path", Width = 200, Height = 25 };
        btnUseValue.Click += (_, _) => UseSelectedAttributeValue();

        var btnRemember = new Button { Text = "Zapamatovat atribut jako zdroj", Width = 220, Height = 25 };
        btnRemember.Click += (_, _) => RememberSelectedAttribute();

        buttons.Controls.Add(btnUseValue);
        buttons.Controls.Add(btnRemember);

        Controls.Add(_attributes);
        Controls.Add(buttons);
        Controls.Add(top);

        _path.TextChanged += (_, _) =>
        {
            if (_suppress) return;
            Workspace.CurrentElementPath = _path.Text;
        };

        _suffix.TextChanged += (_, _) =>
        {
            if (_suppress) return;
            Workspace.CurrentSuffix = _suffix.Text;
        };

        _candidates.SelectedIndexChanged += (_, _) =>
        {
            if (_suppress) return;
            if (_candidates.SelectedItem is ElementPathCandidate candidate)
                _path.Text = candidate.Value;
        };

        _attributes.DoubleClick += (_, _) => UseSelectedAttributeValue();
    }

    protected override void Subscribe()
    {
        Workspace.ElementPicked += node => OnUi(() => ShowElement(node));

        Workspace.ElementPathSuggested += suggestion => OnUi(() =>
        {
            _suppress = true;
            try { _path.Text = suggestion; }
            finally { _suppress = false; }
        });
        Workspace.EditRequested += element => OnUi(() =>
        {
            _suppress = true;
            try
            {
                _path.Text = element.ElementPath;
                _suffix.Text = element.Column ?? element.EventCode ?? string.Empty;
                Workspace.CurrentElementPath = _path.Text;
                Workspace.CurrentSuffix = _suffix.Text;
            }
            finally
            {
                _suppress = false;
            }
        });
    }

    private void ShowElement(DomNode node)
    {
        _suppress = true;
        try
        {
            _attributes.BeginUpdate();
            _attributes.Items.Clear();

            AddRow("(tag)", node.Tag);
            if (!string.IsNullOrWhiteSpace(node.Text)) AddRow("(text)", node.Text!);
            foreach (var kv in node.Attrs.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
                AddRow(kv.Key, kv.Value);
            if (!string.IsNullOrWhiteSpace(node.Css)) AddRow("(css selector)", node.Css!);
            if (!string.IsNullOrWhiteSpace(node.XPath)) AddRow("(xpath)", node.XPath!);

            _attributes.EndUpdate();

            var candidates = Workspace.GetPathCandidates(node);
            _candidates.Items.Clear();
            foreach (var candidate in candidates)
                _candidates.Items.Add(candidate);

            if (candidates.Count > 0)
            {
                _candidates.SelectedIndex = 0;
                _path.Text = candidates[0].Value;
            }
            else
            {
                _path.Text = string.Empty;
            }

            _suffix.Text = string.Empty;

            Workspace.CurrentElementPath = _path.Text;
            Workspace.CurrentSuffix = string.Empty;
            Workspace.RememberDomIndex(_path.Text, node.Index);
        }
        finally
        {
            _suppress = false;
        }
    }

    private void AddRow(string name, string value)
    {
        var item = new ListViewItem(name);
        item.SubItems.Add(value);
        _attributes.Items.Add(item);
    }

    private void UseSelectedAttributeValue()
    {
        if (_attributes.SelectedItems.Count == 0)
        {
            Workspace.Status("Vyber atribut v seznamu.");
            return;
        }

        _path.Text = _attributes.SelectedItems[0].SubItems[1].Text.Trim();
        Workspace.Status("element_path nastaven z atributu.");
    }

    private void RememberSelectedAttribute()
    {
        if (_attributes.SelectedItems.Count == 0)
        {
            Workspace.Status("Vyber atribut, který nese název elementu (in_ref_element_path).");
            return;
        }

        var attribute = _attributes.SelectedItems[0].Text;
        if (attribute.StartsWith('('))
        {
            Workspace.Status("Tuto položku nelze použít jako zdroj – vyber skutečný atribut.");
            return;
        }

        Workspace.RememberPathAttribute(attribute);

        if (Workspace.CurrentNode is not null)
            ShowElement(Workspace.CurrentNode);
    }
}
