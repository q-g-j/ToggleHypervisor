using Logging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToggleHypervisor.Models;

namespace ToggleHypervisor.Services
{
    public class SettingsFileReader : ServiceBase
    {
        public SettingsFileReader()
        {
            fileLocations = App.Current.Services.GetService<FileLocations>();

            fileLogger = App.Current.Services.GetService<FileLogger>();
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

            return settingsData;
        }
    }
}
