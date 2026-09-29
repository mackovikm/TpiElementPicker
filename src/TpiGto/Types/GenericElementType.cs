using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Element bez specifického typu – jen společné vlastnosti M_Pf_Element.*.</summary>
public sealed class GenericElementType : TpiElementType
{
    public override string Code => "ELEMENT";

    public override string Name => "Obecný element";

    public override string Description => "Element bez specifického typu – jen společné vlastnosti M_Pf_Element.*.";

    public override int SortOrder => 900;

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield break;
    }
}
