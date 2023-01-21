using Microsoft.Extensions.DependencyInjection;
using QGJSoft.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using ToggleHypervisor.Models;

namespace ToggleHypervisor.Services
{
    internal class WindowsVersionChecker : ServiceBase
    {
        private readonly FileLogger fileLogger;

        public WindowsVersionChecker()
        {
            fileLogger = FileLoggerFactory.GetFileLogger();
            LogEvent += fileLogger.LogWriteLine;
        }

        public string OsFullVersionString { get; set; } = "";

        internal bool HasOSChanged()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject managementObject in searcher.Get().Cast<ManagementObject>())
            {
                OsFullVersionString = managementObject["Caption"].ToString() + " " + managementObject["Version"].ToString();
            }

            var settingsData = App.Current.Services.GetService<SettingsData>();
            if (settingsData != null )
            {
                if (settingsData.LastKnownOSVersion != OsFullVersionString)
                {
                    return true;
                }
            }

            return false;
        }

        internal bool IsHyperVCapable()
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    FileName = "powershell",
                    Arguments = "if ((Get-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V-Hypervisor) -eq $null) { Write-Output \"false\" }",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
            try
            {
                process.Start();

                process.WaitForExit();
                var result = process.StandardOutput;

                if (result.ReadLine() == "false")
                {
                    return false;
                }
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

            return true;
        }
    }
}
