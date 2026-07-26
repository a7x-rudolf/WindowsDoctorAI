using System;
using System.IO;
using System.Text.Json;

namespace WindowsDoctorAI.Helpers;

/// <summary>
/// Simple JSON-based persisted settings store.
/// This app runs unpackaged (WindowsPackageType=None), so
/// Windows.Storage.ApplicationData.Current is NOT available.
/// We store settings manually under %LocalAppData%\WindowsDoctorAI\settings.json
/// </summary>
internal static class AppSettingsStore
{
    private class SettingsModel
    {
        public string Theme { get; set; } = "Dark";
        public string Language { get; set; } = "id";
    }

    private static readonly string SettingsFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "WindowsDoctorAI");

    private static readonly string SettingsFilePath = Path.Combine(SettingsFolder, "settings.json");

    private static SettingsModel _cache = Load();

    private static SettingsModel Load()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                var model = JsonSerializer.Deserialize<SettingsModel>(json);
                if (model != null)
                    return model;
            }
        }
        catch
        {
            // Ignore corrupt/missing settings file, fall back to defaults
        }

        return new SettingsModel();
    }

    private static void Save()
    {
        try
        {
            Directory.CreateDirectory(SettingsFolder);
            var json = JsonSerializer.Serialize(_cache, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFilePath, json);
        }
        catch
        {
            // Best-effort persistence; failing to save should never crash the app
        }
    }

    public static string GetTheme() => _cache.Theme;

    public static void SetTheme(string theme)
    {
        _cache.Theme = theme;
        Save();
    }

    public static string GetLanguage() => _cache.Language;

    public static void SetLanguage(string language)
    {
        _cache.Language = language;
        Save();
    }
}
