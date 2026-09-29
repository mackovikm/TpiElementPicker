using TpiElementPicker.Models;
using TpiElementPicker.Workspace;
using WeifenLuo.WinFormsUI.Docking;

namespace TpiElementPicker.Forms.Docking;

/// <summary>Strom DOM aktuální stránky (obdoba Solution Exploreru).</summary>
public sealed class DomTreeWindow : ToolWindowBase
{
    private readonly TreeView _tree = new() { Dock = DockStyle.Fill, HideSelection = false };
    private readonly TextBox _filter = new() { Dock = DockStyle.Top, PlaceholderText = "Filtr (tag, id, text)…" };
    private readonly Dictionary<int, TreeNode> _byIndex = new();

    private DomNode? _root;
    private bool _suppress;

    public DomTreeWindow(PickerWorkspace workspace) : base(workspace)
    {
        Text = "DOM strom";
        TabText = "DOM strom";
        Initialize();
    }

    protected override DockState DefaultDockState => DockState.DockRight;

    protected override void BuildUi()
    {
        Controls.Add(_tree);
        Controls.Add(_filter);

        _tree.AfterSelect += async (_, e) =>
        {
            if (_suppress) return;
            if (e.Node?.Tag is DomNode node)
                await Workspace.HighlightAsync(node);
        };

        _filter.TextChanged += (_, _) => ApplyFilter();
    }

    protected override void Subscribe()
    {
        Workspace.TreeLoaded += root => OnUi(() => Rebuild(root));
        Workspace.ElementPicked += node => OnUi(() => SelectByIndex(node.Index));
    }

    private void Rebuild(DomNode root)
    {
        _root = root;
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (_root is null) return;

        var filter = _filter.Text.Trim();

        _tree.BeginUpdate();
        try
        {
            _tree.Nodes.Clear();
            _byIndex.Clear();

            var rootNode = CreateNode(_root, filter);
            if (rootNode is not null)
            {
                _tree.Nodes.Add(rootNode);
                rootNode.Expand();
                if (!string.IsNullOrEmpty(filter))
                    rootNode.ExpandAll();
            }
        }
        finally
        {
            _tree.EndUpdate();
        }
    }

    private TreeNode? CreateNode(DomNode node, string filter)
    {
        var children = new List<TreeNode>();
        foreach (var child in node.Children)
        {
            var created = CreateNode(child, filter);
            if (created is not null)
                children.Add(created);
        }

        var matches = string.IsNullOrEmpty(filter) || Matches(node, filter);
        if (!matches && children.Count == 0)
            return null;

        var treeNode = new TreeNode(node.Caption()) { Tag = node };
        treeNode.Nodes.AddRange(children.ToArray());

        if (node.Index >= 0)
            _byIndex[node.Index] = treeNode;

        if (matches && !string.IsNullOrEmpty(filter))
            treeNode.BackColor = Color.FromArgb(255, 248, 196);

        return treeNode;
    }

    private static bool Matches(DomNode node, string filter)
        => node.Caption().Contains(filter, StringComparison.OrdinalIgnoreCase);

    private void SelectByIndex(int index)
    {
        if (index < 0 || !_byIndex.TryGetValue(index, out var treeNode)) return;

        _suppress = true;
        try
        {
            _tree.SelectedNode = treeNode;
            treeNode.EnsureVisible();
        }
        finally
        {
            _suppress = false;
        }
    }
}
