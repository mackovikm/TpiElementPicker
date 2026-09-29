using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Jedna záložka. Do cesty se přidává M_PF_DT_TAB_ITEM.NAME, typicky název child tabulky.</summary>
public sealed class TabItemElementType : TpiElementType
{
    public override string Code => "TAB_ITEM";

    public override string Name => "Záložka (Tab item)";

    public override string Description => "Jedna záložka. Do cesty se přidává M_PF_DT_TAB_ITEM.NAME, typicky název child tabulky.";

    public override int SortOrder => 40;

    public override string? PathHint => "<element tabů>.<NÁZEV_ZÁLOŽKY>, např. RelTabs.POLOZKY_FA";

    public override bool RequiresSubElement => true;

    public override string SubElementLabel => "název záložky (M_Pf_Dt_Tab_Item.Name)";

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "li", "a", "button" },
        Roles = new[] { "tab" },
        ClassHints = new[] { "tab-item", "nav-item", "tabitem" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Label",
            "Název záložky",
            GtoValueKind.Text,
            example: "Položky Faktúry", gmsgId: 800825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Left_To_Right",
            "Směr zleva doprava 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 700825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Background_Color",
            "Barva pozadí záložky",
            GtoValueKind.Color,
            gmsgId: 600825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Head_Color",
            "Barva záložky",
            GtoValueKind.Color,
            gmsgId: 500825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Head_Text_Color",
            "Barva textu záložky",
            GtoValueKind.Color,
            gmsgId: 400825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Head_Color_Active",
            "Barva aktivní záložky",
            GtoValueKind.Color,
            gmsgId: 300825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Head_Text_Color_Active",
            "Barva textu aktivní záložky",
            GtoValueKind.Color,
            gmsgId: 200825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Indx",
            "Pořadí záložky – doloženo příkladem v Gattrib_Overload.docx",
            GtoValueKind.Number,
            example: "2", source: GtoPropertySource.Documentation);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Visible",
            "Zobrazení záložky 0/1",
            GtoValueKind.Bool01,
            example: "0", source: GtoPropertySource.CisGto);
    }
}
