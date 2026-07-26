using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using WindowsDoctorAI.Dialogs;
using WindowsDoctorAI.Models;
using WindowsDoctorAI.ViewModels;

namespace WindowsDoctorAI.Views;

public sealed partial class HistoryPage : Page
{
    public MainViewModel? ViewModel { get; private set; }

    public HistoryPage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is MainViewModel vm)
        {
            ViewModel = vm;
            if (ViewModel.LoadHistoryCommand.CanExecute(null))
                await ViewModel.LoadHistoryCommand.ExecuteAsync(null);
        }
    }

    private void GoToScan_Click(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(e);
        this.Frame?.Navigate(typeof(DiagnosticPage), ViewModel);
    }

    private async void ViewDetail_Click(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(e);
        if (ViewModel == null || sender is not Button btn || btn.Tag is not ScanHistoryEntry entry)
            return;

        await ViewModel.ViewHistoryDetailCommand.ExecuteAsync(entry);

        if (ViewModel.SelectedHistoryDetail != null)
        {
            var dialog = new HistoryDetailDialog(ViewModel.SelectedHistoryDetail)
            {
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }

    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(e);
        if (ViewModel == null || sender is not Button btn || btn.Tag is not ScanHistoryEntry entry)
            return;

        var confirm = new ContentDialog
        {
            Title = "Delete scan?",
            Content = $"Remove the scan from {entry.DateDisplay}? This can't be undone.",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.XamlRoot
        };

        var result = await confirm.ShowAsync();
        if (result == ContentDialogResult.Primary)
            await ViewModel.DeleteHistoryEntryCommand.ExecuteAsync(entry);
    }

    private async void ClearAll_Click(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(e);
        if (ViewModel == null) return;

        var confirm = new ContentDialog
        {
            Title = "Clear all scan history?",
            Content = "This removes every saved scan session. This can't be undone.",
            PrimaryButtonText = "Clear All",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.XamlRoot
        };

        var result = await confirm.ShowAsync();
        if (result == ContentDialogResult.Primary)
            await ViewModel.ClearHistoryCommand.ExecuteAsync(null);
    }
}
