﻿using Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Reflection;

namespace ToggleHypervisor.Services
{
    public class ComponentsInstaller : ServiceBase
    {
        public ComponentsInstaller()
        {
            fileLogger = App.Current.Services.GetService<FileLogger>();
            LogEvent += fileLogger.LogWriteLine;
        }

        private readonly FileLogger fileLogger;

        public void Install()
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    FileName = "powershell",
                    Arguments = "Enable-WindowsOptionalFeature -Online -NoRestart -FeatureName Microsoft-Hyper-V-All,Microsoft-Hyper-V",
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
