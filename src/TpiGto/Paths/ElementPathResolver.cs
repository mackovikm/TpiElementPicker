using System.Text.RegularExpressions;
using TpiGto.Model;

namespace TpiGto.Paths;

/// <summary>Kandidát na element_path nalezený u prvku nebo jeho rodiče.</summary>
public sealed class ElementPathCandidate
{
    public ElementPathCandidate(string attribute, string rawValue, string value, int depth, string tag)
    {
        Attribute = attribute;
        RawValue = rawValue;
        Value = value;
        Depth = depth;
        Tag = tag;
    }

    /// <summary>Název atributu, ze kterého hodnota pochází.</summary>
    public string Attribute { get; }

    /// <summary>Původní hodnota atributu.</summary>
    public string RawValue { get; }

    /// <summary>Hodnota po ořezání předpon/přípon a aplikaci regexu.</summary>
    public string Value { get; }

    /// <summary>0 = přímo kliknutý prvek, 1 = rodič, 2 = prarodič…</summary>
    public int Depth { get; }

    public string Tag { get; }

    public string SourceDescription
        => Depth == 0 ? $"{Tag}[{Attribute}]" : $"{Tag}[{Attribute}] (rodič +{Depth})";

    public override string ToString() => $"{Value}   ←  {SourceDescription}";
}

/// <summary>
/// Odvozuje <c>in_ref_element_path</c> z prvku stránky podle <see cref="ElementPathOptions"/>.
/// </summary>
public sealed class ElementPathResolver
{
    public ElementPathResolver(ElementPathOptions? options = null)
    {
        Options = options ?? new ElementPathOptions();
    }

    public ElementPathOptions Options { get; set; }

    /// <summary>Nejlepší návrh, nebo null když se nic nenašlo.</summary>
    public string? Resolve(IElementDescriptor element)
        => GetCandidates(element).FirstOrDefault()?.Value;

    /// <summary>
    /// Všichni kandidáti – nejdřív podle pořadí atributů v nastavení, pak ostatní
    /// atributy prvku (ty se v aplikaci nabízejí k označení jako zdroj element_path).
    /// </summary>
    public IReadOnlyList<ElementPathCandidate> GetCandidates(IElementDescriptor element)
    {
        var result = new List<ElementPathCandidate>();
        if (element is null) return result;

        var current = element;
        var depth = 0;
        var maxDepth = Options.SearchAncestors ? Math.Max(0, Options.MaxAncestorDepth) : 0;

        while (current is not null && depth <= maxDepth)
        {
            foreach (var attribute in Options.SourceAttributes)
            {
                if (!TryGetAttribute(current, attribute, out var raw))
                    continue;
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                var value = Normalize(raw);
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                if (!result.Any(c => c.Value == value && c.Attribute == attribute))
                    result.Add(new ElementPathCandidate(attribute, raw, value, depth, current.Tag));
            }

            current = current.Parent;
            depth++;
        }

        // Doplnit zbývající atributy kliknutého prvku jako ruční kandidáty.
        foreach (var kv in element.Attributes)
        {
            if (string.IsNullOrWhiteSpace(kv.Value)) continue;
            if (Options.SourceAttributes.Contains(kv.Key, StringComparer.OrdinalIgnoreCase)) continue;
            if (kv.Key.Equals("class", StringComparison.OrdinalIgnoreCase)) continue;
            if (kv.Key.Equals("style", StringComparison.OrdinalIgnoreCase)) continue;

            var value = Normalize(kv.Value);
            if (string.IsNullOrWhiteSpace(value)) continue;

            result.Add(new ElementPathCandidate(kv.Key, kv.Value, value, 0, element.Tag));
        }

        return result;
    }

    /// <summary>Složí výsledný element_path z názvu elementu a přípony (sloupec nebo event).</summary>
    public string Compose(string elementPath, string? suffix)
    {
        if (string.IsNullOrWhiteSpace(suffix))
            return elementPath ?? string.Empty;

        var sep = string.IsNullOrEmpty(Options.Separator) ? "." : Options.Separator;
        return $"{elementPath}{sep}{suffix}";
    }

    private static bool TryGetAttribute(IElementDescriptor element, string attribute, out string value)
    {
        foreach (var kv in element.Attributes)
        {
            if (string.Equals(kv.Key, attribute, StringComparison.OrdinalIgnoreCase))
            {
                value = kv.Value ?? string.Empty;
                return true;
            }
        }

        value = string.Empty;
        return false;
    }

    private string Normalize(string raw)
    {
        var value = raw.Trim();

        if (!string.IsNullOrWhiteSpace(Options.ExtractRegex))
        {
            try
            {
                var match = Regex.Match(value, Options.ExtractRegex!, RegexOptions.CultureInvariant);
                if (match.Success)
                {
                    var group = match.Groups["path"];
                    value = group.Success ? group.Value : match.Value;
                }
            }
            catch (ArgumentException)
            {
                // chybný regex v nastavení nesmí shodit odvozování
            }
        }

        foreach (var prefix in Options.TrimPrefixes)
            if (!string.IsNullOrEmpty(prefix) && value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                value = value[prefix.Length..];

        foreach (var suffix in Options.TrimSuffixes)
            if (!string.IsNullOrEmpty(suffix) && value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                value = value[..^suffix.Length];

        return value.Trim();
    }
}
