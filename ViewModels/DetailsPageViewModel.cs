using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ToggleHypervisor.Services;
using ToggleHypervisor.Views;
using QGJSoft.Logging;
using ToggleHypervisor.Models;
using QGJSoft.SettingsFile;
using System.Reflection;

namespace ToggleHypervisor.ViewModels
{
    public class DetailsPageViewModel : ViewModelBase
    {
        public DetailsPageViewModel()
        {
            fileLogger = FileLoggerFactory.GetFileLogger();
            LogEvent += fileLogger.LogWriteLine;

            ButtonBackCommand = new RelayCommand(() => ButtonBackAction());
            ButtonToggleFlagCommand = new RelayCommand(() => ButtonToggleFlagAction());
            ButtonToggleComponentsCommand = new RelayCommand(() => ButtonToggleComponentsAction());
            ButtonRebootCommand = new RelayCommand(() => ButtonRebootAction());
            checkTaskDelegates = new CheckTaskDelegate[2]
            {
                GetIsHypervisorlaunchtypeFlagSetTask,
                GetAreComponentsInstalledTask
            };

            settingsData = App.Current.Services.GetService<SettingsData>();
            mainWindowViewModel = App.Current.Services.GetService<MainWindowViewModel>();
            mainPageViewModel = App.Current.Services.GetService<MainPageViewModel>();

            hypervisorChecker = new HypervisorChecker();
            hypervisorSwitcher = new HypervisorSwitcher();
            componentsInstaller = new ComponentsInstaller();
            componentsRemover = new ComponentsRemover();
        }

        private bool firstRun = true;
        private bool wasFlagInitiallySet = false;
        private bool isFlagSet = false;
        private bool isFlagCheckFinished = false;
        private bool wereComponentsInitiallyInstalled = false;
        private bool areComponentsInstalled = false;
        private bool isComponentsCheckFinished = false;

        private Task setInitialBooleansTask;
        private delegate Task CheckTaskDelegate();
        private readonly CheckTaskDelegate[] checkTaskDelegates;
        private readonly Task[] checkTasks = new Task[2];

        private readonly FileLogger fileLogger;
        private readonly SettingsData settingsData;
        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly MainPageViewModel mainPageViewModel;

        private readonly HypervisorChecker hypervisorChecker;
        private readonly HypervisorSwitcher hypervisorSwitcher;
        private readonly ComponentsInstaller componentsInstaller;
        private readonly ComponentsRemover componentsRemover;

        private string labelStatusHypervisorlaunchtype = "Boot flag \"hypervisorlaunchtype\" set:";
        public string LabelStatusHypervisorlaunchtype
        {
            get => labelStatusHypervisorlaunchtype;
            set
            {
                labelStatusHypervisorlaunchtype = value;
                OnPropertyChanged();
            }
        }

        private string labelStatusHypervisorlaunchtypeResult = "";
        public string LabelStatusHypervisorlaunchtypeResult
        {
            get => labelStatusHypervisorlaunchtypeResult;
            set
            {
                labelStatusHypervisorlaunchtypeResult = value;
                OnPropertyChanged();
            }
        }

        private string labelStatusComponentsInstalled = "Hyper-V components installed:";
        public string LabelStatusComponentsInstalled
        {
            get => labelStatusComponentsInstalled;
            set
            {
                labelStatusComponentsInstalled = value;
                OnPropertyChanged();
            }
        }

        private string labelStatusComponentsInstalledResult = "";
        public string LabelStatusComponentsInstalledResult
        {
            get => labelStatusComponentsInstalledResult;
            set
            {
                labelStatusComponentsInstalledResult = value;
                OnPropertyChanged();
            }
        }

        private string buttonToggleComponentsText;

        public string ButtonToggleComponentsText
        {
            get => buttonToggleComponentsText;
            set
            {
                buttonToggleComponentsText = value;
                OnPropertyChanged();
            }
        }

        private string checkBoxRebootText = "Reboot on toggle";
        public string CheckBoxRebootText
        {
            get => checkBoxRebootText;
            set
            {
                checkBoxRebootText = value;
                OnPropertyChanged();
            }
        }

        private bool checkBoxRebootIsChecked;
        public bool CheckBoxRebootIsChecked
        {
            get => checkBoxRebootIsChecked;
            set
            {
                checkBoxRebootIsChecked = value;
                settingsData.RebootAfterToggle = value;
                var fileLocations = App.Current.Services.GetService<FileLocations>();

                try
                {
                    SettingsFileWriter<SettingsData>.Write(fileLocations.SettingsFileName, settingsData);
                }
                catch (Exception ex)
                {
                    var loggerEventArgs = new LoggerEventArgs(
                        "",
                        GetType().Name,
                        MethodBase.GetCurrentMethod().ToString(),
                        ex
                        );
                    RaiseLogEvent(this, loggerEventArgs);
                }

                OnPropertyChanged();
            }
        }

