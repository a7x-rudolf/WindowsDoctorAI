using System;
using System.Collections.Generic;

namespace WindowsDoctorAI.Models;

/// <summary>
/// Summary row for a single persisted scan session (Scan History list / trend).
/// The full per-item results for a session are loaded lazily via
/// ScanHistoryService.GetSessionResultsAsync(SessionId) when the user opens details.
/// </summary>
public class ScanHistoryEntry
{
    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public double OverallScore { get; set; }
    public string HealthRating { get; set; } = string.Empty;
    public int TotalIssues { get; set; }
    public int CriticalIssues { get; set; }
    public int WarningIssues { get; set; }
    public int InfoIssues { get; set; }
    public int HealthyCount { get; set; }
    public double ScanDurationSeconds { get; set; }

    public string DateDisplay => Timestamp.ToString("dd MMM yyyy, HH:mm");
    public string ScoreDisplay => $"{OverallScore:F0}";
    public string DurationDisplay => $"{ScanDurationSeconds:F1}";
    public string IssuesSummary => TotalIssues == 0
        ? "No issues found"
        : $"{TotalIssues} issue(s) — {CriticalIssues} critical, {WarningIssues} warning";
}

/// <summary>
/// A single diagnostic result attached to a persisted scan session, used when
/// rehydrating the full detail view for a past scan.
/// </summary>
public class ScanHistoryResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SessionId { get; set; } = string.Empty;
    public DiagnosticCategory Category { get; set; }
    public string CategoryDisplayName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public Severity Severity { get; set; }
    public double Score { get; set; }
}

public class ScanHistoryDetail
{
    public ScanHistoryEntry Session { get; set; } = new();
    public List<ScanHistoryResult> Results { get; set; } = new();
}
