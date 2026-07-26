using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using WindowsDoctorAI.Helpers;
using WindowsDoctorAI.Models;
using WindowsDoctorAI.Views;

namespace WindowsDoctorAI.Dialogs;

public sealed partial class RepairProgressDialog : ContentDialog
{
    private readonly DateTime _startTime = DateTime.Now;
    private readonly RepairAction _action;

    public RepairProgressDialog(RepairAction action)
    {
        this.InitializeComponent();
        ThemeService.ApplyToDialog(this);
        _action = action;
        SubtitleText.Text = action.Name;
    }

    public void AddLog(string message)
    {
        var elapsed = DateTime.Now - _startTime;
        var timeStr = $"{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";

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
            Glyph = IconGlyphs.Info,
            FontSize = 11,
            Foreground = App.Current.Resources["StatusInfoBrush"] as Brush,
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
        StatusText.Text = message;

        LogScrollViewer.UpdateLayout();
        LogScrollViewer.ChangeView(null, LogScrollViewer.ScrollableHeight, null, disableAnimation: true);
    }

    public void OnCompleted(bool success)
    {
        ProgressBar.IsIndeterminate = false;
        ProgressBar.Value = 100;
        this.IsPrimaryButtonEnabled = true;

        if (success)
        {
            TitleText.Text = "Repair Completed";
            SubtitleText.Text = $"{_action.Name} was executed successfully";
            HeaderIcon.Glyph = IconGlyphs.CheckMark;
            HeaderIconBorder.Background = App.Current.Resources["StatusHealthyBrush"] as Brush;
            AddCompletionLog(true);

            this.PrimaryButtonText = "Close";
        }
        else
        {
            TitleText.Text = "Repair Failed";
            SubtitleText.Text = _action.ResultMessage ?? "The repair action did not complete successfully";
            HeaderIcon.Glyph = IconGlyphs.Critical;
            HeaderIconBorder.Background = App.Current.Resources["StatusCriticalBrush"] as Brush;
            AddCompletionLog(false);

            this.PrimaryButtonText = "Close";
        }

        if (_action.RequiresReboot && success)
        {
            SubtitleText.Text += "\nA system reboot is required to complete this action.";
        }
    }

    private void AddCompletionLog(bool success)
    {
        var elapsed = DateTime.Now - _startTime;
        var timeStr = $"{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";

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
            Glyph = success ? IconGlyphs.CheckMark : IconGlyphs.Critical,
            FontSize = 11,
            Foreground = App.Current.Resources[
                success ? "StatusHealthyBrush" : "StatusCriticalBrush"] as Brush,
            Margin = new Thickness(0, 0, 8, 0),
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(icon, 1);

        var msgText = new TextBlock
        {
            Text = success ? "Repair completed successfully" : "Repair failed",
            FontSize = 12,
            FontFamily = new FontFamily("Cascadia Code, Consolas"),
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            TextWrapping = TextWrapping.Wrap
        };
        Grid.SetColumn(msgText, 2);

        line.Children.Add(timeText);
        line.Children.Add(icon);
        line.Children.Add(msgText);

        LogPanel.Children.Add(line);
    }
}