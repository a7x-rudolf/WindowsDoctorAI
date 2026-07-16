using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace WindowsDoctorAI.Converters;

public class RiskLevelToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var risk = value?.ToString()?.ToLowerInvariant() ?? "low";

        string resourceKey = risk switch
        {
            "low" => "StatusHealthyBrush",
            "medium" => "StatusWarningBrush",
            "high" => "StatusCriticalBrush",
            _ => "TextFillColorPrimaryBrush"
        };

        try
        {
            return (Brush)App.Current.Resources[resourceKey];
        }
        catch
        {
            return new SolidColorBrush(Microsoft.UI.Colors.Gray);
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}