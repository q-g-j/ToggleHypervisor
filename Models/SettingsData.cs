using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToggleHypervisor.Models
{
    public class SettingsData
    {
        public bool RebootAfterToggle { get; set; }
        public int MaxLogFileSizeInKB { get; set; }
    }
}
