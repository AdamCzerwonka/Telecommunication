using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using XModem.Desktop.ViewModels;

namespace XModem.Desktop.Helpers;

public static class ViewModelHelpers
{
    public static IServiceCollection AddViewModel<TView, TViewModel>(this IServiceCollection services)
        where TView : Window, new() where TViewModel : ViewModel
    {
        services.AddSingleton<TViewModel>();
        services.AddSingleton<TView>(provider => new TView()
        {
            DataContext = provider.GetRequiredService<TViewModel>()
        });

        return services;
    }
}