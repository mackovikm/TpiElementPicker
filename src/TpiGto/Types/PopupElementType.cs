using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Vyskakovací okno. Popup lze přes GTO i nově vytvořit (in_element_typ = popup).</summary>
public sealed class PopupElementType : TpiElementType
{
    public override string Code => "POPUP";

    public override string Name => "Popup";

    public override string Description => "Vyskakovací okno. Popup lze přes GTO i nově vytvořit (in_element_typ = popup).";

    public override int SortOrder => 25;

    public override bool IncludesWidgetProperties => true;

    public override ElementMatchRule MatchRule => new()
    {
        Roles = new[] { "dialog" },
        ClassHints = new[] { "popup", "modal", "dialog" },
        NameHints = new[] { "Confirm", "Popup" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Popup.Title",
            "Titulek popupu",
            GtoValueKind.Text,
            gmsgId: 4000825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Popup.Open",
            "Otevření popupu 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 4100825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Popup.Modal",
            "Modální okno 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 4300825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Popup.Resizable",
            "Lze měnit velikost 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 4200825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Popup.Draggable",
            "Lze přesouvat 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 4400825);
    }
}
