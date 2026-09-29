using TpiGto.Model;

namespace TpiGto.Types;

/// <summary>Celá obrazovka / PageFlow okno.</summary>
public sealed class WindowElementType : TpiElementType
{
    public override string Code => "WINDOW";

    public override string Name => "Obrazovka (Window)";

    public override string Description => "Celá obrazovka / PageFlow okno.";

    public override int SortOrder => 10;

    public override ElementMatchRule MatchRule => new()
    {
        Tags = new[] { "body", "html" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Window.Title",
            "Text titulku obrazovky",
            GtoValueKind.Text);

        yield return new GtoPropertyDefinition(
            "M_Pf_Window.Readonly",
            "Readonly 0/1 pro celou obrazovku",
            GtoValueKind.Bool01, example: "1");
    }
}
