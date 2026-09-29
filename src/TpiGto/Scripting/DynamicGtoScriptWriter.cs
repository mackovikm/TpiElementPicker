using System.Text;
using TpiGto.Mapping;
using TpiGto.Model;

namespace TpiGto.Scripting;

/// <summary>
/// Runtime GTO podle dynamicke_GTO.docx – makro <c>@GATTRIB_OVERLOAD</c>, které se
/// vkládá do theSql plain bloku MWF. V režimu <see cref="GtoMode.DynamicData"/> píše
/// <c>@GATTRIB_OVERLOAD_DATA</c>, které se provede až po namapování business dat do
/// GMSG. Volitelně obalí makra kostrou MWF podle frameworkTPI.docx.
/// </summary>
public sealed class DynamicGtoScriptWriter : IGtoScriptWriter
{
    private readonly string _macro;

    public DynamicGtoScriptWriter(GtoMode mode = GtoMode.Dynamic)
    {
        if (mode is not (GtoMode.Dynamic or GtoMode.DynamicData))
            throw new ArgumentOutOfRangeException(nameof(mode),
                "Zapisovač podporuje jen runtime režimy GTO.");

        Mode = mode;
        _macro = mode == GtoMode.DynamicData ? "@GATTRIB_OVERLOAD_DATA" : "@GATTRIB_OVERLOAD";
    }

    public GtoMode Mode { get; }

    public void WriteBlock(StringBuilder sb, GtoBlockContext ctx)
    {
        var a = ctx.Assignment;
        var kind = ctx.Registry.FindProperty(ctx.Element.TypeCode, a.PropertyName)?.ValueKind
                   ?? GtoValueKind.Text;

        if (!string.IsNullOrWhiteSpace(a.Note))
            sb.AppendLine($"-- {a.Note}");

        sb.AppendLine(
            $"{_macro}({GtoValueFormatter.Quote(a.PropertyName)}," +
            $"{GtoValueFormatter.Quote(ctx.ElementPath)}," +
            $"{GtoValueFormatter.ForMacro(a.Value, kind)});");
    }

    public string Wrap(string blocks, GtoScriptOptions o, string pfName)
    {
        var macros = blocks.TrimEnd();

        if (!o.WrapDynamicInMwf)
            return macros + Environment.NewLine;

        var sb = new StringBuilder();
        sb.AppendLine("DECLARE");
        sb.AppendLine("  --");
        sb.AppendLine("  l_krok_id    NUMBER;");
        sb.AppendLine("  l_msg_id_out NUMBER;");
        sb.AppendLine("  l_info       VARCHAR2(256);");
        sb.AppendLine("  --");
        sb.AppendLine("BEGIN");
        sb.AppendLine("  --");
        sb.AppendLine("  SRV.GENDEF_OBJ.INIT_MD_EDIT (");
        sb.AppendLine($"    in_user      => {GtoValueFormatter.Quote(o.User)}");
        sb.AppendLine($"   ,in_termin    => {o.Termin}");
        sb.AppendLine("  );");
        sb.AppendLine("  --");
        sb.AppendLine("  SRV.GENDEF_OBJ.DEFINE_MWF_STEP (");
        sb.AppendLine($"    step        => {GtoValueFormatter.Quote(o.MwfFirstStep)}");
        sb.AppendLine("   ,prevStep    => '0'");
        sb.AppendLine("   ,nextStep    => '0'");
        sb.AppendLine("   ,foreignStep => NULL");
        sb.AppendLine("   ,block_type  => 'PLAIN'");
        sb.AppendLine("   ,theSql      => q'[");
        foreach (var line in macros.Replace("\r\n", "\n").Split('\n'))
            sb.AppendLine("        " + line);
        sb.AppendLine("   ]'");
        sb.AppendLine($"   ,popis       => {GtoValueFormatter.Quote(o.MwfNotice)}");
        sb.AppendLine("  );");
        sb.AppendLine("  --");
        sb.AppendLine("  SRV.GENDEF_OBJ.CREATE_MWF (");
        sb.AppendLine($"    xMwfName      => {GtoValueFormatter.Quote(o.MwfName)},");
        sb.AppendLine($"    xMwfNotice    => {GtoValueFormatter.Quote(o.MwfNotice)},");
        sb.AppendLine($"    xMwfFirstStep => {GtoValueFormatter.Quote(o.MwfFirstStep)},");
        sb.AppendLine($"    xMwfType      => {GtoValueFormatter.Quote(o.MwfType)}");
        sb.AppendLine("  );");
        sb.AppendLine("  --");
        sb.AppendLine("  SRV.GENDEF_OBJ.WAIT4MSG;");
        sb.AppendLine("  --");
        sb.AppendLine("END;");
        sb.AppendLine("/");
        return sb.ToString();
    }
}
