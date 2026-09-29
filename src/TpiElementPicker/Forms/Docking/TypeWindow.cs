using System.Text;
using TpiElementPicker.Models;
using TpiElementPicker.Workspace;
using TpiGto.Mapping;
using TpiGto.Model;
using WeifenLuo.WinFormsUI.Docking;

namespace TpiElementPicker.Forms.Docking;

/// <summary>
/// Přiřazení typu prvku TPI a vyplnění vlastností GTO – obdoba okna Properties
/// ve Visual Studiu.
/// </summary>
public sealed class TypeWindow : ToolWindowBase
{
    private readonly ComboBox _type = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _event = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Label _info = new() { Dock = DockStyle.Fill, AutoEllipsis = true };
    private readonly DataGridView _grid = new()
    {
        Dock = DockStyle.Fill,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        RowHeadersVisible = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        EditMode = DataGridViewEditMode.EditOnEnter
    };

    private const string ModeStatic = "Statické";
    private const string ModeDynamic = "Dynamické";

    private bool _suppress;

    public TypeWindow(PickerWorkspace workspace) : base(workspace)
    {
        Text = "Typ a vlastnosti";
        TabText = "Typ a vlastnosti";
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
            Height = 92,
            Padding = new Padding(6, 6, 6, 0)
        };
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        top.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
        top.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
        top.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        _type.Dock = DockStyle.Fill;
        _event.Dock = DockStyle.Fill;

