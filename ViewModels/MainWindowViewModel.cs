﻿using QGJSoft.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using ToggleHypervisor.Models;
using ToggleHypervisor.Services;
using ToggleHypervisor.Views;
using System.Windows;

namespace ToggleHypervisor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            fileLogger = FileLoggerFactory.GetFileLogger();
            LogEvent += fileLogger.LogWriteLine;

            Task.Run(() =>
            {
                string message = new string('*', 5);
                message += " Program started ";
                message += new string('*', 5);

                var loggerEventArgs = new LoggerEventArgs(
                    message,
                    GetType().Name,
                    MethodBase.GetCurrentMethod().Name,
                    null
                    );
                RaiseLogEvent(this, loggerEventArgs);
            });

            WindowsVersionChecker windowsVersionChecker = new WindowsVersionChecker();

            if (windowsVersionChecker.HasOSChanged())
            {
                var settingsData = App.Current.Services.GetService<SettingsData>();
                settingsData.LastKnownOSVersion = windowsVersionChecker.OsFullVersionString;
                SettingsFileWriter settingsFileWriter = new SettingsFileWriter();
                settingsFileWriter.Write();

                windowsVersionChecker = new WindowsVersionChecker();
                if (!windowsVersionChecker.IsHyperVCapable())
                {
                    var messageBoxResult = MessageBox.Show("Your Windows version doesn't support Hyper-V.\n\nPress OK to close the application.", "Error", MessageBoxButton.OK);
                    App.Current.Shutdown();
                }
            }
        }

        private MainPageViewModel mainPageViewModel;
        private DetailsPageViewModel detailsPageViewModel;

        private readonly FileLogger fileLogger;

        private UserControl currentPage;
        public UserControl CurrentPage
        {
            get => currentPage;
            set
            {
                currentPage = value;
                if (currentPage is MainPage)
                {
                    mainPageViewModel.RunChecks();
                }
                if (currentPage is DetailsPage)
                {
                    detailsPageViewModel.RunChecks();
                }
                OnPropertyChanged();
            }
        }

        public void Initialize()
        {
            mainPageViewModel = App.Current.Services.GetService<MainPageViewModel>();
            detailsPageViewModel = App.Current.Services.GetService<DetailsPageViewModel>();
        }
    }
}
