using CommunityToolkit.Mvvm.ComponentModel;

namespace WindowsDoctorAI.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _statusMessage = "Ready";
}