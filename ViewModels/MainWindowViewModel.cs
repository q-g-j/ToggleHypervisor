using QGJSoft.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using ToggleHypervisor.Models;
using ToggleHypervisor.Services;
using ToggleHypervisor.Views;

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
