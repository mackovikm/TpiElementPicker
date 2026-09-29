using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Sloupec tabulky.</summary>
public sealed class TableColumnElementType : TpiElementType
{
    public override string Code => "TABLE_COLUMN";

    public override string Name => "Sloupec tabulky (Table column)";

    public override string Description => "Sloupec tabulky.";

    public override int SortOrder => 60;

    public override string? PathHint =>
        "<element tabulky>.<FYZICKÝ_NÁZEV_SLOUPCE>, na seznamu object_list.<SLOUPEC>; " +
        "sloupec je technický název z databáze, ne popisek v hlavičce";

    /// <summary>Na obrazovce typu seznam je tabulkou object_list.</summary>
    public override string? ListScreenElementName => "object_list";

    /// <summary>Bez sloupce by se změna vztahovala na celou tabulku.</summary>
    public override bool RequiresColumn => true;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "th", "col", "td" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Virtual_Column_Name",
            "Název sloupce tabulky",
            GtoValueKind.Text, example: "Název tabulky (vlastnosti)");

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Column_Indx",
            "Pořadí sloupce tabulky",
            GtoValueKind.Number);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Visible",
            "Zobrazení sloupce 0/1",
            GtoValueKind.Bool01, example: "0");

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Readonly",
            "Readonly 0/1 pro sloupec tabulky",
            GtoValueKind.Bool01, example: "1");

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Size_Width",
            "Šířka sloupce",
            GtoValueKind.Number);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Min_Text_Width",
            "Minimální šířka sloupce",
            GtoValueKind.Number);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Tooltip",
            "Tooltip pro sloupec tabulky",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Format_Hint",
            "Definice formátu sloupce tabulky",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Html_Style",
            "CSS styly pro sloupec tabulky",
            GtoValueKind.Css, example: "background-color: #0062cc;border-color: #005cbf;");

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Ref_Column",
            "Přetypování sloupce na zobrazení business popisu",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Sorted_Order",
            "Řadit podle definovaného sloupečku – dosadit dodané číslo",
            GtoValueKind.Number);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Sorted_Enabled",
            "Povolení řazení sloupců 0/1",
            GtoValueKind.Bool01, example: "1");

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Column.Filter_Enabled",
            "Povolení filtrování ve sloupcích 0/1",
            GtoValueKind.Bool01, example: "1");
    }
}
