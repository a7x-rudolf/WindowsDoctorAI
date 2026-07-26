using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WindowsDoctorAI.Models;
using WindowsDoctorAI.Views;

namespace WindowsDoctorAI.Dialogs;

public sealed partial class HistoryDetailDialog : ContentDialog
{
    private readonly ScanHistoryDetail _detail;

    public HistoryDetailDialog(ScanHistoryDetail detail)
    {
        this.InitializeComponent();
        ThemeService.ApplyToDialog(this);
        _detail = detail;

        PopulateDialog();
    }

    private void PopulateDialog()
    {
        var session = _detail.Session;

        TitleText.Text = $"Scan on {session.DateDisplay}";
        MetaText.Text = $"{session.HealthRating} — {session.IssuesSummary} — took {session.ScanDurationSeconds:F1}s";
        ScoreText.Text = session.ScoreDisplay;

        var color = session.OverallScore switch
        {
            >= 90 => ColorHelper.FromArgb(255, 15, 123, 15),
            >= 60 => ColorHelper.FromArgb(255, 183, 113, 13),
            _ => ColorHelper.FromArgb(255, 196, 43, 28)
        };
        ScoreBadge.Background = new SolidColorBrush(color);

        ResultsList.ItemsSource = _detail.Results;
    }
}
