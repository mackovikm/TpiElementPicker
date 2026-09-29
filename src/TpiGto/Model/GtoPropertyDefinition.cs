namespace TpiGto.Model;

/// <summary>
/// Definice jedné vlastnosti z číselníku GTO (cis_gto.docx) – hodnota parametru
/// <c>in_gattrib_overload_name</c> u statického GTO, resp. 1. parametr makra
/// <c>@GATTRIB_OVERLOAD</c> u dynamického GTO.
/// </summary>
public sealed class GtoPropertyDefinition
{
    public GtoPropertyDefinition(
        string name,
        string description,
        GtoValueKind valueKind = GtoValueKind.Text,
        bool allowStatic = true,
        bool allowDynamic = true,
        string? example = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Název vlastnosti GTO nesmí být prázdný.", nameof(name));

        Name = name.Trim();
        Description = description ?? string.Empty;
        ValueKind = valueKind;
        AllowStatic = allowStatic;
        AllowDynamic = allowDynamic;
        Example = example;
    }

    /// <summary>Např. <c>M_Pf_Dt_Button.Text</c>.</summary>
    public string Name { get; }

    /// <summary>Popis z číselníku cis_gto.docx.</summary>
    public string Description { get; }

    public GtoValueKind ValueKind { get; }

    /// <summary>Lze použít ve statickém GTO (CREATE_GATTRIB_OVERLOAD).</summary>
    public bool AllowStatic { get; }

    /// <summary>Lze použít v dynamickém GTO (@GATTRIB_OVERLOAD v plain bloku MWF).</summary>
    public bool AllowDynamic { get; }

    /// <summary>Ukázková hodnota (zobrazuje se jako nápověda).</summary>
    public string? Example { get; }

    /// <summary>Skupina vlastnosti = část názvu před tečkou, např. <c>M_Pf_Dt_Button</c>.</summary>
    public string Group
    {
        get
        {
            var dot = Name.IndexOf('.');
            return dot > 0 ? Name[..dot] : Name;
        }
    }

    /// <summary>
    /// Kategorie odvozená z názvu – framework rozlišuje vlastnosti pro data/text
    /// (<c>M_Pf_Dt_*</c>), pro vzhled (<c>M_Pf_*</c>), obecné vlastnosti elementu
    /// (<c>M_Pf_Element.*</c>) a napojení událostí (<c>M_Pf_Connection.*</c>).
    /// </summary>
    public string Category
    {
        get
        {
            if (Name.StartsWith("M_Pf_Element.", StringComparison.OrdinalIgnoreCase)) return "Obecné";
            if (Name.StartsWith("M_Pf_Connection.", StringComparison.OrdinalIgnoreCase)) return "Události";
            if (Name.StartsWith("M_Pf_Dt_", StringComparison.OrdinalIgnoreCase) ||
                Name.StartsWith("M_Pf_Df_", StringComparison.OrdinalIgnoreCase)) return "Data / text";
            return "Vzhled";
        }
    }

    public override string ToString() => Name;
}
