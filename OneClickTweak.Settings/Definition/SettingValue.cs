using System.Text.Json.Serialization;
using OneClickTweak.Settings.Serialization;

namespace OneClickTweak.Settings.Definition;

public record SettingValue
{
    /// <summary>
    /// Translation key relative to definition key
    /// </summary>
    [JsonConverter(typeof(SingleOrArrayConverter<List<string>, string>))]
    public List<string>? Name { get; set; }

    /// <summary>`
    /// Serialized value
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// If true, value is default
    /// </summary>
    public bool IsDefault { get; set; }
}