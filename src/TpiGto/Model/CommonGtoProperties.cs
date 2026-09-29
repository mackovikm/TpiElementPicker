namespace TpiGto.Model;

/// <summary>
/// Vlastnosti, které nepatří jednomu typu prvku.
/// <para>
/// <see cref="Element"/> – M_Pf_Element.* lze podle číselníku GMSG použít na libovolný
/// element; registr je přidává ke každému typu s
/// <see cref="TpiElementType.IncludesCommonProperties"/>.
/// </para>
/// <para>
/// <see cref="Widget"/> – M_Pf_Widget.* má každý vizuální element (layout, popup,
/// tabulka, lineedit…); registr je přidává k typům s
/// <see cref="TpiElementType.IncludesWidgetProperties"/>.
/// </para>
/// </summary>
public static class CommonGtoProperties
{
    /// <summary>Obecné vlastnosti elementu – M_Pf_Element.*.</summary>
    public static IReadOnlyList<GtoPropertyDefinition> Element { get; } = new List<GtoPropertyDefinition>
    {
        new("M_Pf_Element.Visible", "Zobrazení elementu 0/1 – nejpoužívanější gattrib",
            GtoValueKind.Bool01, example: "0", gmsgId: 1800825),
        new("M_Pf_Element.Sortorder", "Pořadí elementu",
            GtoValueKind.Number, example: "1", gmsgId: 2100825),
        new("M_Pf_Element.Sortorder_X", "Pořadí elementu ve vodorovném směru",
            GtoValueKind.Number, gmsgId: 2000825),
        new("M_Pf_Element.Sortorder_Y", "Pořadí elementu ve svislém směru – přehazování pořadí prvků formuláře",
            GtoValueKind.Number, example: "4", gmsgId: 1900825),
        new("M_Pf_Element.Highlight", "Zvýraznění elementu 0/1",
            GtoValueKind.Bool01, example: "1", gmsgId: 1700825),
        new("M_Pf_Element.M_Pf_Element_Name", "Zařazení elementu pod jiný element – vyplní se název nadřízeného elementu",
            GtoValueKind.ElementName, example: "WinLayout_Head", gmsgId: 16400825),
        new("M_Pf_Element.M_Pf_SubElement_Name", "Navázání podřízeného elementu – vyplní se jeho název",
            GtoValueKind.ElementName, gmsgId: 16500825),
        new("M_Pf_Element.M_Cis_Pf_Element_Typ", "Typ elementu – používá se při vytváření nového elementu",
            GtoValueKind.Text, example: "layout", gmsgId: 16300825)
    };

    /// <summary>Vlastnosti widgetu – velikost, pozice, dostupnost.</summary>
    public static IReadOnlyList<GtoPropertyDefinition> Widget { get; } = new List<GtoPropertyDefinition>
    {
        new("M_Pf_Widget.Enabled", "Dostupnost prvku 0/1", GtoValueKind.Bool01, example: "1", gmsgId: 8300825),
        new("M_Pf_Widget.Base_Size_Width", "Výchozí šířka", GtoValueKind.Number, gmsgId: 7800825),
        new("M_Pf_Widget.Base_Size_Height", "Výchozí výška", GtoValueKind.Number, gmsgId: 7700825),
        new("M_Pf_Widget.Min_Size_Width", "Minimální šířka", GtoValueKind.Number, gmsgId: 8200825),
        new("M_Pf_Widget.Min_Size_Height", "Minimální výška", GtoValueKind.Number, gmsgId: 8100825),
        new("M_Pf_Widget.Max_Size_Width", "Maximální šířka", GtoValueKind.Number, gmsgId: 8000825),
        new("M_Pf_Widget.Max_Size_Height", "Maximální výška", GtoValueKind.Number, gmsgId: 7900825),
        new("M_Pf_Widget.Grow", "Roztahování prvku", GtoValueKind.Number, gmsgId: 7600825),
        new("M_Pf_Widget.Shrink", "Smršťování prvku", GtoValueKind.Number, gmsgId: 7500825),
        new("M_Pf_Widget.Left_To_Right", "Směr zleva doprava 0/1", GtoValueKind.Bool01, example: "1", gmsgId: 7400825),
        new("M_Pf_Widget.X", "Pozice X", GtoValueKind.Number, gmsgId: 7300825),
        new("M_Pf_Widget.Y", "Pozice Y", GtoValueKind.Number, gmsgId: 7200825),
        new("M_Pf_Widget.M_Cis_Pf_Position", "Způsob umístění prvku", GtoValueKind.Text, gmsgId: 7100825)
    };

    /// <summary>Zpětná kompatibilita – dřívější název pro <see cref="Element"/>.</summary>
    public static IReadOnlyList<GtoPropertyDefinition> All => Element;
}
