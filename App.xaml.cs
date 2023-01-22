using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ToggleHypervisor.Services;
using ToggleHypervisor.Views;
using ToggleHypervisor.ViewModels;
using ToggleHypervisor.Models;
using QGJSoft.Logging;
using System.IO;
using System.Diagnostics;

namespace ToggleHypervisor
{
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();

            string folderName = "ToggleHypervisor";
            string fileName = "Settings.json";

            var fileLocations = Current.Services.GetService<FileLocations>();
            fileLocations.AppDataRoaming = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            fileLocations.SettingsFolderName = Path.Combine(fileLocations.AppDataRoaming, folderName);
            fileLocations.SettingsFileName = Path.Combine(fileLocations.SettingsFolderName, fileName);

            var settingsData = Current.Services.GetService<SettingsData>();

            bool isSettingsFileValid = false;

            if (SettingsFileValidator.FileExists())
            {
                if (SettingsFileValidator.IsValid())
                {
                    isSettingsFileValid = true;
                    var settingsDataInFile = SettingsFileReader.Load();
                    settingsData.LastKnownOSVersion = settingsDataInFile.LastKnownOSVersion;
                    settingsData.MaxLogFileSizeInKB = settingsDataInFile.MaxLogFileSizeInKB;
                    settingsData.RebootAfterToggle = settingsDataInFile.RebootAfterToggle;
                }
            }
            if (!isSettingsFileValid)
            {
                SettingsFileCreator.GetInstance().Create();
            }

            InitializeComponent();

            MainWindowViewModel mainWindowViewModel = Services.GetService<MainWindowViewModel>();
            mainWindowViewModel.Initialize();
            mainWindowViewModel.CurrentPage = new MainPage();

            MainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
            MainWindow.Show();
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services;

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<SettingsData>();
            services.AddSingleton<FileLocations>();
            services.AddSingleton<FileLogger>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainPageViewModel>();
            services.AddSingleton<DetailsPageViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
