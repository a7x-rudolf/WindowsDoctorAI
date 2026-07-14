using Microsoft.UI.Xaml;

namespace WindowsDoctorAI.Views;

public static class ThemeService
{
    public static ElementTheme CurrentTheme { get; private set; } = ElementTheme.Default;

    public static void SetTheme(ElementTheme theme, XamlRoot? xamlRoot)
    {
        CurrentTheme = theme;

        // Apply to root element
        if (xamlRoot?.Content is FrameworkElement root)
        {
            root.RequestedTheme = theme;
        }

        // Save preference (bisa extend nanti dengan Settings persistence)
    }

    public static void ApplyToWindow(Window window, ElementTheme theme)
    {
        if (window.Content is FrameworkElement root)
        {
            root.RequestedTheme = theme;
        }
    }
}