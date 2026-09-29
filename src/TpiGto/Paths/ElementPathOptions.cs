namespace TpiGto.Paths;

/// <summary>
/// Nastavení odvozování <c>in_ref_element_path</c> z prvku stránky.
/// <para>
/// Ve frameworku TPI je element_path název elementu (např.
/// <c>ContainerL_PTS_VRSTVA_DAT_FILTR_PTS_CIS_TYP_DAT_KOD_Field</c>), ne CSS selektor.
/// Který atribut vygenerované stránky tento název nese, se nastavuje zde – v aplikaci
/// stačí u kliknutého prvku označit správný atribut a uloží se sem.
/// </para>
/// </summary>
public sealed class ElementPathOptions
{
    /// <summary>
    /// Atributy prohledávané v tomto pořadí. První nalezený vyhrává.
    /// <para>
    /// Podle školení nese název elementu atribut <c>name</c> – <c>id</c> je generované
    /// stránkou, a proto se pro GTO nepoužívá (zůstává až na konci jako záloha).
    /// </para>
    /// </summary>
    public List<string> SourceAttributes { get; set; } = new()
    {
        "name",
        "data-element-path",
        "data-element",
        "data-tpi-element",
        "data-name",
        "id"
    };

    /// <summary>Hledat zdrojový atribut i u rodičů, pokud ho prvek sám nemá.</summary>
    public bool SearchAncestors { get; set; } = true;

    /// <summary>Maximální počet úrovní rodičů při hledání.</summary>
    public int MaxAncestorDepth { get; set; } = 8;

    /// <summary>Předpony odstraněné z nalezené hodnoty (např. framework přidává "pf_").</summary>
    public List<string> TrimPrefixes { get; set; } = new();

    /// <summary>Přípony odstraněné z nalezené hodnoty (např. "_input", "_wrap").</summary>
    public List<string> TrimSuffixes { get; set; } = new();

    /// <summary>
    /// Regulární výraz s pojmenovanou skupinou <c>path</c>. Je-li vyplněn, použije se
    /// na hodnotu atributu a výsledkem je obsah skupiny.
    /// Např. <c>^ctl00_(?&lt;path&gt;.+)$</c>.
    /// </summary>
    public string? ExtractRegex { get; set; }

    /// <summary>Oddělovač mezi elementem a sloupcem / eventem.</summary>
    public string Separator { get; set; } = ".";
}
