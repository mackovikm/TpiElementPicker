namespace TpiGto.Mapping;

/// <summary>Způsob zápisu GTO.</summary>
public enum GtoMode
{
    /// <summary>Samostatný skript s CREATE_GATTRIB_OVERLOAD – trvalá úprava vzhledu.</summary>
    Static = 0,

    /// <summary>Makro @GATTRIB_OVERLOAD v plain bloku MWF – úprava až po spuštění MWF.</summary>
    Dynamic = 1
}

/// <summary>
/// Jedna nastavená vlastnost prvku = jeden blok GTO.
/// </summary>
public sealed class GtoAssignment
{
    /// <summary>Hodnota in_gattrib_overload_name, např. M_Pf_Dt_Button.Text.</summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>Hodnota in_new_value (u dynamického GTO 3. parametr makra).</summary>
    public string Value { get; set; } = string.Empty;

    public GtoMode Mode { get; set; } = GtoMode.Static;

    /// <summary>in_poradi – podle dokumentace vždy 1.</summary>
    public int Order { get; set; } = 1;

    /// <summary>in_zruseno – 0 pro zavedení, 1 pro zrušení existujícího.</summary>
    public int Cancelled { get; set; }

    /// <summary>Poznámka k zadání (nepromítá se do skriptu).</summary>
    public string? Note { get; set; }

    /// <summary>Vlastnost se vygeneruje jen když je zaškrtnutá.</summary>
    public bool Enabled { get; set; } = true;

    public GtoAssignment Clone() => (GtoAssignment)MemberwiseClone();
}
