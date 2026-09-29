using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Element typu assigner (přiřazovací prvek). Podporuje event right_click.</summary>
public sealed class AssignerElementType : TpiElementType
{
    public override string Code => "ASSIGNER";

    public override string Name => "Assigner";

    public override string Description => "Element typu assigner (přiřazovací prvek). Podporuje event right_click.";

    public override int SortOrder => 140;

    public override ElementMatchRule MatchRule => new()
    {
        ClassHints = new[] { "assigner" },
        NameHints = new[] { "Assigner" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Assigner.Text",
            "Definice textu pro element typu assigner",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Assigner.Tooltyp",
            "Tooltip pro element typu assigner (v číselníku je skutečně 'Tooltyp')",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Assigner.Readonly",
            "Readonly 0/1 pro element typu assigner",
            GtoValueKind.Bool01, example: "1");
    }
}
