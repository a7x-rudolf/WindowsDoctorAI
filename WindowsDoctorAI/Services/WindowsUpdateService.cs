using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading.Tasks;
using WindowsDoctorAI.Helpers;
using WindowsDoctorAI.Models;

namespace WindowsDoctorAI.Services;

public class WindowsUpdateService
{
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync()
    {
        var results = new List<DiagnosticResult>();
        results.AddRange(CheckUpdateService());
        results.AddRange(await CheckLastUpdateAsync());
        results.AddRange(CheckPendingReboot());
        return results;
    }

    private List<DiagnosticResult> CheckUpdateService()
    {
        var results = new List<DiagnosticResult>();
        try
        {
            using var sc = new ServiceController("wuauserv");
            var r = new DiagnosticResult
            {
                Category = DiagnosticCategory.WindowsUpdate, CategoryDisplayName = "Windows Update",
                Title = "Windows Update Service",
                Details = $"Status: {sc.Status} | Start: {sc.StartType}",
                Status = DiagnosticStatus.Completed, Source = "UpdateService"
            };

            if (sc.StartType == ServiceStartMode.Disabled)
            {
                r.Severity = Severity.Critical;
                r.Description = "Windows Update service is DISABLED. No security updates!";
                r.Score = 10;
                r.AvailableActions.Add(new RepairAction
                {
                    Name = "Enable Windows Update Service",
                    Description = "Set to automatic and start service.",
                    ActionType = RepairActionType.Automatic,
                    RequiresAdmin = true, RiskLevel = "Low",
                    RelatedCategory = DiagnosticCategory.WindowsUpdate, EstimatedTimeSeconds = 10,
                    ExecuteAsync = async () => {
                        await ProcessHelper.RunCommandAsync("sc", "config wuauserv start= auto");
                        var r = await ProcessHelper.RunCommandAsync("net", "start wuauserv");
                        return r.ExitCode == 0;
                    }
                });
            }
            else if (sc.Status == ServiceControllerStatus.Running)
            {
                r.Severity = Severity.Healthy;
                r.Description = "Windows Update service is running normally.";
                r.Score = 100;
            }
            else
            {
                r.Severity = Severity.Warning;
                r.Description = $"Windows Update service status: {sc.Status}.";
                r.Score = 60;
                r.AvailableActions.Add(new RepairAction
                {
                    Name = "Start Windows Update Service",
                    Description = "Start the wuauserv service.",
                    ActionType = RepairActionType.ServiceRestart, Command = "net",
                    Arguments = "start wuauserv",
                    RequiresAdmin = true, RiskLevel = "Low",
                    RelatedCategory = DiagnosticCategory.WindowsUpdate, EstimatedTimeSeconds = 10
                });
            }
            results.Add(r);
        }
        catch { }
        return results;
    }

    private async Task<List<DiagnosticResult>> CheckLastUpdateAsync()
    {
        var results = new List<DiagnosticResult>();
        try
        {
            var output = await ProcessHelper.RunPowerShellAsync(
                "(Get-HotFix | Sort-Object InstalledOn -Descending | Select-Object -First 1).InstalledOn.ToString(''yyyy-MM-dd'')",
                30000);

            if (DateTime.TryParse(output.Trim(), out var last))
            {
                var days = (DateTime.Now - last).TotalDays;
                var r = new DiagnosticResult
                {
                    Category = DiagnosticCategory.WindowsUpdate, CategoryDisplayName = "Windows Update",
                    Title = "Last Windows Update",
                    Details = $"Last updated: {last:yyyy-MM-dd} ({days:F0} days ago)",
                    Status = DiagnosticStatus.Completed, Source = "UpdateService"
                };

                if (days > 60)
                {
                    r.Severity = Severity.Critical;
                    r.Description = $"No updates for {days:F0} days! System vulnerable.";
                    r.Score = 15;
                    r.AvailableActions.Add(new RepairAction
                    {
                        Name = "Open Windows Update",
                        Description = "Check and install available updates.",
                        ActionType = RepairActionType.OpenSettings, Command = "ms-settings:windowsupdate",
                        RequiresAdmin = false, RiskLevel = "None",
                        RelatedCategory = DiagnosticCategory.WindowsUpdate, EstimatedTimeSeconds = 5
                    });
                    r.AvailableActions.Add(new RepairAction
                    {
                        Name = "Force Update Check",
                        Description = "Trigger Windows Update scan.",
                        ActionType = RepairActionType.Automatic,
                        RequiresAdmin = true, RiskLevel = "None",
                        RelatedCategory = DiagnosticCategory.WindowsUpdate, EstimatedTimeSeconds = 30,
                        ExecuteAsync = async () => {
                            await ProcessHelper.RunPowerShellAsync(
                                "(New-Object -ComObject Microsoft.Update.AutoUpdate).DetectNow()");
                            return true;
                        }
                    });
                }
                else if (days > 30)
                {
                    r.Severity = Severity.Warning;
                    r.Description = $"Last update was {days:F0} days ago.";
                    r.Score = 55;
                    r.AvailableActions.Add(new RepairAction
                    {
                        Name = "Open Windows Update",
                        Description = "Check for available updates.",
                        ActionType = RepairActionType.OpenSettings, Command = "ms-settings:windowsupdate",
                        RequiresAdmin = false, RiskLevel = "None",
                        RelatedCategory = DiagnosticCategory.WindowsUpdate, EstimatedTimeSeconds = 5
                    });
                }
                else
                {
                    r.Severity = Severity.Healthy;
                    r.Description = $"System updated {days:F0} days ago. Up to date.";
                    r.Score = 100;
                }
                results.Add(r);
            }
        }
        catch { }
        return results;
    }

    private List<DiagnosticResult> CheckPendingReboot()
    {
        var results = new List<DiagnosticResult>();
        try
        {
            var val = RegistryHelper.GetValue(Microsoft.Win32.RegistryHive.LocalMachine,
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\RebootRequired", null);

            if (val != null)
            {
                var r = new DiagnosticResult
                {
                    Category = DiagnosticCategory.WindowsUpdate, CategoryDisplayName = "Windows Update",
                    Title = "Pending Reboot for Updates",
                    Description = "A restart is required to complete Windows Update installation.",
                    Severity = Severity.Warning, Status = DiagnosticStatus.Completed,
                    Score = 60, Source = "UpdateService"
                };
                r.AvailableActions.Add(new RepairAction
                {
                    Name = "Schedule Restart (5 min)",
                    Description = "Restart in 5 minutes to apply updates.",
                    ActionType = RepairActionType.CommandLine, Command = "shutdown.exe",
                    Arguments = "/r /t 300 /c \"Applying Windows Updates. Save your work.\"",
                    RequiresAdmin = true, RequiresReboot = true, RiskLevel = "Medium",
                    RelatedCategory = DiagnosticCategory.WindowsUpdate, EstimatedTimeSeconds = 5
                });
                results.Add(r);
            }
        }
        catch { }
        return results;
    }
}