using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using ToggleHypervisor.Models;
using Microsoft.Extensions.DependencyInjection;
using QGJSoft.Logging;
using System.Reflection;

namespace ToggleHypervisor.Services
{
    public class SettingsFileCreator : ServiceBase
    {
        public SettingsFileCreator()
        {
            fileLocations = App.Current.Services.GetService<FileLocations>();

            fileLogger = FileLoggerFactory.GetFileLogger();
            LogEvent += fileLogger.LogWriteLine;
        }

        private readonly FileLogger fileLogger;
        private readonly FileLocations fileLocations;

        public void Create()
        {
            string appDataRoaming = fileLocations.AppDataRoaming;
            string settingsFolder = fileLocations.SettingsFolderName;
            string settingsFile = fileLocations.SettingsFileName;
            try
            {
                Directory.CreateDirectory(Path.Combine(appDataRoaming, settingsFolder));
                SettingsData settingsData = new SettingsData();
                File.WriteAllText(fileLocations.SettingsFileName, JsonConvert.SerializeObject(settingsData, Formatting.Indented));
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

        public static SettingsFileCreator GetInstance()
        {
            return new SettingsFileCreator();
        }
    }
}
