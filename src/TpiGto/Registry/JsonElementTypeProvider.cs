using System.Text.Json;
using System.Text.Json.Serialization;
using TpiGto.Model;

namespace TpiGto.Registry;

/// <summary>
/// Načte typy prvků z JSON souboru. Slouží k rozšíření číselníku bez rekompilace
/// (nový typ, nová vlastnost, nebo přepsání vestavěného typu stejným kódem).
/// </summary>
public sealed class JsonElementTypeProvider : ITpiElementTypeProvider
{
    private readonly string _path;

    public JsonElementTypeProvider(string path)
    {
        _path = path;
    }

    public string Origin => Path.GetFileName(_path);

    public IEnumerable<TpiElementType> GetTypes()
    {
        if (!File.Exists(_path))
            return Array.Empty<TpiElementType>();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        var doc = JsonSerializer.Deserialize<JsonCatalog>(File.ReadAllText(_path), options);
        if (doc?.Types is null)
            return Array.Empty<TpiElementType>();

        return doc.Types
            .Where(t => !string.IsNullOrWhiteSpace(t.Code))
            .Select(t => (TpiElementType)new JsonDefinedElementType(t, Origin))
            .ToList();
    }

    internal sealed class JsonCatalog
    {
        public int Version { get; set; } = 1;
        public string? Note { get; set; }
        public List<JsonType>? Types { get; set; }
    }

    internal sealed class JsonType
    {
        public string Code { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PathHint { get; set; }

        /// <summary>Např. object_list pro tabulku na obrazovce typu seznam.</summary>
        public string? ListScreenElementName { get; set; }

        /// <summary>Vyžaduje sloupec v element_path.</summary>
        public bool RequiresColumn { get; set; }

        public bool AppendsEvent { get; set; }
        public bool IncludesCommonProperties { get; set; } = true;
        public int SortOrder { get; set; } = 500;
        public JsonMatch? Match { get; set; }
        public List<JsonProperty>? Properties { get; set; }
    }

    internal sealed class JsonMatch
    {
        public List<string>? Tags { get; set; }
        public List<string>? InputTypes { get; set; }
        public List<string>? Roles { get; set; }
        public List<string>? ClassHints { get; set; }
        public List<string>? NameHints { get; set; }
        public List<string>? Attributes { get; set; }
    }

    internal sealed class JsonProperty
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        /// <summary>Text | Bool01 | Number | Css | Color | MwfName | ElementName | Sql</summary>
        public string? ValueKind { get; set; }

        public bool AllowStatic { get; set; } = true;
        public bool AllowDynamic { get; set; } = true;
        public string? Example { get; set; }
    }
}

/// <summary>Typ prvku definovaný v JSON souboru.</summary>
public sealed class JsonDefinedElementType : TpiElementType
{
    private readonly JsonElementTypeProvider.JsonType _json;

    internal JsonDefinedElementType(JsonElementTypeProvider.JsonType json, string origin)
    {
        _json = json;
        Origin = origin;
    }

    public override string Code => _json.Code.Trim().ToUpperInvariant();
    public override string Name => string.IsNullOrWhiteSpace(_json.Name) ? Code : _json.Name!;
    public override string Description => _json.Description ?? string.Empty;
    public override string? PathHint => _json.PathHint;
    public override string? ListScreenElementName => _json.ListScreenElementName;
    public override bool RequiresColumn => _json.RequiresColumn;
    public override bool AppendsEvent => _json.AppendsEvent;
    public override bool IncludesCommonProperties => _json.IncludesCommonProperties;
    public override int SortOrder => _json.SortOrder;
    public override string Origin { get; }

    public override ElementMatchRule MatchRule => _json.Match is null
        ? ElementMatchRule.None
        : new ElementMatchRule
        {
            Tags = _json.Match.Tags ?? new List<string>(),
            InputTypes = _json.Match.InputTypes ?? new List<string>(),
            Roles = _json.Match.Roles ?? new List<string>(),
            ClassHints = _json.Match.ClassHints ?? new List<string>(),
            NameHints = _json.Match.NameHints ?? new List<string>(),
            Attributes = _json.Match.Attributes ?? new List<string>()
        };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        if (_json.Properties is null)
            yield break;

        foreach (var p in _json.Properties)
        {
            if (string.IsNullOrWhiteSpace(p.Name))
                continue;

            var kind = Enum.TryParse<GtoValueKind>(p.ValueKind, true, out var parsed)
                ? parsed
                : GtoValueKind.Text;

            yield return new GtoPropertyDefinition(
                p.Name, p.Description ?? string.Empty, kind, p.AllowStatic, p.AllowDynamic, p.Example);
        }
    }
}
