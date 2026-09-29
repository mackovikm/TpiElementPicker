using WeifenLuo.WinFormsUI.Docking;
using TpiElementPicker.Workspace;

namespace TpiElementPicker.Forms.Docking;

/// <summary>
/// Základ dokovatelného okna. Potomek dostane workspace a v <see cref="BuildUi"/>
/// poskládá svůj obsah; v <see cref="Subscribe"/> se napojí na události workspace.
/// </summary>
public abstract class ToolWindowBase : DockContent
{
    protected ToolWindowBase(PickerWorkspace workspace)
    {
        Workspace = workspace;
        ShowHint = DefaultDockState;
        DockAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop
                    | DockAreas.DockBottom | DockAreas.Document | DockAreas.Float;
        HideOnClose = true;
    }

    protected PickerWorkspace Workspace { get; }

    /// <summary>Výchozí umístění okna při prvním spuštění.</summary>
    protected virtual DockState DefaultDockState => DockState.DockRight;

    /// <summary>
    /// Zavolej v konstruktoru potomka po nastavení Text/TabText – poskládá UI a
    /// napojí události.
    /// </summary>
    protected void Initialize()
    {
        BuildUi();
        Subscribe();
    }

    protected abstract void BuildUi();

    protected virtual void Subscribe()
    {
    }

    /// <summary>Spuštění akce na UI vlákně (události workspace mohou přijít odjinud).</summary>
    protected void OnUi(Action action)
    {
        if (IsDisposed) return;
        if (InvokeRequired) BeginInvoke(action);
        else action();
    }

    protected override string GetPersistString() => GetType().FullName ?? GetType().Name;
}
