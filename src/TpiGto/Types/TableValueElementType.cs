using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Konkrétní hodnota / buňka tabulky.</summary>
public sealed class TableValueElementType : TpiElementType
{
    public override string Code => "TABLE_VALUE";

    public override string Name => "Hodnota v tabulce (Table value)";

    public override string Description => "Konkrétní hodnota / buňka tabulky.";

    public override int SortOrder => 70;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "td" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Value.Value",
            "Hodnota buňky",
            GtoValueKind.Text,
            gmsgId: 1000825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Value.Special_Flag",
            "Speciální příznak buňky",
            GtoValueKind.Text,
            gmsgId: 900825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Value.Readonly",
            "Readonly 0/1",
            GtoValueKind.Bool01,
            example: "1", source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Df_Table_Value.Html_Style",
            "CSS styly buňky (v cis_gto.docx je prefix M_Pf_Df_)",
            GtoValueKind.Css,
            example: "background-color: #0062cc;", source: GtoPropertySource.CisGto);
    }
}
