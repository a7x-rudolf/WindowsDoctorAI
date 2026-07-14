using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsDoctorAI.Models;
using WindowsDoctorAI.Services;

namespace WindowsDoctorAI.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly DiagnosticEngine _engine = new();
    private readonly RepairService _repair = new();
    private readonly SystemInfoService _sysInfo = new();
    private readonly ReportExportService _report = new();
    private CancellationTokenSource? _cts;
    private SystemHealthScore? _lastScore;

    [ObservableProperty] private double _overallScore;
    [ObservableProperty] private string _healthRating = "Not Scanned";
    [ObservableProperty] private string _healthDescription = "Run a diagnostic scan to check your system health.";
    [ObservableProperty] private string _currentCategory = string.Empty;
    [ObservableProperty] private int _progressPercent;
    [ObservableProperty] private bool _isScanning;
    [ObservableProperty] private bool _hasScanResults;
    [ObservableProperty] private int _totalIssues;
    [ObservableProperty] private int _criticalCount;
    [ObservableProperty] private int _warningCount;
    [ObservableProperty] private int _healthyCount;
    [ObservableProperty] private string _scanDuration = string.Empty;

    public ObservableCollection<DiagnosticResult> AllResults { get; } = new();
    public ObservableCollection<RepairAction> PendingRepairs { get; } = new();
    public Dictionary<string, string> SystemInfo { get; private set; } = new();

    // ===== Events for UI to hook into for dialogs =====
    public event Action? ScanRequested;             // UI opens ScanProgressDialog
    public event Action<string>? CategoryStarted;
    public event Action<string>? CategoryCompleted;
    public event Action<int>? ProgressUpdated;
    public event Action<DiagnosticResult>? ResultFound;
    public event Action<SystemHealthScore>? ScanCompleted;
    public event Action? ScanCancelled;
    public event Action<string>? ScanFailed;

    public event Action<RepairAction>? RepairStarted;    // UI opens RepairProgressDialog
    public event Action<RepairAction, string>? RepairProgress;
    public event Action<RepairAction, bool>? RepairCompleted;

    public MainViewModel()
    {
        _engine.OnCategoryStarted += c =>
        {
            CurrentCategory = c;
            CategoryStarted?.Invoke(c);
        };
        _engine.OnProgressUpdated += (c, p) =>
        {
            ProgressPercent = p;
            ProgressUpdated?.Invoke(p);
        };
        _engine.OnResultFound += r =>
        {
            ResultFound?.Invoke(r);
        };

        _repair.OnProgress += (a, msg) =>
        {
            StatusMessage = msg;
            RepairProgress?.Invoke(a, msg);
        };
    }

    [RelayCommand]
    private async Task RunScanAsync()
    {
        if (IsScanning) return;

        IsScanning = true;
        IsBusy = true;
        HasScanResults = false;
        AllResults.Clear();
        PendingRepairs.Clear();
        StatusMessage = "Starting diagnostic scan...";

        // Fire event to open dialog
        ScanRequested?.Invoke();

        _cts = new CancellationTokenSource();

        try
        {
            SystemInfo = _sysInfo.GetSystemInfo();

            // Track category completions
            string? lastCat = null;
            _engine.OnCategoryStarted += CatStartHandler;

            void CatStartHandler(string cat)
            {
                if (lastCat != null && lastCat != cat)
                    CategoryCompleted?.Invoke(lastCat);
                lastCat = cat;
            }

            var (results, score) = await _engine.RunFullDiagnosticAsync(_cts.Token);

            _engine.OnCategoryStarted -= CatStartHandler;

            // Complete last category
            if (lastCat != null)
                CategoryCompleted?.Invoke(lastCat);

            _lastScore = score;

            foreach (var r in results)
            {
                AllResults.Add(r);
                if (r.Severity != Severity.Healthy)
                    foreach (var a in r.AvailableActions)
                        PendingRepairs.Add(a);
            }

            OverallScore = Math.Round(score.OverallScore);
            HealthRating = score.HealthRating;
            HealthDescription = score.HealthDescription;
            TotalIssues = score.TotalIssues;
            CriticalCount = score.CriticalIssues;
            WarningCount = score.WarningIssues;
            HealthyCount = results.Count(r => r.Severity == Severity.Healthy);
            ScanDuration = $"{score.ScanDuration.TotalSeconds:F1}s";
            HasScanResults = true;
            StatusMessage = $"Scan complete. Found {TotalIssues} issue(s).";

            ScanCompleted?.Invoke(score);
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Scan cancelled.";
            ScanCancelled?.Invoke();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            ScanFailed?.Invoke(ex.Message);
        }
        finally
        {
            IsScanning = false;
            IsBusy = false;
            ProgressPercent = 100;
            CurrentCategory = string.Empty;
        }
    }

    [RelayCommand]
    private void CancelScan() => _cts?.Cancel();

    [RelayCommand]
    private async Task ExecuteRepairAsync(RepairAction? action)
    {
        if (action == null) return;
        IsBusy = true;
        StatusMessage = $"Executing: {action.Name}...";
        RepairStarted?.Invoke(action);

        try
        {
            var success = await _repair.ExecuteAsync(action);
            RepairCompleted?.Invoke(action, success);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            RepairCompleted?.Invoke(action, false);
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task ExecuteAllRepairsAsync()
    {
        if (PendingRepairs.Count == 0) return;
        IsBusy = true;
        int ok = 0, fail = 0;
        var safe = PendingRepairs
            .Where(r => r.ActionType == RepairActionType.Automatic &&
                       (r.RiskLevel == "Low" || r.RiskLevel == "None") &&
                        r.Status == RepairStatus.Pending).ToList();

        StatusMessage = $"Executing {safe.Count} safe repairs...";
        foreach (var a in safe)
        {
            try { if (await _repair.ExecuteAsync(a)) ok++; else fail++; }
            catch { fail++; }
        }
        StatusMessage = $"Done. Success: {ok}, Failed: {fail}";
        IsBusy = false;
    }

    [RelayCommand]
    private async Task ExportReportAsync()
    {
        if (!HasScanResults || _lastScore == null) return;
        IsBusy = true;
        StatusMessage = "Generating report...";
        try
        {
            var path = await _report.ExportToHtmlAsync([.. AllResults], _lastScore, SystemInfo);
            StatusMessage = $"Report saved: {path}";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        catch (Exception ex) { StatusMessage = $"Export error: {ex.Message}"; }
        finally { IsBusy = false; }
    }
}