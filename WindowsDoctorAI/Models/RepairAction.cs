using System;
using System.Threading.Tasks;

namespace WindowsDoctorAI.Models;

public enum RepairActionType
{
    Automatic, Manual, CommandLine, RegistryFix,
    ServiceRestart, SystemTool, OpenSettings, FileOperation
}

public enum RepairStatus
{
    Pending, InProgress, Completed, Failed, RequiresReboot, Cancelled
}

public class RepairAction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RepairActionType ActionType { get; set; }
    public RepairStatus Status { get; set; } = RepairStatus.Pending;
    public string Command { get; set; } = string.Empty;
    public string Arguments { get; set; } = string.Empty;
    public bool RequiresAdmin { get; set; } = true;
    public bool RequiresReboot { get; set; } = false;
    public string RiskLevel { get; set; } = "Low";
    public string ResultMessage { get; set; } = string.Empty;
    public Func<Task<bool>>? ExecuteAsync { get; set; }
    public DiagnosticCategory RelatedCategory { get; set; }
    public int EstimatedTimeSeconds { get; set; } = 30;
}