using QGJSoft.Logging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using ToggleHypervisor.Models;

namespace ToggleHypervisor.Services
{
    public class SettingsFileReader : ServiceBase
    {
        public SettingsFileReader()
        {
            fileLocations = App.Current.Services.GetService<FileLocations>();
            fileLogger = FileLoggerFactory.GetFileLogger();
            LogEvent += fileLogger.LogWriteLine;
        }

        private readonly FileLogger fileLogger;
        private readonly FileLocations fileLocations;

        public bool FileExists()
        {
            return File.Exists(fileLocations.SettingsFileName);
        }

        public SettingsData Load()
        {
            SettingsData settingsData = null;

            try
            {
                using (StreamReader streamReader = new StreamReader(fileLocations.SettingsFileName))
                {
                    settingsData = JsonConvert.DeserializeObject<SettingsData>(streamReader.ReadToEnd());
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

            if (settingsData != null)
            {
                return settingsData;
            }
            else
            {
                return new SettingsData();
            }
        }
    }
}
