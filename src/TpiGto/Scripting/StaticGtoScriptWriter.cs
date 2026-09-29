using System.Text;
using TpiGto.Mapping;

namespace TpiGto.Scripting;

/// <summary>
/// Iniciální (statické) GTO podle staticke_GTO.docx a Gattrib_Overload.docx – samostatný
/// skript s bloky SRV.GENDEF_OBJ.CREATE_GATTRIB_OVERLOAD. Konfigurace přežije
/// pregenerování PageFlow a použije se při jeho inicializaci.
/// </summary>
public sealed class StaticGtoScriptWriter : IGtoScriptWriter
{
    public GtoMode Mode => GtoMode.Static;

    public void WriteBlock(StringBuilder sb, GtoBlockContext ctx)
    {
        var o = ctx.Options;
        var i = o.Indent;
        var a = ctx.Assignment;

        if (!string.IsNullOrWhiteSpace(a.Note))
            sb.AppendLine($"{i}-- {a.Note}");

        // in_element_typ se plní jen když se element tímto overloadem teprve vytváří.
        var elementTyp = string.IsNullOrWhiteSpace(ctx.Element.CreateElementTyp)
            ? "NULL"
            : GtoValueFormatter.Quote(ctx.Element.CreateElementTyp);

        if (elementTyp != "NULL")
            sb.AppendLine($"{i}-- Vytvoření nového elementu typu {ctx.Element.CreateElementTyp} " +
                          "– nezapomeň ho zařadit pod existující element (M_Pf_Element.M_Pf_Element_Name).");

        sb.AppendLine($"{i}SRV.GENDEF_OBJ.CREATE_GATTRIB_OVERLOAD (");
        sb.AppendLine($"{i} in_pf_name               => {GtoValueFormatter.Quote(ctx.PfName)}");
        sb.AppendLine($"{i},in_ref_element_path      => {GtoValueFormatter.Quote(ctx.ElementPath)}");
        sb.AppendLine($"{i},in_gattrib_overload_name => {GtoValueFormatter.Quote(a.PropertyName)}");
        sb.AppendLine($"{i},in_gattrib_overload_typ  => {GtoValueFormatter.Quote(a.OverloadTypeName)}");
        sb.AppendLine($"{i},in_new_value             => {GtoValueFormatter.Quote(a.Value)}");
        sb.AppendLine($"{i},in_poradi                => {(a.Order <= 0 ? 1 : a.Order)}");
        sb.AppendLine($"{i},in_ref_login             => {Reference(a.RefLogin)}");
        sb.AppendLine($"{i},in_ref_role              => {Reference(a.RefRole)}");
        sb.AppendLine($"{i},in_element_typ           => {elementTyp}");
        sb.AppendLine($"{i},in_zruseno               => {(a.Cancelled == 1 ? 1 : 0)}");
        sb.AppendLine($"{i});");
        sb.AppendLine();
    }

    /// <summary>in_ref_login / in_ref_role – ID se zapisuje bez apostrofů.</summary>
    private static string Reference(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return "NULL";

        var trimmed = value.Trim();
        return trimmed.All(char.IsDigit) ? trimmed : GtoValueFormatter.Quote(trimmed);
    }

    public string Wrap(string blocks, GtoScriptOptions o, string pfName)
    {
        if (!o.IncludeScriptBody)
            return blocks.TrimEnd() + Environment.NewLine;

        var sb = new StringBuilder();

        if (o.AddOutputReminder)
        {
            sb.AppendLine("-- Před spuštěním zapni v developeru DBMS Output pro tuto databázi –");
            sb.AppendLine("-- jinak nepoznáš, že skript skončil OK (nepovolená hodnota projde bez chyby).");
            sb.AppendLine();
        }

        sb.AppendLine("DECLARE");
        sb.AppendLine("  l_krok_id            NUMBER;");
        sb.AppendLine("  l_msg_id_out         NUMBER;");
        sb.AppendLine("  l_info               VARCHAR2(256);");
        sb.AppendLine("  l_aplikacia          NUMBER;");
        sb.AppendLine("  l_m_tabulka          srv.gendef_obj.t_m_tabulka;");
        sb.AppendLine("  l_m_sloupec          srv.gendef_obj.t_m_sloupec;");
        sb.AppendLine("  l_m_cis_dom_dat_typ  srv.gendef_obj.t_m_cis_dom_dat_typ;");
        sb.AppendLine("  l_index              NUMBER;");
        sb.AppendLine("BEGIN");
        sb.AppendLine("  --");
        sb.AppendLine("  SRV.GENDEF_OBJ.INIT_MD_EDIT (");
        sb.AppendLine($"    in_user      => {GtoValueFormatter.Quote(o.User)}");
        sb.AppendLine($"   ,in_termin    => {o.Termin}");
        sb.AppendLine("  );");
        sb.AppendLine("  --");
        sb.AppendLine();
        sb.Append(blocks.TrimEnd());
        sb.AppendLine();
        sb.AppendLine();

        if (o.AppendCacheReset)
        {
            sb.AppendLine("  -- Reset cache page flow – bez něj se změna na obrazovce nemusí projevit.");
            if (string.IsNullOrWhiteSpace(o.CacheResetStatement))
            {
                sb.AppendLine("  -- TODO: doplň volání procedury pro reset cache page flow");
                sb.AppendLine("  --       (Nastavení → Skript → CacheResetStatement).");
            }
            else
            {
                var statement = o.CacheResetStatement
                    .Replace("{PF}", pfName ?? string.Empty, StringComparison.Ordinal)
                    .TrimEnd();

                foreach (var line in statement.Replace("\r\n", "\n").Split('\n'))
                    sb.AppendLine("  " + line);
            }

            sb.AppendLine();
        }

        sb.AppendLine("  --");
        sb.AppendLine("  SRV.GENDEF_OBJ.WAIT4MSG;");
        sb.AppendLine("  --");
        sb.AppendLine("END;");
        sb.AppendLine("/");
        return sb.ToString();
    }
}
