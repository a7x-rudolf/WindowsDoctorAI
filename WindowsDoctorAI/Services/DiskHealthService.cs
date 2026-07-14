using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WindowsDoctorAI.Helpers;
using WindowsDoctorAI.Models;

namespace WindowsDoctorAI.Services;

public class DiskHealthService
{
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync()
    {
        var results = new List<DiagnosticResult>();

        // Jalankan sync work di background thread agar tidak block UI
        await Task.Run(() =>
        {
            results.AddRange(CheckDiskSpace());
            results.AddRange(CheckSmartStatus());
            results.AddRange(CheckTempFiles());
            results.AddRange(CheckDiskOptimization());
        });

        return results;
    }

    private List<DiagnosticResult> CheckDiskSpace()
    {
        var results = new List<DiagnosticResult>();
        foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Fixed))
        {
            var freePercent = (double)drive.AvailableFreeSpace / drive.TotalSize * 100;
            var freeGb = drive.AvailableFreeSpace / (1024.0 * 1024 * 1024);
            var totalGb = drive.TotalSize / (1024.0 * 1024 * 1024);
            var usedGb = totalGb - freeGb;

            var result = new DiagnosticResult
            {
                Category = DiagnosticCategory.DiskHealth,
                CategoryDisplayName = "Disk Health",
                Title = $"Drive {drive.Name} Space",
                Details = $"Total: {totalGb:F1} GB | Used: {usedGb:F1} GB | Free: {freeGb:F1} GB ({freePercent:F1}%)",
                Status = DiagnosticStatus.Completed,
                Source = "DiskHealthService"
            };

            if (freePercent < 5)
            {
                result.Severity = Severity.Critical;
                result.Description = $"Drive {drive.Name} critically low on space ({freePercent:F1}% free).";
                result.Score = 10;
                result.AvailableActions.Add(new RepairAction
                {
                    Name = "Run Disk Cleanup",
                    Description = "Launch Windows Disk Cleanup to remove unnecessary files.",
                    ActionType = RepairActionType.SystemTool,
                    Command = "cleanmgr.exe",
                    Arguments = $"/d {drive.Name[0]}",
                    RequiresAdmin = true,
                    RiskLevel = "Low",
                    RelatedCategory = DiagnosticCategory.DiskHealth,
                    EstimatedTimeSeconds = 60
                });
                result.AvailableActions.Add(new RepairAction
                {
                    Name = "Open Storage Settings",
                    Description = "Open Windows Storage settings to manage disk space.",
                    ActionType = RepairActionType.OpenSettings,
                    Command = "ms-settings:storagesense",
                    RequiresAdmin = false,
                    RiskLevel = "None",
                    RelatedCategory = DiagnosticCategory.DiskHealth,
                    EstimatedTimeSeconds = 5
                });
                result.AvailableActions.Add(new RepairAction
                {
                    Name = "Clear Temporary Files",
                    Description = "Automatically delete temporary files to free space.",
                    ActionType = RepairActionType.Automatic,
                    RequiresAdmin = true,
                    RiskLevel = "Low",
                    RelatedCategory = DiagnosticCategory.DiskHealth,
                    EstimatedTimeSeconds = 30,
                    ExecuteAsync = async () => await ClearTempFilesAsync()
                });
            }
            else if (freePercent < 15)
            {
                result.Severity = Severity.Warning;
                result.Description = $"Drive {drive.Name} low on space ({freePercent:F1}% free).";
                result.Score = 50;
                result.AvailableActions.Add(new RepairAction
                {
                    Name = "Run Disk Cleanup",
                    Description = "Launch Disk Cleanup utility.",
                    ActionType = RepairActionType.SystemTool,
                    Command = "cleanmgr.exe",
                    Arguments = $"/d {drive.Name[0]}",
                    RequiresAdmin = true,
                    RiskLevel = "Low",
                    RelatedCategory = DiagnosticCategory.DiskHealth,
                    EstimatedTimeSeconds = 60
                });
                result.AvailableActions.Add(new RepairAction
                {
                    Name = "Enable Storage Sense",
                    Description = "Enable automatic cleanup via Storage Sense.",
                    ActionType = RepairActionType.OpenSettings,
                    Command = "ms-settings:storagesense",
                    RequiresAdmin = false,
                    RiskLevel = "None",
                    RelatedCategory = DiagnosticCategory.DiskHealth,
                    EstimatedTimeSeconds = 5
                });
            }
            else
            {
                result.Severity = Severity.Healthy;
                result.Description = $"Drive {drive.Name} has sufficient free space ({freePercent:F1}% free).";
                result.Score = 100;
            }
            results.Add(result);
        }
        return results;
    }

    // Fix CS1998: hapus async/Task karena tidak ada await di dalamnya
    private List<DiagnosticResult> CheckSmartStatus()
    {
        var results = new List<DiagnosticResult>();
        try
        {
            var disks = WmiHelper.Query("SELECT Model, Status, Size FROM Win32_DiskDrive");
            foreach (var disk in disks)
            {
                var model = disk.GetValueOrDefault("Model", "Unknown").ToString() ?? "Unknown";
                var status = disk.GetValueOrDefault("Status", "Unknown").ToString() ?? "Unknown";
                var sizeGb = Convert.ToDouble(disk.GetValueOrDefault("Size", 0)) / (1024.0 * 1024 * 1024);

                var result = new DiagnosticResult
                {
                    Category = DiagnosticCategory.DiskHealth,
                    CategoryDisplayName = "Disk Health",
                    Title = $"S.M.A.R.T.: {model}",
                    Details = $"Model: {model} | Size: {sizeGb:F0} GB | Status: {status}",
                    Status = DiagnosticStatus.Completed,
                    Source = "DiskHealthService.SMART"
                };

                if (status.Equals("OK", StringComparison.OrdinalIgnoreCase))
                {
                    result.Severity = Severity.Healthy;
                    result.Description = $"Disk {model} is healthy.";
                    result.Score = 100;
                }
                else if (status.Equals("Pred Fail", StringComparison.OrdinalIgnoreCase))
                {
                    result.Severity = Severity.Critical;
                    result.Description = $"Disk {model} predicts failure! Back up immediately!";
                    result.Score = 5;
                    result.AvailableActions.Add(new RepairAction
                    {
                        Name = "Open Backup Settings",
                        Description = "Back up your data immediately - disk failure predicted.",
                        ActionType = RepairActionType.OpenSettings,
                        Command = "ms-settings:backup",
                        RequiresAdmin = false,
                        RiskLevel = "None",
                        RelatedCategory = DiagnosticCategory.DiskHealth,
                        EstimatedTimeSeconds = 5
                    });
                }
                else
                {
                    result.Severity = Severity.Warning;
                    result.Description = $"Disk {model} status: {status}. Monitor closely.";
                    result.Score = 55;
                    result.AvailableActions.Add(new RepairAction
                    {
                        Name = "Run CHKDSK",
                        Description = "Schedule disk check on next reboot.",
                        ActionType = RepairActionType.CommandLine,
                        Command = "cmd.exe",
                        Arguments = "/c chkdsk C: /f /r",
                        RequiresAdmin = true,
                        RequiresReboot = true,
                        RiskLevel = "Medium",
                        RelatedCategory = DiagnosticCategory.DiskHealth,
                        EstimatedTimeSeconds = 300
                    });
                }
                results.Add(result);
            }
        }
        catch (Exception ex)
        {
            results.Add(new DiagnosticResult
            {
                Category = DiagnosticCategory.DiskHealth,
                CategoryDisplayName = "Disk Health",
                Title = "S.M.A.R.T. Check Error",
                Description = $"Error reading disk data: {ex.Message}",
                Severity = Severity.Info,
                Status = DiagnosticStatus.Failed,
                Score = 80
            });
        }
        return results;
    }

    private List<DiagnosticResult> CheckTempFiles()
    {
        long totalSize = 0;
        int totalFiles = 0;
        var tempPaths = new[]
        {
            Path.GetTempPath(),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp")
        }.Distinct();

        foreach (var p in tempPaths)
        {
            if (!Directory.Exists(p)) continue;
            try
            {
                var files = Directory.GetFiles(p, "*", SearchOption.TopDirectoryOnly);
                totalFiles += files.Length;
                totalSize += files.Sum(f =>
                {
                    try { return new FileInfo(f).Length; }
                    catch { return 0L; }
                });
            }
            catch { }
        }

        var sizeMb = totalSize / (1024.0 * 1024);
        var result = new DiagnosticResult
        {
            Category = DiagnosticCategory.DiskHealth,
            CategoryDisplayName = "Disk Health",
            Title = "Temporary Files",
            Details = $"{totalFiles:N0} files - {sizeMb:F1} MB",
            Status = DiagnosticStatus.Completed,
            Source = "DiskHealthService.Temp"
        };

        if (sizeMb > 2000)
        {
            result.Severity = Severity.Warning;
            result.Description = $"Large temp file accumulation: {sizeMb:F1} MB ({totalFiles:N0} files).";
            result.Score = 60;
        }
        else if (sizeMb > 500)
        {
            result.Severity = Severity.Info;
            result.Description = $"Moderate temp files: {sizeMb:F1} MB ({totalFiles:N0} files).";
            result.Score = 80;
        }
        else
        {
            result.Severity = Severity.Healthy;
            result.Description = $"Temp files acceptable: {sizeMb:F1} MB ({totalFiles:N0} files).";
            result.Score = 100;
        }

        if (sizeMb > 500)
        {
            result.AvailableActions.Add(new RepairAction
            {
                Name = "Clear Temporary Files",
                Description = $"Delete {totalFiles:N0} temp files ({sizeMb:F1} MB).",
                ActionType = RepairActionType.Automatic,
                RequiresAdmin = true,
                RiskLevel = "Low",
                RelatedCategory = DiagnosticCategory.DiskHealth,
                EstimatedTimeSeconds = 30,
                ExecuteAsync = async () => await ClearTempFilesAsync()
            });
        }

        return new List<DiagnosticResult> { result };
    }

    private List<DiagnosticResult> CheckDiskOptimization()
    {
        var result = new DiagnosticResult
        {
            Category = DiagnosticCategory.DiskHealth,
            CategoryDisplayName = "Disk Health",
            Title = "Disk Optimization",
            Status = DiagnosticStatus.Completed,
            Source = "DiskHealthService.Optimization"
        };

        var disks = WmiHelper.Query("SELECT Model FROM Win32_DiskDrive");
        bool hasSsd = disks.Any(d =>
            d.GetValueOrDefault("Model", "").ToString()!
                .Contains("SSD", StringComparison.OrdinalIgnoreCase) ||
            d.GetValueOrDefault("Model", "").ToString()!
                .Contains("NVMe", StringComparison.OrdinalIgnoreCase));

        if (hasSsd)
        {
            result.Severity = Severity.Healthy;
            result.Description = "SSD detected. TRIM is managed automatically by Windows.";
            result.Score = 100;
        }
        else
        {
            result.Severity = Severity.Info;
            result.Description = "HDD detected. Consider running disk defragmentation.";
            result.Score = 80;
            result.AvailableActions.Add(new RepairAction
            {
                Name = "Open Optimize Drives",
                Description = "Open Defragment and Optimize Drives tool.",
                ActionType = RepairActionType.SystemTool,
                Command = "dfrgui.exe",
                RequiresAdmin = true,
                RiskLevel = "Low",
                RelatedCategory = DiagnosticCategory.DiskHealth,
                EstimatedTimeSeconds = 600
            });
        }

        return new List<DiagnosticResult> { result };
    }

    private async Task<bool> ClearTempFilesAsync()
    {
        int deleted = 0;
        var tempPaths = new[]
        {
            Path.GetTempPath(),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp")
        }.Distinct();

        await Task.Run(() =>
        {
            foreach (var p in tempPaths)
            {
                if (!Directory.Exists(p)) continue;
                try
                {
                    foreach (var f in Directory.GetFiles(p))
                    {
                        try { File.Delete(f); deleted++; }
                        catch { }
                    }
                    foreach (var d in Directory.GetDirectories(p))
                    {
                        try { Directory.Delete(d, true); deleted++; }
                        catch { }
                    }
                }
                catch { }
            }
        });
        return deleted > 0;
    }
}