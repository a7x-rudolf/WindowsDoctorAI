using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using WindowsDoctorAI.Helpers;
using WindowsDoctorAI.Models;

namespace WindowsDoctorAI.Dialogs;

public sealed partial class ScanProgressDialog : ContentDialog
{
    private readonly Dictionary<string, ScanCategoryRow> _rows = new();
    private DateTime _startTime = DateTime.Now;

    private static readonly List<(string Key, string Name, string Glyph)> Categories = new()
    {
        ("Performance",      "Performance",       IconGlyphs.Performance),
        ("Disk Health",      "Disk Health",       IconGlyphs.Disk),
        ("Network",          "Network",           IconGlyphs.Network),
        ("Security",         "Security",          IconGlyphs.Security),
        ("Windows Update",   "Windows Update",    IconGlyphs.Update),
        ("Drivers",          "Drivers",           IconGlyphs.SystemInfo),
        ("Startup Programs", "Startup Programs",  IconGlyphs.Startup),
    };

    public ScanProgressDialog()
    {
        this.InitializeComponent();
        BuildCategoryRows();
        _startTime = DateTime.Now;
        this.Opened += (s, e) => { _startTime = DateTime.Now; };
    }

    private void BuildCategoryRows()
    {
        CategoryList.Children.Clear();
        _rows.Clear();

        foreach (var (key, name, glyph) in Categories)
        {
            var row = new ScanCategoryRow(name, glyph);
            _rows[key] = row;
            CategoryList.Children.Add(row.Container);
        }
    }

    public void SetCategoryStarted(string categoryName)
    {
        if (_rows.TryGetValue(categoryName, out var row))
        {
            row.SetActive();
            SubtitleText.Text = $"Analyzing {categoryName}...";
            ProgressLabelText.Text = $"Scanning {categoryName}";
        }
        AddLog(LogLevel.Info, $"Started scanning: {categoryName}");
    }

    public void SetCategoryDone(string categoryName)
    {
        if (_rows.TryGetValue(categoryName, out var row))
        {
            row.SetDone();
        }
    }

    public void UpdateProgress(int percent)
    {
        ProgressBar.Value = Math.Max(0, Math.Min(100, percent));
        ProgressPercentText.Text = $"{percent}%";
    }

    public void OnResultFound(DiagnosticResult result)
    {
        var level = result.Severity switch
        {
            Severity.Critical => LogLevel.Critical,
            Severity.Warning => LogLevel.Warning,
            Severity.Healthy => LogLevel.Success,
            _ => LogLevel.Info
        };
        AddLog(level, result.Title);
    }

    public void OnScanCompleted(SystemHealthScore score)
    {
        foreach (var row in _rows.Values)
        {
            if (!row.IsCompleted) row.SetDone();
        }

        UpdateProgress(100);
        TitleText.Text = "Scan Complete";
        SubtitleText.Text = $"Found {score.TotalIssues} issue(s) - " +
                            $"{score.CriticalIssues} critical, {score.WarningIssues} warnings";
        ProgressLabelText.Text = "Scan complete";

        AddLog(LogLevel.Success, $"Scan finished! Score: {score.OverallScore:F0}/100");

        this.PrimaryButtonText = "View Results";
        this.CloseButtonText = "";
    }

    public void OnScanCancelled()
    {
        TitleText.Text = "Scan Cancelled";
        SubtitleText.Text = "The scan was interrupted before completion";
        AddLog(LogLevel.Warning, "Scan cancelled by user");

        this.PrimaryButtonText = "Close";
        this.CloseButtonText = "";
    }

    public void OnScanError(string errorMsg)
    {
        TitleText.Text = "Scan Error";
        SubtitleText.Text = errorMsg;
        AddLog(LogLevel.Critical, $"Error: {errorMsg}");

        this.PrimaryButtonText = "Close";
        this.CloseButtonText = "";
    }

