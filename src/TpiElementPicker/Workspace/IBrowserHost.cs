namespace TpiElementPicker.Workspace;

/// <summary>
/// Co umí okno s prohlížečem. Ostatní okna mluví s prohlížečem jen přes tento interface,
/// takže na sobě navzájem nezávisí.
/// </summary>
public interface IBrowserHost
{
    Task NavigateAsync(string url);

    Task SetPickModeAsync(bool on);

    Task RequestTreeAsync();

    Task HighlightAsync(int domIndex);

    Task<bool> ScrollToSelectorAsync(string cssSelector);
}
