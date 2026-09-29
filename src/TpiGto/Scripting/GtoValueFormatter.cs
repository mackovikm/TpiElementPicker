using TpiGto.Model;

namespace TpiGto.Scripting;

/// <summary>Zápis hodnoty do PL/SQL – uvozování a rozpoznání SQL výrazu.</summary>
public static class GtoValueFormatter
{
    /// <summary>Zdvojí apostrofy v textové hodnotě.</summary>
    public static string EscapeLiteral(string? value)
        => (value ?? string.Empty).Replace("'", "''");

    /// <summary>Hodnota v apostrofech – pro in_new_value statického GTO.</summary>
    public static string Quote(string? value)
        => $"'{EscapeLiteral(value)}'";

    /// <summary>
    /// True, pokud se hodnota má do makra vložit bez apostrofů – tedy jde o SELECT,
    /// výraz v závorce, nebo jiné makro frameworku (@…).
    /// </summary>
    public static bool IsSqlExpression(string? value, GtoValueKind kind)
    {
        if (kind == GtoValueKind.Sql) return true;
        if (string.IsNullOrWhiteSpace(value)) return false;

        var trimmed = value!.TrimStart();
        return trimmed.StartsWith("(", StringComparison.Ordinal)
            || trimmed.StartsWith("@", StringComparison.Ordinal)
            || trimmed.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith("CASE", StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith("DECODE", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Hodnota pro 3. parametr makra @GATTRIB_OVERLOAD.</summary>
    public static string ForMacro(string? value, GtoValueKind kind)
        => IsSqlExpression(value, kind) ? (value ?? string.Empty).Trim() : Quote(value);
}
