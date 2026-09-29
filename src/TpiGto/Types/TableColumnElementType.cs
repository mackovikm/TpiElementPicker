using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Sloupec tabulky. Do cesty se přidává M_PF_DT_TABLE_COLUMN.NAME – fyzický název sloupce v databázi.</summary>
public sealed class TableColumnElementType : TpiElementType
{
    public override string Code => "TABLE_COLUMN";

    public override string Name => "Sloupec tabulky (Table column)";

    public override string Description => "Sloupec tabulky. Do cesty se přidává M_PF_DT_TABLE_COLUMN.NAME – fyzický název sloupce v databázi.";

    public override int SortOrder => 60;

    public override string? PathHint => "<element tabulky>.<SLOUPEC>, např. ObjectList.DODAVATEL_ID nebo child_POLOZKY_FA.CAS";

    /// <summary>Tabulka seznamu se ve frameworku jmenuje ObjectList.</summary>
    public override string? ListScreenElementName => "ObjectList";

    public override bool RequiresSubElement => true;

    public override string SubElementLabel => "sloupec (fyzický název v DB)";

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "th", "col", "td" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Visible",
            "Zobrazení sloupce 0/1",
            GtoValueKind.Bool01,
            example: "0", gmsgId: 14300825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Column_Indx",
            "Pořadí sloupce",
            GtoValueKind.Number,
            example: "4", gmsgId: 13900825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Virtual_Column_Name",
            "Text v hlavičce sloupce",
            GtoValueKind.Text,
            example: "Číslo dokladu", gmsgId: 14000825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Readonly",
            "Readonly 0/1 pro sloupec",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 13700825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Size_Width",
            "Šířka sloupce",
            GtoValueKind.Number,
            gmsgId: 14500825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Sorted_Enabled",
            "Povolení řazení sloupce 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 14100825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Filter_Enabled",
            "Povolení filtrování sloupce 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 14200825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Filter_Text",
            "Předvyplněný text filtru",
            GtoValueKind.Text,
            gmsgId: 14600825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Hideable",
            "Sloupec lze skrýt uživatelem 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 14400825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Data_Type",
            "Datový typ sloupce",
            GtoValueKind.Text,
            gmsgId: 14700825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.M_Cis_Pf_Format",
            "Formát zobrazení sloupce",
            GtoValueKind.Text,
            gmsgId: 14800825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.New_Default_Value",
            "Výchozí hodnota pro nový řádek",
            GtoValueKind.Text,
            gmsgId: 13800825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Graph_Min",
            "Minimum pro grafické zobrazení",
            GtoValueKind.Number,
            gmsgId: 14900825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Graph_Max",
            "Maximum pro grafické zobrazení",
            GtoValueKind.Number,
            gmsgId: 15000825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Tooltip",
            "Tooltip sloupce",
            GtoValueKind.Text,
            source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Format_Hint",
            "Formát sloupce",
            GtoValueKind.Text,
            source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Min_Text_Width",
            "Minimální šířka sloupce",
            GtoValueKind.Number,
            source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Ref_Column",
            "Přetypování sloupce na business popis",
            GtoValueKind.Text,
            source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Sorted_Order",
            "Výchozí řazení tabulky podle sloupce",
            GtoValueKind.Number,
            source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Html_Style",
            "CSS styly sloupce",
            GtoValueKind.Css,
            example: "background-color: #0062cc;", source: GtoPropertySource.CisGto);
    }
}
