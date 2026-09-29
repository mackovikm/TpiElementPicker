using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Jednořádkové vstupní pole, ve frameworku typicky ..._Field. Podporuje event blur a right_click.</summary>
public sealed class LineeditElementType : TpiElementType
{
    public override string Code => "LINEEDIT";

    public override string Name => "Lineedit (textové pole)";

    public override string Description => "Jednořádkové vstupní pole, ve frameworku typicky ..._Field. Podporuje event blur a right_click.";

    public override int SortOrder => 100;

    public override bool IncludesWidgetProperties => true;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "input" },
        InputTypes = new[] { "text", "number", "date", "password", "search", "tel", "email", "" },
        NameHints = new[] { "_Field", "Lineedit" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Lineedit.Text",
            "Hodnota / text pole",
            GtoValueKind.Text,
            gmsgId: 6800825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Lineedit.Readonly",
            "Readonly 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 6900825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Lineedit.Max_Length",
            "Maximální délka",
            GtoValueKind.Number,
            example: "20", gmsgId: 2300825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Lineedit.Placeholder_Text",
            "Placeholder",
            GtoValueKind.Text,
            gmsgId: 2400825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Lineedit.Input_Mask",
            "Vstupní maska",
            GtoValueKind.Text,
            gmsgId: 2200825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Lineedit.Clear_Button_Enabled",
            "Tlačítko pro vymazání 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 2500825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Lineedit.Data_Type",
            "Datový typ pole",
            GtoValueKind.Text,
            gmsgId: 2900825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Lineedit.M_Cis_Pf_Echo_Mode",
            "Režim zobrazení (např. heslo)",
            GtoValueKind.Text,
            gmsgId: 2600825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Lineedit.M_Cis_Pf_Format",
            "Formát hodnoty",
            GtoValueKind.Text,
            gmsgId: 2700825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Lineedit.M_Cis_Pf_Lineedit_Format",
            "Formát lineeditu",
            GtoValueKind.Text,
            gmsgId: 2800825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Lineedit.Html_Style",
            "CSS styly pole – doloženo příkladem v katalogu maker (@GATTRIB_OVERLOAD_DATA)",
            GtoValueKind.Css,
            example: "background-color:red", source: GtoPropertySource.Documentation);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Lineedit.Tooltip",
            "Tooltip pole",
            GtoValueKind.Text,
            source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Lineedit.User_Edit",
            "Vynulovat lineedit 0/1",
            GtoValueKind.Bool01,
            example: "0", source: GtoPropertySource.CisGto);
    }
}
