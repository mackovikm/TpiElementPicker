using System.Text.Json;
using TpiGto.Model;
using System.Text.Json.Serialization;

namespace TpiGto.Mapping;

/// <summary>
/// Uložené mapování jedné obrazovky – vstup pro generátor Oracle skriptů.
/// </summary>
public sealed class ElementMappingDocument
{
    /// <summary>in_pf_name – název PageFlow, ke kterému mapování patří.</summary>
    public string PfName { get; set; } = string.Empty;

    /// <summary>
    /// Typ obrazovky – na seznamu se tabulka jmenuje ObjectList, na detailu má
    /// element vlastní název v rámci page flow.
    /// </summary>
    public TpiScreenKind ScreenKind { get; set; } = TpiScreenKind.Unknown;

    public string? Url { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedUtc { get; set; } = DateTime.UtcNow;

    public List<MappedElement> Elements { get; set; } = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter() }
    };

    public string ToJson() => JsonSerializer.Serialize(this, JsonOptions);

    public void Save(string path)
    {
        ModifiedUtc = DateTime.UtcNow;
        File.WriteAllText(path, ToJson(), System.Text.Encoding.UTF8);
    }

    public static ElementMappingDocument Load(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<ElementMappingDocument>(json, JsonOptions)
               ?? new ElementMappingDocument();
    }
}
