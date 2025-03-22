using System.Text.Json.Serialization;
using OneClickTweak.Settings.Serialization;

namespace OneClickTweak.Settings.Definition;

public record Setting
{
    /// <summary>
    /// Platforms the setting version applies to
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumArrayConverter))]
    public SupportedPlatform? Platform { get; set; }

    /// <summary>
    /// Translation key relative to definition key, or absolute if contains "."
    /// </summary>
    public required ICollection<string> Name { get; set; }
    
    /// <summary>
    /// Tags used to describe the setting
    /// </summary>
    public HashSet<string>? Tags { get; set; }

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
    /// Nested merged specific values
    /// </summary>
    public ICollection<Setting>? Settings { get; set; }

    /// <summary>
    /// Predefined possible value choices, if applicable
    /// </summary>
    public ICollection<SettingValue>? Values { get; set; }

    /// <summary>
    /// Custom options used by handler
    /// </summary>
    public Dictionary<string, string> Options { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}