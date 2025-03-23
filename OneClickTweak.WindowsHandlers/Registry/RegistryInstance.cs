using OneClickTweak.Settings.Runtime;

namespace OneClickTweak.WindowsHandlers.Registry;

public class RegistryInstance : SettingsInstance
{
    public required string RootKey { get; set; }

    public string? Location { get; set; }
    
    public Hive? Hive { get; set; }
}