        private string labelStatusOverallVisibility = "Hidden";
        public string LabelStatusOverallVisibility
        {
            get => labelStatusOverallVisibility;
            set
            {
                labelStatusOverallVisibility = value;
                OnPropertyChanged();
            }
        }

        private string labelStatusOverallResultVisibility = "Hidden";
        public string LabelStatusOverallResultVisibility
        {
            get => labelStatusOverallResultVisibility;
            set
            {
                labelStatusOverallResultVisibility = value;
                OnPropertyChanged();
            }
        }

        private string buttonToggleFlagIsEnabled = "False";
        public string ButtonToggleFlagIsEnabled
        {
            get => buttonToggleFlagIsEnabled;
            set
            {
                buttonToggleFlagIsEnabled = value;
                OnPropertyChanged();
            }
        }

        private string buttonToggleComponentsIsEnabled = "False";
        public string ButtonToggleComponentsIsEnabled
        {
            get => buttonToggleComponentsIsEnabled;
            set
            {
                buttonToggleComponentsIsEnabled = value;
                OnPropertyChanged();
            }
        }

        private string buttonBackIsEnabled = "True";
        public string ButtonBackIsEnabled
        {
            get => buttonBackIsEnabled;
            set
            {
                buttonBackIsEnabled = value;
                OnPropertyChanged();
            }
        }

        private string buttonRebootVisibility = "Hidden";
        public string ButtonRebootVisibility
        {
            get => buttonRebootVisibility;
            set
            {
                buttonRebootVisibility = value;
                OnPropertyChanged();
            }
        }

        private Task GetIsHypervisorlaunchtypeFlagSetTask()
        {
            return new Task(() =>
            {
                ButtonToggleFlagIsEnabled = "False";

                if (hypervisorChecker.IsHypervisorlaunchtypeFlagSet())
                {
                    isFlagSet = true;
                    LabelStatusHypervisorlaunchtypeResult = "Yes";
                    ButtonToggleFlagIsEnabled = "True";
                }
                else
                {
                    isFlagSet = false;
                    LabelStatusHypervisorlaunchtypeResult = "No";
                    ButtonToggleFlagIsEnabled = "True";
                }

                isFlagCheckFinished = true;

                if (isComponentsCheckFinished)
                {
                    LabelStatusOverallVisibility = "Visible";
                    LabelStatusOverallResultVisibility = "Visible";
                }
            });
        }

        private Task GetAreComponentsInstalledTask()
        {
            return new Task(() =>
            {
                ButtonToggleComponentsIsEnabled = "False";

                if (hypervisorChecker.AreComponentsInstalled())
                {
                    areComponentsInstalled = true;
                    LabelStatusComponentsInstalledResult = "Yes";
                    ButtonToggleComponentsText = "Remove";
                    ButtonToggleComponentsIsEnabled = "True";
                }
                else
                {
                    areComponentsInstalled = false;
                    LabelStatusComponentsInstalledResult = "No";
                    ButtonToggleComponentsText = "Install";
                    ButtonToggleComponentsIsEnabled = "True";
                }

                isComponentsCheckFinished = true;

                if (isFlagCheckFinished)
                {
                    LabelStatusOverallVisibility = "Visible";
                    LabelStatusOverallResultVisibility = "Visible";
                }

                ButtonToggleComponentsIsEnabled = "True";
            });
        }

        private Task GetIntitialBooleansTask()
        {
            return new Task(() =>
            {
                wasFlagInitiallySet = hypervisorChecker.IsHypervisorlaunchtypeFlagSet();
                wereComponentsInitiallyInstalled = hypervisorChecker.AreComponentsInstalled();
            });
        }

        public void RunChecks()
        {
            if (firstRun)
            {
                firstRun = false;
                setInitialBooleansTask = GetIntitialBooleansTask();
                setInitialBooleansTask.Start();
            }

            LabelStatusComponentsInstalledResult = "";
            LabelStatusHypervisorlaunchtypeResult = "";
            ButtonToggleComponentsText = "Wait...";

            var sd = new SettingsData();
            try
            {
                var fileLocations = App.Current.Services.GetService<FileLocations>();
                sd = SettingsFileReader<SettingsData>.Load(fileLocations.SettingsFileName);
            }
            catch (Exception ex)
            {
                var loggerEventArgs = new LoggerEventArgs(
                    "",
                    GetType().Name,
                    MethodBase.GetCurrentMethod().ToString(),
                    ex
                    );
                RaiseLogEvent(this, loggerEventArgs);
            }

            settingsData.MaxLogFileSizeInKB = sd.MaxLogFileSizeInKB;
            settingsData.RebootAfterToggle = sd.RebootAfterToggle;

            if (settingsData.RebootAfterToggle)
            {
                CheckBoxRebootIsChecked = true;
            }
            else
            {
                CheckBoxRebootIsChecked = false;
            }

            for (int i = 0; i < checkTasks.Length; i++)
            {
                checkTasks[i] = checkTaskDelegates[i]();
                checkTasks[i].Start();
            }

            ToggleRebootButtonVisibility();
        }

