using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WindowsDoctorAI.ViewModels;

namespace WindowsDoctorAI.Views;

public sealed partial class AboutPage : Page
{
    public AboutPage() => this.InitializeComponent();
    protected override void OnNavigatedTo(NavigationEventArgs e) => base.OnNavigatedTo(e);
}