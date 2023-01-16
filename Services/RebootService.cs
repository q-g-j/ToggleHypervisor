using Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Reflection;

namespace ToggleHypervisor.Services
{
    internal class RebootService : ServiceBase
    {
        public RebootService()
        {
            fileLogger = FileLoggerFactory.GetFileLogger();
            LogEvent += fileLogger.LogWriteLine;
        }

        private readonly FileLogger fileLogger;

        internal void Reboot()
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    FileName = "cmd",
                    Arguments = "/c shutdown /r /t 0",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };

            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                var loggerEventArgs = new LoggerEventArgs(
                    String.Empty,
                    GetType().Name,
                    MethodBase.GetCurrentMethod().Name,
                    ex);
                RaiseLogEvent(this, loggerEventArgs);
            }
        }
    }
}
