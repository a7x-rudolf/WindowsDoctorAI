using Microsoft.UI.Xaml.Data;
using System;

namespace WindowsDoctorAI.Converters;

/// <summary>
/// Converts a score (0–100) to a fraction (0.0–1.0) for ScaleTransform.
/// </summary>
public class ScoreToFractionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        double score = value switch
        {
            double d => d,
            int i => i,
            float f => f,
            _ => 0
        };

        return Math.Clamp(score / 100.0, 0.0, 1.0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}