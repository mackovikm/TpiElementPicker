using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Sloupec zobrazený v comboboxu.</summary>
public sealed class ComboboxColumnElementType : TpiElementType
{
    public override string Code => "COMBOBOX_COLUMN";

    public override string Name => "Sloupec comboboxu";

    public override string Description => "Sloupec zobrazený v comboboxu.";

    public override int SortOrder => 90;

    public override string? PathHint => "<element comboboxu>.<SLOUPEC>";

    public override bool RequiresSubElement => true;

    public override string SubElementLabel => "sloupec comboboxu";

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "option" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox_Column.Virtual_Column_Name",
            "Název sloupce zobrazený v comboboxu",
            GtoValueKind.Text,
            gmsgId: 13300825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox_Column.Name",
            "Technický název sloupce",
            GtoValueKind.Text,
            gmsgId: 13400825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox_Column.Column_Indx",
            "Pořadí sloupce",
            GtoValueKind.Number,
            gmsgId: 13200825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox_Column.Show_Column",
            "Zobrazení sloupce 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 13500825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox_Column.M_Cis_Pf_Format",
            "Formát zobrazení sloupce",
            GtoValueKind.Text,
            gmsgId: 13600825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox_Column.Sortorder",
            "Seřazení sloupce",
            GtoValueKind.Number,
            source: GtoPropertySource.CisGto);
    }
}
