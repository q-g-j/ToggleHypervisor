namespace ToggleHypervisor.Models
{
    public class SettingsData
    {
        public SettingsData(bool rebootAfterToggle = false, int maxLogFileSizeInKB = 256)
        {
            RebootAfterToggle = rebootAfterToggle;
            MaxLogFileSizeInKB = maxLogFileSizeInKB;
        }

        public bool RebootAfterToggle { get; set; }
        public int MaxLogFileSizeInKB { get; set; }
    }
}
