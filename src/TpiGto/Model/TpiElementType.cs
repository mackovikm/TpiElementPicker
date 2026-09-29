namespace TpiGto.Model;

/// <summary>
/// Základní třída typu prvku TPI.
/// <para>
/// <b>Přidání nového typu:</b> vytvoř novou třídu v namespace <c>TpiGto.Types</c>, která dědí
/// z této třídy, přepiš <see cref="Code"/>, <see cref="Name"/> a <see cref="DefineProperties"/>.
/// Registr typů si ji najde sám (reflexí), nic dalšího registrovat netřeba.
/// Alternativa bez rekompilace: doplnit typ do souboru <c>elementTypes.custom.json</c>.
/// </para>
/// </summary>
public abstract class TpiElementType
{
    private IReadOnlyList<GtoPropertyDefinition>? _properties;

    /// <summary>Interní kód typu, např. <c>LINEEDIT</c>. Musí být unikátní.</summary>
    public abstract string Code { get; }

    /// <summary>Název zobrazený uživateli.</summary>
    public abstract string Name { get; }

    /// <summary>Popis typu (k čemu ve frameworku TPI slouží).</summary>
    public virtual string Description => string.Empty;

    /// <summary>Nápověda k tvaru in_ref_element_path pro tento typ.</summary>
    public virtual string? PathHint => null;

    /// <summary>
    /// Název elementu, který tento typ má na obrazovce typu seznam – např. tabulka
    /// seznamu se ve frameworku vždy jmenuje <c>ObjectList</c>. Aplikace ho použije
    /// jako návrh in_ref_element_path. Null = odvozuje se z prvku stránky.
    /// </summary>
    public virtual string? ListScreenElementName => null;

    /// <summary>
    /// True, pokud element_path musí obsahovat i podřízený prvek – sloupec tabulky,
    /// položku menu nebo záložku. Bez něj by se změna vztahovala na celý element.
    /// </summary>
    public virtual bool RequiresSubElement => false;

    /// <summary>Jak se podřízený prvek jmenuje (pro nápovědu v UI).</summary>
    public virtual string SubElementLabel => "sloupec";

    /// <summary>
    /// True, pokud má element i widget (velikost, pozice, enabled) – registr pak
    /// k typu přidá vlastnosti M_Pf_Widget.*.
    /// </summary>
    public virtual bool IncludesWidgetProperties => false;

    /// <summary>
    /// True, pokud se za element_path doplňuje event (typ CONNECTION),
    /// tedy <c>&lt;element&gt;.&lt;event&gt;</c>.
    /// </summary>
    public virtual bool AppendsEvent => false;

    /// <summary>
    /// True, pokud se k typu přidávají i společné vlastnosti M_Pf_Element.* .
    /// </summary>
    public virtual bool IncludesCommonProperties => true;

    /// <summary>Pravidlo pro automatické nabídnutí typu podle kliknutého prvku.</summary>
    public virtual ElementMatchRule MatchRule => ElementMatchRule.None;

    /// <summary>Pořadí v nabídce (nižší = výše).</summary>
    public virtual int SortOrder => 100;

    /// <summary>Odkud definice pochází – pro uživatele (BUILT-IN / název JSON souboru).</summary>
    public virtual string Origin => "built-in";

    /// <summary>Vlastnosti specifické pro tento typ (bez společných M_Pf_Element.*).</summary>
    public IReadOnlyList<GtoPropertyDefinition> Properties
        => _properties ??= DefineProperties().ToList();

    /// <summary>Definice vlastností typu – jediné, co musí potomek doplnit.</summary>
    protected abstract IEnumerable<GtoPropertyDefinition> DefineProperties();

    public override string ToString() => Name;
}
