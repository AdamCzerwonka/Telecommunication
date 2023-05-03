using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using XModem.Core;
using XModem.Desktop.Helpers;
using XModem.Desktop.Services;
using XModem.Desktop.ViewModels;
using XModem.Desktop.Views;

namespace XModem.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddViewModel<PortSettingsView, PortSettingsViewModel>();
            services.AddViewModel<ModeSelectionView, ModeSelectionViewModel>();
            services.AddViewModel<SenderView, SenderViewModel>();
            services.AddViewModel<ReceiverView, ReceiverViewModel>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>(provider => new MainWindow()
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });

            services.AddSingleton<Func<Type, ViewModel>>(provider =>
                viewModelType => (ViewModel)provider.GetRequiredService(viewModelType));

            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<SerialPortConfiguration>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var startWindom = _serviceProvider.GetRequiredService<MainWindow>();
            startWindom.Show();
            base.OnStartup(e);
        }
    }
}