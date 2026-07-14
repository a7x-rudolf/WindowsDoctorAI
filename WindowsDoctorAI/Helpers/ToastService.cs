using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;

namespace WindowsDoctorAI.Helpers;

public enum ToastType
{
    Info,
    Success,
    Warning,
    Error
}

public static class ToastService
{
    private static Panel? _hostPanel;

    /// <summary>
    /// Register the root panel where toasts will appear.
    /// </summary>
    public static void RegisterHost(Panel host)
    {
        _hostPanel = host;
    }

    /// <summary>
    /// Show a toast notification.
    /// </summary>
    public static void Show(XamlRoot? xamlRoot, ToastType type, string title, string message, int durationMs = 3500)
    {
        if (_hostPanel == null)
        {
            System.Diagnostics.Debug.WriteLine("ToastService: Host panel not registered");
            return;
        }

        try
        {
            var (bgKey, fgKey, glyph) = type switch
            {
                ToastType.Success => ("StatusHealthyBgBrush", "StatusHealthyBrush", IconGlyphs.CheckMark),
                ToastType.Warning => ("StatusWarningBgBrush", "StatusWarningBrush", IconGlyphs.Warning),
                ToastType.Error => ("StatusCriticalBgBrush", "StatusCriticalBrush", IconGlyphs.Critical),
                _ => ("StatusInfoBgBrush", "StatusInfoBrush", IconGlyphs.Info)
            };

            var toast = new Border
            {
                Background = App.Current.Resources["LayerFillColorDefaultBrush"] as Brush,
                BorderBrush = App.Current.Resources["CardStrokeColorDefaultBrush"] as Brush,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14, 12, 14, 12),
                Margin = new Thickness(0, 4, 0, 4),
                MinWidth = 320,
                MaxWidth = 400,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var iconBorder = new Border
            {
                Width = 32,
                Height = 32,
                CornerRadius = new CornerRadius(16),
                Background = App.Current.Resources[bgKey] as Brush,
                Margin = new Thickness(0, 0, 12, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Child = new FontIcon
                {
                    Glyph = glyph,
                    FontSize = 13,
                    Foreground = App.Current.Resources[fgKey] as Brush
                }
            };
            Grid.SetColumn(iconBorder, 0);

            var textPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Spacing = 2 };
            textPanel.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 13,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });
            textPanel.Children.Add(new TextBlock
            {
                Text = message,
                FontSize = 12,
                Foreground = App.Current.Resources["TextFillColorSecondaryBrush"] as Brush,
                TextWrapping = TextWrapping.Wrap,
                MaxLines = 3
            });
            Grid.SetColumn(textPanel, 1);

            var closeBtn = new Button
            {
                Content = new FontIcon { Glyph = IconGlyphs.Close, FontSize = 10 },
                Background = new SolidColorBrush(Colors.Transparent),
                BorderThickness = new Thickness(0),
                Padding = new Thickness(6),
                MinWidth = 24,
                MinHeight = 24,
                CornerRadius = new CornerRadius(4),
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(4, 0, 0, 0)
            };
            Grid.SetColumn(closeBtn, 2);

            grid.Children.Add(iconBorder);
            grid.Children.Add(textPanel);
            grid.Children.Add(closeBtn);

            toast.Child = grid;

            var container = FindOrCreateToastContainer();
            container.Children.Add(toast);

            // Close handler
            void CloseToast()
            {
                try
                {
                    container.Children.Remove(toast);
                }
                catch { }
            }

            closeBtn.Click += (s, e) => CloseToast();

            // Auto dismiss
            var dismissTimer = _hostPanel.DispatcherQueue.CreateTimer();
            dismissTimer.Interval = TimeSpan.FromMilliseconds(durationMs);
            dismissTimer.IsRepeating = false;
            dismissTimer.Tick += (s, e) =>
            {
                CloseToast();
                dismissTimer.Stop();
            };
            dismissTimer.Start();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ToastService error: {ex.Message}");
        }
    }

    private static StackPanel FindOrCreateToastContainer()
    {
        // Find existing container
        foreach (var child in _hostPanel!.Children)
        {
            if (child is StackPanel sp && sp.Name == "ToastContainer")
                return sp;
        }

        // Create new
        var container = new StackPanel
        {
            Name = "ToastContainer",
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(0, 48, 20, 0),
            Spacing = 6
        };

        Canvas.SetZIndex(container, 9999);

        _hostPanel.Children.Add(container);
        return container;
    }
}