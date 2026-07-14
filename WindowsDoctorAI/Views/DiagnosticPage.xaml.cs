using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WindowsDoctorAI.ViewModels;

namespace WindowsDoctorAI.Views;

public sealed partial class DiagnosticPage : Page
{
    public MainViewModel ViewModel { get; private set; } = null!;
    public DiagnosticPage() => this.InitializeComponent();

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is MainViewModel vm) ViewModel = vm;
    }

    private async void Start_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.RunScanCommand.CanExecute(null))
            await ViewModel.RunScanCommand.ExecuteAsync(null);
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) =>
        ViewModel.CancelScanCommand.Execute(null);
}