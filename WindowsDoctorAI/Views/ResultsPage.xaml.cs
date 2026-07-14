using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;
using WindowsDoctorAI.Dialogs;
using WindowsDoctorAI.Models;
using WindowsDoctorAI.ViewModels;

namespace WindowsDoctorAI.Views;

public sealed partial class ResultsPage : Page
{
    public MainViewModel ViewModel { get; private set; } = null!;
    private string _currentFilter = "All";

    public ResultsPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is MainViewModel vm)
        {
            ViewModel = vm;
            ApplyFilter();
            UpdateFilterButtons();

            ViewModel.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(ViewModel.HasScanResults))
                {
                    DispatcherQueue.TryEnqueue(ApplyFilter);
                }
            };
        }
    }

    private void Filter_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string tag)
        {
            _currentFilter = tag;
            UpdateFilterButtons();
            ApplyFilter();
        }
    }

    private void UpdateFilterButtons()
    {
        var activeBg = App.Current.Resources["LayerFillColorDefaultBrush"] as Brush;
        var inactiveBg = new SolidColorBrush(Colors.Transparent);

        FilterAllBtn.Background = _currentFilter == "All" ? activeBg : inactiveBg;
        FilterCriticalBtn.Background = _currentFilter == "Critical" ? activeBg : inactiveBg;
        FilterWarningBtn.Background = _currentFilter == "Warning" ? activeBg : inactiveBg;
        FilterHealthyBtn.Background = _currentFilter == "Healthy" ? activeBg : inactiveBg;

        FilterAllBtn.FontWeight = _currentFilter == "All"
            ? Microsoft.UI.Text.FontWeights.SemiBold : Microsoft.UI.Text.FontWeights.Normal;
        FilterCriticalBtn.FontWeight = _currentFilter == "Critical"
            ? Microsoft.UI.Text.FontWeights.SemiBold : Microsoft.UI.Text.FontWeights.Normal;
        FilterWarningBtn.FontWeight = _currentFilter == "Warning"
            ? Microsoft.UI.Text.FontWeights.SemiBold : Microsoft.UI.Text.FontWeights.Normal;
        FilterHealthyBtn.FontWeight = _currentFilter == "Healthy"
            ? Microsoft.UI.Text.FontWeights.SemiBold : Microsoft.UI.Text.FontWeights.Normal;
    }

    private void ApplyFilter()
    {
        if (ViewModel == null) return;

        var query = ViewModel.AllResults.AsEnumerable();

        if (_currentFilter != "All"
            && System.Enum.TryParse<Severity>(_currentFilter, out var sev))
        {
            query = query.Where(r => r.Severity == sev);
        }

        var list = query
            .OrderBy(r => r.Severity == Severity.Critical ? 0
                        : r.Severity == Severity.Warning ? 1
                        : r.Severity == Severity.Info ? 2 : 3)
            .ToList();

        ResultsList.ItemsSource = list;
        CountText.Text = $"{list.Count} result(s)";
    }

    private async void ResultItem_Click(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border border && border.Tag is DiagnosticResult result)
        {
            var dialog = new ResultDetailDialog(result, ViewModel)
            {
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }

    private async void Export_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel?.ExportReportCommand.CanExecute(null) == true)
            await ViewModel.ExportReportCommand.ExecuteAsync(null);
    }
}