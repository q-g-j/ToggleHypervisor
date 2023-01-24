using System;
using System.Management;
using System.Diagnostics;
using System.IO;
using QGJSoft.Logging;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ToggleHypervisor.Models;
using System.Linq;

namespace ToggleHypervisor.Services
{
    public class HypervisorChecker : ServiceBase
    {
        public HypervisorChecker()
        {
            settingsData = App.Current.Services.GetService<SettingsData>();
            LogEvent += FileLogger.LogWriteLine;
        }

        private readonly SettingsData settingsData;

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
                RaiseLogEvent("ToggleHypervisor.log", settingsData.MaxLogFileSizeInKB, loggerEventArgs);
            }

            return false;
        }

        public bool AreComponentsInstalled()
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OptionalFeature where Name LIKE '%Hyper%'"))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                foreach (ManagementObject obj in collection.Cast<ManagementObject>())
                {
                    if (
                        obj["Name"].ToString() == "Microsoft-Hyper-V-All" &&
                        int.Parse(obj["InstallState"].ToString()) != 1)
                    {
                        return false;
                    }
                    else if (
                        obj["Name"].ToString() == "Microsoft-Hyper-V" &&
                        int.Parse(obj["InstallState"].ToString()) != 1)
                    {
                        return false;
                    }
                }
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
                RaiseLogEvent("ToggleHypervisor.log", settingsData.MaxLogFileSizeInKB, loggerEventArgs);
            }

            return false;
        }
    }
}
