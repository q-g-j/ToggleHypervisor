using Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToggleHypervisor.Models;

namespace ToggleHypervisor.Services
{
    internal class RebootService : ServiceBase
    {
        public RebootService()
        {
            fileLogger = App.Current.Services.GetService<FileLogger>();
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
                LoggerEventArgs loggerEventArgs = GetLoggerEventArgs(
                    String.Empty,
                    GetType().Name,
                    MethodBase.GetCurrentMethod().Name,
                    ex
                    );
                RaiseLogEvent(this, loggerEventArgs);
            }
        }
    }
}
