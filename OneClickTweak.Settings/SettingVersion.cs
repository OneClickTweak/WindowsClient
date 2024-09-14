namespace OneClickTweak.Settings;

public class SettingVersion
{
    /// <summary>
    /// Platforms the setting version applies to
    /// </summary>
    public SettingPlatform? Platform { get; set; }

    /// <summary>
    /// Handler
    /// </summary>
    public string Handler { get; set; }

    /// <summary>
    /// Path within the handler context
    /// </summary>
    public ICollection<string> Path { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// Value key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Value data type
    /// </summary>
    public SettingType ValueType { get; set; }

    /// <summary>
    /// Predefined possible values, if applicable
    /// </summary>
    public ICollection<SettingValue> Values { get; set; } = Array.Empty<SettingValue>();
}