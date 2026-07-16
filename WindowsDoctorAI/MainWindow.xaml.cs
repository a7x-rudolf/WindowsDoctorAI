using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using WindowsDoctorAI.Dialogs;
using WindowsDoctorAI.Helpers;
using WindowsDoctorAI.Services;
using WindowsDoctorAI.ViewModels;
using WindowsDoctorAI.Views;

namespace WindowsDoctorAI;

public sealed partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; } = new MainViewModel();
    private ScanProgressDialog? _currentScanDialog;

    public MainWindow()
    {
        this.InitializeComponent();
        // ═══════════════════════════════════════════════════════════
        // APPLY THEME (defaults to Dark on first run, otherwise loads
        // the user's last saved preference via ThemeService)
        // ═══════════════════════════════════════════════════════════
        if (this.Content is FrameworkElement root)
        {
            root.RequestedTheme = ThemeService.CurrentTheme;
        }
        UpdateThemeToggleVisual();

        // ═══════════════════════════════════════════════════════════
        // APPLY LANGUAGE (defaults to Indonesian on first run, otherwise
        // loads the user's last saved preference via LocalizationService)
        // ═══════════════════════════════════════════════════════════
        ApplyLocalization();
        UpdateLanguageToggleVisual();

        // Register toast host (use RootGrid from MainWindow.xaml)
        Helpers.ToastService.RegisterHost(RootGrid);

        // ═══════════════════════════════════════════════════════════
        // WINDOW CONFIGURATION
        // ═══════════════════════════════════════════════════════════
        this.Title = "Windows Doctor AI";
        this.AppWindow.Resize(new Windows.Graphics.SizeInt32(1400, 900));

        // ═══════════════════════════════════════════════════════════
        // SET APPLICATION ICON (Title Bar & Taskbar)
        // ═══════════════════════════════════════════════════════════
        try
        {
            var iconPath = System.IO.Path.Combine(
                AppContext.BaseDirectory,
                "Assets",
                "AppIcon.ico");

            if (System.IO.File.Exists(iconPath))
            {
                this.AppWindow.SetIcon(iconPath);
            }
            else
            {
                // Fallback: relative path
                this.AppWindow.SetIcon("Assets\\AppIcon.ico");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Icon load error: {ex.Message}");
        }

        // ═══════════════════════════════════════════════════════════
        // USER INFO & ADMIN STATUS
        // ═══════════════════════════════════════════════════════════
        bool isAdmin = AdminHelper.IsRunningAsAdmin();
        UpdateAdminBadge(isAdmin);

        try
        {
            UserNameText.Text = Environment.UserName;
            UserAvatarText.Text = string.IsNullOrEmpty(Environment.UserName)
                ? "?" : Environment.UserName[0].ToString().ToUpper();
            UserRoleText.Text = isAdmin
                ? LocalizationService.GetString("User_ElevatedSession")
                : LocalizationService.GetString("User_StandardUser");

            if (!isAdmin)
            {
                UserStatusIcon.Fill = new SolidColorBrush(ColorHelper.FromArgb(255, 183, 113, 13));
            }
        }
        catch { }

        // ═══════════════════════════════════════════════════════════
        // EVENT SUBSCRIPTIONS
        // ═══════════════════════════════════════════════════════════

        // Keep the toggles in sync if theme/language is changed elsewhere (e.g. SettingsPage)
        ThemeService.ThemeChanged += (_) => DispatcherQueue.TryEnqueue(UpdateThemeToggleVisual);
        LocalizationService.LanguageChanged += (_) => DispatcherQueue.TryEnqueue(() =>
        {
            ApplyLocalization();
            UpdateLanguageToggleVisual();
        });

        // Status bar listener
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;

        // Hook scan events for dialog
        ViewModel.ScanRequested += OnScanRequested;
        ViewModel.CategoryStarted += (c) => _currentScanDialog?.SetCategoryStarted(c);
        ViewModel.CategoryCompleted += (c) => _currentScanDialog?.SetCategoryDone(c);
        ViewModel.ProgressUpdated += (p) => _currentScanDialog?.UpdateProgress(p);
        ViewModel.ResultFound += (r) => _currentScanDialog?.OnResultFound(r);
        ViewModel.ScanCompleted += (s) => _currentScanDialog?.OnScanCompleted(s);
        ViewModel.ScanCancelled += () => _currentScanDialog?.OnScanCancelled();
        ViewModel.ScanFailed += (msg) => _currentScanDialog?.OnScanError(msg);

        // Toast notification untuk repair
        ViewModel.RepairCompleted += (action, success) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    if (this.Content?.XamlRoot != null)
                    {
                        ToastService.Show(
                            this.Content.XamlRoot,
                            success ? ToastType.Success : ToastType.Error,
                            success ? "Repair Completed" : "Repair Failed",
                            success
                                ? $"{action.Name} was executed successfully"
                                : action.ResultMessage ?? "The repair did not complete",
                            durationMs: 3000
                        );
                    }
                }
                catch { }
            });
        };

        // ═══════════════════════════════════════════════════════════
        // INITIAL NAVIGATION
        // ═══════════════════════════════════════════════════════════
        if (NavView.MenuItems.Count > 1)
            NavView.SelectedItem = NavView.MenuItems[1];
    }

    private async void OnScanRequested()
    {
        try
        {
            _currentScanDialog = new ScanProgressDialog
            {
                XamlRoot = this.Content.XamlRoot
            };

            var result = await _currentScanDialog.ShowAsync();
            _currentScanDialog = null;

            // If user clicked "View Results", navigate to Results
            if (result == ContentDialogResult.Primary && ViewModel.HasScanResults)
            {
                // Safer navigation - iterate through MenuItems directly
                foreach (var item in NavView.MenuItems)
                {
                    if (item is NavigationViewItem nvi && nvi.Tag?.ToString() == "results")
                    {
                        NavView.SelectedItem = nvi;
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Dialog error: {ex.Message}");
        }
    }

    private void UpdateAdminBadge(bool isAdmin)
    {
        AdminStatusText.Text = isAdmin
            ? LocalizationService.GetString("Status_Administrator")
            : LocalizationService.GetString("Status_StandardUserLimited");
        AdminStatusText.Foreground = isAdmin
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 15, 123, 15))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 183, 113, 13));
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.StatusMessage):
                    if (!string.IsNullOrEmpty(ViewModel.StatusMessage))
                        StatusText.Text = ViewModel.StatusMessage;
                    break;
                case nameof(ViewModel.IsBusy):
                    StatusProgress.IsActive = ViewModel.IsBusy;
                    StatusDot.Fill = ViewModel.IsBusy
                        ? new SolidColorBrush(ColorHelper.FromArgb(255, 0, 103, 192))
                        : new SolidColorBrush(ColorHelper.FromArgb(255, 15, 123, 15));
                    break;
                case nameof(ViewModel.HasScanResults):
                    if (ViewModel.HasScanResults)
                        LastScanText.Text = string.Format(
                            LocalizationService.GetString("Status_LastScan"),
                            DateTime.Now.ToString("HH:mm"));
                    break;
            }
        });
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item)
        {
            var tag = item.Tag?.ToString() ?? "dashboard";
            NavigateToPage(tag);
        }
    }

    private void NavigateToPage(string tag)
    {
        switch (tag)
        {
            case "dashboard": ContentFrame.Navigate(typeof(DashboardPage), ViewModel); break;
            case "scan": ContentFrame.Navigate(typeof(DiagnosticPage), ViewModel); break;
            case "results": ContentFrame.Navigate(typeof(ResultsPage), ViewModel); break;
            case "repair": ContentFrame.Navigate(typeof(RepairPage), ViewModel); break;
            case "history": ContentFrame.Navigate(typeof(HistoryPage), ViewModel); break;
            case "system": ContentFrame.Navigate(typeof(SystemInfoPage), ViewModel); break;
            case "settings": ContentFrame.Navigate(typeof(SettingsPage), ViewModel); break;
            case "about": ContentFrame.Navigate(typeof(AboutPage), ViewModel); break;
        }
    }

    // ═══════════════════════════════════════════════════════════
    // THEME TOGGLE
    // ═══════════════════════════════════════════════════════════
    private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
    {
        ThemeService.ToggleTheme(this.Content?.XamlRoot);
        UpdateThemeToggleVisual();
    }

    private void UpdateThemeToggleVisual()
    {
        bool isDark = ThemeService.CurrentTheme == ElementTheme.Dark;

        // Show the action the button performs (tap to switch TO the other mode)
        ThemeToggleIcon.Glyph = isDark ? "\uE706" : "\uE708"; // Sunny / QuietHours(moon)
        ThemeToggleLabel.Text = isDark ? "Light" : "Dark";
        ToolTipService.SetToolTip(ThemeToggleButton, LocalizationService.GetString("Theme_ToggleTooltip"));
    }

    // ═══════════════════════════════════════════════════════════
    // LANGUAGE TOGGLE
    // ═══════════════════════════════════════════════════════════
    private void LangEnButton_Click(object sender, RoutedEventArgs e) => SetLanguage(LocalizationService.English);

    private void LangIdButton_Click(object sender, RoutedEventArgs e) => SetLanguage(LocalizationService.Indonesian);

    private void SetLanguage(string code)
    {
        LocalizationService.SetLanguage(code);
        ApplyLocalization();
        UpdateLanguageToggleVisual();
    }

    private void UpdateLanguageToggleVisual()
    {
        bool isEnglish = LocalizationService.CurrentLanguage == LocalizationService.English;

        var activeBrush = App.Current.Resources["AccentButtonBackground"] as Brush
                           ?? new SolidColorBrush(ColorHelper.FromArgb(255, 0, 120, 212));
        var inactiveBrush = new SolidColorBrush(Colors.Transparent);

        LangEnButton.Background = isEnglish ? activeBrush : inactiveBrush;
        LangIdButton.Background = !isEnglish ? activeBrush : inactiveBrush;

        ToolTipService.SetToolTip(LangEnButton, LocalizationService.GetString("Language_ToggleTooltip"));
        ToolTipService.SetToolTip(LangIdButton, LocalizationService.GetString("Language_ToggleTooltip"));
    }

    private void ApplyLocalization()
    {
        HeaderDiagnostics.Content = LocalizationService.GetString("Nav_Diagnostics");
        NavDashboard.Content = LocalizationService.GetString("Nav_Dashboard");
        NavScan.Content = LocalizationService.GetString("Nav_Scan");
        NavResults.Content = LocalizationService.GetString("Nav_Results");
        NavRepair.Content = LocalizationService.GetString("Nav_Repair");

        HeaderInsights.Content = LocalizationService.GetString("Nav_Insights");
        NavHistory.Content = LocalizationService.GetString("Nav_History");
        NavSystem.Content = LocalizationService.GetString("Nav_System");

        NavSettings.Content = LocalizationService.GetString("Nav_Settings");
        NavAbout.Content = LocalizationService.GetString("Nav_About");

        StatusText.Text = LocalizationService.GetString("Status_Ready");
        LastScanText.Text = ViewModel.HasScanResults
            ? string.Format(LocalizationService.GetString("Status_LastScan"), DateTime.Now.ToString("HH:mm"))
            : LocalizationService.GetString("Status_LastScanNever");

        bool isAdmin = AdminHelper.IsRunningAsAdmin();
        AdminStatusText.Text = isAdmin
            ? LocalizationService.GetString("Status_Administrator")
            : LocalizationService.GetString("Status_StandardUserLimited");
        UserRoleText.Text = isAdmin
            ? LocalizationService.GetString("User_ElevatedSession")
            : LocalizationService.GetString("User_StandardUser");
    }
}