using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace WindowsDoctorAI.Converters;

public class StatusTypeToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
    {
        var s = value?.ToString() ?? "Healthy";
        return s switch
        {
            "Healthy" => new SolidColorBrush(ColorHelper.FromArgb(20, 15, 123, 15)),
            "Warning" => new SolidColorBrush(ColorHelper.FromArgb(20, 183, 113, 13)),
            "Critical" => new SolidColorBrush(ColorHelper.FromArgb(20, 196, 43, 28)),
            "Info" => new SolidColorBrush(ColorHelper.FromArgb(20, 0, 95, 184)),
            _ => new SolidColorBrush(ColorHelper.FromArgb(20, 128, 128, 128))
        };
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}

public class StatusTypeToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
    {
        var s = value?.ToString() ?? "Healthy";
        return s switch
        {
            "Healthy" => new SolidColorBrush(ColorHelper.FromArgb(255, 15, 123, 15)),
            "Warning" => new SolidColorBrush(ColorHelper.FromArgb(255, 183, 113, 13)),
            "Critical" => new SolidColorBrush(ColorHelper.FromArgb(255, 196, 43, 28)),
            "Info" => new SolidColorBrush(ColorHelper.FromArgb(255, 0, 95, 184)),
            _ => new SolidColorBrush(Colors.Gray)
        };
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}

public class ScoreToWidthConverter : IValueConverter
{
    public double MaxWidth { get; set; } = 200;

    public object Convert(object value, Type t, object p, string l)
    {
        if (value is double s)
            return Math.Max(0, Math.Min(MaxWidth, MaxWidth * (s / 100.0)));
        return 0.0;
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}

public class ScoreToProgressBrushConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
    {
        double s = value is double d ? d : 0;
        var brush = new LinearGradientBrush { StartPoint = new(0, 0), EndPoint = new(1, 0) };
        if (s >= 75)
        {
            brush.GradientStops.Add(new GradientStop { Color = ColorHelper.FromArgb(255, 15, 123, 15), Offset = 0 });
            brush.GradientStops.Add(new GradientStop { Color = ColorHelper.FromArgb(255, 43, 168, 43), Offset = 1 });
        }
        else if (s >= 50)
        {
            brush.GradientStops.Add(new GradientStop { Color = ColorHelper.FromArgb(255, 183, 113, 13), Offset = 0 });
            brush.GradientStops.Add(new GradientStop { Color = ColorHelper.FromArgb(255, 143, 87, 9), Offset = 1 });
        }
        else
        {
            brush.GradientStops.Add(new GradientStop { Color = ColorHelper.FromArgb(255, 196, 43, 28), Offset = 0 });
            brush.GradientStops.Add(new GradientStop { Color = ColorHelper.FromArgb(255, 143, 30, 21), Offset = 1 });
        }
        return brush;
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}

// Returns DoubleCollection for StrokeDashArray
public class ScoreToDashArrayConverter : IValueConverter
{
    public double Radius { get; set; } = 80;

    public object Convert(object value, Type t, object p, string l)
    {
        double circumference = 2 * Math.PI * Radius;
        var collection = new DoubleCollection
        {
            circumference,
            circumference
        };
        return collection;
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}

// Returns single double for StrokeDashOffset
public class ScoreToDashOffsetConverter : IValueConverter
{
    public double Radius { get; set; } = 80;

    public object Convert(object value, Type t, object p, string l)
    {
        double s = value is double d ? d : 0;
        double circumference = 2 * Math.PI * Radius;
        double offset = circumference * (1.0 - (s / 100.0));
        return offset;
    }
    public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
}