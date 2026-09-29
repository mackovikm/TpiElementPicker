namespace TpiGto.Model;

/// <summary>
/// Pravidlo, podle kterého se typu prvku nabízí ke kliknutému prvku stránky.
/// Slouží jen k předvyplnění nabídky – uživatel může typ vždy přepsat.
/// </summary>
public sealed class ElementMatchRule
{
    public static readonly ElementMatchRule None = new();

    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> InputTypes { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();

    /// <summary>Podřetězce hledané v CSS třídách prvku.</summary>
    public IReadOnlyList<string> ClassHints { get; init; } = Array.Empty<string>();

    /// <summary>Podřetězce hledané v id / name prvku.</summary>
    public IReadOnlyList<string> NameHints { get; init; } = Array.Empty<string>();

    /// <summary>Atributy, jejichž samotná přítomnost typ indikuje (např. data-tpi-type).</summary>
    public IReadOnlyList<string> Attributes { get; init; } = Array.Empty<string>();

    /// <summary>Základní váha pravidla – vyšší číslo vyhrává při shodě více typů.</summary>
    public int Weight { get; init; } = 10;

    /// <summary>
    /// Skóre shody s daným prvkem. 0 = neshoda. Používá <see cref="Matching.ElementTypeMatcher"/>.
    /// </summary>
    public int Score(IElementDescriptor element)
    {
        if (element is null) return 0;

        var score = 0;

        if (Tags.Count > 0 && Contains(Tags, element.Tag)) score += Weight;
        else if (Tags.Count > 0) return 0; // tag je tvrdá podmínka, je-li zadán

        if (InputTypes.Count > 0)
        {
            if (Contains(InputTypes, element.InputType)) score += Weight + 5;
            else if (string.Equals(element.Tag, "input", StringComparison.OrdinalIgnoreCase)) return 0;
        }

        if (Roles.Count > 0 && Contains(Roles, element.Role)) score += Weight + 5;

        foreach (var hint in ClassHints)
            if (element.Classes.Any(c => c.Contains(hint, StringComparison.OrdinalIgnoreCase)))
            {
                score += Weight;
                break;
            }

        foreach (var hint in NameHints)
        {
            var id = element.Id ?? string.Empty;
            var name = element.Name ?? string.Empty;
            if (id.Contains(hint, StringComparison.OrdinalIgnoreCase) ||
                name.Contains(hint, StringComparison.OrdinalIgnoreCase))
            {
                score += Weight + 5;
                break;
            }
        }

        foreach (var attr in Attributes)
            if (element.Attributes.ContainsKey(attr))
            {
                score += Weight + 10;
                break;
            }

        return score;
    }

    private static bool Contains(IReadOnlyList<string> list, string? value)
        => !string.IsNullOrEmpty(value) &&
           list.Any(x => string.Equals(x, value, StringComparison.OrdinalIgnoreCase));
}
