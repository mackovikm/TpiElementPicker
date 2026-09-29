namespace TpiGto.Model;

/// <summary>
/// Datový charakter hodnoty vlastnosti GTO (in_new_value). Určuje, jak se hodnota
/// validuje a jak se zapisuje do generovaného skriptu.
/// </summary>
public enum GtoValueKind
{
    /// <summary>Libovolný text. Do skriptu se zapíše v apostrofech.</summary>
    Text = 0,

    /// <summary>Hodnota 0/1.</summary>
    Bool01 = 1,

    /// <summary>Číslo (index, šířka, pořadí, margin…).</summary>
    Number = 2,

    /// <summary>CSS styl, např. 'background-color: #0062cc;border-color: #005cbf;'.</summary>
    Css = 3,

    /// <summary>Barva – CSS zápis barvy (#RRGGBB, rgb(), název).</summary>
    Color = 4,

    /// <summary>Název MWF, který se má spustit.</summary>
    MwfName = 5,

    /// <summary>Název jiného elementu (např. cílový element pro vnoření).</summary>
    ElementName = 6,

    /// <summary>
    /// SQL výraz / SELECT. Typicky jen u dynamického GTO – do makra se vkládá bez apostrofů.
    /// </summary>
    Sql = 7
}
