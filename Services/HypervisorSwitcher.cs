using Logging;
using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ToggleHypervisor.Services
{
    public class HypervisorSwitcher : ServiceBase
    {
        public HypervisorSwitcher()
        {
            fileLogger = App.Current.Services.GetService<FileLogger>();
            LogEvent += fileLogger.LogWriteLine;
        }

        private readonly FileLogger fileLogger;

        public void Switch(bool mode)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                    Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess ? @"Sysnative\cmd.exe" : @"System32\cmd.exe"),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            process.StartInfo.Arguments = mode ? "/c bcdedit /set hypervisorlaunchtype auto" : "/c bcdedit /set hypervisorlaunchtype off";

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
