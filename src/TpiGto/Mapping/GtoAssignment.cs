namespace TpiGto.Mapping;

/// <summary>Způsob zápisu GTO.</summary>
public enum GtoMode
{
    /// <summary>
    /// Iniciální (statické) GTO – samostatný skript s CREATE_GATTRIB_OVERLOAD.
    /// Konfigurace zůstane i po pregenerování PageFlow a použije se při jeho inicializaci.
    /// </summary>
    Static = 0,

    /// <summary>
    /// Runtime GTO – makro <c>@GATTRIB_OVERLOAD</c> v MWF. Platí pro konkrétní
    /// iteraci (sekvenci) PageFlow.
    /// </summary>
    Dynamic = 1,

    /// <summary>
    /// Runtime GTO nad daty – makro <c>@GATTRIB_OVERLOAD_DATA</c>. Provede se až po
    /// namapování business dat do GMSG, takže hodnota může na datech záviset
    /// (např. obarvit pole podle jeho hodnoty).
    /// </summary>
    DynamicData = 2
}

/// <summary>
/// Typ iniciálního overloadu – určuje, kdy a pro koho se spustí
/// (Gattrib_Overload.docx, sloupec TYP).
/// </summary>
public enum GtoOverloadType
{
    /// <summary>Spouští se při inicializaci PageFlow, nezávisle na uživateli i roli.</summary>
    Default = 0,

    /// <summary>Podle role přihlášeného uživatele – vyžaduje m_role.id v in_ref_role.</summary>
    Role = 1,

    /// <summary>Podle přihlášeného uživatele – vyžaduje tpi_uzivatel.id v in_ref_login.</summary>
    User = 2,

    /// <summary>
    /// Bezpečnostní overload – spouští se při inicializaci i při každé iteraci,
    /// takže přebije i runtime overload.
    /// </summary>
    Security = 3
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

    /// <summary>Typ overloadu – uplatní se jen u statického GTO.</summary>
    public GtoOverloadType OverloadType { get; set; } = GtoOverloadType.Default;

    /// <summary>in_ref_login – tpi_uzivatel.id, povinné pro USER_OVERLOAD.</summary>
    public string? RefLogin { get; set; }

    /// <summary>in_ref_role – m_role.id, povinné pro ROLE_OVERLOAD.</summary>
    public string? RefRole { get; set; }

    /// <summary>in_poradi – pořadí, v jakém se overloady vykonají.</summary>
    public int Order { get; set; } = 1;

    /// <summary>in_zruseno – 0 pro zavedení, 1 pro zrušení existujícího.</summary>
    public int Cancelled { get; set; }

    /// <summary>Poznámka k zadání (nepromítá se do skriptu jinak než komentářem).</summary>
    public string? Note { get; set; }

    /// <summary>Vlastnost se vygeneruje jen když je zaškrtnutá.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Hodnota parametru in_gattrib_overload_typ.</summary>
    public string OverloadTypeName => OverloadType switch
    {
        GtoOverloadType.Role => "ROLE_OVERLOAD",
        GtoOverloadType.User => "USER_OVERLOAD",
        GtoOverloadType.Security => "SECURITY_OVERLOAD",
        _ => "DEFAULT_OVERLOAD"
    };

    public GtoAssignment Clone() => (GtoAssignment)MemberwiseClone();
}
