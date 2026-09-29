using System.Text;
using TpiGto.Mapping;
using TpiGto.Registry;

namespace TpiGto.Scripting;

/// <summary>Kontext jednoho generovaného bloku GTO.</summary>
public sealed class GtoBlockContext
{
    public GtoBlockContext(string pfName, MappedElement element, GtoAssignment assignment,
        TpiTypeRegistry registry, GtoScriptOptions options)
    {
        PfName = pfName;
        Element = element;
        Assignment = assignment;
        Registry = registry;
        Options = options;
    }

    public string PfName { get; }
    public MappedElement Element { get; }
    public GtoAssignment Assignment { get; }
    public TpiTypeRegistry Registry { get; }
    public GtoScriptOptions Options { get; }

    public string ElementPath => Element.EffectivePath();
}

/// <summary>
/// Zapisovač skriptu pro jeden režim GTO. Nový výstupní formát = nová implementace
/// tohoto interface; generátor je vybírá podle <see cref="GtoMode"/>.
/// </summary>
public interface IGtoScriptWriter
{
    GtoMode Mode { get; }

    /// <summary>Zapíše jeden blok (jednu vlastnost).</summary>
    void WriteBlock(StringBuilder sb, GtoBlockContext context);

    /// <summary>Obalí bloky hlavičkou a patičkou skriptu.</summary>
    /// <param name="blocks">Vygenerované bloky.</param>
    /// <param name="options">Nastavení skriptu.</param>
    /// <param name="pfName">Název page flow – potřebný např. pro reset cache.</param>
    string Wrap(string blocks, GtoScriptOptions options, string pfName);
}
