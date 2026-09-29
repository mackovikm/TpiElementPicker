namespace TpiGto.Scripting;

/// <summary>Nastavení generovaných skriptů (hlavička podle staticke_GTO.docx / frameworkTPI.docx).</summary>
public sealed class GtoScriptOptions
{
    /// <summary>in_user v SRV.GENDEF_OBJ.INIT_MD_EDIT.</summary>
    public string User { get; set; } = "PZSV_KUBA";

    /// <summary>in_termin v SRV.GENDEF_OBJ.INIT_MD_EDIT.</summary>
    public string Termin { get; set; } = "1800011";

    /// <summary>in_gattrib_overload_typ – podle dokumentace vždy DEFAULT_OVERLOAD.</summary>
    public string OverloadType { get; set; } = "DEFAULT_OVERLOAD";

    /// <summary>Obalit bloky celým tělem skriptu (DECLARE … END;).</summary>
    public bool IncludeScriptBody { get; set; } = true;

    /// <summary>
    /// Vložit do hlavičky skriptu připomínku zapnout DBMS Output. Bez něj není vidět,
    /// že skript skončil OK – nepovolená hodnota projde bez povšimnutí.
    /// </summary>
    public bool AddOutputReminder { get; set; } = true;

    /// <summary>U dynamického GTO obalit makra kostrou MWF podle frameworkTPI.docx.</summary>
    public bool WrapDynamicInMwf { get; set; }

    public string MwfName { get; set; } = string.Empty;
    public string MwfNotice { get; set; } = string.Empty;
    public string MwfFirstStep { get; set; } = "b010";
    public string MwfType { get; set; } = "USER";

    /// <summary>Odsazení bloků v těle skriptu.</summary>
    public string Indent { get; set; } = "  ";

    /// <summary>
    /// Přidat na konec statického skriptu reset cache page flow. Bez něj se změna
    /// nemusí na obrazovce projevit – page flow si drží cache zobrazení atributů.
    /// Cenou je pomalejší první načtení obrazovky.
    /// </summary>
    public bool AppendCacheReset { get; set; } = true;

    /// <summary>
    /// Volání procedury pro reset cache page flow. Doplň podle prostředí, např.
    /// <c>SRV.NEJAKY_PACKAGE.RESET_PAGE_FLOW('{PF}');</c> – zástupný text
    /// <c>{PF}</c> se nahradí názvem page flow. Prázdné = do skriptu se vloží
    /// jen připomínka v komentáři.
    /// </summary>
    public string CacheResetStatement { get; set; } = string.Empty;
}
