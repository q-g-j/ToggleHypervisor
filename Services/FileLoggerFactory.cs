using Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToggleHypervisor.Models;

namespace ToggleHypervisor.Services
{
    internal class FileLoggerFactory
    {
        public static FileLogger GetFileLogger()
        {
            var settingsData = App.Current.Services.GetService<SettingsData>();
            return new FileLogger("ToggleHypervisor.log", settingsData.MaxLogFileSizeInKB);
        }
    }
}
