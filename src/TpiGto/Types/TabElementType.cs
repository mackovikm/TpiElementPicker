using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Hlavní element se záložkami, např. RelTabs.</summary>
public sealed class TabElementType : TpiElementType
{
    public override string Code => "TAB";

    public override string Name => "Záložkový kontejner (Tab)";

    public override string Description => "Hlavní element se záložkami, např. RelTabs.";

    public override int SortOrder => 30;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "ul", "div", "nav" },
        Roles = new[] { "tablist" },
        ClassHints = new[] { "tabs", "nav-tabs", "tabstrip" },
        NameHints = new[] { "Tabs" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Tab.Background_Color",
            "Pozadí hlavní záložky",
            GtoValueKind.Color);

        yield return new GtoPropertyDefinition(
            "M_Pf_Tab.Head_Color",
            "Barva hlavní záložky",
            GtoValueKind.Color);

        yield return new GtoPropertyDefinition(
            "M_Pf_Tab.Head_Text_Color",
            "Barva textu hlavní záložky",
            GtoValueKind.Color);

        yield return new GtoPropertyDefinition(
            "M_Pf_Tab.Head_Color_Active",
            "Barva hlavní aktivní záložky",
            GtoValueKind.Color);

        yield return new GtoPropertyDefinition(
            "M_Pf_Tab.Head_Text_Color_Active",
            "Barva textu hlavní aktivní záložky",
            GtoValueKind.Color);

        yield return new GtoPropertyDefinition(
            "M_Pf_Tab.Active_Item",
            "Index aktivní záložky (viz makra @GET_TAB_ITEM_INDX_4_TABLE) – zdroj: makra.docx",
            GtoValueKind.Sql, example: "@GET_TAB_ITEM_INDX_4_TABLE('PZSV_RYCHLOST')");
    }
}
