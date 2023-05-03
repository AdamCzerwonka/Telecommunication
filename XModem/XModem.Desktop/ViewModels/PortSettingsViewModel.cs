using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XModem.Desktop.Services;

namespace XModem.Desktop.ViewModels;

public class PortSettingsViewModel : ViewModel
{
    private INavigationService _navigationService = null!;

    public INavigationService NavigationService
    {
        get => _navigationService;
        set
        {
            _navigationService = value;
            OnPropertyChanged();
        }
    }

    public IRelayCommand NextCommand { get; set; }

    public PortSettingsViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
        NextCommand = new RelayCommand(() => NavigationService.NavigateTo<ModeSelectionViewModel>());
    }
}