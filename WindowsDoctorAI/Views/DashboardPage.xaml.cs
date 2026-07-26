using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Linq;
using WindowsDoctorAI.Helpers;
using WindowsDoctorAI.Models;
using WindowsDoctorAI.ViewModels;

namespace WindowsDoctorAI.Views;

public sealed partial class DashboardPage : Page
{
    public MainViewModel ViewModel { get; private set; } = null!;

    public DashboardPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is MainViewModel vm)
        {
            ViewModel = vm;
            LoadCategoryHealth();

            ViewModel.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(ViewModel.HasScanResults))
                {
                    DispatcherQueue.TryEnqueue(LoadCategoryHealth);
                }
            };
        }
    }

    private void LoadCategoryHealth()
    {
        if (ViewModel == null || !ViewModel.HasScanResults)
        {
            CategoryRepeater.ItemsSource = null;
            return;
        }

        var categories = new List<CategoryHealthItem>();
        var grouped = ViewModel.AllResults.GroupBy(r => r.Category);

        foreach (var grp in grouped)
        {
            var results = grp.ToList();
            var avgScore = results.Average(r => r.Score);
            var criticalCount = results.Count(r => r.Severity == Severity.Critical);
            var warningCount = results.Count(r => r.Severity == Severity.Warning);

            string statusType = "Healthy";
            string statusLabel = "Healthy";

            if (criticalCount > 0)
            {
                statusType = "Critical";
                statusLabel = "Attention";
            }
            else if (warningCount > 0)
            {
                statusType = "Warning";
                statusLabel = warningCount > 2 ? "Slow" : "Fair";
            }
            else if (avgScore >= 90)
            {
                statusType = "Healthy";
                statusLabel = "Excellent";
            }

            string desc = results.Count == 1
                ? results[0].Description
                : $"{results.Count} checks - {(criticalCount + warningCount)} issue(s) found";

            categories.Add(new CategoryHealthItem
            {
                Glyph = GetGlyphForCategory(grp.Key),
                Name = GetDisplayName(grp.Key),
                Description = desc.Length > 80 ? string.Concat(desc.AsSpan(0, 77), "...") : desc,
                StatusLabel = statusLabel,
                StatusType = statusType,
                Score = System.Math.Round(avgScore)
            });
        }

        CategoryRepeater.ItemsSource = categories;
    }

    private static string GetGlyphForCategory(DiagnosticCategory cat) => cat switch
    {
        DiagnosticCategory.DiskHealth => IconGlyphs.Disk,
        DiagnosticCategory.Performance => IconGlyphs.Performance,
        DiagnosticCategory.Network => IconGlyphs.Network,
        DiagnosticCategory.Security => IconGlyphs.Security,
        DiagnosticCategory.WindowsUpdate => IconGlyphs.Update,
        DiagnosticCategory.Drivers => IconGlyphs.SystemInfo,
        DiagnosticCategory.StartupPrograms => IconGlyphs.Startup,
        _ => IconGlyphs.Info
    };

    private static string GetDisplayName(DiagnosticCategory cat) => cat switch
    {
        DiagnosticCategory.DiskHealth => "Disk Health",
        DiagnosticCategory.Performance => "Performance",
        DiagnosticCategory.Network => "Network",
        DiagnosticCategory.Security => "Security",
        DiagnosticCategory.WindowsUpdate => "Windows Update",
        DiagnosticCategory.Drivers => "Drivers",
        DiagnosticCategory.StartupPrograms => "Startup",
        _ => cat.ToString()
    };

    private async void RunScan_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel?.RunScanCommand.CanExecute(null) == true)
            await ViewModel.RunScanCommand.ExecuteAsync(null);
    }

    private async void ExportReport_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel?.ExportReportCommand.CanExecute(null) == true)
            await ViewModel.ExportReportCommand.ExecuteAsync(null);
    }

    private async void FixAll_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel?.ExecuteAllRepairsCommand.CanExecute(null) != true)
            return;

        var safeRepairs = ViewModel.PendingRepairs
            .Where(r => r.ActionType == RepairActionType.Automatic &&
                       (r.RiskLevel == "Low" || r.RiskLevel == "None") &&
                        r.Status == RepairStatus.Pending)
            .ToList();

        if (safeRepairs.Count == 0)
        {
            var dialog = new ContentDialog
            {
                Title = "No Repairs Available",
                Content = "There are no safe automatic repairs available at this time. Please review the results manually.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
            return;
        }

        var confirmDialog = new ContentDialog
        {
            Title = "Execute Safe Repairs",
            Content = $"This will execute {safeRepairs.Count} safe automatic repair(s) with low or no risk.\n\n" +
                      $"Repairs to be executed:\n" +
                      string.Join("\n", safeRepairs.Select(r => $"• {r.Name}")),
            PrimaryButtonText = "Execute All",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        var result = await confirmDialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.ExecuteAllRepairsCommand.ExecuteAsync(null);
        }
    }
}