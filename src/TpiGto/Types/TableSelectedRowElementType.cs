using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Označení řádku tabulky. Řádky jde označovat i makrem @MARK_TABLE_ROWS.</summary>
public sealed class TableSelectedRowElementType : TpiElementType
{
    public override string Code => "TABLE_SELECTED_ROW";

    public override string Name => "Označený řádek tabulky";

    public override string Description => "Označení řádku tabulky. Řádky jde označovat i makrem @MARK_TABLE_ROWS.";

    public override int SortOrder => 75;

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Table_Selected_Row.Marked",
            "Označení řádku 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 10600825);
    }
}
