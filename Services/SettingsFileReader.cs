using QGJSoft.Logging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using ToggleHypervisor.Models;

namespace ToggleHypervisor.Services
{
    public class SettingsFileReader
    {
        public static SettingsData Load()
        {
            var fileLocations = App.Current.Services.GetService<FileLocations>();

            SettingsData settingsData = null;

            try
            {
                using (StreamReader streamReader = new StreamReader(fileLocations.SettingsFileName))
                {
                    settingsData = JsonConvert.DeserializeObject<SettingsData>(streamReader.ReadToEnd());
                }
            }
            catch
            {
                return new SettingsData();
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
