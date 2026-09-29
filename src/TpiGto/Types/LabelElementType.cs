using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Textový popisek. Každý nápis na obrazovce je samostatný element.</summary>
public sealed class LabelElementType : TpiElementType
{
    public override string Code => "LABEL";

    public override string Name => "Label (popisek)";

    public override string Description => "Textový popisek. Každý nápis na obrazovce je samostatný element.";

    public override int SortOrder => 120;

    public override bool IncludesWidgetProperties => true;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "label", "span", "h1", "h2", "h3", "h4", "legend" },
        NameHints = new[] { "Label" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Label.Text",
            "Text labelu",
            GtoValueKind.Text,
            gmsgId: 6700825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Label.Text_Color",
            "Barva textu",
            GtoValueKind.Color,
            gmsgId: 6300825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Label.Background_Color",
            "Barva pozadí",
            GtoValueKind.Color,
            gmsgId: 6200825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Label.Margin",
            "Margin labelu",
            GtoValueKind.Number,
            gmsgId: 6500825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Label.Indent",
            "Odsazení textu",
            GtoValueKind.Number,
            gmsgId: 6400825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Label.Word_Wrap",
            "Zalamování textu 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 6600825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Label.M_Cis_Pf_Hor_Align",
            "Vodorovné zarovnání",
            GtoValueKind.Text,
            gmsgId: 6100825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Label.M_Cis_Pf_Ver_Align",
            "Svislé zarovnání",
            GtoValueKind.Text,
            gmsgId: 6000825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Label.Tooltip",
            "Tooltip labelu",
            GtoValueKind.Text,
            source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Label.Html_Style",
            "CSS styly labelu",
            GtoValueKind.Css,
            example: "background-color: #0062cc;", source: GtoPropertySource.CisGto);
    }
}
