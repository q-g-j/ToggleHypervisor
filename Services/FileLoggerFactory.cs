using QGJSoft.Logging;
using Microsoft.Extensions.DependencyInjection;
using ToggleHypervisor.Models;

namespace ToggleHypervisor.Services
{
    internal class FileLoggerFactory
    {
        public static FileLogger GetFileLogger()
        {
            if (fileLoggerSingleton == null)
            {
                var settingsData = App.Current.Services.GetService<SettingsData>();
                fileLoggerSingleton = new FileLogger("ToggleHypervisor.log", settingsData.MaxLogFileSizeInKB);
            }

            return fileLoggerSingleton;
        }

        private static FileLogger fileLoggerSingleton = null;
    }
}
