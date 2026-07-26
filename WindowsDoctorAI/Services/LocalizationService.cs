using System;
using System.Collections.Generic;
using WindowsDoctorAI.Helpers;

namespace WindowsDoctorAI.Services;

/// <summary>
/// Lightweight code-based localization service (English / Indonesian).
/// Not using .resw resources because this app is unpackaged, which makes
/// the standard Windows resource-manager pipeline unreliable. Instead we
/// keep a simple key -> translation dictionary per language.
///
/// To localize more UI, add the string as a key below (both languages),
/// then call LocalizationService.GetString("YourKey") wherever the text
/// is set, and refresh it inside a LanguageChanged handler.
/// </summary>
public static class LocalizationService
{
    public const string English = "en";
    public const string Indonesian = "id";

    public static string CurrentLanguage { get; private set; } = LoadInitialLanguage();

    public static event Action<string>? LanguageChanged;

    private static string LoadInitialLanguage()
    {
        var saved = AppSettingsStore.GetLanguage();
        return saved == English ? English : Indonesian;
    }

    public static void SetLanguage(string languageCode)
    {
        CurrentLanguage = languageCode == English ? English : Indonesian;
        AppSettingsStore.SetLanguage(CurrentLanguage);
        LanguageChanged?.Invoke(CurrentLanguage);
    }

    public static string GetString(string key)
    {
        if (Strings.TryGetValue(key, out var translations) &&
            translations.TryGetValue(CurrentLanguage, out var value))
        {
            return value;
        }

        return key; // fallback so missing keys never crash the UI
    }

    private static readonly Dictionary<string, Dictionary<string, string>> Strings = new()
    {
        ["AppTagline"] = new() { [English] = "Windows Doctor AI", [Indonesian] = "Windows Doctor AI" },
        ["Nav_Diagnostics"] = new() { [English] = "DIAGNOSTICS", [Indonesian] = "DIAGNOSTIK" },
        ["Nav_Dashboard"] = new() { [English] = "Dashboard", [Indonesian] = "Dasbor" },
        ["Nav_Scan"] = new() { [English] = "Diagnostic Scan", [Indonesian] = "Pindai Diagnostik" },
        ["Nav_Results"] = new() { [English] = "Scan Results", [Indonesian] = "Hasil Pindaian" },
        ["Nav_Repair"] = new() { [English] = "Repair Actions", [Indonesian] = "Tindakan Perbaikan" },
        ["Nav_Insights"] = new() { [English] = "INSIGHTS", [Indonesian] = "WAWASAN" },
        ["Nav_History"] = new() { [English] = "Scan History", [Indonesian] = "Riwayat Pindaian" },
        ["Nav_System"] = new() { [English] = "System Info", [Indonesian] = "Info Sistem" },
        ["Nav_Settings"] = new() { [English] = "Settings", [Indonesian] = "Pengaturan" },
        ["Nav_About"] = new() { [English] = "About", [Indonesian] = "Tentang" },

        ["Status_Ready"] = new() { [English] = "System Ready", [Indonesian] = "Sistem Siap" },
        ["Status_LastScanNever"] = new() { [English] = "Last scan: Never", [Indonesian] = "Pindaian terakhir: Belum pernah" },
        ["Status_LastScan"] = new() { [English] = "Last scan: {0}", [Indonesian] = "Pindaian terakhir: {0}" },
        ["Status_Administrator"] = new() { [English] = "Administrator", [Indonesian] = "Administrator" },
        ["Status_StandardUserLimited"] = new() { [English] = "Standard User (Limited)", [Indonesian] = "Pengguna Standar (Terbatas)" },
        ["User_ElevatedSession"] = new() { [English] = "Elevated Session", [Indonesian] = "Sesi Elevasi" },
        ["User_StandardUser"] = new() { [English] = "Standard User", [Indonesian] = "Pengguna Standar" },

        ["Theme_ToggleTooltip"] = new() { [English] = "Switch theme", [Indonesian] = "Ganti tema" },
        ["Language_ToggleTooltip"] = new() { [English] = "Switch language", [Indonesian] = "Ganti bahasa" },
    };
}
