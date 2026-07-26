using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsDoctorAI.Models;

namespace WindowsDoctorAI.Services;

public class DiagnosticEngine
{
    private readonly DiskHealthService _disk = new();
    private readonly PerformanceDiagnosticService _perf = new();
    private readonly NetworkDiagnosticService _net = new();
    private readonly SecurityDiagnosticService _sec = new();
    private readonly WindowsUpdateService _update = new();
    private readonly DriverDiagnosticService _driver = new();
    private readonly StartupOptimizerService _startup = new();

    public event Action<string>? OnCategoryStarted;
    public event Action<string, int>? OnProgressUpdated;
    public event Action<DiagnosticResult>? OnResultFound;

    public async Task<(List<DiagnosticResult> Results, SystemHealthScore Score)> RunFullDiagnosticAsync(
        CancellationToken ct = default)
    {
        var all = new List<DiagnosticResult>();
        var sw = Stopwatch.StartNew();

        var cats = new (string Name, Func<Task<List<DiagnosticResult>>> Fn)[]
        {
            ("Performance",      _perf.RunDiagnosticsAsync),
            ("Disk Health",      _disk.RunDiagnosticsAsync),
            ("Network",          _net.RunDiagnosticsAsync),
            ("Security",         _sec.RunDiagnosticsAsync),
            ("Windows Update",   _update.RunDiagnosticsAsync),
            ("Drivers",          _driver.RunDiagnosticsAsync),
            ("Startup Programs", _startup.RunDiagnosticsAsync),
        };

        for (int i = 0; i < cats.Length; i++)
        {
            ct.ThrowIfCancellationRequested();
            var (name, fn) = cats[i];
            OnCategoryStarted?.Invoke(name);
            OnProgressUpdated?.Invoke(name, (int)((double)i / cats.Length * 100));

            try
            {
                var res = await fn();
                foreach (var r in res) { all.Add(r); OnResultFound?.Invoke(r); }
            }
            catch (Exception ex)
            {
                all.Add(new DiagnosticResult
                {
                    Category = DiagnosticCategory.SystemFiles, CategoryDisplayName = name,
                    Title = $"{name} Scan Error", Description = $"Error: {ex.Message}",
                    Severity = Severity.Info, Status = DiagnosticStatus.Failed, Score = 80
                });
            }
        }

        OnProgressUpdated?.Invoke("Complete", 100);
        sw.Stop();

        var score = CalculateScore(all, sw.Elapsed);
        return (all, score);
    }

    private SystemHealthScore CalculateScore(List<DiagnosticResult> results, TimeSpan duration)
    {
        var s = new SystemHealthScore
        {
            ScanTime = DateTime.Now, ScanDuration = duration,
            TotalIssues = results.Count(r => r.Severity != Severity.Healthy),
            CriticalIssues = results.Count(r => r.Severity == Severity.Critical),
            WarningIssues = results.Count(r => r.Severity == Severity.Warning),
            InfoIssues = results.Count(r => r.Severity == Severity.Info)
        };

        foreach (var grp in results.GroupBy(r => r.Category))
            s.CategoryScores[grp.Key] = grp.Average(r => r.Score);

        s.OverallScore = s.CategoryScores.Count > 0 ? s.CategoryScores.Values.Average() : 100;
        s.OverallScore -= s.CriticalIssues * 5;
        s.OverallScore = Math.Max(0, Math.Min(100, s.OverallScore));
        return s;
    }
}