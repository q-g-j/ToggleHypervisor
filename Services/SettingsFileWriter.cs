using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ToggleHypervisor.Models;
using System.Windows.Shapes;
using Logging;
using System.Reflection;
using System.Threading;
using static System.Diagnostics.Debug;
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
