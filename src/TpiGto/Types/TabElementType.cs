using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Element se záložkami, typicky RelTabs.</summary>
public sealed class TabElementType : TpiElementType
{
    public override string Code => "TAB";

    public override string Name => "Záložkový kontejner (Tab)";

    public override string Description => "Element se záložkami, typicky RelTabs.";

    public override int SortOrder => 30;

    public override string? PathHint => "RelTabs";

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "ul", "div", "nav" },
        Roles = new[] { "tablist" },
        ClassHints = new[] { "tabs", "nav-tabs", "tabstrip" },
        NameHints = new[] { "RelTabs", "Tabs" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Tab.Active_Item",
            "Index aktivní záložky – jde dosadit i makrem @GET_TAB_ITEM_INDX_4_TABLE",
            GtoValueKind.Sql,
            example: "@GET_TAB_ITEM_INDX_4_TABLE('PZSV_RYCHLOST')", gmsgId: 8400825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Tab.Background_Color",
            "Pozadí hlavní záložky",
            GtoValueKind.Color,
            gmsgId: 8900825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Tab.Head_Color",
            "Barva hlavní záložky",
            GtoValueKind.Color,
            gmsgId: 8800825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Tab.Head_Text_Color",
            "Barva textu hlavní záložky",
            GtoValueKind.Color,
            gmsgId: 8700825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Tab.Head_Color_Active",
            "Barva aktivní záložky",
            GtoValueKind.Color,
            gmsgId: 8600825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Tab.Head_Text_Color_Active",
            "Barva textu aktivní záložky",
            GtoValueKind.Color,
            gmsgId: 8500825);
    }
}
