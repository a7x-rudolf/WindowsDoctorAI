using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace WindowsDoctorAI.Helpers;

public static class RegistryHelper
{
    public static List<string> GetStartupPrograms()
    {
        var programs = new List<string>();
        string[] runKeys = {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run"
        };
        foreach (var keyPath in runKeys)
        {
            foreach (var hive in new[] { Registry.LocalMachine, Registry.CurrentUser })
            {
                try
                {
                    using var key = hive.OpenSubKey(keyPath);
                    if (key == null) continue;
                    foreach (var name in key.GetValueNames())
                    {
                        var val = key.GetValue(name)?.ToString();
                        if (!string.IsNullOrEmpty(val))
                            programs.Add($"[{(hive == Registry.LocalMachine ? "HKLM" : "HKCU")}] {name}: {val}");
                    }
                }
                catch { }
            }
        }
        return programs;
    }

    public static object? GetValue(RegistryHive hive, string subKey, string? valueName)
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
            using var key = baseKey.OpenSubKey(subKey);
            return valueName == null ? key : key?.GetValue(valueName);
        }
        catch { return null; }
    }

    public static bool SetValue(RegistryHive hive, string subKey, string valueName, object value, RegistryValueKind kind)
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
            using var key = baseKey.OpenSubKey(subKey, writable: true);
            key?.SetValue(valueName, value, kind);
            return true;
        }
        catch { return false; }
    }

    public static bool DeleteValue(RegistryHive hive, string subKey, string valueName)
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
            using var key = baseKey.OpenSubKey(subKey, writable: true);
            key?.DeleteValue(valueName, throwOnMissingValue: false);
            return true;
        }
        catch { return false; }
    }
}