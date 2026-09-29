using TpiElementPicker.Workspace;
using TpiGto.Mapping;
using TpiGto.Scripting;
using WeifenLuo.WinFormsUI.Docking;

namespace TpiElementPicker.Forms.Docking;

/// <summary>Kontroly mapování – obdoba Error Listu ve Visual Studiu.</summary>
public sealed class IssuesWindow : ToolWindowBase
{
    private readonly ListView _list = new()
    {
        Dock = DockStyle.Fill,
        View = View.Details,
        FullRowSelect = true,
        GridLines = true,
        MultiSelect = false
    };

    public IssuesWindow(PickerWorkspace workspace) : base(workspace)
    {
        Text = "Kontroly";
        TabText = "Kontroly";
        Initialize();
    }

    protected override DockState DefaultDockState => DockState.DockBottom;

    protected override void BuildUi()
    {
        _list.Columns.Add("Závažnost", 90);
        _list.Columns.Add("element_path", 280);
        _list.Columns.Add("Popis", 520);

        Controls.Add(_list);

        _list.DoubleClick += (_, _) =>
        {
            if (_list.SelectedItems.Count == 0) return;
            var path = _list.SelectedItems[0].SubItems[1].Text;

            var element = Workspace.Document.Elements.FirstOrDefault(e =>
                string.Equals(e.EffectivePath(), path, StringComparison.OrdinalIgnoreCase));

            if (element is not null)
                Workspace.RequestEdit(element);
        };
    }

    protected override void Subscribe()
    {
        Workspace.IssuesChanged += issues => OnUi(() => ShowIssues(issues));
    }

    private void ShowIssues(IReadOnlyList<GtoValidationIssue> issues)
    {
        _list.BeginUpdate();
        try
        {
            _list.Items.Clear();
            foreach (var issue in issues)
            {
                var item = new ListViewItem(issue.Severity) { Tag = issue };
                item.SubItems.Add(issue.ElementPath);
                item.SubItems.Add(issue.Message);
                item.ForeColor = issue.Severity switch
                {
                    GtoValidationIssue.Error => Color.Firebrick,
                    GtoValidationIssue.Warning => Color.DarkGoldenrod,
                    _ => Color.SteelBlue
                };
                _list.Items.Add(item);
            }
        }
        finally
        {
            _list.EndUpdate();
        }

        var errors = issues.Count(i => i.Severity == GtoValidationIssue.Error);
        TabText = issues.Count == 0
            ? "Kontroly"
            : $"Kontroly ({issues.Count}{(errors > 0 ? $", chyb: {errors}" : "")})";

        if (issues.Count > 0)
            Activate();
    }
}
