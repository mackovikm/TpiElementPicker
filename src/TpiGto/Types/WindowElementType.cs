using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Celá obrazovka. Sem patří i nastavení vstupních bodů PageFlow (M_Pf.*), které se zadávají na element Window.</summary>
public sealed class WindowElementType : TpiElementType
{
    public override string Code => "WINDOW";

    public override string Name => "Obrazovka (Window)";

    public override string Description => "Celá obrazovka. Sem patří i nastavení vstupních bodů PageFlow (M_Pf.*), které se zadávají na element Window.";

    public override int SortOrder => 10;

    public override string? PathHint => "Window";

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "body", "html" },
        NameHints = new[] { "Window" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Window.Title",
            "Text titulku obrazovky",
            GtoValueKind.Text,
            gmsgId: 9400825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Window.Icon",
            "Ikona obrazovky",
            GtoValueKind.Text,
            gmsgId: 9300825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Window.Show_External",
            "Zobrazení mimo hlavní okno 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 9000825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Window.Ss_Layout_Sortorder",
            "Pořadí v layoutu service selektoru",
            GtoValueKind.Number,
            gmsgId: 9100825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Window.M_Pf_Cis_Ss_Layout",
            "Layout service selektoru",
            GtoValueKind.Text,
            gmsgId: 9200825);

        yield return new GtoPropertyDefinition(
            "Window.Show_external",
            "Zobrazení mimo hlavní okno – starší varianta téhož parametru",
            GtoValueKind.Bool01,
            gmsgId: 100825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Window.Readonly",
            "Readonly 0/1 pro celou obrazovku",
            GtoValueKind.Bool01,
            example: "1", source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf.Mwf_Name_Init",
            "MWF spuštěný při inicializaci PageFlow. Nelze konfigurovat v runtime.",
            GtoValueKind.MwfName,
            allowDynamic: false, example: "NAZOV_MWF", source: GtoPropertySource.Documentation);

        yield return new GtoPropertyDefinition(
            "M_Pf.M_Mwf_Name_Reinit",
            "MWF spuštěný při reinicializaci (F5 / @REFRESH_PAGE_FLOW). Lze i v runtime.",
            GtoValueKind.MwfName,
            source: GtoPropertySource.Documentation);

        yield return new GtoPropertyDefinition(
            "M_Pf.Mwf_Name_Leave",
            "MWF spuštěný při zavření PageFlow uživatelem",
            GtoValueKind.MwfName,
            source: GtoPropertySource.Documentation);

        yield return new GtoPropertyDefinition(
            "M_Pf.M_Mwf_Name_Leave_Sys",
            "Systémový MWF při zavření PageFlow – konfiguruje systém",
            GtoValueKind.MwfName,
            source: GtoPropertySource.Documentation);
    }
}
