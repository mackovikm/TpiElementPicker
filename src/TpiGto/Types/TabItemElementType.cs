using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Jedna záložka uvnitř záložkového kontejneru.</summary>
public sealed class TabItemElementType : TpiElementType
{
    public override string Code => "TAB_ITEM";

    public override string Name => "Záložka (Tab item)";

    public override string Description => "Jedna záložka uvnitř záložkového kontejneru.";

    public override int SortOrder => 40;

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
            "Definice názvu záložky",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Indx",
            "Pořadí zobrazení záložky",
            GtoValueKind.Number);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Visible",
            "Zobrazení záložky 0/1",
            GtoValueKind.Bool01, example: "0");

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Background_Color",
            "Barva pozadí záložky",
            GtoValueKind.Color);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Head_Color",
            "Barva záložky",
            GtoValueKind.Color);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Head_Text_Color",
            "Barva textu záložky",
            GtoValueKind.Color);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Head_Color_Active",
            "Barva aktivní záložky",
            GtoValueKind.Color);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Tab_Item.Head_Text_Color_Active",
            "Barva textu aktivní záložky",
            GtoValueKind.Color);
    }
}
