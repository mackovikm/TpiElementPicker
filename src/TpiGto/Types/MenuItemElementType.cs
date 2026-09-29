using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Položka menu. Do cesty se přidává M_PF_DT_MENU_ITEM.NAME.</summary>
public sealed class MenuItemElementType : TpiElementType
{
    public override string Code => "MENU_ITEM";

    public override string Name => "Položka menu (Menu item)";

    public override string Description => "Položka menu. Do cesty se přidává M_PF_DT_MENU_ITEM.NAME.";

    public override int SortOrder => 150;

    public override string? PathHint => "MainMenu.<NÁZEV_POLOŽKY>, např. MainMenu.SAVE_AND_CLOSE";

    public override bool RequiresSubElement => true;

    public override string SubElementLabel => "položka menu (M_Pf_Dt_Menu_Item.Name)";

    public override ElementMatchRule MatchRule => new()
    {
        Roles = new[] { "menuitem" },
        ClassHints = new[] { "menu-item", "dropdown-item", "menuitem" },
        NameHints = new[] { "MainMenu", "MWF_" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Menu_Item.Visible",
            "Zobrazení položky menu 0/1",
            GtoValueKind.Bool01,
            example: "0", gmsgId: 15100825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Menu_Item.Text",
            "Text položky menu",
            GtoValueKind.Text,
            gmsgId: 5500825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Menu_Item.Flag",
            "Příznak položky menu",
            GtoValueKind.Text,
            gmsgId: 5400825);

        yield return new GtoPropertyDefinition(
            "M_Pf_Dt_Menu_Item.Indx",
            "Pořadí položky menu",
            GtoValueKind.Number,
            source: GtoPropertySource.CisGto);
    }
}
