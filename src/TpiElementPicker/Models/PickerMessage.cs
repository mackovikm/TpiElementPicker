using System.Text.Json.Serialization;

namespace TpiElementPicker.Models;

/// <summary>Zpráva z picker.js do hostitelské aplikace.</summary>
public sealed class PickerMessage
{
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("url")] public string? Url { get; set; }
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("count")] public int Count { get; set; }
    [JsonPropertyName("on")] public bool On { get; set; }

    /// <summary>Kliknutý prvek (type = pick).</summary>
    [JsonPropertyName("node")] public DomNode? Node { get; set; }

    /// <summary>Rodiče kliknutého prvku, od nejbližšího.</summary>
    [JsonPropertyName("ancestors")] public List<DomNode> Ancestors { get; set; } = new();

    /// <summary>Kořen DOM stromu (type = tree).</summary>
    [JsonPropertyName("root")] public DomNode? Root { get; set; }

    /// <summary>Naváže rodiče na kliknutý prvek podle pole ancestors.</summary>
    public void LinkPickChain()
    {
        if (Node is null) return;

        DomNode current = Node;
        foreach (var ancestor in Ancestors)
        {
            current.ParentNode = ancestor;
            current = ancestor;
        }
    }
}
