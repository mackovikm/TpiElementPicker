using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Element typu list.</summary>
public sealed class ListElementType : TpiElementType
{
    public override string Code => "LIST";

    public override string Name => "Seznam (List)";

    public override string Description => "Element typu list.";

    public override int SortOrder => 160;

    public override bool IncludesWidgetProperties => true;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "ul", "ol" },
        Roles = new[] { "list" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_List.M_Cis_Pf_List_Style_Type_Child",
            "Styl odrážek položek",
            GtoValueKind.Text,
            gmsgId: 10400825);
    }
}
