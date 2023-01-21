using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToggleHypervisor.Models;

namespace ToggleHypervisor.Services
{
    internal class SettingsFileValidator
    {
        internal static bool FileExists()
        {
            var fileLocations = App.Current.Services.GetService<FileLocations>();
            return File.Exists(fileLocations.SettingsFileName);
        }

        internal static bool IsValid()
        {
            var fileLocations = App.Current.Services.GetService<FileLocations>();

            try
            {
                using (StreamReader streamReader = new StreamReader(fileLocations.SettingsFileName))
                {
                    var jsonSerializerSettings = new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Error
                    };
                    JsonConvert.DeserializeObject<SettingsData>(streamReader.ReadToEnd(), jsonSerializerSettings);
                }
            }
            catch (JsonSerializationException)
            {
                return false;
            }

            return true;
        }
    }
}
