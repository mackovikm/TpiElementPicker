namespace TpiGto.Model;

/// <summary>Odkud definice vlastnosti pochází.</summary>
public enum GtoPropertySource
{
    /// <summary>Seznam vlastností GMSG z Gattrib_Overload.docx (m_cis_gattrib_overload).</summary>
    GmsgCatalogue = 0,

    /// <summary>Číselník cis_gto.docx – výběr, který používá tým.</summary>
    CisGto = 1,

    /// <summary>Doloženo v katalogu maker nebo v jiné dokumentaci, ne v seznamu GMSG.</summary>
    Documentation = 2,

    /// <summary>Vlastní rozšíření (JSON nebo vlastní třída).</summary>
    Custom = 3
}

/// <summary>
/// Definice jedné vlastnosti GMSG, kterou lze přetížit – hodnota parametru
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
        string? example = null,
        long? gmsgId = null,
        GtoPropertySource source = GtoPropertySource.GmsgCatalogue)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Název vlastnosti GTO nesmí být prázdný.", nameof(name));

        Name = name.Trim();
        Description = description ?? string.Empty;
        ValueKind = valueKind;
        AllowStatic = allowStatic;
        AllowDynamic = allowDynamic;
        Example = example;
        GmsgId = gmsgId;
        Source = source;
    }

    /// <summary>Např. <c>M_Pf_Dt_Button.Text</c>.</summary>
    public string Name { get; }

    /// <summary>Popis vlastnosti.</summary>
    public string Description { get; }

    public GtoValueKind ValueKind { get; }

    /// <summary>Lze použít ve statickém GTO (CREATE_GATTRIB_OVERLOAD).</summary>
    public bool AllowStatic { get; }

    /// <summary>Lze použít v dynamickém GTO (@GATTRIB_OVERLOAD v MWF).</summary>
    public bool AllowDynamic { get; }

    /// <summary>Ukázková hodnota (zobrazuje se jako nápověda).</summary>
    public string? Example { get; }

    /// <summary>
    /// ID z číselníku <c>M_CIS_GATTRIB_OVERLOAD</c> – pro vlastnosti doložené
    /// seznamem GMSG v Gattrib_Overload.docx. Používá se při zápisu konfigurace
    /// přímo do <c>M_GATTRIB_OVERLOAD_GMSG</c>.
    /// </summary>
    public long? GmsgId { get; }

    public GtoPropertySource Source { get; }

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
    /// (<c>M_Pf_Element.*</c>, <c>M_Pf_Widget.*</c>) a napojení událostí
    /// (<c>M_Pf_Connection.*</c>).
    /// </summary>
    public string Category
    {
        get
        {
            if (Name.StartsWith("M_Pf_Element.", StringComparison.OrdinalIgnoreCase) ||
                Name.StartsWith("M_Pf_Widget.", StringComparison.OrdinalIgnoreCase)) return "Obecné";
            if (Name.StartsWith("M_Pf_Connection", StringComparison.OrdinalIgnoreCase)) return "Události";
            if (Name.StartsWith("M_Pf.", StringComparison.OrdinalIgnoreCase)) return "PageFlow";
            if (Name.StartsWith("M_Pf_Dt_", StringComparison.OrdinalIgnoreCase) ||
                Name.StartsWith("M_Pf_Df_", StringComparison.OrdinalIgnoreCase)) return "Data / text";
            return "Vzhled";
        }
    }

    public override string ToString() => Name;
}
