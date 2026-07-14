using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace WindowsDoctorAI.Helpers;

public static class ProcessHelper
{
    public static async Task<(int ExitCode, string Output, string Error)> RunCommandAsync(
        string fileName, string arguments, int timeoutMs = 120000)
    {
        var output = new StringBuilder();
        var error = new StringBuilder();
        try
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };
            process.OutputDataReceived += (s, e) => { if (e.Data != null) output.AppendLine(e.Data); };
            process.ErrorDataReceived += (s, e) => { if (e.Data != null) error.AppendLine(e.Data); };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            var completed = await Task.Run(() => process.WaitForExit(timeoutMs));
            if (!completed) { process.Kill(entireProcessTree: true); return (-1, output.ToString(), "Timeout"); }
            return (process.ExitCode, output.ToString(), error.ToString());
        }
        catch (Exception ex) { return (-1, string.Empty, ex.Message); }
    }

    public static async Task<string> RunPowerShellAsync(string script, int timeoutMs = 120000)
    {
        var result = await RunCommandAsync("powershell.exe",
            $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"{script}\"", timeoutMs);
        return result.Output;
    }

    public static void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
    }
}