        top.Controls.Add(new Label { Text = "Typ prvku:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        top.Controls.Add(_type, 1, 0);
        top.Controls.Add(new Label { Text = "Event:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        top.Controls.Add(_event, 1, 1);
        top.Controls.Add(_info, 1, 2);

        BuildGridColumns();

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 38, Padding = new Padding(6) };

        var btnApply = new Button { Text = "Uložit prvek do mapování (F5)", Width = 220, Height = 25 };
        btnApply.Click += (_, _) => Apply();

        var btnClear = new Button { Text = "Vyprázdnit hodnoty", Width = 150, Height = 25 };
        btnClear.Click += (_, _) => ClearValues();

        buttons.Controls.Add(btnApply);
        buttons.Controls.Add(btnClear);

        Controls.Add(_grid);
        Controls.Add(buttons);
        Controls.Add(top);

        // Nejdřív události – FillTypes už s comboboxem událostí pracuje.
        FillEvents();
        FillTypes();

        _type.SelectedIndexChanged += (_, _) => OnTypeChanged();
        _event.SelectedIndexChanged += (_, _) =>
        {
            if (_suppress) return;
            if (_event.SelectedItem is TpiEvent tpiEvent)
                Workspace.CurrentSuffix = tpiEvent.Code;
        };
    }

    private void BuildGridColumns()
    {
        _grid.Columns.Clear();

        _grid.Columns.Add(new DataGridViewCheckBoxColumn
        {
            HeaderText = "✔", Name = "colEnabled", FillWeight = 20F
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Vlastnost (in_gattrib_overload_name)",
            Name = "colProperty", ReadOnly = true, FillWeight = 130F
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Hodnota (in_new_value)", Name = "colValue", FillWeight = 100F
        });

        var mode = new DataGridViewComboBoxColumn
        {
            HeaderText = "Režim", Name = "colMode", FillWeight = 45F
        };
        mode.Items.AddRange(ModeStatic, ModeDynamic);
        _grid.Columns.Add(mode);

        _grid.Columns.Add(new DataGridViewCheckBoxColumn
        {
            HeaderText = "Zrušit", Name = "colCancelled", FillWeight = 30F
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Kategorie", Name = "colCategory", ReadOnly = true, FillWeight = 45F,
            ToolTipText = "M_Pf_Dt_* = data/text, M_Pf_* = vzhled, M_Pf_Element.* = obecné, M_Pf_Connection.* = události"
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Popis z číselníku", Name = "colDescription", ReadOnly = true, FillWeight = 160F
        });
    }

    protected override void Subscribe()
    {
        Workspace.ElementPicked += node => OnUi(() => OnElementPicked(node));
        Workspace.EditRequested += element => OnUi(() => LoadFromMapping(element));
        Workspace.CatalogReloaded += () => OnUi(FillTypes);
    }

    // ------------------------------------------------------------------ typy

    private void FillTypes()
    {
        _suppress = true;
        try
        {
            _type.Items.Clear();
            foreach (var type in Workspace.Registry.Types)
                _type.Items.Add(type);

            if (_type.Items.Count > 0 && _type.SelectedIndex < 0)
                _type.SelectedIndex = 0;
        }
        finally
        {
            _suppress = false;
        }

        OnTypeChanged();
    }

    private void FillEvents()
    {
        _event.Items.Clear();
        _event.Items.Add("(žádný)");
        foreach (var tpiEvent in TpiEvents.All)
            _event.Items.Add(tpiEvent);
        _event.SelectedIndex = 0;
        _event.Enabled = false;
    }

    private TpiElementType? SelectedType => _type.SelectedItem as TpiElementType;

    private void OnTypeChanged()
    {
        var type = SelectedType;
        if (type is null) return;

        _event.Enabled = type.AppendsEvent;
        if (!type.AppendsEvent && _event.Items.Count > 0 && _event.SelectedIndex != 0)
            _event.SelectedIndex = 0;

        var info = new StringBuilder(type.Description);
        if (!string.IsNullOrWhiteSpace(type.PathHint))
            info.Append("   |   element_path: ").Append(type.PathHint);
        if (type.RequiresColumn)
            info.Append("   |   nutný sloupec (fyzický název v DB)");
        if (!string.Equals(type.Origin, "built-in", StringComparison.OrdinalIgnoreCase))
            info.Append("   |   zdroj: ").Append(type.Origin);
        _info.Text = info.ToString();

        if (!_suppress)
        {
            LoadProperties(type, null);
            Workspace.SuggestPathForType(type);
        }
    }

    private void OnElementPicked(DomNode node)
    {
        var suggestions = Workspace.Matcher.Suggest(node);
        var best = suggestions.FirstOrDefault()?.Type ?? Workspace.Registry.Find("ELEMENT");

        SelectType(best);

        var existing = Workspace.FindMapping(Workspace.CurrentElementPath);
        if (existing is not null)
        {
            LoadFromMapping(existing);
            Workspace.Status("Prvek už v mapování je – načteny jeho hodnoty.");
            return;
        }

        if (SelectedType is not null)
            LoadProperties(SelectedType, null);

        _info.Text = suggestions.Count > 0
            ? "Návrh podle prvku: " + string.Join(", ", suggestions.Take(3).Select(s => s.Type.Name))
            : "Pro prvek není žádný návrh typu – vyber ručně.";
    }

    private void SelectType(TpiElementType? type)
    {
        if (type is null) return;

        _suppress = true;
        try
        {
            for (var i = 0; i < _type.Items.Count; i++)
                if (ReferenceEquals(_type.Items[i], type))
                {
                    _type.SelectedIndex = i;
                    break;
                }
        }
        finally
        {
            _suppress = false;
        }

        OnTypeChanged();
    }

    // ------------------------------------------------------------ vlastnosti

    private void LoadProperties(TpiElementType type, MappedElement? existing)
    {
        _grid.Rows.Clear();

        foreach (var property in Workspace.Registry.GetProperties(type))
        {
            var assignment = existing?.Assignments.FirstOrDefault(a =>
                string.Equals(a.PropertyName, property.Name, StringComparison.OrdinalIgnoreCase));

            var index = _grid.Rows.Add(
                assignment?.Enabled ?? false,
                property.Name,
                assignment?.Value ?? string.Empty,
                assignment is null || assignment.Mode == GtoMode.Static ? ModeStatic : ModeDynamic,
                assignment?.Cancelled == 1,
                property.Category,
                property.Description);

            var row = _grid.Rows[index];
            row.Tag = property;

            if (!string.IsNullOrWhiteSpace(property.Example))
                row.Cells[2].ToolTipText = "Příklad: " + property.Example;
        }
    }

    private void LoadFromMapping(MappedElement element)
    {
        var type = Workspace.Registry.Find(element.TypeCode);
        if (type is null)
        {
            Workspace.Status($"Typ '{element.TypeCode}' není v číselníku.");
            return;
        }

        SelectType(type);

        if (!string.IsNullOrWhiteSpace(element.EventCode))
        {
            _suppress = true;
            try
            {
                for (var i = 0; i < _event.Items.Count; i++)
                    if (_event.Items[i] is TpiEvent e &&
                        string.Equals(e.Code, element.EventCode, StringComparison.OrdinalIgnoreCase))
                    {
                        _event.SelectedIndex = i;
                        break;
                    }
            }
            finally
            {
                _suppress = false;
            }
        }

        LoadProperties(type, element);
        Activate();
    }

    private void ClearValues()
    {
        foreach (DataGridViewRow row in _grid.Rows)
        {
            row.Cells[0].Value = false;
            row.Cells[2].Value = string.Empty;
            row.Cells[4].Value = false;
        }
    }

    /// <summary>Uloží aktuální prvek do mapování (volá se i z hlavního okna přes F5).</summary>
    public void Apply()
    {
        var type = SelectedType;
        if (type is null)
        {
            Workspace.Status("Nejdřív vyber typ prvku.");
            return;
        }

        if (string.IsNullOrWhiteSpace(Workspace.CurrentElementPath))
        {
            MessageBox.Show("Vyplň in_ref_element_path v okně Prvek.", "Mapování",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _grid.EndEdit();

        var assignments = new List<GtoAssignment>();

        foreach (DataGridViewRow row in _grid.Rows)
        {
            if (row.Tag is not GtoPropertyDefinition property) continue;

            var enabled = row.Cells[0].Value is true;
            var value = Convert.ToString(row.Cells[2].Value) ?? string.Empty;
            var cancelled = row.Cells[4].Value is true;

            if (!enabled && string.IsNullOrWhiteSpace(value) && !cancelled)
                continue;

            assignments.Add(new GtoAssignment
            {
                PropertyName = property.Name,
                Value = value,
                Mode = Convert.ToString(row.Cells[3].Value) == ModeDynamic ? GtoMode.Dynamic : GtoMode.Static,
                Cancelled = cancelled ? 1 : 0,
                Enabled = enabled || cancelled
            });
        }

        var eventCode = _event.SelectedItem as TpiEvent;
        Workspace.ApplyMapping(type, eventCode?.Code, assignments);
    }
}
