using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsDoctorAI.Models;

namespace WindowsDoctorAI.Services;

public class ReportExportService
{
    public async Task<string> ExportToHtmlAsync(List<DiagnosticResult> results,
        SystemHealthScore score, Dictionary<string, string> sysInfo)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html><html><head><meta charset=''utf-8''>");
        sb.AppendLine("<title>WindowsDoctorAI Report</title>");
        sb.AppendLine("<style>body{font-family:Segoe UI,sans-serif;margin:40px;background:#f5f5f5}");
        sb.AppendLine(".box{max-width:900px;margin:auto;background:#fff;padding:30px;border-radius:8px;box-shadow:0 2px 8px rgba(0,0,0,.1)}");
        sb.AppendLine("h1{color:#1a1a2e;border-bottom:3px solid #0078d4;padding-bottom:10px}");
        sb.AppendLine(".score{font-size:48px;font-weight:bold;text-align:center;padding:20px;border-radius:8px;margin:20px 0}");
        sb.AppendLine(".excellent{background:#e8f5e9;color:#2e7d32}.good{background:#e3f2fd;color:#1565c0}");
        sb.AppendLine(".fair{background:#fff3e0;color:#ef6c00}.poor{background:#fce4ec;color:#c62828}");
        sb.AppendLine(".r{border-left:4px solid;padding:12px 16px;margin:6px 0;border-radius:0 4px 4px 0}");
        sb.AppendLine(".Healthy{border-color:#4caf50;background:#f1f8e9}.Info{border-color:#2196f3;background:#e3f2fd}");
        sb.AppendLine(".Warning{border-color:#ff9800;background:#fff8e1}.Critical{border-color:#f44336;background:#ffebee}");
        sb.AppendLine(".act{background:#e8eaf6;padding:6px 10px;margin:3px 0;border-radius:4px;font-size:.85em}");
        sb.AppendLine("table{width:100%;border-collapse:collapse}td,th{padding:7px 12px;border-bottom:1px solid #eee;text-align:left}");
        sb.AppendLine("th{background:#f5f5f5;font-weight:600}</style></head><body><div class=''box''>");
        sb.AppendLine("<h1>WindowsDoctorAI - Diagnostic Report</h1>");
        sb.AppendLine($"<p style=''color:#666;font-size:.85em''>Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Duration: {score.ScanDuration.TotalSeconds:F1}s</p>");

        var cls = score.OverallScore >= 90 ? "excellent" : score.OverallScore >= 75 ? "good" : score.OverallScore >= 60 ? "fair" : "poor";
        sb.AppendLine($"<div class=''score {cls}''>{score.OverallScore:F0}/100 - {score.HealthRating}</div>");
        sb.AppendLine($"<p style=''text-align:center''>{score.HealthDescription}</p>");

        sb.AppendLine("<h2>Summary</h2><table>");
        sb.AppendLine($"<tr><th>Total Issues</th><td>{score.TotalIssues}</td></tr>");
        sb.AppendLine($"<tr><th>Critical</th><td style=''color:red''>{score.CriticalIssues}</td></tr>");
        sb.AppendLine($"<tr><th>Warnings</th><td style=''color:orange''>{score.WarningIssues}</td></tr>");
        sb.AppendLine($"<tr><th>Info</th><td style=''color:blue''>{score.InfoIssues}</td></tr></table>");

        sb.AppendLine("<h2>System Information</h2><table>");
        foreach (var kv in sysInfo) sb.AppendLine($"<tr><th>{kv.Key}</th><td>{kv.Value}</td></tr>");
        sb.AppendLine("</table>");

        foreach (var grp in results.GroupBy(r => r.CategoryDisplayName).OrderBy(g => g.Key))
        {
            sb.AppendLine($"<h2>{grp.Key}</h2>");
            foreach (var r in grp.OrderBy(x => x.Severity == Severity.Healthy ? 3 : x.Severity == Severity.Info ? 2 : x.Severity == Severity.Warning ? 1 : 0))
            {
                sb.AppendLine($"<div class=''r {r.Severity}''><strong>[{r.Severity}] {r.Title}</strong><br>{r.Description}");
                if (!string.IsNullOrEmpty(r.Details)) sb.AppendLine($"<br><small style=''color:#666''>{r.Details}</small>");
                foreach (var a in r.AvailableActions)
                    sb.AppendLine($"<div class=''act''>- {a.Name}: {a.Description} (Risk: {a.RiskLevel})</div>");
                sb.AppendLine("</div>");
            }
        }

        sb.AppendLine("<hr><p style=''text-align:center;color:#999;font-size:.8em''>WindowsDoctorAI System Diagnostic Tool</p>");
        sb.AppendLine("</div></body></html>");

        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            $"WindowsDoctorAI_Report_{DateTime.Now:yyyyMMdd_HHmmss}.html");
        await File.WriteAllTextAsync(path, sb.ToString());
        return path;
    }
}