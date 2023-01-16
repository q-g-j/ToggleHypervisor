using CommunityToolkit.Mvvm.Input;
using Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ToggleHypervisor.Models;
using ToggleHypervisor.Services;
using ToggleHypervisor.Views;

namespace ToggleHypervisor.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            //System.Diagnostics.Debug.WriteLine("MainPageViewModel constructor");

            OpenDetailsCommand = new RelayCommand(OpenDetailsAction);
            ToggleCommand = new RelayCommand(ToggleAction);
            checkTaskDelegates = new CheckTaskDelegate[2]
            {
                GetStatusOverallTask,
                GetIsHyperviserlaunchtypeFlagSetTask
            };

            if (settingsData == null)
            {
                settingsData = App.Current.Services.GetService<SettingsData>();
            }
            if (mainWindowViewModel == null)
            {
                mainWindowViewModel = App.Current.Services.GetService<MainWindowViewModel>();
            }

            fileLogger = App.Current.Services.GetService<FileLogger>();
            LogEvent += fileLogger.LogWriteLine;
        }

        private bool isHypervisorlaunchtypeSet;

        private delegate Task CheckTaskDelegate();
        private readonly CheckTaskDelegate[] checkTaskDelegates;
        private readonly Task[] checkTasks = new Task[2];

        private readonly FileLogger fileLogger;
        private SettingsData settingsData;
        private MainWindowViewModel mainWindowViewModel;

        private string labelStatusOverall = "Hyper-V enabled and usable:";
        public string LabelStatusOverall
        {
            get => labelStatusOverall;
            set
            {
                labelStatusOverall = value;
                OnPropertyChanged();
            }
        }

        private string labelStatusOverallResult = "";
        public string LabelStatusOverallResult
        {
            get => labelStatusOverallResult;
            set
            {
                labelStatusOverallResult = value;
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

        private string labelIsHypervisorlaunchtypeSet = "The Hypervisor boot flag is set.";
        public string LabelIsHypervisorlaunchtypeSet
        {
            get => labelIsHypervisorlaunchtypeSet;
            set
            {
                labelIsHypervisorlaunchtypeSet = value;
                OnPropertyChanged();
            }
        }

        private string buttonToggleText = "Toggle";
        public string ButtonToggleText
        {
            get => buttonToggleText;
            set
            {
                buttonToggleText = value;
                OnPropertyChanged();
            }
        }

        private Task GetStatusOverallTask()
        {
            return new Task(() =>
            {
                LabelStatusOverallResult = String.Empty;

                if (hypervisorChecker.IsEnabledOverall())
                {
                    LabelStatusOverallResult = "Yes";
                    LabelStatusOverallResultVisibility = "Visible";
                }
                else
                {
                    LabelStatusOverallResult = "No";
                    LabelStatusOverallResultVisibility = "Visible";
                }
            });
        }

        private Task GetIsHyperviserlaunchtypeFlagSetTask()
        {
            return new Task(() =>
            {
                isHypervisorlaunchtypeSet = hypervisorChecker.IsHypervisorlaunchtypeFlagSet();
                UpdateLabelIsHypervisorlaunchtypeSet();
            });
        }

        public void RunChecks()
        {
            var sd = settingsFileReader.Load();
            settingsData.MaxLogFileSizeInKB = sd.MaxLogFileSizeInKB;
            settingsData.RebootAfterToggle = sd.RebootAfterToggle;

            for (int i = 0; i < checkTasks.Length; i++)
            {
                checkTasks[i] = checkTaskDelegates[i]();
                checkTasks[i].Start();
            }

            if (settingsData.RebootAfterToggle)
            {
                ButtonToggleText = "Toggle and reboot";
            }
            else
            {
                ButtonToggleText = "Toggle";
            }
        }

        private void UpdateLabelIsHypervisorlaunchtypeSet()
        {
            if (isHypervisorlaunchtypeSet)
            {
                LabelIsHypervisorlaunchtypeSet = "The Hypervisor boot flag is set.";
            }
            else
            {
                LabelIsHypervisorlaunchtypeSet = "The Hypervisor boot flag is NOT set.";
            }
        }

        public ICommand OpenDetailsCommand { get; set; }

        private void OpenDetailsAction()
        {
            mainWindowViewModel.CurrentPage = new DetailsPage();
        }

        public ICommand ToggleCommand { get; set; }

        private void ToggleAction()
        {
            isHypervisorlaunchtypeSet = !isHypervisorlaunchtypeSet;
            hypervisorSwitcher.Switch(isHypervisorlaunchtypeSet);
            UpdateLabelIsHypervisorlaunchtypeSet();

            if (settingsData.RebootAfterToggle)
            {
                new RebootService().Reboot();
            }
        }
    }
}
