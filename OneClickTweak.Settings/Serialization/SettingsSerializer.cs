using System.Text.Json;
using System.Text.Json.Serialization;

namespace OneClickTweak.Settings.Serialization;

public static class SettingsSerializer
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    };
}