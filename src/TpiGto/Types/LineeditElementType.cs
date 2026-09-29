using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Jednořádkové vstupní pole, ve frameworku typicky ..._Field.</summary>
public sealed class LineeditElementType : TpiElementType
{
    public override string Code => "LINEEDIT";

    public override string Name => "Lineedit (textové pole)";

    public override string Description => "Jednořádkové vstupní pole, ve frameworku typicky ..._Field.";

    public override int SortOrder => 100;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "input" },
        InputTypes = new[] { "text", "number", "date", "password", "search", "tel", "email", "" },
        NameHints = new[] { "_Field" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Lineedit.Text",
            "Definice textu lineedit",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Lineedit.Readonly",
            "Readonly 0/1 pro element typu lineedit",
            GtoValueKind.Bool01, example: "1");

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Lineedit.User_Edit",
            "Vynulovat lineedit. 0/1",
            GtoValueKind.Bool01, example: "0");

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Lineedit.Tooltip",
            "Tooltip pro element typu lineedit",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Lineedit.Max_Length",
            "Maximální délka lineeditu",
            GtoValueKind.Number, example: "20");

        yield return new GtoPropertyDefinition(
            "M_Pf_Lineedit.Placeholder_Text",
            "Placeholder text pro lineedit",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Lineedit.Html_Style",
            "CSS styly lineeditu",
            GtoValueKind.Css, example: "background-color: #0062cc;border-color: #005cbf;");
    }
}