    private void AddLog(LogLevel level, string message)
    {
        var elapsed = DateTime.Now - _startTime;
        var timeStr = $"{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";

        var (glyph, brush) = level switch
        {
            LogLevel.Success => (IconGlyphs.CheckMark, App.Current.Resources["StatusHealthyBrush"] as Brush),
            LogLevel.Warning => (IconGlyphs.Warning, App.Current.Resources["StatusWarningBrush"] as Brush),
            LogLevel.Critical => (IconGlyphs.Critical, App.Current.Resources["StatusCriticalBrush"] as Brush),
            _ => (IconGlyphs.Info, App.Current.Resources["StatusInfoBrush"] as Brush)
        };

        var line = new Grid { Margin = new Thickness(0, 2, 0, 2) };
        line.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(42) });
        line.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        line.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var timeText = new TextBlock
        {
            Text = timeStr,
            FontSize = 11,
            FontFamily = new FontFamily("Cascadia Code, Consolas"),
            Foreground = App.Current.Resources["TextFillColorTertiaryBrush"] as Brush
        };
        Grid.SetColumn(timeText, 0);

        var icon = new FontIcon
        {
            Glyph = glyph,
            FontSize = 11,
            Foreground = brush,
            Margin = new Thickness(0, 0, 8, 0),
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(icon, 1);

        var msgText = new TextBlock
        {
            Text = message,
            FontSize = 12,
            FontFamily = new FontFamily("Cascadia Code, Consolas"),
            TextWrapping = TextWrapping.Wrap
        };
        Grid.SetColumn(msgText, 2);

        line.Children.Add(timeText);
        line.Children.Add(icon);
        line.Children.Add(msgText);

        LogPanel.Children.Add(line);

        LogScrollViewer.UpdateLayout();
        LogScrollViewer.ChangeView(null, LogScrollViewer.ScrollableHeight, null, disableAnimation: true);
    }

    private enum LogLevel { Info, Success, Warning, Critical }

    private class ScanCategoryRow
    {
        public Grid Container { get; }
        private readonly FontIcon _statusIcon;
        private readonly TextBlock _statusText;
        private readonly TextBlock _nameText;
        public bool IsCompleted { get; private set; }
        public string Glyph { get; }

        public ScanCategoryRow(string name, string glyph)
        {
            Container = new Grid
            {
                Padding = new Thickness(10, 8, 10, 8),
                CornerRadius = new CornerRadius(6),
                Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent)
            };
            Container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(28) });
            Container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var iconBorder = new Border
            {
                Width = 24,
                Height = 24,
                CornerRadius = new CornerRadius(12),
                Background = App.Current.Resources["ControlFillColorTertiaryBrush"] as Brush,
                Margin = new Thickness(0, 0, 12, 0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            _statusIcon = new FontIcon
            {
                Glyph = IconGlyphs.Empty,
                FontSize = 10,
                Foreground = App.Current.Resources["TextFillColorTertiaryBrush"] as Brush
            };
            iconBorder.Child = _statusIcon;
            Grid.SetColumn(iconBorder, 0);

            var textPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            _nameText = new TextBlock
            {
                Text = name,
                FontSize = 13,
                FontWeight = Microsoft.UI.Text.FontWeights.Normal
            };
            _statusText = new TextBlock
            {
                Text = "Waiting...",
                FontSize = 11,
                Foreground = App.Current.Resources["TextFillColorTertiaryBrush"] as Brush
            };
            textPanel.Children.Add(_nameText);
            textPanel.Children.Add(_statusText);
            Grid.SetColumn(textPanel, 1);

            Container.Children.Add(iconBorder);
            Container.Children.Add(textPanel);
            Glyph = glyph;
        }

        public void SetActive()
        {
            Container.Background = App.Current.Resources["StatusInfoBgBrush"] as Brush;
            var iconBorder = (Border)Container.Children[0];
            iconBorder.Background = App.Current.Resources["BrandAccentBrush"] as Brush;
            _statusIcon.Glyph = IconGlyphs.Timer;
            _statusIcon.Foreground = new SolidColorBrush(Microsoft.UI.Colors.White);
            _nameText.FontWeight = Microsoft.UI.Text.FontWeights.SemiBold;
            _nameText.Foreground = App.Current.Resources["BrandAccentBrush"] as Brush;
            _statusText.Text = "Scanning...";
            _statusText.Foreground = App.Current.Resources["BrandAccentBrush"] as Brush;
        }

        public void SetDone()
        {
            IsCompleted = true;
            Container.Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
            var iconBorder = (Border)Container.Children[0];
            iconBorder.Background = App.Current.Resources["StatusHealthyBrush"] as Brush;
            _statusIcon.Glyph = IconGlyphs.CheckMark;
            _statusIcon.Foreground = new SolidColorBrush(Microsoft.UI.Colors.White);
            _nameText.FontWeight = Microsoft.UI.Text.FontWeights.Normal;
            _nameText.Foreground = App.Current.Resources["TextFillColorPrimaryBrush"] as Brush;
            _statusText.Text = "Completed";
            _statusText.Foreground = App.Current.Resources["StatusHealthyBrush"] as Brush;
        }
    }
}