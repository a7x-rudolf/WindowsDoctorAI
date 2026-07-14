using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WindowsDoctorAI.Helpers;
using WindowsDoctorAI.Models;

namespace WindowsDoctorAI.Services;

public class PerformanceDiagnosticService
{
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync()
    {
        var results = new List<DiagnosticResult>();
        results.AddRange(await CheckCpuUsageAsync());
        results.AddRange(CheckMemoryUsage());
        results.AddRange(CheckHighResourceProcesses());
        results.AddRange(CheckUptime());
        return results;
    }

    private async Task<List<DiagnosticResult>> CheckCpuUsageAsync()
    {
        var results = new List<DiagnosticResult>();
        try
        {
            using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            await Task.Delay(1000);
            var cpu = cpuCounter.NextValue();

            var result = new DiagnosticResult
            {
                Category = DiagnosticCategory.Performance, CategoryDisplayName = "Performance",
                Title = "CPU Usage", Details = $"Current: {cpu:F1}%",
                Status = DiagnosticStatus.Completed, Source = "PerformanceService"
            };

            if (cpu > 90)
            {
                result.Severity = Severity.Critical;
                result.Description = $"CPU critically high at {cpu:F1}%. System may be unresponsive.";
                result.Score = 15;
                result.AvailableActions.Add(new RepairAction
                {
                    Name = "Open Task Manager",
                    Description = "Identify and terminate high-CPU processes.",
                    ActionType = RepairActionType.SystemTool, Command = "taskmgr.exe",
                    RequiresAdmin = false, RiskLevel = "None",
                    RelatedCategory = DiagnosticCategory.Performance, EstimatedTimeSeconds = 5
                });
                result.AvailableActions.Add(new RepairAction
                {
                    Name = "Set Balanced Power Plan",
                    Description = "Switch to Balanced power plan.",
                    ActionType = RepairActionType.CommandLine, Command = "powercfg.exe",
                    Arguments = "/setactive 381b4222-f694-41f0-9685-ff5bb260df2e",
                    RequiresAdmin = true, RiskLevel = "Low",
                    RelatedCategory = DiagnosticCategory.Performance, EstimatedTimeSeconds = 5
                });
            }
            else if (cpu > 70)
            {
                result.Severity = Severity.Warning;
                result.Description = $"CPU elevated at {cpu:F1}%.";
                result.Score = 50;
                result.AvailableActions.Add(new RepairAction
                {
                    Name = "Open Task Manager",
                    Description = "Review running processes.",
                    ActionType = RepairActionType.SystemTool, Command = "taskmgr.exe",
                    RequiresAdmin = false, RiskLevel = "None",
                    RelatedCategory = DiagnosticCategory.Performance, EstimatedTimeSeconds = 5
                });
            }
            else
            {
                result.Severity = Severity.Healthy;
                result.Description = $"CPU normal at {cpu:F1}%.";
                result.Score = 100;
            }
            results.Add(result);
        }
        catch (Exception ex)
        {
            results.Add(new DiagnosticResult
            {
                Category = DiagnosticCategory.Performance, CategoryDisplayName = "Performance",
                Title = "CPU Check Error", Description = $"Error: {ex.Message}",
                Severity = Severity.Info, Status = DiagnosticStatus.Failed, Score = 80
            });
        }
        return results;
    }

