namespace TpiGto.Model;

/// <summary>
/// Událost, na kterou lze přes M_Pf_Connection napojit MWF.
/// element_path se pak zapisuje ve tvaru <c>&lt;element&gt;.&lt;event&gt;</c>,
/// např. <c>ContainerL_PTS_VRSTVA_DAT_FILTR_PTS_CIS_TYP_DAT_KOD_Field.blur</c>.
/// </summary>
public sealed class TpiEvent
{
    public TpiEvent(string code, string name, string connectionProperty)
    {
        Code = code;
        Name = name;
        ConnectionProperty = connectionProperty;
    }

    public string Code { get; }
    public string Name { get; }

    /// <summary>M_Pf_Connection.Supported nebo M_Pf_Connection.Enabled.</summary>
    public string ConnectionProperty { get; }

    public override string ToString() => Name;
}

/// <summary>Události podporované frameworkem TPI (cis_gto.docx, makra.docx).</summary>
public static class TpiEvents
{
    public const string Supported = "M_Pf_Connection.Supported";
    public const string Enabled = "M_Pf_Connection.Enabled";

    public static readonly TpiEvent Blur = new("blur", "blur – opuštění pole", Supported);
    public static readonly TpiEvent Click = new("click", "click", Supported);
    public static readonly TpiEvent RightClick = new("right_click", "right_click", Supported);
    public static readonly TpiEvent DeleteRow = new("delete_row", "delete_row", Enabled);
    public static readonly TpiEvent BulkChange = new("bulk_change", "bulk_change", Enabled);
    public static readonly TpiEvent AddRow = new("add_row", "add_row", Enabled);

    public static IReadOnlyList<TpiEvent> All { get; } = new[]
    {
        Blur, Click, RightClick, DeleteRow, BulkChange, AddRow
    };

    public static TpiEvent? ByCode(string? code)
        => All.FirstOrDefault(e => string.Equals(e.Code, code, StringComparison.OrdinalIgnoreCase));
}
