using TpiGto.Model;

namespace TpiGto.Naming;

/// <summary>
/// Konvence názvů TPI odvozené ze školení:
/// fyzický název tabulky je v názvu page flow vždy všechno za prefixem <c>LIST</c>,
/// a podle prefixu se pozná i typ obrazovky.
/// </summary>
public static class TpiNaming
{
    public const string ListPrefix = "LIST";

    private static readonly string[] DetailPrefixes = { "EDIT", "DETAIL", "FORM", "NEW" };

    /// <summary>
    /// Fyzický název tabulky v databázi odvozený z názvu page flow –
    /// <c>LIST_PZSV_DOPRAVNI_URCENI</c> → <c>PZSV_DOPRAVNI_URCENI</c>.
    /// Vrací null, když název page flow prefix LIST nemá.
    /// </summary>
    public static string? TableNameFromPageFlow(string? pageFlow)
    {
        if (string.IsNullOrWhiteSpace(pageFlow)) return null;

        var pf = pageFlow.Trim();
        if (!pf.StartsWith(ListPrefix, StringComparison.OrdinalIgnoreCase))
            return null;

        var rest = pf[ListPrefix.Length..].TrimStart('_', '-', ' ');
        return string.IsNullOrWhiteSpace(rest) ? null : rest;
    }

    /// <summary>Typ obrazovky odhadnutý z názvu page flow.</summary>
    public static TpiScreenKind ScreenKindFromPageFlow(string? pageFlow)
    {
        if (string.IsNullOrWhiteSpace(pageFlow)) return TpiScreenKind.Unknown;

        var pf = pageFlow.Trim();

        if (pf.StartsWith(ListPrefix, StringComparison.OrdinalIgnoreCase))
            return TpiScreenKind.List;

        foreach (var prefix in DetailPrefixes)
            if (pf.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return TpiScreenKind.Detail;

        return TpiScreenKind.Unknown;
    }

    /// <summary>
    /// Vypadá text jako název page flow? (prefix + podtržítko + velká písmena).
    /// Používá se při hledání názvu page flow v síťové komunikaci.
    /// </summary>
    public static bool LooksLikePageFlow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        var v = value.Trim();
        if (v.Length is < 5 or > 128) return false;
        if (!v.Contains('_')) return false;

        return v.All(c => char.IsLetterOrDigit(c) || c == '_') &&
               v.All(c => !char.IsLower(c));
    }
}
