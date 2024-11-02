using AdvancedSharpAdbClient.Models;
using OneClickTweak.Settings.Runtime;

namespace OneClickTweak.AndroidHandlers;

public class AdbInstance : SettingsInstance
{
    public DeviceData Device { get; set; }
}
