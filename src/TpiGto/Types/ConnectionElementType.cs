using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Připnutí MWF na akci uživatele. Do element_path se doplňuje event.</summary>
public sealed class ConnectionElementType : TpiElementType
{
    public override string Code => "CONNECTION";

    public override string Name => "Connection (napojení MWF na event)";

    public override string Description => "Připnutí MWF na akci uživatele. Do element_path se doplňuje event.";

    public override int SortOrder => 910;

    public override string? PathHint => "<element>.<event>, např. ContainerL_..._Field.blur";

    public override bool AppendsEvent => true;

    public override bool IncludesCommonProperties => false;

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Connection.Supported",
            "Podpora připnutí MWF pro element_path pro blur, right_click, click. 0/1",
            GtoValueKind.Bool01, example: "1");

        yield return new GtoPropertyDefinition(
            "M_Pf_Connection.Enabled",
            "Podpora element_path pro delete_row, bulk_change, add_row. 0/1",
            GtoValueKind.Bool01, example: "1");

        yield return new GtoPropertyDefinition(
            "M_Pf_Connection.M_Wf_Name",
            "Název spouštěného MWF při zavedení M_Pf_Connection.Supported",
            GtoValueKind.MwfName, example: "EDIT_PTS_VRSTVA_DAT_FILTR_KOD_VRSTVY_BLUR");
    }
}
