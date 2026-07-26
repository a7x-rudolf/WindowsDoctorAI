using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading.Tasks;
using WindowsDoctorAI.Helpers;
using WindowsDoctorAI.Models;

namespace WindowsDoctorAI.Services;

public class SecurityDiagnosticService
{
    // Fix CS1998: tambah await Task.Run agar benar-benar async
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync()
    {
        var results = new List<DiagnosticResult>();

        // Jalankan checks di background thread agar tidak blocking UI
        await Task.Run(() =>
        {
            results.AddRange(CheckAntivirus());
            results.AddRange(CheckFirewall());
            results.AddRange(CheckUac());
            results.AddRange(CheckSecurityServices());
        });

        return results;
    }

    private List<DiagnosticResult> CheckAntivirus()
    {
        var results = new List<DiagnosticResult>();
        try
        {
            var avData = WmiHelper.Query(
                "SELECT displayName, productState FROM AntiVirusProduct",
                @"\\.\root\SecurityCenter2");

            if (avData.Count == 0)
            {
                var r = new DiagnosticResult
                {
                    Category = DiagnosticCategory.Security,
                    CategoryDisplayName = "Security",
                    Title = "Antivirus Status",
                    Description = "No antivirus detected! System unprotected.",
                    Severity = Severity.Critical,
                    Status = DiagnosticStatus.Completed,
                    Score = 0,
                    Source = "SecurityService"
                };
                r.AvailableActions.Add(new RepairAction
                {
                    Name = "Open Windows Security",
                    Description = "Enable Windows Defender or install antivirus.",
                    ActionType = RepairActionType.OpenSettings,
                    Command = "ms-settings:windowsdefender",
                    RequiresAdmin = false,
                    RiskLevel = "None",
                    RelatedCategory = DiagnosticCategory.Security,
                    EstimatedTimeSeconds = 5
                });
                results.Add(r);
            }
            else
            {
                foreach (var av in avData)
                {
                    var name = av.GetValueOrDefault("displayName", "Unknown").ToString() ?? "Unknown";
                    var state = Convert.ToInt32(av.GetValueOrDefault("productState", 0));
                    bool enabled = ((state >> 12) & 0xF) == 1;
                    bool upToDate = ((state >> 4) & 0xF) == 0;

                    var r = new DiagnosticResult
                    {
                        Category = DiagnosticCategory.Security,
                        CategoryDisplayName = "Security",
                        Title = $"Antivirus: {name}",
                        Details = $"Enabled: {enabled} | Up-to-date: {upToDate}",
                        Status = DiagnosticStatus.Completed,
                        Source = "SecurityService"
                    };

                    if (!enabled)
                    {
                        r.Severity = Severity.Critical;
                        r.Description = $"{name} is NOT enabled!";
                        r.Score = 10;
                        r.AvailableActions.Add(new RepairAction
                        {
                            Name = "Open Windows Security",
                            Description = "Enable real-time protection.",
                            ActionType = RepairActionType.OpenSettings,
                            Command = "ms-settings:windowsdefender",
                            RequiresAdmin = false,
                            RiskLevel = "None",
                            RelatedCategory = DiagnosticCategory.Security,
                            EstimatedTimeSeconds = 5
                        });
                    }
                    else if (!upToDate)
                    {
                        r.Severity = Severity.Warning;
                        r.Description = $"{name} definitions are outdated.";
                        r.Score = 50;
                        r.AvailableActions.Add(new RepairAction
                        {
                            Name = "Update Defender Definitions",
                            Description = "Download latest virus definitions.",
                            ActionType = RepairActionType.Automatic,
                            RequiresAdmin = true,
                            RiskLevel = "None",
                            RelatedCategory = DiagnosticCategory.Security,
                            EstimatedTimeSeconds = 60,
                            ExecuteAsync = async () =>
                            {
                                await ProcessHelper.RunPowerShellAsync("Update-MpSignature");
                                return true;
                            }
                        });
                    }
                    else
                    {
                        r.Severity = Severity.Healthy;
                        r.Description = $"{name} is enabled and up-to-date.";
                        r.Score = 100;
                    }
                    results.Add(r);
                }
            }
        }
        catch (Exception ex)
        {
            results.Add(new DiagnosticResult
            {
                Category = DiagnosticCategory.Security,
                CategoryDisplayName = "Security",
                Title = "Antivirus Check Error",
                Description = $"Error: {ex.Message}",
                Severity = Severity.Info,
                Status = DiagnosticStatus.Failed,
                Score = 70
            });
        }
        return results;
    }

