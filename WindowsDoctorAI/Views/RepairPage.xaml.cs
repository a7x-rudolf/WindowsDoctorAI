using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WindowsDoctorAI.Dialogs;
using WindowsDoctorAI.Models;
using WindowsDoctorAI.ViewModels;

namespace WindowsDoctorAI.Views;

public sealed partial class RepairPage : Page
{
    public MainViewModel ViewModel { get; private set; } = null!;

    public RepairPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is MainViewModel vm)
            ViewModel = vm;
    }

    private async void Execute_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not RepairAction action) return;

        // Show confirm dialog
        var confirmDialog = new ConfirmRepairDialog(action)
        {
            XamlRoot = this.XamlRoot
        };

        var result = await confirmDialog.ShowAsync();
        if (result != ContentDialogResult.Primary) return;

        // Show progress dialog and execute
        var progressDialog = new RepairProgressDialog(action)
        {
            XamlRoot = this.XamlRoot
        };

        // Hook events
        void OnProgress(RepairAction a, string msg)
        {
            if (a.Id == action.Id)
                DispatcherQueue.TryEnqueue(() => progressDialog.AddLog(msg));
        }

        void OnCompleted(RepairAction a, bool success)
        {
            if (a.Id == action.Id)
                DispatcherQueue.TryEnqueue(() => progressDialog.OnCompleted(success));
        }

        ViewModel.RepairProgress += OnProgress;
        ViewModel.RepairCompleted += OnCompleted;

        var progressTask = progressDialog.ShowAsync();
        await ViewModel.ExecuteRepairCommand.ExecuteAsync(action);
        await progressTask;

        ViewModel.RepairProgress -= OnProgress;
        ViewModel.RepairCompleted -= OnCompleted;
    }

    private async void FixAll_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new ContentDialog
        {
            Title = "Fix All Safe Issues",
            Content = "Execute all low-risk automatic repairs?\n\nManual and medium/high-risk actions will be skipped for your safety.",
            PrimaryButtonText = "Continue",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.XamlRoot
        };

        if (await dlg.ShowAsync() == ContentDialogResult.Primary)
        {
            await ViewModel.ExecuteAllRepairsCommand.ExecuteAsync(null);
        }
    }
}