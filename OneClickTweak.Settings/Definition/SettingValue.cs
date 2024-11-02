namespace OneClickTweak.Settings.Definition;

public record SettingValue
{
    /// <summary>
    /// If registered, identifier of the value
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Translation key relative to definition key
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Serialized value
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// If true, value is default
    /// </summary>
    public bool IsDefault { get; set; }
}