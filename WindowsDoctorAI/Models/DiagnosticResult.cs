using System;
using System.Collections.Generic;

namespace WindowsDoctorAI.Models;

public enum Severity { Healthy, Info, Warning, Critical }
public enum DiagnosticStatus { Pending, Running, Completed, Failed, Skipped }

public class DiagnosticResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DiagnosticCategory Category { get; set; }
    public string CategoryDisplayName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public Severity Severity { get; set; } = Severity.Healthy;
    public DiagnosticStatus Status { get; set; } = DiagnosticStatus.Pending;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public List<RepairAction> AvailableActions { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new();
    public double Score { get; set; } = 100.0;
    public string Source { get; set; } = string.Empty;
}