    private List<DiagnosticResult> CheckMemoryUsage()
    {
        var results = new List<DiagnosticResult>();
        try
        {
            var data = WmiHelper.Query("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
            if (data.Count > 0)
            {
                var total = Convert.ToDouble(data[0]["TotalVisibleMemorySize"]);
                var free = Convert.ToDouble(data[0]["FreePhysicalMemory"]);
                var used = total - free;
                var pct = used / total * 100;
                var totalGb = total / 1024 / 1024;
                var usedGb = used / 1024 / 1024;
                var freeGb = free / 1024 / 1024;

                var result = new DiagnosticResult
                {
                    Category = DiagnosticCategory.Performance, CategoryDisplayName = "Performance",
                    Title = "Memory (RAM) Usage",
                    Details = $"Total: {totalGb:F1} GB | Used: {usedGb:F1} GB | Free: {freeGb:F1} GB ({pct:F1}%)",
                    Status = DiagnosticStatus.Completed, Source = "PerformanceService"
                };

                if (pct > 90)
                {
                    result.Severity = Severity.Critical;
                    result.Description = $"Memory critically high at {pct:F1}%.";
                    result.Score = 15;
                    result.AvailableActions.Add(new RepairAction
                    {
                        Name = "Open Task Manager",
                        Description = "Identify memory-hungry processes.",
                        ActionType = RepairActionType.SystemTool, Command = "taskmgr.exe",
                        RequiresAdmin = false, RiskLevel = "None",
                        RelatedCategory = DiagnosticCategory.Performance, EstimatedTimeSeconds = 5
                    });
                    result.AvailableActions.Add(new RepairAction
                    {
                        Name = "Open Resource Monitor",
                        Description = "View detailed memory usage.",
                        ActionType = RepairActionType.SystemTool, Command = "resmon.exe",
                        RequiresAdmin = false, RiskLevel = "None",
                        RelatedCategory = DiagnosticCategory.Performance, EstimatedTimeSeconds = 5
                    });
                }
                else if (pct > 75)
                {
                    result.Severity = Severity.Warning;
                    result.Description = $"Memory elevated at {pct:F1}%.";
                    result.Score = 55;
                    result.AvailableActions.Add(new RepairAction
                    {
                        Name = "Open Task Manager",
                        Description = "Review memory usage.",
                        ActionType = RepairActionType.SystemTool, Command = "taskmgr.exe",
                        RequiresAdmin = false, RiskLevel = "None",
                        RelatedCategory = DiagnosticCategory.Performance, EstimatedTimeSeconds = 5
                    });
                }
                else
                {
                    result.Severity = Severity.Healthy;
                    result.Description = $"Memory normal at {pct:F1}%.";
                    result.Score = 100;
                }
                results.Add(result);
            }
        }
        catch { }
        return results;
    }

    private List<DiagnosticResult> CheckHighResourceProcesses()
    {
        var results = new List<DiagnosticResult>();
        try
        {
            var procs = Process.GetProcesses()
                .Where(p => { try { return p.WorkingSet64 > 500 * 1024 * 1024; } catch { return false; } })
                .OrderByDescending(p => { try { return p.WorkingSet64; } catch { return 0L; } })
                .Take(5).ToList();

            if (procs.Count > 0)
            {
                var details = string.Join("\n", procs.Select(p =>
                {
                    try { return $"  {p.ProcessName} (PID:{p.Id}) - {p.WorkingSet64 / (1024 * 1024):N0} MB"; }
                    catch { return $"  {p.ProcessName}"; }
                }));
                var result = new DiagnosticResult
                {
                    Category = DiagnosticCategory.Performance, CategoryDisplayName = "Performance",
                    Title = "High Memory Processes",
                    Description = $"{procs.Count} process(es) using over 500 MB RAM.",
                    Details = details,
                    Severity = procs.Count > 3 ? Severity.Warning : Severity.Info,
                    Status = DiagnosticStatus.Completed,
                    Score = procs.Count > 3 ? 60 : 85, Source = "PerformanceService"
                };
                result.AvailableActions.Add(new RepairAction
                {
                    Name = "Open Resource Monitor",
                    Description = "View detailed process analysis.",
                    ActionType = RepairActionType.SystemTool, Command = "resmon.exe",
                    RequiresAdmin = false, RiskLevel = "None",
                    RelatedCategory = DiagnosticCategory.Performance, EstimatedTimeSeconds = 5
                });
                results.Add(result);
            }
        }
        catch { }
        return results;
    }

    private List<DiagnosticResult> CheckUptime()
    {
        var results = new List<DiagnosticResult>();
        try
        {
            var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
            var result = new DiagnosticResult
            {
                Category = DiagnosticCategory.Performance, CategoryDisplayName = "Performance",
                Title = "System Uptime",
                Details = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m",
                Status = DiagnosticStatus.Completed, Source = "PerformanceService"
            };

            if (uptime.TotalDays > 30)
            {
                result.Severity = Severity.Warning;
                result.Description = $"Running for {uptime.Days} days without restart. Reboot recommended.";
                result.Score = 50;
                result.AvailableActions.Add(new RepairAction
                {
                    Name = "Schedule Restart (5 min)",
                    Description = "Restart in 5 minutes to clear memory and apply updates.",
                    ActionType = RepairActionType.CommandLine, Command = "shutdown.exe",
                    Arguments = "/r /t 300 /c \"WindowsDoctorAI: Scheduled restart. Save your work.\"",
                    RequiresAdmin = true, RequiresReboot = true, RiskLevel = "Medium",
                    RelatedCategory = DiagnosticCategory.Performance, EstimatedTimeSeconds = 5
                });
            }
            else if (uptime.TotalDays > 7)
            {
                result.Severity = Severity.Info;
                result.Description = $"Uptime: {uptime.Days} days. Consider periodic restart.";
                result.Score = 80;
            }
            else
            {
                result.Severity = Severity.Healthy;
                result.Description = $"Uptime is {uptime.Days} days. Healthy.";
                result.Score = 100;
            }
            results.Add(result);
        }
        catch { }
        return results;
    }
}