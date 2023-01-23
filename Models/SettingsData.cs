using Newtonsoft.Json;

namespace ToggleHypervisor.Models
{
    public class SettingsData
    {
        public SettingsData()
        {
            LastKnownOSVersion = "";
            IsOSHyperVCapable = false;
            RebootAfterToggle = false;
            MaxLogFileSizeInKB = 256;
        }

        public SettingsData(string lastKnownOSVersion, bool isOSHyperVCapable, bool rebootAfterToggle, int maxLogFileSizeInKB)
        {
            LastKnownOSVersion = lastKnownOSVersion;
            IsOSHyperVCapable = isOSHyperVCapable;
            RebootAfterToggle = rebootAfterToggle;
            MaxLogFileSizeInKB = maxLogFileSizeInKB;
        }

        public string LastKnownOSVersion { get; set; }
        public bool IsOSHyperVCapable { get; set; }
        public bool RebootAfterToggle { get; set; }
        public int MaxLogFileSizeInKB { get; set; }
    }
}
