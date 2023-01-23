using QGJSoft.Logging;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToggleHypervisor.Services;

namespace ToggleHypervisor.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged, IFileLogger
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event Action<string, int, LoggerEventArgs> LogEvent;

        void IFileLogger.OnLogEvent(string logFileFullPath, int maxLogFileSize, LoggerEventArgs eventArgs)
        {
            RaiseLogEvent(logFileFullPath, maxLogFileSize, eventArgs);
        }

        protected virtual void RaiseLogEvent(string logFileFullPath, int maxLogFileSize, LoggerEventArgs eventArgs)
        {
            LogEvent?.Invoke(logFileFullPath, maxLogFileSize, eventArgs);
        }
    }
}
