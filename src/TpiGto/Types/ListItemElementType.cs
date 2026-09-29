using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Jedna položka elementu list.</summary>
public sealed class ListItemElementType : TpiElementType
{
    public override string Code => "LIST_ITEM";

    public override string Name => "Položka seznamu (List item)";

    public override string Description => "Jedna položka elementu list.";

    public override int SortOrder => 161;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "li" },
        Roles = new[] { "listitem" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_List_Item.Value",
            "Hodnota položky",
            GtoValueKind.Text,
            gmsgId: 3800825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_List_Item.M_Cis_Pf_List_Style_Type_Child",
            "Styl odrážky položky",
            GtoValueKind.Text,
            gmsgId: 3900825);
    }
}
