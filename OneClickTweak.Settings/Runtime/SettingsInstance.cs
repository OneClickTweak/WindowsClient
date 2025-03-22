using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Users;

namespace OneClickTweak.Settings.Runtime;

public class SettingsInstance
{
    public required string Handler { get; init; }

    public required SettingScope Scope { get; init; }

    public required Dictionary<string, object> Options { get; set; }

    public UserInstance? User { get; init; }
    
    public string? Version { get; init; }
}