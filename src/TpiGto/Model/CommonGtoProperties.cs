namespace TpiGto.Model;

/// <summary>
/// Vlastnosti M_Pf_Element.*, které lze podle číselníku použít na libovolný element.
/// Registr je přidává ke každému typu, který má <see cref="TpiElementType.IncludesCommonProperties"/> = true.
/// </summary>
public static class CommonGtoProperties
{
    public static IReadOnlyList<GtoPropertyDefinition> All { get; } = new List<GtoPropertyDefinition>
    {
        new("M_Pf_Element.M_Pf_Element_Name", "Vnoření elementu do jiného. Vyplní se dodaný název.",
            GtoValueKind.ElementName, example: "WinLayout_Head"),
        new("M_Pf_Element.Sortorder", "Pořadí elementu", GtoValueKind.Number, example: "1"),
        new("M_Pf_Element.Visible", "Zobrazení elementu 0/1", GtoValueKind.Bool01, example: "0")
    };
}
