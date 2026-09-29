using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Připnutí MWF na akci uživatele. Do cesty se přidává typ eventu.</summary>
public sealed class ConnectionElementType : TpiElementType
{
    public override string Code => "CONNECTION";

    public override string Name => "Connection (napojení MWF na event)";

    public override string Description => "Připnutí MWF na akci uživatele. Do cesty se přidává typ eventu.";

    public override int SortOrder => 910;

    public override string? PathHint => "<element>.<event>, např. ContainerL_..._Field.blur";

    public override bool AppendsEvent => true;

    public override bool IncludesCommonProperties => false;

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Connection.Supported",
            "Podpora eventu (blur, click, right_click) 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 1100825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Connection.Enabled",
            "Podpora eventu (delete_row, bulk_change, add_row) 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 1500825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Connection.M_Wf_Name",
            "Název spouštěného MWF",
            GtoValueKind.MwfName,
            example: "EDIT_PTS_..._BLUR", source: GtoPropertySource.Documentation);

        yield return new GtoPropertyDefinition(
            "M_Pf_Connection.M_Mwf_Name_Before",
            "MWF spuštěný před hlavním MWF eventu",
            GtoValueKind.MwfName,
            source: GtoPropertySource.Documentation);

        yield return new GtoPropertyDefinition(
            "M_Pf_Connection.M_Mwf_Name_After",
            "MWF spuštěný po hlavním MWF eventu",
            GtoValueKind.MwfName,
            source: GtoPropertySource.Documentation);

        yield return new GtoPropertyDefinition(
            "M_Pf_Connection.M_Wf_Id",
            "ID spouštěného workflow",
            GtoValueKind.Number,
            gmsgId: 1600825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Connection.Sortorder",
            "Pořadí zpracování eventu",
            GtoValueKind.Number,
            gmsgId: 1400825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Connection.Flag",
            "Příznak eventu",
            GtoValueKind.Text,
            gmsgId: 1200825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Connection.Finit",
            "Finit příznak eventu",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 1300825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Connection_Subelement.M_Pf_Dt_Menu_Item_Id",
            "Napojení eventu na položku menu",
            GtoValueKind.Number,
            gmsgId: 10500825);
    }
}
