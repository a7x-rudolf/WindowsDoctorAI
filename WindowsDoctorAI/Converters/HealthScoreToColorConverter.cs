using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace WindowsDoctorAI.Converters;

public class HealthScoreToColorConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
    {
        if (value is double s) return s switch
        {
            >= 90 => new SolidColorBrush(ColorHelper.FromArgb(255, 15, 123, 15)),     // Healthy
            >= 75 => new SolidColorBrush(ColorHelper.FromArgb(255, 0, 103, 192)),      // Info
            >= 60 => new SolidColorBrush(ColorHelper.FromArgb(255, 183, 113, 13)),     // Warning
            >= 40 => new SolidColorBrush(ColorHelper.FromArgb(255, 183, 113, 13)),     // Warning
            _ => new SolidColorBrush(ColorHelper.FromArgb(255, 196, 43, 28))       // Critical
        };
        return new SolidColorBrush(Colors.Gray);
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}

// Score -> Gradient Brush (untuk score circle)
public class HealthScoreToGradientConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
    {
        if (value is double s)
        {
            var gradient = new LinearGradientBrush { StartPoint = new(0, 0), EndPoint = new(1, 1) };
            if (s >= 90)
            {
                gradient.GradientStops.Add(new GradientStop { Color = ColorHelper.FromArgb(255, 15, 123, 15), Offset = 0 });
                gradient.GradientStops.Add(new GradientStop { Color = ColorHelper.FromArgb(255, 43, 168, 43), Offset = 1 });
            }
            else if (s >= 60)
            {
                gradient.GradientStops.Add(new GradientStop { Color = ColorHelper.FromArgb(255, 183, 113, 13), Offset = 0 });
                gradient.GradientStops.Add(new GradientStop { Color = ColorHelper.FromArgb(255, 143, 87, 9), Offset = 1 });
            }
            else
            {
                gradient.GradientStops.Add(new GradientStop { Color = ColorHelper.FromArgb(255, 196, 43, 28), Offset = 0 });
                gradient.GradientStops.Add(new GradientStop { Color = ColorHelper.FromArgb(255, 143, 30, 21), Offset = 1 });
            }
            return gradient;
        }
        return new SolidColorBrush(Colors.Gray);
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}

// Score -> "Excellent" / "Good" / etc.
public class HealthScoreToRatingConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
    {
        if (value is double s) return s switch
        {
            >= 90 => "EXCELLENT",
            >= 75 => "GOOD",
            >= 60 => "FAIR",
            >= 40 => "POOR",
            _ => "CRITICAL"
        };
        return "N/A";
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}