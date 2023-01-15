using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ToggleHypervisor.Services;
using ToggleHypervisor.Views;
using ToggleHypervisor.ViewModels;
using ToggleHypervisor.Models;
using System.IO;
using static System.Diagnostics.Debug;
using System.Threading.Tasks;
using Logging;
using System.Reflection;

namespace ToggleHypervisor
{
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();

            new AdminChecker().ReRunAsAdmin();
            new SettingsFileCreator().Create();

            InitializeComponent();
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services;

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<FileLocations>();
            services.AddSingleton(new FileLogger("ToggleHypervisor.log", 256));
            services.AddSingleton<SettingsData>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainPageViewModel>();
            services.AddSingleton<DetailsPageViewModel>();

            return services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindowViewModel mainWindowViewModel = Services.GetService<MainWindowViewModel>();
            mainWindowViewModel.Initialize();
            mainWindowViewModel.CurrentPage = new MainPage();

            MainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
            MainWindow.Show();
        }
    }
}
