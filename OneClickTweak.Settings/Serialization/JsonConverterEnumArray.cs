using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OneClickTweak.Settings.Serialization;

public sealed class JsonStringEnumArrayConverter : JsonConverterFactory
{
    public JsonStringEnumArrayConverter() { }

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converter = Activator.CreateInstance(
            typeof(JsonConverterEnumArray<>).MakeGenericType(typeToConvert),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null) as JsonConverter;

        return converter ?? throw new JsonException("Could not create converter.");
    }
}

internal class JsonConverterEnumArray<T> : JsonConverter<T>
    where T : struct, Enum
{
    public override bool CanConvert(Type type)
    {
        return type.IsEnum;
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        string enumString = default;
        var first = true;
        while (true)
        {
            reader.Read();

            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var enumValue = reader.GetString();
            if (first)
            {
                first = false;
            }
            else
            {
                enumString += ",";
            }

            enumString += enumValue ?? string.Empty;
        }

        if (!Enum.TryParse(enumString, out T value))
        {
            throw new JsonException();
        }

        return value;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var enumString = value.ToString().Split(',');

        writer.WriteStartArray();

        foreach (var enumValue in enumString)
        {
            writer.WriteStringValue(enumValue.TrimStart());
        }

        writer.WriteEndArray();
    }
}