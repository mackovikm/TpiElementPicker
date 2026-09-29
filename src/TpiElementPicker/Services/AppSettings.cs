using System.Text.Json;
using System.Text.Json.Serialization;
using TpiGto.Paths;
using TpiGto.Scripting;

namespace TpiElementPicker.Services;

/// <summary>
/// Nastavení aplikace v %APPDATA%\TpiElementPicker\settings.json.
/// Obsahuje mimo jiné pravidlo pro odvození in_ref_element_path – to se dá
/// nastavit z aplikace jedním klikem u kliknutého prvku.
/// </summary>
public sealed class AppSettings
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string AppDataDir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TpiElementPicker");

    public static string SettingsPath => Path.Combine(AppDataDir, "settings.json");

    /// <summary>Volitelné rozšíření číselníku typů (vedle .exe).</summary>
    public static string CustomTypesPath => Path.Combine(
        AppContext.BaseDirectory, "Data", "elementTypes.custom.json");

    /// <summary>Uložené rozložení dokovaných oken.</summary>
    public static string LayoutPath => Path.Combine(AppDataDir, "layout.xml");

    public string LastUrl { get; set; } = string.Empty;
    public string LastPfName { get; set; } = string.Empty;
    public string LastMappingFile { get; set; } = string.Empty;

    /// <summary>Motiv dokovacího rozhraní: VS2015Blue | VS2015Light | VS2015Dark.</summary>
    public string Theme { get; set; } = "VS2015Blue";

    public ElementPathOptions ElementPath { get; set; } = new();
    public GtoScriptOptions Script { get; set; } = new();

    /// <summary>
    /// Regulární výrazy, kterými se v síťové komunikaci hledá název page flow.
    /// Skupina <c>pf</c> (nebo první skupina) je nalezená hodnota.
    /// Podle školení se název page flow zjišťuje v konzoli prohlížeče ze síťových
    /// požadavků – tohle to dělá automaticky.
    /// </summary>
    public List<string> PageFlowPatterns { get; set; } = new()
    {
        "\"m_pf_name\"\\s*:\\s*\"(?<pf>[A-Za-z0-9_]+)\"",
        "\"pf_name\"\\s*:\\s*\"(?<pf>[A-Za-z0-9_]+)\"",
        "\"pageFlow(?:Name)?\"\\s*:\\s*\"(?<pf>[A-Za-z0-9_]+)\"",
        "[?&](?:pf|pfName|pf_name|pageFlow|page_flow)=(?<pf>[A-Za-z0-9_]+)",
        "\\b(?<pf>(?:LIST|EDIT|DETAIL|FORM|NEW)_[A-Z0-9][A-Z0-9_]{2,})\\b"
    };

    /// <summary>Maximální velikost odpovědi, ve které se název page flow hledá (v bajtech).</summary>
    public int PageFlowScanLimitBytes { get; set; } = 512 * 1024;

    /// <summary>Sledovat síťovou komunikaci a hledat v ní název page flow.</summary>
    public bool DetectPageFlow { get; set; } = true;

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
                return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsPath), JsonOptions)
                       ?? new AppSettings();
        }
        catch
        {
            // poškozené nastavení nesmí zabránit startu
        }
        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(AppDataDir);
            File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, JsonOptions));
        }
        catch
        {
            // ignorovat
        }
    }
}
