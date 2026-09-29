using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using TpiElementPicker.Models;
using TpiElementPicker.Services;
using TpiElementPicker.Workspace;
using TpiGto.Naming;
using WeifenLuo.WinFormsUI.Docking;

namespace TpiElementPicker.Forms.Docking;

/// <summary>Dokument s webovou aplikací (WebView2) a napojením na picker.js.</summary>
public sealed class BrowserWindow : ToolWindowBase, IBrowserHost
{
    private readonly WebView2 _webView = new() { Dock = DockStyle.Fill };
    private readonly List<Regex> _pageFlowPatterns = new();
    private bool _ready;
    private bool _initStarted;

    public BrowserWindow(PickerWorkspace workspace) : base(workspace)
    {
        Text = "Webová aplikace";
        TabText = "Webová aplikace";
        Initialize();
        workspace.Browser = this;
    }

    protected override DockState DefaultDockState => DockState.Document;

    /// <summary>
    /// WebView2 se inicializuje až když má okno handle – tedy po jeho zobrazení
    /// v dokovací ploše nebo v plovoucím okně.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (_initStarted) return;
        _initStarted = true;
        _ = InitializeWebViewAsync();
    }

    protected override void BuildUi()
    {
        ((System.ComponentModel.ISupportInitialize)_webView).BeginInit();
        Controls.Add(_webView);
        ((System.ComponentModel.ISupportInitialize)_webView).EndInit();
    }

    // ------------------------------------------------------------- WebView2

    private async Task InitializeWebViewAsync()
    {
        try
        {
            var userData = Path.Combine(AppSettings.AppDataDir, "WebView2");
            Directory.CreateDirectory(userData);

            var environment = await CoreWebView2Environment.CreateAsync(null, userData);
            await _webView.EnsureCoreWebView2Async(environment);

            var core = _webView.CoreWebView2;
            core.Settings.AreDevToolsEnabled = true;
            core.Settings.IsStatusBarEnabled = false;

            await core.AddScriptToExecuteOnDocumentCreatedAsync(LoadPickerScript());

            core.WebMessageReceived += OnWebMessageReceived;
            core.NavigationCompleted += OnNavigationCompleted;
            core.BasicAuthenticationRequested += OnBasicAuthenticationRequested;

            if (Workspace.Settings.DetectPageFlow)
                SetupPageFlowDetection(core);

            _ready = true;
            Workspace.Status("WebView2 připraven.");

            if (!string.IsNullOrWhiteSpace(Workspace.Settings.LastUrl))
                await NavigateAsync(Workspace.Settings.LastUrl);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "WebView2 se nepodařilo inicializovat.\r\n\r\n" + ex.Message +
                "\r\n\r\nZkontroluj, že je nainstalován WebView2 Runtime (Evergreen).",
                "WebView2", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static string LoadPickerScript()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Scripts", "picker.js");
        if (!File.Exists(path))
            throw new FileNotFoundException("Nenalezen soubor Scripts\\picker.js.", path);
        return File.ReadAllText(path, Encoding.UTF8);
    }

    private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        PickerMessage? message;
        try
        {
            message = JsonSerializer.Deserialize<PickerMessage>(e.TryGetWebMessageAsString());
        }
        catch (Exception ex)
        {
            Workspace.Status("Nečitelná zpráva ze stránky: " + ex.Message);
            return;
        }

        if (message is not null)
            Workspace.HandleBrowserMessage(message);
    }

    private void OnBasicAuthenticationRequested(object? sender, CoreWebView2BasicAuthenticationRequestedEventArgs e)
    {
        var profile = Workspace.Profiles.Profiles
            .FirstOrDefault(p => p.Mode == LoginMode.HttpBasic &&
                                 !string.IsNullOrWhiteSpace(p.Url) &&
                                 e.Uri.StartsWith(p.Url, StringComparison.OrdinalIgnoreCase));

        if (profile is null) return;

        e.Response.UserName = profile.UserName;
        e.Response.Password = profile.Password;
    }

    private async void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        Workspace.Status(e.IsSuccess ? "Stránka načtena." : $"Navigace selhala ({e.WebErrorStatus}).");
        if (!e.IsSuccess) return;

        var profile = Workspace.Profiles.Profiles.FirstOrDefault(p =>
            p.Mode == LoginMode.FormAutoFill &&
            !string.IsNullOrWhiteSpace(p.Url) &&
            (_webView.Source?.ToString() ?? string.Empty).StartsWith(p.Url, StringComparison.OrdinalIgnoreCase));

        if (profile is not null)
            await TryAutoFillLoginAsync(profile);

        if (Workspace.PickMode)
            await SetPickModeAsync(true);
    }

    private async Task TryAutoFillLoginAsync(ConnectionProfile profile)
    {
        var script =
            $"__tpi.fillLogin({JsonSerializer.Serialize(profile.UserSelector)}," +
            $"{JsonSerializer.Serialize(profile.UserName)}," +
            $"{JsonSerializer.Serialize(profile.PasswordSelector)}," +
            $"{JsonSerializer.Serialize(profile.Password)}," +
            $"{JsonSerializer.Serialize(profile.SubmitSelector)})";

        try
        {
            await _webView.CoreWebView2.ExecuteScriptAsync(script);
            Workspace.Status("Přihlašovací formulář vyplněn.");
        }
        catch (Exception ex)
        {
            Workspace.Status("Auto-fill selhal: " + ex.Message);
        }
    }

    // --------------------------------------------------- detekce page flow

    // Podle školení se název page flow zjišťuje v konzoli prohlížeče ze síťových
    // požadavků. Tohle dělá totéž automaticky – prochází URL, těla požadavků
    // i odpovědí a hledá v nich názvy podle regulárních výrazů z nastavení.
    private void SetupPageFlowDetection(CoreWebView2 core)
    {
        _pageFlowPatterns.Clear();

        foreach (var pattern in Workspace.Settings.PageFlowPatterns)
        {
            try
            {
                _pageFlowPatterns.Add(new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant));
            }
            catch (ArgumentException)
            {
                Workspace.Status($"Chybný regulární výraz pro PageFlow: {pattern}");
            }
        }

        if (_pageFlowPatterns.Count == 0) return;

        core.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);

        core.WebResourceRequested += (_, e) =>
        {
            ScanForPageFlow(e.Request.Uri, "URL");

            var body = ReadStreamSafely(e.Request.Content);
            if (!string.IsNullOrEmpty(body))
                ScanForPageFlow(body, "požadavek " + ShortenUri(e.Request.Uri));
        };

        core.WebResourceResponseReceived += async (_, e) =>
        {
            if (!IsTextResponse(e.Response)) return;

            try
            {
                var stream = await e.Response.GetContentAsync();
                var text = ReadStreamSafely(stream);
                if (!string.IsNullOrEmpty(text))
                    ScanForPageFlow(text, "odpověď " + ShortenUri(e.Request.Uri));
            }
            catch
            {
                // obsah odpovědi nemusí být k dispozici – není důvod nic hlásit
            }
        };
    }

    private static bool IsTextResponse(CoreWebView2WebResourceResponseView response)
    {
        try
        {
            if (!response.Headers.Contains("Content-Type")) return false;

            var contentType = response.Headers.GetHeader("Content-Type");
            return contentType.Contains("json", StringComparison.OrdinalIgnoreCase)
                || contentType.Contains("xml", StringComparison.OrdinalIgnoreCase)
                || contentType.Contains("text", StringComparison.OrdinalIgnoreCase)
                || contentType.Contains("javascript", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private string? ReadStreamSafely(Stream? stream)
    {
        if (stream is null) return null;

        try
        {
            var limit = Math.Max(4096, Workspace.Settings.PageFlowScanLimitBytes);
            var position = stream.CanSeek ? stream.Position : 0L;

            var buffer = new byte[limit];
            var read = stream.Read(buffer, 0, buffer.Length);

            // Stream požadavku patří prohlížeči – vrátit ho tam, kde byl.
            if (stream.CanSeek)
                stream.Position = position;

            return read <= 0 ? null : Encoding.UTF8.GetString(buffer, 0, read);
        }
        catch
        {
            return null;
        }
    }

    private void ScanForPageFlow(string text, string source)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        foreach (var pattern in _pageFlowPatterns)
        {
            foreach (Match match in pattern.Matches(text))
            {
                if (!match.Success) continue;

                var value = match.Groups["pf"].Success
                    ? match.Groups["pf"].Value
                    : match.Groups.Count > 1 ? match.Groups[1].Value : match.Value;

                if (TpiNaming.LooksLikePageFlow(value))
                    Workspace.ReportPageFlowCandidate(value, source);
            }
        }
    }

    private static string ShortenUri(string uri)
    {
        if (string.IsNullOrEmpty(uri)) return string.Empty;

        try
        {
            var parsed = new Uri(uri);
            var path = parsed.AbsolutePath;
            return path.Length <= 60 ? path : "…" + path[^60..];
        }
        catch
        {
            return uri.Length <= 60 ? uri : uri[..60] + "…";
        }
    }

    // ----------------------------------------------------------- IBrowserHost

    public async Task NavigateAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;
        if (!url.Contains("://", StringComparison.Ordinal))
            url = "https://" + url;

        Workspace.Settings.LastUrl = url;

        if (!_ready)
        {
            Workspace.Status("WebView2 se ještě inicializuje…");
            return;
        }

        try
        {
            _webView.CoreWebView2.Navigate(url);
            Workspace.Status("Načítám " + url);
        }
        catch (Exception ex)
        {
            Workspace.Status("Chyba navigace: " + ex.Message);
        }

        await Task.CompletedTask;
    }

    public async Task SetPickModeAsync(bool on)
    {
        if (!_ready) return;
        await _webView.CoreWebView2.ExecuteScriptAsync($"__tpi.setPick({(on ? "true" : "false")})");
    }

    public async Task RequestTreeAsync()
    {
        if (!_ready) return;
        Workspace.Status("Načítám DOM strom…");
        await _webView.CoreWebView2.ExecuteScriptAsync("__tpi.sendTree()");
    }

    public async Task HighlightAsync(int domIndex)
    {
        if (!_ready || domIndex < 0) return;
        await _webView.CoreWebView2.ExecuteScriptAsync($"__tpi.highlight({domIndex})");
        await _webView.CoreWebView2.ExecuteScriptAsync($"__tpi.describeIndex({domIndex})");
    }

    public async Task<bool> ScrollToSelectorAsync(string cssSelector)
    {
        if (!_ready) return false;

        var script = "(function(){var e=document.querySelector(" +
                     JsonSerializer.Serialize(cssSelector) +
                     ");if(e){e.scrollIntoView({block:'center'});return true;}return false;})()";

        var result = await _webView.CoreWebView2.ExecuteScriptAsync(script);
        return string.Equals(result, "true", StringComparison.OrdinalIgnoreCase);
    }
}
