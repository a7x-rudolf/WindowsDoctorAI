using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;
using WindowsDoctorAI.Services;
using WindowsDoctorAI.ViewModels;

namespace WindowsDoctorAI.Views;

public sealed partial class SystemInfoPage : Page
{
    public MainViewModel? ViewModel { get; private set; }

    public SystemInfoPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is MainViewModel vm)
        {
            ViewModel = vm;
            LoadSystemInfo();
        }
    }

    private void LoadSystemInfo()
    {
        try
        {
            var info = new SystemInfoService().GetSystemInfo();
            SysInfoRepeater.ItemsSource = info.Select(kv => new { kv.Key, Value = kv.Value ?? "" }).ToList();
        }
        catch { }
    }
}