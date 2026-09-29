using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Textový popisek.</summary>
public sealed class LabelElementType : TpiElementType
{
    public override string Code => "LABEL";

    public override string Name => "Label (popisek)";

    public override string Description => "Textový popisek.";

    public override int SortOrder => 120;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "label", "span", "h1", "h2", "h3", "h4", "legend" },
        NameHints = new[] { "_Label", "Label" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Label.Text",
            "Definice textu labelu",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Label.Tooltip",
            "Tooltip pro element typu label",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Label.Text_Color",
            "Barva textu labelu",
            GtoValueKind.Color);

        yield return new GtoPropertyDefinition(
            "M_Pf_Label.Background_Color",
            "Barva pozadí labelu",
            GtoValueKind.Color);

        yield return new GtoPropertyDefinition(
            "M_Pf_Label.Margin",
            "Margin labelu",
            GtoValueKind.Number);

        yield return new GtoPropertyDefinition(
            "M_Pf_Label.Html_Style",
            "CSS styly labelu",
            GtoValueKind.Css, example: "background-color: #0062cc;border-color: #005cbf;");
    }
}
