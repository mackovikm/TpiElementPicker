using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Jeden uzel stromu.</summary>
public sealed class TreeNodeElementType : TpiElementType
{
    public override string Code => "TREE_NODE";

    public override string Name => "Uzel stromu (Tree node)";

    public override string Description => "Jeden uzel stromu.";

    public override int SortOrder => 156;

    public override ElementMatchRule MatchRule => new()
    {
        Roles = new[] { "treeitem" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tree_Node.Text",
            "Text uzlu",
            GtoValueKind.Text,
            gmsgId: 9700825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tree_Node.Visible",
            "Zobrazení uzlu 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 9800825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tree_Node.Selectable",
            "Uzel lze vybrat 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 9900825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tree_Node.Marked",
            "Označení uzlu 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 10000825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tree_Node.Expanded",
            "Rozbalení uzlu 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 10100825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tree_Node.Disabled",
            "Zakázaný uzel 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 10200825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tree_Node.Checked",
            "Zaškrtnutí uzlu 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 10300825);
    }
}
