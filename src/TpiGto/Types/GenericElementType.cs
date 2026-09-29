using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Element bez specifického typu – jen společné vlastnosti M_Pf_Element.* a M_Pf_Widget.*.</summary>
public sealed class GenericElementType : TpiElementType
{
    public override string Code => "ELEMENT";

    public override string Name => "Obecný element";

    public override string Description => "Element bez specifického typu – jen společné vlastnosti M_Pf_Element.* a M_Pf_Widget.*.";

    public override int SortOrder => 900;

    public override bool IncludesWidgetProperties => true;

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield break;
    }
}
