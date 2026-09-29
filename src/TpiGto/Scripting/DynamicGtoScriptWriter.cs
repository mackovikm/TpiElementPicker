using System.Text;
using TpiGto.Mapping;
using TpiGto.Model;

namespace TpiGto.Scripting;

/// <summary>
/// Dynamické GTO podle dynamicke_GTO.docx – makro @GATTRIB_OVERLOAD, které se vkládá
/// do theSql plain bloku MWF. Volitelně obalí makra kostrou MWF podle frameworkTPI.docx.
/// </summary>
public sealed class DynamicGtoScriptWriter : IGtoScriptWriter
{
    public GtoMode Mode => GtoMode.Dynamic;

    public void WriteBlock(StringBuilder sb, GtoBlockContext ctx)
    {
        var a = ctx.Assignment;
        var kind = ctx.Registry.FindProperty(ctx.Element.TypeCode, a.PropertyName)?.ValueKind
                   ?? GtoValueKind.Text;

        if (!string.IsNullOrWhiteSpace(a.Note))
            sb.AppendLine($"-- {a.Note}");

        sb.AppendLine(
            $"@GATTRIB_OVERLOAD({GtoValueFormatter.Quote(a.PropertyName)}," +
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