    private List<DiagnosticResult> CheckFirewall()
    {
        var results = new List<DiagnosticResult>();
        try
        {
            using var sc = new ServiceController("MpsSvc");
            var r = new DiagnosticResult
            {
                Category = DiagnosticCategory.Security,
                CategoryDisplayName = "Security",
                Title = "Windows Firewall",
                Details = $"Service: MpsSvc | Status: {sc.Status}",
                Status = DiagnosticStatus.Completed,
                Source = "SecurityService"
            };

            if (sc.Status == ServiceControllerStatus.Running)
            {
                r.Severity = Severity.Healthy;
                r.Description = "Windows Firewall is running.";
                r.Score = 100;
            }
            else
            {
                r.Severity = Severity.Critical;
                r.Description = "Windows Firewall is NOT running!";
                r.Score = 10;
                r.AvailableActions.Add(new RepairAction
                {
                    Name = "Start Firewall Service",
                    Description = "Start the Windows Firewall service.",
                    ActionType = RepairActionType.ServiceRestart,
                    Command = "net",
                    Arguments = "start MpsSvc",
                    RequiresAdmin = true,
                    RiskLevel = "Low",
                    RelatedCategory = DiagnosticCategory.Security,
                    EstimatedTimeSeconds = 10,
                    ExecuteAsync = async () =>
                    {
                        var res = await ProcessHelper.RunCommandAsync("net", "start MpsSvc");
                        return res.ExitCode == 0;
                    }
                });
                r.AvailableActions.Add(new RepairAction
                {
                    Name = "Open Firewall Settings",
                    Description = "Open Windows Security firewall page.",
                    ActionType = RepairActionType.OpenSettings,
                    Command = "ms-settings:windowsdefender",
                    RequiresAdmin = false,
                    RiskLevel = "None",
                    RelatedCategory = DiagnosticCategory.Security,
                    EstimatedTimeSeconds = 5
                });
            }
            results.Add(r);
        }
        catch { }
        return results;
    }

    private List<DiagnosticResult> CheckUac()
    {
        var results = new List<DiagnosticResult>();
        try
        {
            var val = RegistryHelper.GetValue(
                Microsoft.Win32.RegistryHive.LocalMachine,
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",
                "EnableLUA");

            var r = new DiagnosticResult
            {
                Category = DiagnosticCategory.Security,
                CategoryDisplayName = "Security",
                Title = "User Account Control (UAC)",
                Status = DiagnosticStatus.Completed,
                Source = "SecurityService"
            };

            if (val != null && Convert.ToInt32(val) == 1)
            {
                r.Severity = Severity.Healthy;
                r.Description = "UAC is enabled. System is protected.";
                r.Score = 100;
            }
            else
            {
                r.Severity = Severity.Critical;
                r.Description = "UAC is DISABLED! Malware can make unrestricted changes.";
                r.Score = 15;
                r.AvailableActions.Add(new RepairAction
                {
                    Name = "Enable UAC",
                    Description = "Re-enable User Account Control. Requires reboot.",
                    ActionType = RepairActionType.RegistryFix,
                    RequiresAdmin = true,
                    RequiresReboot = true,
                    RiskLevel = "Low",
                    RelatedCategory = DiagnosticCategory.Security,
                    EstimatedTimeSeconds = 5,
                    ExecuteAsync = async () => await Task.Run(() =>
                        RegistryHelper.SetValue(
                            Microsoft.Win32.RegistryHive.LocalMachine,
                            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",
                            "EnableLUA", 1,
                            Microsoft.Win32.RegistryValueKind.DWord))
                });
                r.AvailableActions.Add(new RepairAction
                {
                    Name = "Open UAC Settings",
                    Description = "Open UAC configuration dialog.",
                    ActionType = RepairActionType.SystemTool,
                    Command = "UserAccountControlSettings.exe",
                    RequiresAdmin = true,
                    RiskLevel = "None",
                    RelatedCategory = DiagnosticCategory.Security,
                    EstimatedTimeSeconds = 5
                });
            }
            results.Add(r);
        }
        catch { }
        return results;
    }

    private List<DiagnosticResult> CheckSecurityServices()
    {
        var results = new List<DiagnosticResult>();
        var services = new Dictionary<string, string>
        {
            { "WinDefend", "Windows Defender Antivirus" },
            { "SecurityHealthService", "Windows Security Service" },
            { "wscsvc", "Security Center" }
        };

        foreach (var (svc, display) in services)
        {
            try
            {
                using var sc = new ServiceController(svc);
                var r = new DiagnosticResult
                {
                    Category = DiagnosticCategory.Security,
                    CategoryDisplayName = "Security",
                    Title = $"Service: {display}",
                    Details = $"{svc} | Status: {sc.Status}",
                    Status = DiagnosticStatus.Completed,
                    Source = "SecurityService"
                };

                if (sc.Status == ServiceControllerStatus.Running)
                {
                    r.Severity = Severity.Healthy;
                    r.Description = $"{display} is running.";
                    r.Score = 100;
                }
                else
                {
                    r.Severity = Severity.Warning;
                    r.Description = $"{display} is not running (Status: {sc.Status}).";
                    r.Score = 40;
                    var svcCopy = svc;
                    r.AvailableActions.Add(new RepairAction
                    {
                        Name = $"Start {display}",
                        Description = $"Start the {svc} service.",
                        ActionType = RepairActionType.ServiceRestart,
                        Command = "net",
                        Arguments = $"start {svcCopy}",
                        RequiresAdmin = true,
                        RiskLevel = "Low",
                        RelatedCategory = DiagnosticCategory.Security,
                        EstimatedTimeSeconds = 10,
                        ExecuteAsync = async () =>
                        {
                            var res = await ProcessHelper.RunCommandAsync("net", $"start {svcCopy}");
                            return res.ExitCode == 0;
                        }
                    });
                }
                results.Add(r);
            }
            catch { }
        }
        return results;
    }
}