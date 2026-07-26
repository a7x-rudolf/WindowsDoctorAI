using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using WindowsDoctorAI.ViewModels;

namespace WindowsDoctorAI.Views;

public sealed partial class SettingsPage : Page
{
    public MainViewModel? ViewModel { get; private set; }
    private string _currentSection = "Appearance";

    public SettingsPage()
    {
        this.InitializeComponent();

        var currentTheme = ThemeService.CurrentTheme;
        ThemeCombo.SelectedIndex = currentTheme switch
        {
            ElementTheme.Light => 1,
            ElementTheme.Dark => 2,
            _ => 0
        };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is MainViewModel vm)
            ViewModel = vm;
    }

    private void NavSection_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string tag)
        {
            _currentSection = tag;
            UpdateNavButtons();
            ShowSection(tag);
        }

        ArgumentNullException.ThrowIfNull(e);
    }

    private void UpdateNavButtons()
    {
        var activeBg = App.Current.Resources["LayerFillColorDefaultBrush"] as Brush;
        var inactiveBg = new SolidColorBrush(Colors.Transparent);

        NavAppearance.Background = _currentSection == "Appearance" ? activeBg : inactiveBg;
        NavScanPrefs.Background = _currentSection == "ScanPrefs" ? activeBg : inactiveBg;
        NavNotifications.Background = _currentSection == "Notifications" ? activeBg : inactiveBg;
        NavAdvanced.Background = _currentSection == "Advanced" ? activeBg : inactiveBg;
        NavAbout.Background = _currentSection == "About" ? activeBg : inactiveBg;
    }

    private void ShowSection(string section)
    {
        AppearanceSection.Visibility = section == "Appearance" ? Visibility.Visible : Visibility.Collapsed;
        ScanPrefsSection.Visibility = section == "ScanPrefs" ? Visibility.Visible : Visibility.Collapsed;
        NotificationsSection.Visibility = section == "Notifications" ? Visibility.Visible : Visibility.Collapsed;
        AdvancedSection.Visibility = section == "Advanced" ? Visibility.Visible : Visibility.Collapsed;
        AboutSection.Visibility = section == "About" ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ThemeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ThemeCombo.SelectedItem is ComboBoxItem item && item.Tag is string tag)
        {
            var theme = tag switch
            {
                "Light" => ElementTheme.Light,
                "Dark" => ElementTheme.Dark,
                _ => ElementTheme.Default
            };
            ThemeService.SetTheme(theme, this.XamlRoot);
        }
    }
}