using XModem.Desktop.Services;

namespace XModem.Desktop.ViewModels;

public class MainViewModel : ViewModel
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

    public MainViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
        NavigationService.NavigateTo<PortSettingsViewModel>();
    }
}