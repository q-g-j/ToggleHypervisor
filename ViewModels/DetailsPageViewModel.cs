﻿using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ToggleHypervisor.Services;
using ToggleHypervisor.Views;
using Logging;
using ToggleHypervisor.Models;
using System.Collections.Generic;

namespace ToggleHypervisor.ViewModels
{
    public class DetailsPageViewModel : ViewModelBase
    {
        public DetailsPageViewModel()
        {
            ButtonBackCommand = new RelayCommand(() => ButtonBackAction());
            ButtonFixFlagCommand = new RelayCommand(() => ButtonFixFlagAction());
            ButtonFixComponentsCommand = new RelayCommand(() => ButtonFixComponentsAction());
            ButtonRebootCommand = new RelayCommand(() => ButtonRebootAction());
            checkTaskDelegates = new CheckTaskDelegate[2]
            {
                GetIsHypervisorlaunchtypeFlagSetTask,
                GetAreComponentsInstalledTask
            };

            if (settingsData == null)
            {
                settingsData = App.Current.Services.GetService<SettingsData>();
            }
            if (mainWindowViewModel == null)
            {
                mainWindowViewModel = App.Current.Services.GetService<MainWindowViewModel>();
            }

            fileLogger = new FileLogger();
            LogEvent += fileLogger.LogWriteLine;
        }

        private bool isFlagCheckFinished = false;
        private bool isComponentsCheckFinished = false;

        private delegate Task CheckTaskDelegate();
        private readonly CheckTaskDelegate[] checkTaskDelegates;
        private readonly Task[] checkTasks = new Task[2];

        private readonly FileLogger fileLogger;
        private SettingsData settingsData;
        private MainWindowViewModel mainWindowViewModel;

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
                settingsFileWriter.Write();
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

        private string buttonFixFlagIsEnabled = "False";
        public string ButtonFixFlagIsEnabled
        {
            get => buttonFixFlagIsEnabled;
            set
            {
                buttonFixFlagIsEnabled = value;
                OnPropertyChanged();
            }
        }

        private string buttonFixComponentsIsEnabled = "False";
        public string ButtonFixComponentsIsEnabled
        {
            get => buttonFixComponentsIsEnabled;
            set
            {
                buttonFixComponentsIsEnabled = value;
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
                if (hypervisorChecker.IsHypervisorlaunchtypeFlagSet())
                {
                    LabelStatusHypervisorlaunchtypeResult = "Yes";
                }
                else
                {
                    LabelStatusHypervisorlaunchtypeResult = "No";
                    ButtonFixFlagIsEnabled = "True";
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
                if (hypervisorChecker.AreComponentsInstalled())
                {
                    LabelStatusComponentsInstalledResult = "Yes";
                }
                else
                {
                    LabelStatusComponentsInstalledResult = "No";
                    ButtonFixComponentsIsEnabled = "True";
                }
                isComponentsCheckFinished = true;

                if (isFlagCheckFinished)
                {
                    LabelStatusOverallVisibility = "Visible";
                    LabelStatusOverallResultVisibility = "Visible";
                }
            });
        }

        public void RunChecks()
        {
            LabelStatusComponentsInstalledResult = String.Empty;
            LabelStatusHypervisorlaunchtypeResult = String.Empty;

            var sd = settingsFileReader.Load();
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
        }

        public ICommand ButtonBackCommand { get; set; }

        private void ButtonBackAction()
        {
            mainWindowViewModel.CurrentPage = new MainPage();
        }

        public ICommand ButtonFixFlagCommand { get; set; }

        private void ButtonFixFlagAction()
        {
            hypervisorSwitcher.Switch(true);
            LabelStatusHypervisorlaunchtypeResult = "Yes";
            ButtonFixFlagIsEnabled = "False";
            ButtonRebootVisibility = "Visible";
        }

        public ICommand ButtonFixComponentsCommand { get; set; }

        private void ButtonFixComponentsAction()
        {
            ButtonFixComponentsIsEnabled = "False";
            LabelStatusComponentsInstalledResult = "Wait...";

            Task.Run(() =>
            {
                componentsInstaller.Install();

                if (hypervisorChecker.AreComponentsInstalled())
                {
                    LabelStatusComponentsInstalledResult = "Reboot";
                    ButtonRebootVisibility = "Visible";
                }
                else
                {
                    LabelStatusComponentsInstalledResult = "Error";
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
