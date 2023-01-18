using QGJSoft.Logging;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToggleHypervisor.Services;

namespace ToggleHypervisor.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged, IFileLogger
    {
        protected readonly HypervisorChecker hypervisorChecker = new HypervisorChecker();
        protected readonly HypervisorSwitcher hypervisorSwitcher = new HypervisorSwitcher();
        protected readonly ComponentsInstaller componentsInstaller = new ComponentsInstaller();
        protected readonly SettingsFileReader settingsFileReader = new SettingsFileReader();
        protected readonly SettingsFileWriter settingsFileWriter = new SettingsFileWriter();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event Action<object, LoggerEventArgs> LogEvent;

        void IFileLogger.OnLogEvent(object o, LoggerEventArgs eventArgs)
        {
            RaiseLogEvent(o, eventArgs);
        }

        protected virtual void RaiseLogEvent(object o, LoggerEventArgs eventArgs)
        {
            LogEvent?.Invoke(o, eventArgs);
        }
    }
}
