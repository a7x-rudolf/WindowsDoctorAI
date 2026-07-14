using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using WindowsDoctorAI.Helpers;
using WindowsDoctorAI.Models;

namespace WindowsDoctorAI.Converters;

public class SeverityToGlyphConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
    {
        if (value is Severity s) return s switch
        {
            Severity.Healthy => IconGlyphs.CheckMark,
            Severity.Info => IconGlyphs.Info,
            Severity.Warning => IconGlyphs.Warning,
            Severity.Critical => IconGlyphs.Critical,
            _ => IconGlyphs.Info
        };
        return IconGlyphs.Info;
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}

public class RepairStatusToGlyphConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
    {
        if (value is RepairStatus s) return s switch
        {
            RepairStatus.Pending => IconGlyphs.Play,
            RepairStatus.InProgress => IconGlyphs.Timer,
            RepairStatus.Completed => IconGlyphs.CheckMark,
            RepairStatus.Failed => IconGlyphs.Cancel,
            RepairStatus.RequiresReboot => IconGlyphs.Refresh,
            _ => IconGlyphs.Info
        };
        return IconGlyphs.Info;
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}

public class CategoryToGlyphConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
    {
        if (value is DiagnosticCategory c) return c switch
        {
            DiagnosticCategory.DiskHealth => IconGlyphs.Disk,
            DiagnosticCategory.Performance => IconGlyphs.Performance,
            DiagnosticCategory.Network => IconGlyphs.Network,
            DiagnosticCategory.Security => IconGlyphs.Security,
            DiagnosticCategory.WindowsUpdate => IconGlyphs.Update,
            DiagnosticCategory.Drivers => IconGlyphs.SystemInfo,
            DiagnosticCategory.StartupPrograms => IconGlyphs.Startup,
            _ => IconGlyphs.Info
        };
        return IconGlyphs.Info;
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}

public class StringToBoolConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
        => !string.IsNullOrEmpty(value?.ToString());
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}

public class StringToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; } = false;

    public object Convert(object value, Type t, object p, string l)
    {
        bool hasValue = !string.IsNullOrEmpty(value?.ToString());
        if (Invert) hasValue = !hasValue;
        return hasValue ? Visibility.Visible : Visibility.Collapsed;
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}