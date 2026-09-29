using System.Reflection;
using TpiGto.Model;

namespace TpiGto.Registry;

/// <summary>
/// Najde reflexí všechny neabstraktní potomky <see cref="TpiElementType"/>
/// s bezparametrickým konstruktorem. Nový typ prvku tedy stačí přidat jako novou
/// třídu – nikde se neregistruje ručně.
/// </summary>
public sealed class BuiltInElementTypeProvider : ITpiElementTypeProvider
{
    private readonly Assembly _assembly;

    public BuiltInElementTypeProvider() : this(typeof(TpiElementType).Assembly)
    {
    }

    /// <summary>Umožňuje načíst typy i z jiné assembly (vlastní rozšíření).</summary>
    public BuiltInElementTypeProvider(Assembly assembly)
    {
        _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
    }

    public string Origin => _assembly.GetName().Name ?? "built-in";

    public IEnumerable<TpiElementType> GetTypes()
    {
        foreach (var type in _assembly.GetTypes())
        {
            if (type.IsAbstract || !typeof(TpiElementType).IsAssignableFrom(type))
                continue;

            if (type.GetConstructor(Type.EmptyTypes) is null)
                continue;

            TpiElementType? instance = null;
            try
            {
                instance = (TpiElementType?)Activator.CreateInstance(type);
            }
            catch
            {
                // vadný typ nesmí shodit načtení celého číselníku
            }

            if (instance is not null)
                yield return instance;
        }
    }
}
