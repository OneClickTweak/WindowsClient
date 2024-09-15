namespace OneClickTweak.Settings;

public class SettingDefinition : IHasSettings
{
    /// <summary>
    /// Globally registered identifier
    /// Null if not assigned, for only locally applied settings
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Translation key
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Versions for different platform
    /// </summary>
    public ICollection<Setting>? Settings { get; set; }
}