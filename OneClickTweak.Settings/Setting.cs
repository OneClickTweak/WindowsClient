namespace OneClickTweak.Settings;

public class Setting
{
    /// <summary>
    /// Platforms the setting version applies to
    /// </summary>
    public SettingPlatform? Platform { get; set; }

    /// <summary>
    /// Translation key relative to definition key
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Handler
    /// </summary>
    public string? Handler { get; set; }

    /// <summary>
    /// Path within the handler context
    /// </summary>
    public ICollection<string> Path { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// Value key
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Value data type
    /// </summary>
    public SettingType? Type { get; set; }

    /// <summary>
    /// Minimum platform version the setting is available for
    /// </summary>
    public string? MinVersion { get; set; }

    /// <summary>
    /// Maximum platform version the setting is available for
    /// </summary>
    public string? MaxVersion { get; set; }

    /// <summary>
    /// More specific values
    /// </summary>
    public ICollection<Setting> Settings { get; set; } = Array.Empty<Setting>();

    /// <summary>
    /// Predefined possible values, if applicable
    /// </summary>
    public ICollection<SettingValue> Values { get; set; } = Array.Empty<SettingValue>();
}