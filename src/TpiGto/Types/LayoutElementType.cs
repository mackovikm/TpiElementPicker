using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Kontejner / rozvržení části obrazovky.</summary>
public sealed class LayoutElementType : TpiElementType
{
    public override string Code => "LAYOUT";

    public override string Name => "Layout";

    public override string Description => "Kontejner / rozvržení části obrazovky.";

    public override int SortOrder => 20;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "div", "section", "form", "fieldset" },
        ClassHints = new[] { "layout", "container", "panel" },
        NameHints = new[] { "Layout", "Container" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Title",
            "Nadpis layoutu",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Background",
            "Pozadí layoutu",
            GtoValueKind.Color, example: "#FFFFFF");

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Top_Margin",
            "Horní margin layoutu",
            GtoValueKind.Number);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Bottom_Margin",
            "Spodní margin layoutu",
            GtoValueKind.Number);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Left_Margin",
            "Levý margin layoutu",
            GtoValueKind.Number);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Right_Margin",
            "Pravý margin layoutu",
            GtoValueKind.Number);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Top_Padding",
            "Horní padding layoutu",
            GtoValueKind.Number);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Bottom_Padding",
            "Spodní padding layoutu",
            GtoValueKind.Number);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Left_Padding",
            "Levý padding layoutu",
            GtoValueKind.Number);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Right_Padding",
            "Pravý padding layoutu",
            GtoValueKind.Number);
    }
}
