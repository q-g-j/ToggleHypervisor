using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                    var json = streamReader.ReadToEnd();
                    JObject obj = JObject.Parse(json);
                    var properties = typeof(SettingsData).GetProperties();
                    foreach (var property in properties)
                    {
                        if (obj[property.Name] == null)
                        {
                            throw new Exception($"{property.Name} is missing in the json");
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
