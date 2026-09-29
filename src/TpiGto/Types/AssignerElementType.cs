using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Nabídka hodnot k výběru, když se nehodí combobox. Podporuje event right_click.</summary>
public sealed class AssignerElementType : TpiElementType
{
    public override string Code => "ASSIGNER";

    public override string Name => "Assigner";

    public override string Description => "Nabídka hodnot k výběru, když se nehodí combobox. Podporuje event right_click.";

    public override int SortOrder => 140;

    public override bool IncludesWidgetProperties => true;

    public override ElementMatchRule MatchRule => new()
    {
        ClassHints = new[] { "assigner" },
        NameHints = new[] { "Assigner" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Assigner.Foreign_Key_Id",
            "Vybraná hodnota – cizí klíč",
            GtoValueKind.Number,
            gmsgId: 9600825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Assigner.Text",
            "Text assigneru",
            GtoValueKind.Text,
            source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Assigner.Tooltyp",
            "Tooltip assigneru (v číselníku je skutečně 'Tooltyp')",
            GtoValueKind.Text,
            source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Assigner.Readonly",
            "Readonly 0/1",
            GtoValueKind.Bool01,
            example: "1", source: GtoPropertySource.CisGto);
    }
}
