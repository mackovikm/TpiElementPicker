using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Stromový element. Podporuje event right_click.</summary>
public sealed class TreeElementType : TpiElementType
{
    public override string Code => "TREE";

    public override string Name => "Strom (Tree)";

    public override string Description => "Stromový element. Podporuje event right_click.";

    public override int SortOrder => 155;

    public override bool IncludesWidgetProperties => true;

    public override ElementMatchRule MatchRule => new()
    {
        Roles = new[] { "tree" },
        ClassHints = new[] { "tree", "treeview" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Tree.Multi_Select",
            "Vícenásobný výběr 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 5900825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Tree.Show_Border",
            "Zobrazení rámečku 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 5800825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Tree.Show_Checkbox",
            "Zobrazení checkboxů 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 5700825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Tree.Show_Icon",
            "Zobrazení ikon 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 5600825);
    }
}
