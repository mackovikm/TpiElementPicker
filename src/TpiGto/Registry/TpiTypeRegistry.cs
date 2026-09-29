using TpiGto.Model;

namespace TpiGto.Registry;

/// <summary>
/// Číselník typů prvků TPI. Skládá se ze zdrojů (<see cref="ITpiElementTypeProvider"/>);
/// typ ze zdroje zaregistrovaného později přepíše dřívější typ se stejným kódem,
/// takže JSON soubor umí předefinovat vestavěný typ.
/// </summary>
public sealed class TpiTypeRegistry
{
    private readonly List<ITpiElementTypeProvider> _providers = new();
    private readonly Dictionary<string, TpiElementType> _types = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<GtoPropertyDefinition> _commonProperties = new(CommonGtoProperties.All);

    /// <summary>Registr jen s vestavěnými typy.</summary>
    public static TpiTypeRegistry CreateDefault()
    {
        var registry = new TpiTypeRegistry();
        registry.AddProvider(new BuiltInElementTypeProvider());
        registry.Reload();
        return registry;
    }

    /// <summary>Registr s vestavěnými typy a volitelným JSON rozšířením.</summary>
    public static TpiTypeRegistry CreateDefault(string? customJsonPath)
    {
        var registry = new TpiTypeRegistry();
        registry.AddProvider(new BuiltInElementTypeProvider());
        if (!string.IsNullOrWhiteSpace(customJsonPath))
            registry.AddProvider(new JsonElementTypeProvider(customJsonPath!));
        registry.Reload();
        return registry;
    }

    public IReadOnlyList<ITpiElementTypeProvider> Providers => _providers;

    /// <summary>Společné vlastnosti M_Pf_Element.* přidávané k typům.</summary>
    public IReadOnlyList<GtoPropertyDefinition> CommonProperties => _commonProperties;

    public void AddProvider(ITpiElementTypeProvider provider)
    {
        if (provider is null) throw new ArgumentNullException(nameof(provider));
        _providers.Add(provider);
    }

    /// <summary>Znovu načte typy ze všech zdrojů (např. po ruční úpravě JSON souboru).</summary>
    public void Reload()
    {
        _types.Clear();
        foreach (var provider in _providers)
        {
            foreach (var type in provider.GetTypes())
            {
                if (string.IsNullOrWhiteSpace(type.Code))
                    continue;
                _types[type.Code] = type;
            }
        }
    }

    /// <summary>Všechny typy seřazené pro nabídku.</summary>
    public IReadOnlyList<TpiElementType> Types
        => _types.Values.OrderBy(t => t.SortOrder).ThenBy(t => t.Name, StringComparer.CurrentCulture).ToList();

    public TpiElementType? Find(string? code)
        => string.IsNullOrWhiteSpace(code) ? null : _types.TryGetValue(code!, out var t) ? t : null;

    /// <summary>
    /// Vlastnosti nabízené pro daný typ – vlastní vlastnosti typu plus společné M_Pf_Element.*.
    /// </summary>
    public IReadOnlyList<GtoPropertyDefinition> GetProperties(TpiElementType type)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));

        var result = new List<GtoPropertyDefinition>(type.Properties);
        if (type.IncludesCommonProperties)
        {
            foreach (var common in _commonProperties)
                if (!result.Any(p => string.Equals(p.Name, common.Name, StringComparison.OrdinalIgnoreCase)))
                    result.Add(common);
        }
        return result;
    }

    public IReadOnlyList<GtoPropertyDefinition> GetProperties(string typeCode)
    {
        var type = Find(typeCode);
        return type is null ? Array.Empty<GtoPropertyDefinition>() : GetProperties(type);
    }

    public GtoPropertyDefinition? FindProperty(string typeCode, string propertyName)
        => GetProperties(typeCode)
            .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));
}
