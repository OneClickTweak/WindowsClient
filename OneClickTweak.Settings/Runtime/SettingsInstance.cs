using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Users;

namespace OneClickTweak.Settings.Runtime;

public record SettingsInstance
{
    public required SettingScope Scope { get; init; }

    public UserInstance? User { get; init; }

    public string? Path { get; init; }
}