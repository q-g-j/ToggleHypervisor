using System;
using System.Management;
using System.Diagnostics;
using System.IO;
using Logging;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ToggleHypervisor.Services
{
    public class HypervisorChecker : ServiceBase
    {
        public HypervisorChecker()
        {
            fileLogger = App.Current.Services.GetService<FileLogger>();
            LogEvent += fileLogger.LogWriteLine;
        }

        private readonly FileLogger fileLogger;

        public bool IsEnabledOverall()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
                {
                    using (ManagementObjectCollection collection = searcher.Get())
                    {
                        foreach (var obj in collection)
                        {
                            if ((bool)obj["HypervisorPresent"])
                            {
                                return true;
                            }
                        }
                    }
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

            return false;
        }

        public bool AreComponentsInstalled()
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    FileName = "powershell",
                    Arguments = "Get-WindowsOptionalFeature -Online | Where FeatureName -Like \"*Hyper*\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };

            try
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                using (StringReader reader = new StringReader(output))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("Microsoft-Hyper-V-All"))
                        {
                            line = reader.ReadLine();
                            if (!line.Contains("Enabled"))
                            {
                                return false;
                            }
                        }
                        if (line.Contains("Microsoft-Hyper-V"))
                        {
                            line = reader.ReadLine();
                            if (!line.Contains("Enabled"))
                            {
                                return false;
                            }
                        }
                    }
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

        public bool IsHypervisorlaunchtypeFlagSet()
        {
            Process process;
            string output;
            
            process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                                       Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess
                                           ? @"Sysnative\cmd.exe"
                                           : @"System32\cmd.exe"),
                    Arguments = "/c bcdedit /enum",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            try
            {
                process.Start();
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                using (StringReader reader = new StringReader(output))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("hypervisorlaunchtype") && (line.Contains("Auto") || line.Contains("auto")))
                        {
                            return true;
                        }
                    }
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

            return false;
        }
    }
}
