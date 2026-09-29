using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Datová tabulka. Na seznamu se jmenuje ObjectList, child tabulky v detailu child_<TABULKA>.</summary>
public sealed class TableElementType : TpiElementType
{
    public override string Code => "TABLE";

    public override string Name => "Tabulka (Table)";

    public override string Description => "Datová tabulka. Na seznamu se jmenuje ObjectList, child tabulky v detailu child_<TABULKA>.";

    public override int SortOrder => 50;

    public override string? PathHint => "na seznamu ObjectList, v detailu child_<TABULKA> – změny celé tabulky";

    /// <summary>Tabulka seznamu se ve frameworku jmenuje ObjectList.</summary>
    public override string? ListScreenElementName => "ObjectList";

    public override bool IncludesWidgetProperties => true;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "table" },
        Roles = new[] { "grid", "table" },
        ClassHints = new[] { "table", "grid", "datatable" },
        NameHints = new[] { "ObjectList", "child_" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Table.Show_Header",
            "Zobrazení hlavičky tabulky 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 5200825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Table.Show_Grid",
            "Zobrazení mřížky / checkboxů 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 5300825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Table.Sorting_Enabled",
            "Povolení řazení v tabulce 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 5100825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Table.Filter_Enabled",
            "Povolení filtrování tabulky 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 5000825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Table.Delete_Column",
            "Sloupec pro mazání řádků 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 4800825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Table.Inline_Edit",
            "Editace přímo v tabulce 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 4500825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Table.Chars_To_Show",
            "Počet zobrazených znaků",
            GtoValueKind.Number,
            gmsgId: 4900825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Table.M_Cis_Pf_Grid_Style",
            "Styl mřížky",
            GtoValueKind.Text,
            gmsgId: 4700825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Table.M_Cis_Pf_Page_Mode",
            "Režim stránkování",
            GtoValueKind.Text,
            gmsgId: 4600825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table.Readonly",
            "Readonly 0/1 pro tabulku",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 12600825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table.Rows_To_Show",
            "Počet zobrazených řádků",
            GtoValueKind.Number,
            example: "20", gmsgId: 12700825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table.Inline_Edit",
            "Editace přímo v tabulce (data) 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 13000825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table.Select_All",
            "Umožnit označit vše 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 13100825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table.Etl_Id",
            "ETL pro naplnění tabulky",
            GtoValueKind.Number,
            gmsgId: 12900825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table.M_Table_Filter_Id",
            "Filtr tabulky",
            GtoValueKind.Number,
            gmsgId: 12800825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Table.Show_Footer",
            "Zobrazení patičky tabulky 0/1",
            GtoValueKind.Bool01,
            example: "1", source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Table.Show_Filter",
            "Zobrazení filtru tabulky 0/1",
            GtoValueKind.Bool01,
            example: "1", source: GtoPropertySource.CisGto);
    }
}
