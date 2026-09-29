using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Víceřádkové textové pole.</summary>
public sealed class LineeditLongElementType : TpiElementType
{
    public override string Code => "LINEEDIT_LONG";

    public override string Name => "Lineedit long (víceřádkový text)";

    public override string Description => "Víceřádkové textové pole.";

    public override int SortOrder => 110;

    public override bool IncludesWidgetProperties => true;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "textarea" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Lineedit_Long.Text",
            "Text pole",
            GtoValueKind.Text,
            source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Lineedit_Long.Html_Style",
            "CSS styly pole",
            GtoValueKind.Css,
            example: "background-color: #0062cc;", source: GtoPropertySource.CisGto);
    }
}
