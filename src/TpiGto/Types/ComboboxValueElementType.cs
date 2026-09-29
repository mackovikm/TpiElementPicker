using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Konkrétní hodnota v comboboxu.</summary>
public sealed class ComboboxValueElementType : TpiElementType
{
    public override string Code => "COMBOBOX_VALUE";

    public override string Name => "Hodnota comboboxu";

    public override string Description => "Konkrétní hodnota v comboboxu.";

    public override int SortOrder => 95;

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox_Value.Value",
            "Hodnota položky comboboxu",
            GtoValueKind.Text,
            gmsgId: 3300825);
    }
}
