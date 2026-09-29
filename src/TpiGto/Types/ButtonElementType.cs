using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Tlačítko na obrazovce.</summary>
public sealed class ButtonElementType : TpiElementType
{
    public override string Code => "BUTTON";

    public override string Name => "Tlačítko (Button)";

    public override string Description => "Tlačítko na obrazovce.";

    public override int SortOrder => 130;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "button", "input", "a" },
        InputTypes = new[] { "button", "submit", "reset" },
        Roles = new[] { "button" },
        ClassHints = new[] { "btn", "button" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Button.Text",
            "Definice textu tlačítka",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Button.Html_Style",
            "CSS styly pro element typu button",
            GtoValueKind.Css, example: "background-color: #0062cc;border-color: #005cbf;");
    }
}
