using System;
using System.IO;
using Newtonsoft.Json;
using ToggleHypervisor.Models;
using Logging;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ToggleHypervisor.Services
{
    public class SettingsFileWriter : ServiceBase
    {
        public SettingsFileWriter()
        {
            fileLocations = App.Current.Services.GetService<FileLocations>();

            fileLogger = App.Current.Services.GetService<FileLogger>();
            LogEvent += fileLogger.LogWriteLine;
        }

        private readonly FileLogger fileLogger;
        private readonly FileLocations fileLocations;

        public void Write()
        {
            SettingsData settingsData = App.Current.Services.GetService<SettingsData>();

            string jsonString = JsonConvert.SerializeObject(settingsData, Formatting.Indented);

            try
            {
                using (StreamWriter sw = new StreamWriter(fileLocations.SettingsFileName))
                {
                    sw.Write(jsonString);
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
        }
    }
}
