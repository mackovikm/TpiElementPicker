using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TpiElementPicker.Services;

/// <summary>Režim přihlášení k webové aplikaci.</summary>
public enum LoginMode
{
    /// <summary>Uživatel se přihlásí ručně, session drží WebView2.</summary>
    Manual = 0,

    /// <summary>HTTP Basic – hlavička Authorization.</summary>
    HttpBasic = 1,

    /// <summary>Vyplnění přihlašovacího formuláře podle CSS selektorů.</summary>
    FormAutoFill = 2
}

/// <summary>
/// Prostředí TPI. Aplikace je v prohlížeči rozlišuje barevným pruhem –
/// dev hnědý, preprod růžový, produkce modrý.
/// </summary>
public enum TpiEnvironment
{
    Unknown = 0,
    Dev = 1,
    Preprod = 2,
    Prod = 3
}

/// <summary>Uložený profil připojení k aplikaci.</summary>
public sealed class ConnectionProfile
{
    public string Name { get; set; } = "Výchozí";
    public string Url { get; set; } = string.Empty;
    public LoginMode Mode { get; set; } = LoginMode.Manual;

    /// <summary>Prostředí – ať je na první pohled vidět, kde se skripty chystají.</summary>
    public TpiEnvironment Environment { get; set; } = TpiEnvironment.Unknown;
    public string UserName { get; set; } = string.Empty;

    /// <summary>Heslo zašifrované DPAPI (CurrentUser) – base64.</summary>
    public string? ProtectedPassword { get; set; }

    // selektory pro FormAutoFill
    public string UserSelector { get; set; } = "input[type=text]";
    public string PasswordSelector { get; set; } = "input[type=password]";
    public string SubmitSelector { get; set; } = "button[type=submit]";

    [JsonIgnore]
    public string Password
    {
        get => Unprotect(ProtectedPassword);
        set => ProtectedPassword = Protect(value);
    }

    public override string ToString() => string.IsNullOrWhiteSpace(Name) ? Url : Name;

    private static string? Protect(string? plain)
    {
        if (string.IsNullOrEmpty(plain)) return null;
        try
        {
            var bytes = ProtectedData.Protect(
                Encoding.UTF8.GetBytes(plain), null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(bytes);
        }
        catch
        {
            return null;
        }
    }

    private static string Unprotect(string? cipher)
    {
        if (string.IsNullOrEmpty(cipher)) return string.Empty;
        try
        {
            var bytes = ProtectedData.Unprotect(
                Convert.FromBase64String(cipher), null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return string.Empty;
        }
    }
}

/// <summary>Uložení profilů do %APPDATA%\TpiElementPicker\profiles.json.</summary>
public sealed class ProfileStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static string ProfilesPath => Path.Combine(AppSettings.AppDataDir, "profiles.json");

    public List<ConnectionProfile> Profiles { get; private set; } = new();

    public static ProfileStore Load()
    {
        var store = new ProfileStore();
        try
        {
            if (File.Exists(ProfilesPath))
                store.Profiles = JsonSerializer.Deserialize<List<ConnectionProfile>>(
                    File.ReadAllText(ProfilesPath), JsonOptions) ?? new List<ConnectionProfile>();
        }
        catch
        {
            store.Profiles = new List<ConnectionProfile>();
        }
        return store;
    }

    public void Save()
    {
        Directory.CreateDirectory(AppSettings.AppDataDir);
        File.WriteAllText(ProfilesPath, JsonSerializer.Serialize(Profiles, JsonOptions));
    }
}
