using CommunityToolkit.Mvvm.Input;
using XModem.Desktop.Services;

namespace XModem.Desktop.ViewModels;

public class ModeSelectionViewModel : ViewModel
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

    public ModeSelectionViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
        NavigateSenderCommand = new RelayCommand(NavigateSender);
        NavigateReceiverCommand = new RelayCommand(NavigateReceiver);
    }

    public IRelayCommand NavigateSenderCommand { get; }
    public IRelayCommand NavigateReceiverCommand { get; }

    private void NavigateSender()
    {
        NavigationService.NavigateTo<SenderViewModel>();
    }

    private void NavigateReceiver()
    {
        NavigationService.NavigateTo<ReceiverViewModel>();
    }
}