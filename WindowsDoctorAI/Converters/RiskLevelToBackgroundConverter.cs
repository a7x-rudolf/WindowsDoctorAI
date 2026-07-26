using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace WindowsDoctorAI.Converters;

public class RiskLevelToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var risk = value?.ToString()?.ToLowerInvariant() ?? "low";

        string resourceKey = risk switch
        {
            "low" => "StatusHealthyBgBrush",
            "medium" => "StatusWarningBgBrush",
            "high" => "StatusCriticalBgBrush",
            _ => "ControlFillColorTertiaryBrush"
        };

        try
        {
            return (Brush)App.Current.Resources[resourceKey];
        }
        catch
        {
            return new SolidColorBrush(Microsoft.UI.Colors.Transparent);
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}