using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Mapový element (tematizace). Podporuje event right_click a shift_click.</summary>
public sealed class MapElementType : TpiElementType
{
    public override string Code => "MAP";

    public override string Name => "Mapa";

    public override string Description => "Mapový element (tematizace). Podporuje event right_click a shift_click.";

    public override int SortOrder => 165;

    public override string? PathHint => "např. ThemeMap";

    public override bool IncludesWidgetProperties => true;

    public override ElementMatchRule MatchRule => new()
    {
        ClassHints = new[] { "map", "leaflet", "ol-map" },
        NameHints = new[] { "Map" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Map.Map_Position",
            "Výřez mapy – lze dosadit makrem @SDO_GET_MBR_FOR_TUDU / _PO / _TP",
            GtoValueKind.Sql,
            example: "@SDO_GET_MBR_FOR_TUDU(:TUDU)", source: GtoPropertySource.Documentation);
    }
}
