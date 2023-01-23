using QGJSoft.Logging;
using System;

namespace ToggleHypervisor.Services
{
    public class ServiceBase : IFileLogger
    {
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
