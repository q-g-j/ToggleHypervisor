namespace ToggleHypervisor.Models
{
    public class SettingsData
    {
        public SettingsData(string lastKnownOSVersion = "", bool rebootAfterToggle = false, int maxLogFileSizeInKB = 256)
        {
            LastKnownOSVersion = lastKnownOSVersion;
            RebootAfterToggle = rebootAfterToggle;
            MaxLogFileSizeInKB = maxLogFileSizeInKB;
        }

        public string LastKnownOSVersion { get; set; }
        public bool RebootAfterToggle { get; set; }
        public int MaxLogFileSizeInKB { get; set; }
    }
}
