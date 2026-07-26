using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsDoctorAI.Helpers;
using WindowsDoctorAI.Models;

namespace WindowsDoctorAI.Services;

public class DriverDiagnosticService
{
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync()
    {
        var results = new List<DiagnosticResult>();
        results.AddRange(CheckProblemDevices());
        results.AddRange(await CheckDriversAsync());
        return results;
    }

    private List<DiagnosticResult> CheckProblemDevices()
    {
        var results = new List<DiagnosticResult>();
        try
        {
            var devices = WmiHelper.Query(
                "SELECT Name, DeviceID, ConfigManagerErrorCode FROM Win32_PnPEntity WHERE ConfigManagerErrorCode <> 0");

            if (devices.Count == 0)
            {
                results.Add(new DiagnosticResult
                {
                    Category = DiagnosticCategory.Drivers, CategoryDisplayName = "Drivers",
                    Title = "Device Driver Status",
                    Description = "All device drivers working properly.",
                    Severity = Severity.Healthy, Status = DiagnosticStatus.Completed,
                    Score = 100, Source = "DriverService"
                });
            }
            else
            {
                foreach (var dev in devices.Take(10))
                {
                    var name = dev.GetValueOrDefault("Name", "Unknown").ToString() ?? "Unknown";
                    var code = Convert.ToInt32(dev.GetValueOrDefault("ConfigManagerErrorCode", 0));
                    var devId = dev.GetValueOrDefault("DeviceID", "").ToString() ?? "";

                    var r = new DiagnosticResult
                    {
                        Category = DiagnosticCategory.Drivers, CategoryDisplayName = "Drivers",
                        Title = $"Problem Device: {name}",
                        Description = $"Device error Code {code}: {GetErrorDesc(code)}",
                        Details = $"Device ID: {devId}",
                        Severity = code == 22 ? Severity.Info : Severity.Warning,
                        Status = DiagnosticStatus.Completed,
                        Score = code == 22 ? 80 : 40, Source = "DriverService"
                    };
                    r.AvailableActions.Add(new RepairAction
                    {
                        Name = "Open Device Manager",
                        Description = $"Inspect and update driver for '{name}'.",
                        ActionType = RepairActionType.SystemTool, Command = "devmgmt.msc",
                        RequiresAdmin = true, RiskLevel = "None",
                        RelatedCategory = DiagnosticCategory.Drivers, EstimatedTimeSeconds = 5
                    });
                    if (code != 22)
                    {
                        r.AvailableActions.Add(new RepairAction
                        {
                            Name = "Scan for Hardware Changes",
                            Description = "Re-detect hardware and reinstall drivers.",
                            ActionType = RepairActionType.Automatic,
                            RequiresAdmin = true, RiskLevel = "Low",
                            RelatedCategory = DiagnosticCategory.Drivers, EstimatedTimeSeconds = 15,
                            ExecuteAsync = async () => {
                                await ProcessHelper.RunPowerShellAsync(
                                    "Get-PnpDevice | Where-Object {$_.Status -eq ''Error''} | Enable-PnpDevice -Confirm:$false -ErrorAction SilentlyContinue");
                                return true;
                            }
                        });
                    }
                    results.Add(r);
                }
            }
        }
        catch (Exception ex)
        {
            results.Add(new DiagnosticResult
            {
                Category = DiagnosticCategory.Drivers, CategoryDisplayName = "Drivers",
                Title = "Driver Check Error", Description = $"Error: {ex.Message}",
                Severity = Severity.Info, Status = DiagnosticStatus.Failed, Score = 80
            });
        }
        return results;
    }

    private async Task<List<DiagnosticResult>> CheckDriversAsync()
    {
        var results = new List<DiagnosticResult>();
        try
        {
            var out_ = await ProcessHelper.RunPowerShellAsync(
                "Get-WindowsDriver -Online -All | Where-Object {$_.Date -lt (Get-Date).AddYears(-2)} | Measure-Object | Select-Object -ExpandProperty Count",
                30000);

            if (int.TryParse(out_.Trim(), out var count) && count > 0)
            {
                var r = new DiagnosticResult
                {
                    Category = DiagnosticCategory.Drivers, CategoryDisplayName = "Drivers",
                    Title = "Outdated Drivers",
                    Description = $"{count} driver(s) older than 2 years detected.",
                    Details = "Consider updating via Windows Update or manufacturer website.",
                    Severity = count > 5 ? Severity.Warning : Severity.Info,
                    Status = DiagnosticStatus.Completed,
                    Score = count > 5 ? 60 : 75, Source = "DriverService"
                };
                r.AvailableActions.Add(new RepairAction
                {
                    Name = "Check Optional Driver Updates",
                    Description = "Open Windows Update to find driver updates.",
                    ActionType = RepairActionType.OpenSettings,
                    Command = "ms-settings:windowsupdate",
                    RequiresAdmin = false, RiskLevel = "None",
                    RelatedCategory = DiagnosticCategory.Drivers, EstimatedTimeSeconds = 5
                });
                r.AvailableActions.Add(new RepairAction
                {
                    Name = "Open Device Manager",
                    Description = "Review and update drivers in Device Manager.",
                    ActionType = RepairActionType.SystemTool, Command = "devmgmt.msc",
                    RequiresAdmin = true, RiskLevel = "None",
                    RelatedCategory = DiagnosticCategory.Drivers, EstimatedTimeSeconds = 5
                });
                results.Add(r);
            }
        }
        catch { }
        return results;
    }

    private string GetErrorDesc(int code) => code switch
    {
        1 => "Not configured correctly",
        3 => "Driver corrupted or missing",
        10 => "Cannot start",
        12 => "Not enough resources",
        14 => "Restart required",
        18 => "Reinstall drivers",
        22 => "Device is disabled",
        28 => "Drivers not installed",
        31 => "Not working properly",
        43 => "Device reported problems",
        _ => $"Unknown error (Code {code})"
    };
}