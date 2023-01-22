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
