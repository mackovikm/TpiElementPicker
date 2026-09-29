namespace TpiGto.Model;

/// <summary>
/// Minimální popis prvku stránky, se kterým knihovna pracuje. Knihovna je díky tomu
/// nezávislá na WebView2 i na WinForms – hostitelská aplikace jen implementuje tento
/// interface nad svou reprezentací DOM uzlu.
/// </summary>
public interface IElementDescriptor
{
    /// <summary>Název tagu malými písmeny, např. <c>input</c>.</summary>
    string Tag { get; }

    /// <summary>Atribut id (může být prázdný).</summary>
    string? Id { get; }

    /// <summary>Atribut name (může být prázdný).</summary>
    string? Name { get; }

    /// <summary>U &lt;input&gt; hodnota atributu type.</summary>
    string? InputType { get; }

    /// <summary>Hodnota atributu role.</summary>
    string? Role { get; }

    /// <summary>Seznam CSS tříd.</summary>
    IReadOnlyList<string> Classes { get; }

    /// <summary>Všechny atributy prvku.</summary>
    IReadOnlyDictionary<string, string> Attributes { get; }

    /// <summary>Textový obsah (zkrácený).</summary>
    string? Text { get; }

    /// <summary>Rodič prvku, nebo null u kořene.</summary>
    IElementDescriptor? Parent { get; }
}
