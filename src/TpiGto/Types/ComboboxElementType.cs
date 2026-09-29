using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Rozbalovací seznam. Podporuje event right_click.</summary>
public sealed class ComboboxElementType : TpiElementType
{
    public override string Code => "COMBOBOX";

    public override string Name => "Combobox";

    public override string Description => "Rozbalovací seznam. Podporuje event right_click.";

    public override int SortOrder => 80;

    public override bool IncludesWidgetProperties => true;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "select" },
        Roles = new[] { "combobox", "listbox" },
        ClassHints = new[] { "combo", "dropdown", "select2" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Combobox.Default_Index",
            "Výchozí vybraná položka",
            GtoValueKind.Number,
            gmsgId: 3000825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Combobox.Max_Visible_Items",
            "Maximální počet zobrazených položek",
            GtoValueKind.Number,
            example: "10", gmsgId: 3100825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Combobox.Show_Header",
            "Zobrazení hlavičky comboboxu 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 3200825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox.Readonly",
            "Readonly 0/1",
            GtoValueKind.Bool01,
            example: "1", gmsgId: 3400825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox.Current_Index",
            "Aktuálně vybraná položka",
            GtoValueKind.Number,
            gmsgId: 3500825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox.Rows_To_Show",
            "Počet zobrazených řádků",
            GtoValueKind.Number,
            example: "10", gmsgId: 3600825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox.Etl_Id",
            "ETL pro naplnění comboboxu",
            GtoValueKind.Number,
            gmsgId: 3700825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox.Tooltip",
            "Tooltip comboboxu",
            GtoValueKind.Text,
            source: GtoPropertySource.CisGto);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox.User_Edit",
            "Vynulovat combobox 0/1",
            GtoValueKind.Bool01,
            example: "0", source: GtoPropertySource.CisGto);
    }
}
