using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Položka menu, např. MainMenu.PZSV_MWF_POSUN_TERMINU_ETAPY.</summary>
public sealed class MenuItemElementType : TpiElementType
{
    public override string Code => "MENU_ITEM";

    public override string Name => "Položka menu (Menu item)";

    public override string Description => "Položka menu, např. MainMenu.PZSV_MWF_POSUN_TERMINU_ETAPY.";

    public override int SortOrder => 150;

    public override string? PathHint => "MainMenu.<NAZEV_POLOZKY>";

    public override ElementMatchRule MatchRule => new()
    {
        Roles = new[] { "menuitem" },
        ClassHints = new[] { "menu-item", "dropdown-item", "menuitem" },
        NameHints = new[] { "MainMenu", "MWF_" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Menu_Item.Text",
            "Definice textu menu",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Menu_Item.Visible",
            "Zobrazení položky menu 0/1",
            GtoValueKind.Bool01, example: "0");

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Menu_Item.Indx",
            "Pořadí zobrazení položky menu",
            GtoValueKind.Number);
    }
}
