using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WindowsDoctorAI.Helpers;

namespace WindowsDoctorAI.Services;

public class SystemInfoService
{
    public Dictionary<string, string> GetSystemInfo()
    {
        var info = new Dictionary<string, string>();
        try
        {
            info["OS Name"] = WmiHelper.GetStringValue("SELECT Caption FROM Win32_OperatingSystem", "Caption");
            info["OS Version"] = Environment.OSVersion.VersionString;
            info["OS Build"] = WmiHelper.GetStringValue("SELECT BuildNumber FROM Win32_OperatingSystem", "BuildNumber");
            info["Architecture"] = RuntimeInformation.OSArchitecture.ToString();
            info["Computer Name"] = Environment.MachineName;
            info["User Name"] = Environment.UserName;
            info["Processor"] = WmiHelper.GetStringValue("SELECT Name FROM Win32_Processor", "Name");
            info["CPU Cores"] = Environment.ProcessorCount.ToString();
            var ramData = WmiHelper.Query("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
            if (ramData.Count > 0 && ramData[0].TryGetValue("TotalVisibleMemorySize", out object? value))
            {
                var kb = Convert.ToDouble(value);
                info["Total RAM"] = $"{kb / 1024 / 1024:F1} GB";
            }
            info["BIOS Version"] = WmiHelper.GetStringValue("SELECT SMBIOSBIOSVersion FROM Win32_BIOS", "SMBIOSBIOSVersion");
            info["Motherboard"] = WmiHelper.GetStringValue("SELECT Product FROM Win32_BaseBoard", "Product");
            info["Manufacturer"] = WmiHelper.GetStringValue("SELECT Manufacturer FROM Win32_BaseBoard", "Manufacturer");
            var boot = WmiHelper.GetStringValue("SELECT LastBootUpTime FROM Win32_OperatingSystem", "LastBootUpTime");
            if (!string.IsNullOrEmpty(boot) && boot.Length >= 14)
            {
                try
                {
                    var dt = new DateTime(int.Parse(boot[..4]), int.Parse(boot.Substring(4,2)),
                        int.Parse(boot.Substring(6,2)), int.Parse(boot.Substring(8,2)),
                        int.Parse(boot.Substring(10,2)), int.Parse(boot.Substring(12,2)));
                    info["Last Boot"] = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    info["Uptime"] = (DateTime.Now - dt).ToString(@"dd\.hh\:mm\:ss");
                }
                catch { }
            }
        }
        catch (Exception ex) { info["Error"] = ex.Message; }
        return info;
    }
}