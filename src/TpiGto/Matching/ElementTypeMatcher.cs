using TpiGto.Model;
using TpiGto.Registry;

namespace TpiGto.Matching;

/// <summary>Návrh typu prvku podle kliknutého prvku stránky.</summary>
public sealed class ElementTypeSuggestion
{
    public ElementTypeSuggestion(TpiElementType type, int score)
    {
        Type = type;
        Score = score;
    }

    public TpiElementType Type { get; }
    public int Score { get; }

    public override string ToString() => $"{Type.Name} ({Score})";
}

/// <summary>
/// Vybírá typ prvku, který se po kliknutí předvyplní. Pravidla dodává každý typ
/// sám přes <see cref="TpiElementType.MatchRule"/>, matcher je jen vyhodnocuje.
/// </summary>
public sealed class ElementTypeMatcher
{
    private readonly TpiTypeRegistry _registry;

    public ElementTypeMatcher(TpiTypeRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    /// <summary>Všechny typy se skóre &gt; 0, seřazené od nejpravděpodobnějšího.</summary>
    public IReadOnlyList<ElementTypeSuggestion> Suggest(IElementDescriptor element)
    {
        if (element is null) return Array.Empty<ElementTypeSuggestion>();

        return _registry.Types
            .Select(t => new ElementTypeSuggestion(t, t.MatchRule.Score(element)))
            .Where(s => s.Score > 0)
            .OrderByDescending(s => s.Score)
            .ThenBy(s => s.Type.SortOrder)
            .ToList();
    }

    /// <summary>Nejpravděpodobnější typ, jinak obecný ELEMENT.</summary>
    public TpiElementType? Best(IElementDescriptor element)
        => Suggest(element).FirstOrDefault()?.Type ?? _registry.Find("ELEMENT");
}
