using Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToggleHypervisor.Models;
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

        LoggerEventArgs IFileLogger.GetEventArgs(string message, string className, string methodName, Exception e)
        {
            return GetLoggerEventArgs(message, className, methodName, e);
        }

        protected virtual void RaiseLogEvent(object o, LoggerEventArgs eventArgs)
        {
            LogEvent?.Invoke(o, eventArgs);
        }

        protected virtual LoggerEventArgs GetLoggerEventArgs(string message, string className, string methodName, Exception e)
        {
            return new LoggerEventArgs(message, className, methodName, e);
        }
    }
}
