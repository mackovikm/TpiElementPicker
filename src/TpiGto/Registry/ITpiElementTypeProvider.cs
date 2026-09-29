using TpiGto.Model;

namespace TpiGto.Registry;

/// <summary>
/// Zdroj typů prvků. Registr skládá číselník ze všech zaregistrovaných zdrojů,
/// takže typy lze dodávat kódem (třída v Types), JSON souborem i vlastním pluginem.
/// </summary>
public interface ITpiElementTypeProvider
{
    /// <summary>Popis zdroje pro uživatele (zobrazuje se u typu).</summary>
    string Origin { get; }

    IEnumerable<TpiElementType> GetTypes();
}
