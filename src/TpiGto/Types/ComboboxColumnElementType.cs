using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Sloupec zobrazený v comboboxu.</summary>
public sealed class ComboboxColumnElementType : TpiElementType
{
    public override string Code => "COMBOBOX_COLUMN";

    public override string Name => "Sloupec comboboxu";

    public override string Description => "Sloupec zobrazený v comboboxu.";

    public override int SortOrder => 90;

    public override string? PathHint => "<element comboboxu>.<FYZICKÝ_NÁZEV_SLOUPCE>";

    public override bool RequiresColumn => true;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "option" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox_Column.Virtual_Column_Name",
            "Název sloupců zobrazených v comboboxu",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox_Column.Column_Indx",
            "Pořadí zobrazení sloupců v comboboxu",
            GtoValueKind.Number);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox_Column.Show_Column",
            "Zobrazení sloupce v comboboxu 0/1",
            GtoValueKind.Bool01, example: "1");

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox_Column.Sortorder",
            "Seřazení sloupce v elementu typu combobox",
            GtoValueKind.Number);
    }
}
