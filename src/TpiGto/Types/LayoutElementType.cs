using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Kontejner / rozvržení části obrazovky. Layout lze přes GTO i nově vytvořit.</summary>
public sealed class LayoutElementType : TpiElementType
{
    public override string Code => "LAYOUT";

    public override string Name => "Layout";

    public override string Description => "Kontejner / rozvržení části obrazovky. Layout lze přes GTO i nově vytvořit.";

    public override int SortOrder => 20;

    public override bool IncludesWidgetProperties => true;

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
            GtoValueKind.Text,
            gmsgId: 12500825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Grid",
            "Grid rozvržení 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 10700825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Horizontal",
            "Horizontální směr 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 10800825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Wrap",
            "Zalamování prvků 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 10900825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Top_Margin",
            "Horní margin layoutu",
            GtoValueKind.Number,
            gmsgId: 11100825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Bottom_Margin",
            "Spodní margin layoutu",
            GtoValueKind.Number,
            gmsgId: 11200825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Left_Margin",
            "Levý margin layoutu",
            GtoValueKind.Number,
            gmsgId: 11000825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Right_Margin",
            "Pravý margin layoutu",
            GtoValueKind.Number,
            gmsgId: 11300825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Border_Top",
            "Horní rámeček layoutu",
            GtoValueKind.Text,
            gmsgId: 12100825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Border_Bottom",
            "Spodní rámeček layoutu",
            GtoValueKind.Text,
            gmsgId: 12200825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Border_Left",
            "Levý rámeček layoutu",
            GtoValueKind.Text,
            gmsgId: 12300825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Border_Right",
            "Pravý rámeček layoutu",
            GtoValueKind.Text,
            gmsgId: 12400825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.M_Cis_Pf_Layout_Jus_Con",
            "Zarovnání obsahu layoutu (justify content)",
            GtoValueKind.Text,
            gmsgId: 11400825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.M_Cis_Pf_Layout_Jus_Ite",
            "Zarovnání položek layoutu (justify items)",
            GtoValueKind.Text,
            gmsgId: 11500825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.M_Cis_Pf_Grid_Jus_Con",
            "Grid – justify content",
            GtoValueKind.Text,
            gmsgId: 11900825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.M_Cis_Pf_Grid_Jus_Ite",
            "Grid – justify items",
            GtoValueKind.Text,
            gmsgId: 11700825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.M_Cis_Pf_Grid_Ali_Con",
            "Grid – align content",
            GtoValueKind.Text,
            gmsgId: 12000825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.M_Cis_Pf_Grid_Ali_Ite",
            "Grid – align items",
            GtoValueKind.Text,
            gmsgId: 11800825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.M_Cis_Pf_Overflow",
            "Chování při přetečení obsahu",
            GtoValueKind.Text,
            gmsgId: 11600825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Background",
            "Pozadí layoutu",
            GtoValueKind.Color,
            source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Top_Padding",
            "Horní padding layoutu",
            GtoValueKind.Number,
            source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Bottom_Padding",
            "Spodní padding layoutu",
            GtoValueKind.Number,
            source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Left_Padding",
            "Levý padding layoutu",
            GtoValueKind.Number,
            source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Layout.Layout_Right_Padding",
            "Pravý padding layoutu",
            GtoValueKind.Number,
            source: GtoPropertySource.CisGto);
    }
}
