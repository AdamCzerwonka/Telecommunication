using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using XModem.Desktop.Helpers;
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

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var startWindom = _serviceProvider.GetRequiredService<PortSettingsView>();
            startWindom.Show();
            base.OnStartup(e);
        }
    }
}