        private void ToggleRebootButtonVisibility()
        {
            Task.Run(async () =>
            {
                while (setInitialBooleansTask.Status != TaskStatus.RanToCompletion)
                {
                    await Task.Delay(10);
                }

                bool runLoop = true;
                while (runLoop)
                {
                    bool areAllTasksFinished = true;

                    for (int i = 0; i < checkTasks.Length; i++)
                    {
                        if (checkTasks[i].Status != TaskStatus.RanToCompletion)
                        {
                            areAllTasksFinished = false;
                        }
                    }

                    runLoop = !areAllTasksFinished;
                }

                //bool isEnabledOverall = hypervisorChecker.IsEnabledOverall();

                if (
                    wasFlagInitiallySet != isFlagSet ||
                    wereComponentsInitiallyInstalled != areComponentsInstalled
                    )
                {
                    ButtonRebootVisibility = "Visible";
                }
                else
                {
                    ButtonRebootVisibility = "Hidden";
                }
            });
        }

        public ICommand ButtonBackCommand { get; set; }

        private void ButtonBackAction()
        {
            mainWindowViewModel.CurrentPage = new MainPage();
            ButtonRebootVisibility = "Hidden";
        }

        public ICommand ButtonToggleFlagCommand { get; set; }

        private void ButtonToggleFlagAction()
        {
            ButtonToggleFlagIsEnabled = "False";
            ButtonBackIsEnabled = "False";
            ButtonRebootVisibility = "Hidden";
            LabelStatusHypervisorlaunchtypeResult = "Wait...";

            hypervisorSwitcher.Switch(!isFlagSet);

            bool isFlagNowSet = hypervisorChecker.IsHypervisorlaunchtypeFlagSet();

            if (isFlagSet == isFlagNowSet)
            {
                ButtonToggleFlagIsEnabled = "False";
                LabelStatusHypervisorlaunchtypeResult = "Error";
            }
            else
            {
                if (isFlagSet && !isFlagNowSet)
                {
                    LabelStatusHypervisorlaunchtypeResult = "No";
                    mainPageViewModel.LabelIsHypervisorlaunchtypeSet = "The Hypervisor boot flag is NOT set.";
                }
                else
                {
                    LabelStatusHypervisorlaunchtypeResult = "Yes";
                    mainPageViewModel.LabelIsHypervisorlaunchtypeSet = "The Hypervisor boot flag is set.";
                }
                isFlagSet = isFlagNowSet;
                ButtonToggleFlagIsEnabled = "True";
                ButtonBackIsEnabled = "True";
                ToggleRebootButtonVisibility();
            }
        }

        public ICommand ButtonToggleComponentsCommand { get; set; }

        private void ButtonToggleComponentsAction()
        {
            ButtonToggleComponentsIsEnabled = "False";
            ButtonBackIsEnabled = "False";
            ButtonRebootVisibility = "Hidden";
            LabelStatusComponentsInstalledResult = "Wait...";

            Task.Run(() =>
            {
                if (areComponentsInstalled)
                {
                    componentsRemover.Remove();
                }
                else
                {
                    componentsInstaller.Install();
                }

                bool areComponentsNowInstalled = hypervisorChecker.AreComponentsInstalled();

                if (areComponentsInstalled == areComponentsNowInstalled)
                {
                    LabelStatusComponentsInstalledResult = "Error";
                }
                else
                {
                    if (areComponentsInstalled && !areComponentsNowInstalled)
                    {
                        ButtonToggleComponentsText = "Install";
                        LabelStatusComponentsInstalledResult = "No";
                    }
                    else
                    {
                        ButtonToggleComponentsText = "Remove";
                        LabelStatusComponentsInstalledResult = "Yes";
                    }
                    areComponentsInstalled = areComponentsNowInstalled;
                    ButtonToggleComponentsIsEnabled = "True";
                    ButtonBackIsEnabled = "True";
                    ToggleRebootButtonVisibility();
                }

            });
        }

        public ICommand ButtonRebootCommand { get; set; }

        private void ButtonRebootAction()
        {
            new RebootService().Reboot();
        }
    }
}
