using XModem.Desktop.ViewModels;

namespace XModem.Desktop.Services;

public interface INavigationService
{
    ViewModel CurrentView { get; }
    void NavigateTo<T>() where T : ViewModel;
}