using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using OneClickTweak.Settings.Definition;

namespace OneClickTweak.Settings.Serialization;

public static class SettingsSerializer
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers = { SkipEmptyDictionary }
        }
    };
    
    static void SkipEmptyDictionary(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Type != typeof(Setting))
        {
            return;
        }

        foreach (var propertyInfo in typeInfo.Properties)
        {
            if (typeof(IDictionary<string, string>).IsAssignableFrom(propertyInfo.PropertyType))
            {
                propertyInfo.ShouldSerialize = static (obj, value) => value != null && ((IDictionary<string,string>)value).Count > 0;
            }
        }
    }
}