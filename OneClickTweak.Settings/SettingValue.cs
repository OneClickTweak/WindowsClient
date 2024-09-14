namespace OneClickTweak.Settings;

public class SettingValue
{
    /// <summary>
    /// Translation key relative to definition key
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Setting value if inside a Setting
    /// </summary>
    public string? Value { get; set; }
    
    /// <summary>
    /// If true, value is default
    /// </summary>
    public bool IsDefault { get; set; }
}