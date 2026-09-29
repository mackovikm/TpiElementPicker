using System.Text.Json.Serialization;
using TpiGto.Model;

namespace TpiElementPicker.Models;

/// <summary>
/// Uzel DOM tak, jak ho posílá picker.js. Implementuje <see cref="IElementDescriptor"/>,
/// takže s ním umí pracovat knihovna TpiGto (návrh typu, odvození element_path).
/// </summary>
public sealed class DomNode : IElementDescriptor
{
    [JsonPropertyName("idx")] public int Index { get; set; } = -1;
    [JsonPropertyName("tag")] public string Tag { get; set; } = string.Empty;
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("inputType")] public string? InputType { get; set; }
    [JsonPropertyName("role")] public string? Role { get; set; }
    [JsonPropertyName("cls")] public List<string> Cls { get; set; } = new();
    [JsonPropertyName("text")] public string? Text { get; set; }
    [JsonPropertyName("attrs")] public Dictionary<string, string> Attrs { get; set; } = new();
    [JsonPropertyName("css")] public string? Css { get; set; }
    [JsonPropertyName("xpath")] public string? XPath { get; set; }
    [JsonPropertyName("children")] public List<DomNode> Children { get; set; } = new();

    [JsonIgnore] public DomNode? ParentNode { get; set; }

    [JsonIgnore] IReadOnlyList<string> IElementDescriptor.Classes => Cls;
    [JsonIgnore] IReadOnlyDictionary<string, string> IElementDescriptor.Attributes => Attrs;
    [JsonIgnore] IElementDescriptor? IElementDescriptor.Parent => ParentNode;

    /// <summary>Popis uzlu do stromu, např. <c>input#docNo .form-control "Číslo dokladu"</c>.</summary>
    public string Caption()
    {
        var caption = Tag;
        if (!string.IsNullOrWhiteSpace(Id)) caption += "#" + Id;
        if (!string.IsNullOrWhiteSpace(Name)) caption += "[name=" + Name + "]";
        if (Cls.Count > 0) caption += " ." + string.Join(".", Cls.Take(3));
        if (!string.IsNullOrWhiteSpace(Text)) caption += "  \"" + Truncate(Text!, 40) + "\"";
        return caption;
    }

    /// <summary>Naváže rodiče v celém podstromu (po deserializaci).</summary>
    public void LinkParents(DomNode? parent = null)
    {
        ParentNode = parent;
        foreach (var child in Children)
            child.LinkParents(this);
    }

    public IEnumerable<DomNode> Descendants()
    {
        foreach (var child in Children)
        {
            yield return child;
            foreach (var sub in child.Descendants())
                yield return sub;
        }
    }

    private static string Truncate(string value, int max)
        => value.Length <= max ? value : value[..max] + "…";

    public override string ToString() => Caption();
}
