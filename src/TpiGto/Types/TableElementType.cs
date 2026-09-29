using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Datová tabulka na obrazovce, např. child_PZSV_KOL_LOZE.</summary>
public sealed class TableElementType : TpiElementType
{
    public override string Code => "TABLE";

    public override string Name => "Tabulka (Table)";

    public override string Description => "Datová tabulka na obrazovce, např. child_PZSV_KOL_LOZE.";

    public override int SortOrder => 50;

    /// <summary>Na obrazovce typu seznam se tabulka ve frameworku jmenuje vždy object_list.</summary>
    public override string? ListScreenElementName => "object_list";

    public override string? PathHint => "na seznamu: object_list (změny celé tabulky – šířka, filtr, řazení)";

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "table" },
        Roles = new[] { "grid", "table" },
        ClassHints = new[] { "table", "grid", "datatable" },
        NameHints = new[] { "child_" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Table.Show_Header",
            "Zobraz hlavičku tabulky 0/1",
            GtoValueKind.Bool01, example: "1");

        yield return new GtoPropertyDefinition(
            "M_Pf_Table.Show_Footer",
            "Zobrazení patičky tabulky 0/1",
            GtoValueKind.Bool01, example: "1");

        yield return new GtoPropertyDefinition(
            "M_Pf_Table.Show_Filter",
            "Zobrazení filtru tabulky 0/1",
            GtoValueKind.Bool01, example: "1");

        yield return new GtoPropertyDefinition(
            "M_Pf_Table.Show_Grid",
            "Zobrazení checkboxu v tabulce 0/1",
            GtoValueKind.Bool01, example: "1");

        yield return new GtoPropertyDefinition(
            "M_Pf_Table.Filter_Enabled",
            "Povolení filtrování tabulky 0/1",
            GtoValueKind.Bool01, example: "1");

        yield return new GtoPropertyDefinition(
            "M_Pf_Table.Sorting_Enabled",
            "Povolení řazení v tabulce 0/1",
            GtoValueKind.Bool01, example: "1");

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table.Readonly",
            "Readonly 0/1 pro element typu tabulka",
            GtoValueKind.Bool01, example: "1");

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table.Rows_To_Show",
            "Počet zobrazených řádků v tabulce",
            GtoValueKind.Number, example: "20");
    }
}
