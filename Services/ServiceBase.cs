using Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToggleHypervisor.Models;

namespace ToggleHypervisor.Services
{
    public class ServiceBase : IFileLogger
    {
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
