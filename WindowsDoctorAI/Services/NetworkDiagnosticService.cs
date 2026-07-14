using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using WindowsDoctorAI.Helpers;
using WindowsDoctorAI.Models;

namespace WindowsDoctorAI.Services;

public class NetworkDiagnosticService
{
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync()
    {
        var results = new List<DiagnosticResult>();
        results.AddRange(CheckNetworkAdapters());
        results.AddRange(await CheckInternetConnectivityAsync());
        results.AddRange(await CheckDnsResolutionAsync());
        results.AddRange(await CheckLatencyAsync());
        return results;
    }

    private List<DiagnosticResult> CheckNetworkAdapters()
    {
        var results = new List<DiagnosticResult>();
        try
        {
            var active = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback
                         && n.NetworkInterfaceType != NetworkInterfaceType.Tunnel
                         && n.OperationalStatus == OperationalStatus.Up).ToList();

            if (active.Count == 0)
            {
                var r = new DiagnosticResult
                {
                    Category = DiagnosticCategory.Network, CategoryDisplayName = "Network",
                    Title = "No Active Network Adapters",
                    Description = "No active network adapters. Check your connections.",
                    Severity = Severity.Critical, Status = DiagnosticStatus.Completed,
                    Score = 0, Source = "NetworkService"
                };
                r.AvailableActions.Add(new RepairAction
                {
                    Name = "Open Network Settings",
                    Description = "Configure network adapters.",
                    ActionType = RepairActionType.OpenSettings, Command = "ms-settings:network",
                    RequiresAdmin = false, RiskLevel = "None",
                    RelatedCategory = DiagnosticCategory.Network, EstimatedTimeSeconds = 5
                });
                r.AvailableActions.Add(new RepairAction
                {
                    Name = "Reset Network Stack",
                    Description = "Reset TCP/IP, Winsock, flush DNS. Requires reboot.",
                    ActionType = RepairActionType.Automatic,
                    RequiresAdmin = true, RequiresReboot = true, RiskLevel = "Medium",
                    RelatedCategory = DiagnosticCategory.Network, EstimatedTimeSeconds = 15,
                    ExecuteAsync = async () => await ResetNetworkAsync()
                });
                results.Add(r);
            }
            else
            {
                foreach (var a in active)
                {
                    var ip = a.GetIPProperties().UnicastAddresses
                        .FirstOrDefault(x => x.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    results.Add(new DiagnosticResult
                    {
                        Category = DiagnosticCategory.Network, CategoryDisplayName = "Network",
                        Title = $"Adapter: {a.Name}",
                        Description = $"Adapter '{a.Name}' is active.",
                        Details = $"Type: {a.NetworkInterfaceType} | Speed: {a.Speed / 1_000_000.0:F0} Mbps | IP: {ip?.Address}",
                        Severity = Severity.Healthy, Status = DiagnosticStatus.Completed,
                        Score = 100, Source = "NetworkService"
                    });
                }
            }
        }
        catch { }
        return results;
    }

    private async Task<List<DiagnosticResult>> CheckInternetConnectivityAsync()
    {
        string[] hosts = { "8.8.8.8", "1.1.1.1", "208.67.222.222" };
        int ok = 0;
        foreach (var h in hosts)
        {
            try { using var ping = new Ping(); var r = await ping.SendPingAsync(h, 3000); if (r.Status == IPStatus.Success) ok++; }
            catch { }
        }

        var result = new DiagnosticResult
        {
            Category = DiagnosticCategory.Network, CategoryDisplayName = "Network",
            Title = "Internet Connectivity",
            Details = $"Reachable: {ok}/{hosts.Length} servers",
            Status = DiagnosticStatus.Completed, Source = "NetworkService"
        };

        if (ok == 0)
        {
            result.Severity = Severity.Critical;
            result.Description = "No internet connectivity detected.";
            result.Score = 0;
            result.AvailableActions.Add(new RepairAction
            {
                Name = "Run Network Troubleshooter",
                Description = "Launch Windows Network Diagnostics.",
                ActionType = RepairActionType.SystemTool, Command = "msdt.exe",
                Arguments = "/id NetworkDiagnosticsWeb",
                RequiresAdmin = true, RiskLevel = "None",
                RelatedCategory = DiagnosticCategory.Network, EstimatedTimeSeconds = 60
            });
            result.AvailableActions.Add(new RepairAction
            {
                Name = "Reset Network Stack",
                Description = "Reset Winsock, TCP/IP, flush DNS, renew IP.",
                ActionType = RepairActionType.Automatic,
                RequiresAdmin = true, RequiresReboot = true, RiskLevel = "Medium",
                RelatedCategory = DiagnosticCategory.Network, EstimatedTimeSeconds = 15,
                ExecuteAsync = async () => await ResetNetworkAsync()
            });
            result.AvailableActions.Add(new RepairAction
            {
                Name = "Release and Renew IP",
                Description = "Release and renew IP address from DHCP.",
                ActionType = RepairActionType.Automatic,
                RequiresAdmin = true, RiskLevel = "Low",
                RelatedCategory = DiagnosticCategory.Network, EstimatedTimeSeconds = 10,
                ExecuteAsync = async () => {
                    await ProcessHelper.RunCommandAsync("ipconfig.exe", "/release");
                    await Task.Delay(2000);
                    var r = await ProcessHelper.RunCommandAsync("ipconfig.exe", "/renew");
                    return r.ExitCode == 0;
                }
            });
        }
        else if (ok < hosts.Length)
        {
            result.Severity = Severity.Warning;
            result.Description = $"Partial connectivity. {ok}/{hosts.Length} servers reachable.";
            result.Score = 60;
            result.AvailableActions.Add(new RepairAction
            {
                Name = "Flush DNS Cache",
                Description = "Clear DNS resolver cache.",
                ActionType = RepairActionType.CommandLine, Command = "ipconfig.exe",
                Arguments = "/flushdns",
                RequiresAdmin = true, RiskLevel = "None",
                RelatedCategory = DiagnosticCategory.Network, EstimatedTimeSeconds = 5
            });
        }
        else
        {
            result.Severity = Severity.Healthy;
            result.Description = "Internet connectivity is working normally.";
            result.Score = 100;
        }

        return new List<DiagnosticResult> { result };
    }

