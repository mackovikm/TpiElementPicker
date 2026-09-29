using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Tlačítko na obrazovce. Vlastnímu tlačítku lze nastavit Before/MWF/After.</summary>
public sealed class ButtonElementType : TpiElementType
{
    public override string Code => "BUTTON";

    public override string Name => "Tlačítko (Button)";

    public override string Description => "Tlačítko na obrazovce. Vlastnímu tlačítku lze nastavit Before/MWF/After.";

    public override int SortOrder => 130;

    public override bool IncludesWidgetProperties => true;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "button", "input", "a" },
        InputTypes = new[] { "button", "submit", "reset" },
        Roles = new[] { "button" },
        ClassHints = new[] { "btn", "button" },
        NameHints = new[] { "Btn_", "Button" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Button.Text",
            "Text tlačítka",
            GtoValueKind.Text,
            gmsgId: 7000825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Button.Flat",
            "Plochý vzhled 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 9500825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Button.Html_Style",
            "CSS styly tlačítka",
            GtoValueKind.Css,
            example: "background-color: #0062cc;", source: GtoPropertySource.CisGto);
    }
}
