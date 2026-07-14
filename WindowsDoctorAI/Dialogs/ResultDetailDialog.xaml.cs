using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WindowsDoctorAI.Helpers;
using WindowsDoctorAI.Models;
using WindowsDoctorAI.ViewModels;

namespace WindowsDoctorAI.Dialogs;

public sealed partial class ResultDetailDialog : ContentDialog
{
    private readonly DiagnosticResult _result;
    private readonly MainViewModel _viewModel;

    public ResultDetailDialog(DiagnosticResult result, MainViewModel viewModel)
    {
        this.InitializeComponent();
        _result = result;
        _viewModel = viewModel;

        PopulateDialog();
    }

    private void PopulateDialog()
    {
        // Icon + colors
        HeaderIcon.Glyph = _result.Severity switch
        {
            Severity.Healthy => IconGlyphs.CheckMark,
            Severity.Warning => IconGlyphs.Warning,
            Severity.Critical => IconGlyphs.Critical,
            _ => IconGlyphs.Info
        };

        var (bgKey, fgKey) = _result.Severity switch
        {
            Severity.Healthy => ("StatusHealthyBgBrush", "StatusHealthyBrush"),
            Severity.Warning => ("StatusWarningBgBrush", "StatusWarningBrush"),
            Severity.Critical => ("StatusCriticalBgBrush", "StatusCriticalBrush"),
            _ => ("StatusInfoBgBrush", "StatusInfoBrush")
        };

        HeaderIconBorder.Background = App.Current.Resources[bgKey] as Brush;
        HeaderIcon.Foreground = App.Current.Resources[fgKey] as Brush;

        // Text
        TitleText.Text = _result.Title;
        MetaText.Text = $"{_result.CategoryDisplayName} - {_result.Severity} - Score: {_result.Score}";
        DescText.Text = _result.Description;
        DetailsText.Text = _result.Details ?? "";

        // Actions
        var actions = _result.AvailableActions;
        if (actions.Count > 0)
        {
            ActionsList.ItemsSource = actions;
            ActionCountText.Text = $"{actions.Count} ACTION(S)";
            NoActionsHint.Visibility = Visibility.Collapsed;
        }
        else
        {
            ActionsList.Visibility = Visibility.Collapsed;
            ActionCountText.Text = "0 ACTIONS";
        }
    }

    private async void ExecuteAction_Click(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(e);
        if (sender is Button btn && btn.Tag is RepairAction action)
        {
            this.Hide();

            var confirm = new ConfirmRepairDialog(action) { XamlRoot = this.XamlRoot };
            var result = await confirm.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var progress = new RepairProgressDialog(action) { XamlRoot = this.XamlRoot };

                void OnProgress(RepairAction a, string msg)
                {
                    if (a.Id == action.Id)
                        DispatcherQueue.TryEnqueue(() => progress.AddLog(msg));
                }

                void OnCompleted(RepairAction a, bool success)
                {
                    if (a.Id == action.Id)
                        DispatcherQueue.TryEnqueue(() => progress.OnCompleted(success));
                }

                _viewModel.RepairProgress += OnProgress;
                _viewModel.RepairCompleted += OnCompleted;

                var progressTask = progress.ShowAsync();
                await _viewModel.ExecuteRepairCommand.ExecuteAsync(action);
                await progressTask;

                _viewModel.RepairProgress -= OnProgress;
                _viewModel.RepairCompleted -= OnCompleted;
            }
        }
    }
}