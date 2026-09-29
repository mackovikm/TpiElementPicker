namespace TpiGto.Model;

/// <summary>
/// Typ obrazovky TPI. Aplikace má dva základní druhy obrazovek a podle nich se liší
/// skladba <c>in_ref_element_path</c>.
/// </summary>
public enum TpiScreenKind
{
    /// <summary>Neurčeno.</summary>
    Unknown = 0,

    /// <summary>
    /// Seznam – obraz celé databázové tabulky. Element celé tabulky se ve frameworku
    /// jmenuje vždy <c>object_list</c>; sloupec se adresuje jako
    /// <c>object_list.&lt;FYZICKÝ_NÁZEV_SLOUPCE&gt;</c>.
    /// </summary>
    List = 1,

    /// <summary>Detail – formulářová obrazovka nad jedním řádkem tabulky.</summary>
    Detail = 2
}
