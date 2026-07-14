using System;
using System.Collections.Generic;
using System.Management;

namespace WindowsDoctorAI.Helpers;

public static class WmiHelper
{
    public static List<Dictionary<string, object>> Query(string wmiQuery, string scope = @"\\.\root\cimv2")
    {
        var results = new List<Dictionary<string, object>>();
        try
        {
            using var searcher = new ManagementObjectSearcher(scope, wmiQuery);
            foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
            {
                var dict = new Dictionary<string, object>();
                foreach (var prop in obj.Properties)
                {
                    if (prop.Value != null)
                        dict[prop.Name] = prop.Value;
                }
                results.Add(dict);
                obj.Dispose();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"WMI Error: {ex.Message}");
        }
        return results;
    }

    public static string GetStringValue(string wmiQuery, string property, string scope = @"\\.\root\cimv2")
    {
        try
        {
            using var searcher = new ManagementObjectSearcher(scope, wmiQuery);
            foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
            {
                var value = obj[property]?.ToString() ?? string.Empty;
                obj.Dispose();
                return value;
            }
        }
        catch { }
        return string.Empty;
    }
}