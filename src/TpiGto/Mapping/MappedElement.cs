namespace TpiGto.Mapping;

/// <summary>Prvek stránky, kterému uživatel přiřadil typ TPI a vlastnosti GTO.</summary>
public sealed class MappedElement
{
    /// <summary>in_ref_element_path – název elementu ve frameworku TPI.</summary>
    public string ElementPath { get; set; } = string.Empty;

    /// <summary>Kód typu z číselníku, např. LINEEDIT.</summary>
    public string TypeCode { get; set; } = "ELEMENT";

    /// <summary>Zobrazovaný název typu (jen pro čitelnost exportu).</summary>
    public string? TypeName { get; set; }

    /// <summary>Event u typu CONNECTION (blur, click, right_click…).</summary>
    public string? EventCode { get; set; }

    /// <summary>Sloupec u typů TABLE_COLUMN / COMBOBOX_COLUMN.</summary>
    public string? Column { get; set; }

    /// <summary>Popisek pro orientaci v seznamu (text prvku, label).</summary>
    public string? Label { get; set; }

    // --- původ v DOM, pro dohledání prvku na stránce ---
    public string? Tag { get; set; }
    public string? DomId { get; set; }
    public string? DomName { get; set; }
    public string? InputType { get; set; }
    public string? CssSelector { get; set; }
    public string? XPath { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new();

    public List<GtoAssignment> Assignments { get; set; } = new();

    /// <summary>Výsledný element_path včetně sloupce nebo eventu.</summary>
    public string EffectivePath(string separator = ".")
    {
        var suffix = !string.IsNullOrWhiteSpace(Column) ? Column
                   : !string.IsNullOrWhiteSpace(EventCode) ? EventCode
                   : null;

        return string.IsNullOrWhiteSpace(suffix)
            ? ElementPath
            : $"{ElementPath}{separator}{suffix}";
    }

    public override string ToString()
        => $"{EffectivePath()} [{TypeCode}] ({Assignments.Count(a => a.Enabled)})";
}
