using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Users;

namespace OneClickTweak.Settings.Runtime;

public class SettingsInstance
{
    public required SettingScope Scope { get; init; }

    public UserInstance? User { get; init; }

    public string? Path { get; init; }
    
    public string? Version { get; init; }
}