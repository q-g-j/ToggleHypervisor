using QGJSoft.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Reflection;
using ToggleHypervisor.Models;

namespace ToggleHypervisor.Services
{
    public class ComponentsRemover : ServiceBase
    {
        public ComponentsRemover()
        {
            settingsData = App.Current.Services.GetService<SettingsData>();
            LogEvent += FileLogger.LogWriteLine;
        }

        private readonly SettingsData settingsData;

        public void Remove()
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    FileName = "powershell",
                    Arguments = "Disable-WindowsOptionalFeature -Online -NoRestart -FeatureName Microsoft-Hyper-V-All,Microsoft-Hyper-V",
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
                RaiseLogEvent("ToggleHypervisor.log", settingsData.MaxLogFileSizeInKB, loggerEventArgs);
            }
        }
    }
}
