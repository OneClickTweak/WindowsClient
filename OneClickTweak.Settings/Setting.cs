using System.Text.Json.Serialization;
using OneClickTweak.Settings.Serialization;

namespace OneClickTweak.Settings;

public record Setting : IHasSettings
{
    public Setting Copy()
    {
        return this with
        {
            Settings = null,
            Path = Path?.ToList(),
            Values = Values?.ToList()
        };
    }
    
    public void Merge(Setting setting)
    {
        Platform ??= setting.Platform;
        Name ??= setting.Name;
        Handler ??= setting.Handler;
        Scope ??= setting.Scope;
        Path ??= setting.Path?.ToList();
        Key ??= setting.Key;
        Type ??= setting.Type;
        MinVersion ??= setting.MinVersion;
        MaxVersion ??= setting.MaxVersion;
        Values ??= setting.Values?.ToList();
    }

    /// <summary>
    /// Platforms the setting version applies to
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumArrayConverter))]
    public SupportedPlatform? Platform { get; set; }

    /// <summary>
    /// Translation key relative to definition key
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Handler
    /// </summary>
    public string? Handler { get; set; }

    /// <summary>
    /// Scope the setting applies to
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SettingScope? Scope { get; set; }

    /// <summary>
    /// Path within the handler context
    /// </summary>
    public ICollection<string>? Path { get; set; }

    /// <summary>
    /// Value key
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Value data type
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SettingType? Type { get; set; }

    /// <summary>
    /// Minimum platform version the setting is available for, inclusive (greater or equal than)
    /// </summary>
    public string? MinVersion { get; set; }

    /// <summary>
    /// Maximum platform version the setting is available for, exclusive (less than)
    /// </summary>
    public string? MaxVersion { get; set; }

    /// <summary>
    /// More specific values
    /// </summary>
    public ICollection<Setting>? Settings { get; set; }

    /// <summary>
    /// Predefined possible values, if applicable
    /// </summary>
    public ICollection<SettingValue>? Values { get; set; }
}