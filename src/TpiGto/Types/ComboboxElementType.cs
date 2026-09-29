using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Rozbalovací seznam.</summary>
public sealed class ComboboxElementType : TpiElementType
{
    public override string Code => "COMBOBOX";

    public override string Name => "Combobox";

    public override string Description => "Rozbalovací seznam.";

    public override int SortOrder => 80;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "select" },
        Roles = new[] { "combobox", "listbox" },
        ClassHints = new[] { "combo", "dropdown", "select2" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox.Readonly",
            "Readonly 0/1 pro element typu combobox",
            GtoValueKind.Bool01, example: "1");

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox.User_Edit",
            "Vynulovat combobox. 0/1",
            GtoValueKind.Bool01, example: "0");

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox.Tooltip",
            "Tooltip pro element typu combobox",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Combobox.Rows_To_Show",
            "Maximální počet řádků zobrazených v comboboxu",
            GtoValueKind.Number, example: "10");
    }
}
