using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WindowsDoctorAI.Helpers;
using WindowsDoctorAI.Models;

namespace WindowsDoctorAI.Services;

public class RepairService
{
    public event Action<RepairAction, string>? OnProgress;

    public async Task<bool> ExecuteAsync(RepairAction action)
    {
        action.Status = RepairStatus.InProgress;
        OnProgress?.Invoke(action, $"Starting: {action.Name}");

        try
        {
            bool ok;
            if (action.ExecuteAsync != null)
            {
                ok = await action.ExecuteAsync();
            }
            else
            {
                switch (action.ActionType)
                {
                    case RepairActionType.CommandLine:
                    case RepairActionType.ServiceRestart:
                        var r = await ProcessHelper.RunCommandAsync(action.Command, action.Arguments);
                        ok = r.ExitCode == 0;
                        action.ResultMessage = ok ? r.Output.Trim() : r.Error.Trim();
                        break;
                    case RepairActionType.SystemTool:
                    case RepairActionType.OpenSettings:
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = action.Command,
                            Arguments = action.Arguments,
                            UseShellExecute = true
                        });
                        ok = true;
                        action.ResultMessage = "Opened successfully.";
                        break;
                    default:
                        ok = false;
                        action.ResultMessage = "Unsupported action type.";
                        break;
                }
            }

            action.Status = ok ? (action.RequiresReboot ? RepairStatus.RequiresReboot : RepairStatus.Completed)
                               : RepairStatus.Failed;
            OnProgress?.Invoke(action, ok ? $"Done: {action.Name}" : $"Failed: {action.Name}");
            return ok;
        }
        catch (Exception ex)
        {
            action.Status = RepairStatus.Failed;
            action.ResultMessage = ex.Message;
            OnProgress?.Invoke(action, $"Error: {ex.Message}");
            return false;
        }
    }
}