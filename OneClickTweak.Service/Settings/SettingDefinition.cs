namespace OneClickTweak.Service.Settings;

public class SettingDefinition
{
    /// <summary>
    /// Globally registered identifier
    /// Null if not assigned, for only locally applied settings
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Platforms the setting applies to
    /// </summary>
    public ICollection<SettingPlatform> Platforms { get; set; } = Array.Empty<SettingPlatform>();

    /// <summary>
    /// Versions for different platform
    /// </summary>
    public ICollection<SettingVersion> Versions { get; set; } = Array.Empty<SettingVersion>();
}