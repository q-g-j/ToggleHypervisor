using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using QGJSoft.SettingsFile;
using ToggleHypervisor.Views;
using ToggleHypervisor.ViewModels;
using ToggleHypervisor.Models;
using QGJSoft.Logging;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Threading.Tasks;

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
            bool isFileAccessError = false;

            if (SettingsFileValidator<SettingsData>.FileExists(fileLocations.SettingsFileName))
            {
                if (SettingsFileValidator<SettingsData>.IsValid(fileLocations.SettingsFileName))
                {
                    var settingsDataInFile = new SettingsData();

                    isSettingsFileValid = true;
                    try
                    {
                        settingsDataInFile = SettingsFileReader<SettingsData>.Load(fileLocations.SettingsFileName);
                    }
                    catch (Exception ex)
                    {
                        isFileAccessError = true;
                        MessageBox.Show("There was an error processing the settings file:\n\n" +
                            ex.GetType() + ":\n" + ex.Message + "\n\n" +
                            "Press OK to close the application.", "Error", MessageBoxButton.OK);
                        Current.Shutdown();
                    }

                    settingsData.LastKnownOSVersion = settingsDataInFile.LastKnownOSVersion;
                    settingsData.IsOSHyperVCapable = settingsDataInFile.IsOSHyperVCapable;
                    settingsData.MaxLogFileSizeInKB = settingsDataInFile.MaxLogFileSizeInKB;
                    settingsData.RebootAfterToggle = settingsDataInFile.RebootAfterToggle;
                }
            }
            if (!isSettingsFileValid)
            {
                try
                {
                    SettingsFileCreator<SettingsData>.Create(fileLocations.SettingsFolderName, fileLocations.SettingsFileName);
                }
                catch (Exception ex)
                {
                    isFileAccessError = true;
                    MessageBox.Show("There was an error processing the settings file:\n\n" +
                        ex.GetType() + ":\n" + ex.Message + "\n\n" +
                        "Press OK to close the application.", "Error", MessageBoxButton.OK);
                    Current.Shutdown();
                }
            }

            if (!isFileAccessError)
            {
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
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services;

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<SettingsData>();
            services.AddSingleton<FileLocations>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainPageViewModel>();
            services.AddSingleton<DetailsPageViewModel>();

            return services.BuildServiceProvider();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            FileLogger.CancellationTokenSource.Cancel();
            base.OnExit(e);
        }
    }
}
