﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using ToggleHypervisor.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using static System.Diagnostics.Debug;
using Logging;
using System.Net;
using System.Reflection;

namespace ToggleHypervisor.Services
{
    public class SettingsFileCreator : ServiceBase
    {
        public SettingsFileCreator()
        {
            fileLocations = App.Current.Services.GetService<FileLocations>();

            fileLogger = new FileLogger();
            LogEvent += fileLogger.LogWriteLine;
        }

        private readonly FileLogger fileLogger;
        private readonly FileLocations fileLocations;

        public void Create()
        {
            string appDataRoaming = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            string settingsFolder = "ToggleHypervisor";
            string settingsFile = "Settings.json";
            fileLocations.SettingsFolderName = Path.Combine(appDataRoaming, settingsFolder);
            fileLocations.SettingsFileName = Path.Combine(appDataRoaming, settingsFolder, settingsFile);

            if (!File.Exists(fileLocations.SettingsFileName))
            {
                try
                {
                    Directory.CreateDirectory(Path.Combine(appDataRoaming, settingsFolder));

                    SettingsData settingsData = App.Current.Services.GetService<SettingsData>();

                    settingsData.RebootAfterToggle = false;
                    settingsData.MaxLogFileSizeInKB = 4;

                    File.WriteAllText(fileLocations.SettingsFileName, JsonConvert.SerializeObject(settingsData, Formatting.Indented));
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
}
