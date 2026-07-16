using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WindowsDoctorAI.Helpers;

namespace WindowsDoctorAI.Views;

public static class ThemeService
{
    /// <summary>
    /// Currently active theme. Always resolves to Light or Dark (never Default),
    /// so every window/dialog can be synced to an unambiguous value.
    /// Defaults to Dark on first run, then loads the user's saved preference.
    /// </summary>
    public static ElementTheme CurrentTheme { get; private set; } = LoadInitialTheme();

    /// <summary>
    /// Raised whenever the theme changes, so any open window/dialog can react.
    /// </summary>
    public static event Action<ElementTheme>? ThemeChanged;

    private static ElementTheme LoadInitialTheme()
    {
        var saved = AppSettingsStore.GetTheme();
        return saved switch
        {
            "Light" => ElementTheme.Light,
            _ => ElementTheme.Dark // default = Dark
        };
    }

    /// <summary>
    /// Apply + persist a theme, and update the window that owns the given XamlRoot.
    /// </summary>
    public static void SetTheme(ElementTheme theme, XamlRoot? xamlRoot)
    {
        // Never persist "Default" as the toggle model only knows Light/Dark.
        CurrentTheme = theme == ElementTheme.Light ? ElementTheme.Light : ElementTheme.Dark;

        if (xamlRoot?.Content is FrameworkElement root)
        {
            root.RequestedTheme = CurrentTheme;
        }

        AppSettingsStore.SetTheme(CurrentTheme.ToString());
        ThemeChanged?.Invoke(CurrentTheme);
    }

    public static void ApplyToWindow(Window window, ElementTheme theme)
    {
        if (window.Content is FrameworkElement root)
        {
            root.RequestedTheme = theme;
        }
    }

    /// <summary>
    /// Forces a ContentDialog to use the currently active app theme.
    /// ContentDialogs render in their own popup layer and do not always
    /// re-inherit a theme that was changed programmatically after launch,
    /// so we set it explicitly every time one is created.
    /// </summary>
    public static void ApplyToDialog(ContentDialog dialog)
    {
        dialog.RequestedTheme = CurrentTheme;
    }

    /// <summary>
    /// Flips between Light and Dark, applies it to the given XamlRoot's window,
    /// persists it, and returns the resulting theme.
    /// </summary>
    public static ElementTheme ToggleTheme(XamlRoot? xamlRoot)
    {
        var next = CurrentTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
        SetTheme(next, xamlRoot);
        return next;
    }
}
