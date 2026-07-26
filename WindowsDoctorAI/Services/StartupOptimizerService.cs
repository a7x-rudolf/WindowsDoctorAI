using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsDoctorAI.Helpers;
using WindowsDoctorAI.Models;

namespace WindowsDoctorAI.Services;

public class StartupOptimizerService
{
    // Fix CS1998: jalankan di background thread agar benar-benar async
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync()
    {
        var results = new List<DiagnosticResult>();
        List<string> startupItems = new();

        // GetStartupPrograms membaca registry - jalankan di background thread
        await Task.Run(() =>
        {
            startupItems = RegistryHelper.GetStartupPrograms();
        });

        results.AddRange(AnalyzeStartup(startupItems));
        results.Add(ScheduledTasksInfo());

        return results;
    }

    private List<DiagnosticResult> AnalyzeStartup(List<string> items)
    {
        var r = new DiagnosticResult
        {
            Category = DiagnosticCategory.StartupPrograms,
            CategoryDisplayName = "Startup Programs",
            Title = "Startup Programs Analysis",
            Details = $"Found {items.Count} startup entries:\n" +
                      string.Join("\n", items.Count > 15 ? items.GetRange(0, 15) : items),
            Status = DiagnosticStatus.Completed,
            Source = "StartupService"
        };

        if (items.Count > 15)
        {
            r.Severity = Severity.Warning;
            r.Description = $"Too many startup programs ({items.Count}). May slow boot significantly.";
            r.Score = 40;
            r.AvailableActions.Add(new RepairAction
            {
                Name = "Open Task Manager Startup",
                Description = "Review and disable unnecessary startup programs.",
                ActionType = RepairActionType.SystemTool,
                Command = "taskmgr.exe",
                RequiresAdmin = false,
                RiskLevel = "None",
                RelatedCategory = DiagnosticCategory.StartupPrograms,
                EstimatedTimeSeconds = 5
            });
            r.AvailableActions.Add(new RepairAction
            {
                Name = "Open Startup Apps Settings",
                Description = "Manage startup apps in Windows Settings.",
                ActionType = RepairActionType.OpenSettings,
                Command = "ms-settings:startupapps",
                RequiresAdmin = false,
                RiskLevel = "None",
                RelatedCategory = DiagnosticCategory.StartupPrograms,
                EstimatedTimeSeconds = 5
            });
        }
        else if (items.Count > 8)
        {
            r.Severity = Severity.Info;
            r.Description = $"Moderate startup programs ({items.Count}). Review for unnecessary items.";
            r.Score = 70;
            r.AvailableActions.Add(new RepairAction
            {
                Name = "Open Startup Apps Settings",
                Description = "Review startup apps.",
                ActionType = RepairActionType.OpenSettings,
                Command = "ms-settings:startupapps",
                RequiresAdmin = false,
                RiskLevel = "None",
                RelatedCategory = DiagnosticCategory.StartupPrograms,
                EstimatedTimeSeconds = 5
            });
        }
        else
        {
            r.Severity = Severity.Healthy;
            r.Description = $"Startup programs count is acceptable ({items.Count}).";
            r.Score = 100;
        }

        return new List<DiagnosticResult> { r };
    }

    private DiagnosticResult ScheduledTasksInfo()
    {
        return new DiagnosticResult
        {
            Category = DiagnosticCategory.StartupPrograms,
            CategoryDisplayName = "Startup Programs",
            Title = "Scheduled Tasks",
            Description = "Review Task Scheduler for unnecessary scheduled tasks.",
            Severity = Severity.Info,
            Status = DiagnosticStatus.Completed,
            Score = 90,
            Source = "StartupService",
            AvailableActions =
            {
                new RepairAction
                {
                    Name = "Open Task Scheduler",
                    Description = "Manage Windows scheduled tasks.",
                    ActionType = RepairActionType.SystemTool,
                    Command = "taskschd.msc",
                    RequiresAdmin = true,
                    RiskLevel = "None",
                    RelatedCategory = DiagnosticCategory.StartupPrograms,
                    EstimatedTimeSeconds = 5
                }
            }
        };
    }
}