using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using WindowsDoctorAI.Models;

namespace WindowsDoctorAI.Converters;

public class SeverityToColorConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
    {
        if (value is Severity s) return s switch
        {
            Severity.Healthy => new SolidColorBrush(ColorHelper.FromArgb(255, 15, 123, 15)),
            Severity.Info => new SolidColorBrush(ColorHelper.FromArgb(255, 0, 95, 184)),
            Severity.Warning => new SolidColorBrush(ColorHelper.FromArgb(255, 183, 113, 13)),
            Severity.Critical => new SolidColorBrush(ColorHelper.FromArgb(255, 196, 43, 28)),
            _ => new SolidColorBrush(Colors.Gray)
        };
        return new SolidColorBrush(Colors.Gray);
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}

// Severity -> Background tinted (untuk icon container)
public class SeverityToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
    {
        if (value is Severity s) return s switch
        {
            Severity.Healthy => new SolidColorBrush(ColorHelper.FromArgb(20, 15, 123, 15)),
            Severity.Info => new SolidColorBrush(ColorHelper.FromArgb(20, 0, 95, 184)),
            Severity.Warning => new SolidColorBrush(ColorHelper.FromArgb(20, 183, 113, 13)),
            Severity.Critical => new SolidColorBrush(ColorHelper.FromArgb(20, 196, 43, 28)),
            _ => new SolidColorBrush(ColorHelper.FromArgb(20, 128, 128, 128))
        };
        return new SolidColorBrush(Colors.Transparent);
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}

// Severity -> "Healthy" / "Warning" string (untuk badge)
public class SeverityToStringConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
    {
        if (value is Severity s) return s switch
        {
            Severity.Healthy => "Healthy",
            Severity.Info => "Info",
            Severity.Warning => "Warning",
            Severity.Critical => "Critical",
            _ => "Unknown"
        };
        return "Unknown";
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}