    private async Task<List<DiagnosticResult>> CheckDnsResolutionAsync()
    {
        string[] domains = { "www.google.com", "www.microsoft.com", "www.cloudflare.com" };
        int resolved = 0;
        foreach (var d in domains)
        {
            try { var a = await System.Net.Dns.GetHostAddressesAsync(d); if (a.Length > 0) resolved++; }
            catch { }
        }

        var result = new DiagnosticResult
        {
            Category = DiagnosticCategory.Network, CategoryDisplayName = "Network",
            Title = "DNS Resolution",
            Details = $"Resolved {resolved}/{domains.Length} domains",
            Status = DiagnosticStatus.Completed, Source = "NetworkService"
        };

        if (resolved == 0)
        {
            result.Severity = Severity.Critical;
            result.Description = "DNS resolution completely failing.";
            result.Score = 10;
            result.AvailableActions.Add(new RepairAction
            {
                Name = "Flush DNS Cache",
                Description = "Clear DNS resolver cache.",
                ActionType = RepairActionType.CommandLine, Command = "ipconfig.exe",
                Arguments = "/flushdns",
                RequiresAdmin = true, RiskLevel = "None",
                RelatedCategory = DiagnosticCategory.Network, EstimatedTimeSeconds = 5
            });
            result.AvailableActions.Add(new RepairAction
            {
                Name = "Set DNS to Google (8.8.8.8)",
                Description = "Configure adapter to use Google Public DNS.",
                ActionType = RepairActionType.Automatic,
                RequiresAdmin = true, RiskLevel = "Medium",
                RelatedCategory = DiagnosticCategory.Network, EstimatedTimeSeconds = 10,
                ExecuteAsync = async () => {
                    await ProcessHelper.RunPowerShellAsync(
                        "Get-NetAdapter | Where-Object Status -eq Up | Set-DnsClientServerAddress -ServerAddresses ('8.8.8.8','8.8.4.4')");
                    return true;
                }
            });
        }
        else if (resolved < domains.Length)
        {
            result.Severity = Severity.Warning;
            result.Description = $"Partial DNS. {resolved}/{domains.Length} resolved.";
            result.Score = 65;
        }
        else
        {
            result.Severity = Severity.Healthy;
            result.Description = "DNS resolution working correctly.";
            result.Score = 100;
        }

        return new List<DiagnosticResult> { result };
    }

    private async Task<List<DiagnosticResult>> CheckLatencyAsync()
    {
        var results = new List<DiagnosticResult>();
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync("8.8.8.8", 5000);
            if (reply.Status == IPStatus.Success)
            {
                var result = new DiagnosticResult
                {
                    Category = DiagnosticCategory.Network, CategoryDisplayName = "Network",
                    Title = "Network Latency",
                    Details = $"Ping 8.8.8.8: {reply.RoundtripTime} ms",
                    Status = DiagnosticStatus.Completed, Source = "NetworkService"
                };
                if (reply.RoundtripTime > 200)
                {
                    result.Severity = Severity.Warning;
                    result.Description = $"High latency: {reply.RoundtripTime} ms.";
                    result.Score = 50;
                }
                else if (reply.RoundtripTime > 100)
                {
                    result.Severity = Severity.Info;
                    result.Description = $"Moderate latency: {reply.RoundtripTime} ms.";
                    result.Score = 75;
                }
                else
                {
                    result.Severity = Severity.Healthy;
                    result.Description = $"Good latency: {reply.RoundtripTime} ms.";
                    result.Score = 100;
                }
                results.Add(result);
            }
        }
        catch { }
        return results;
    }

    private async Task<bool> ResetNetworkAsync()
    {
        await ProcessHelper.RunCommandAsync("netsh", "winsock reset");
        await ProcessHelper.RunCommandAsync("netsh", "int ip reset");
        await ProcessHelper.RunCommandAsync("ipconfig", "/flushdns");
        await ProcessHelper.RunCommandAsync("ipconfig", "/release");
        await Task.Delay(2000);
        await ProcessHelper.RunCommandAsync("ipconfig", "/renew");
        return true;
    }
}