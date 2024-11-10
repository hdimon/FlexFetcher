using System.Text.Json.Serialization;
using System.Text.Json;

namespace FlexFetcher.Serialization.SystemTextJson.Converters;

public class GenericConverter : JsonConverter<object>
{
    private const string DateTimeOffsetPattern = @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d+)?(?:Z|[+-]\d{2}:\d{2})?$";

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(object);
    }

    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return ReadString(ref reader);
            case JsonTokenType.Number when reader.TryGetInt64(out long l):
                return l;
            case JsonTokenType.Number:
                return reader.GetDecimal();
            case JsonTokenType.True:
                return true;
            case JsonTokenType.False:
                return false;
            case JsonTokenType.StartArray:
                return ReadArray(ref reader, typeToConvert, options);
            case JsonTokenType.StartObject:
                return ReadObject(ref reader, typeToConvert, options);
        }

        // Fallback to default handling for other types
        return JsonSerializer.Deserialize(ref reader, typeToConvert, options);
    }

    public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        var type = value.GetType();

        if (type == typeof(object))
        {
            writer.WriteStartObject();
            writer.WriteEndObject();
            return;
        }

        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }

    private object ReadString(ref Utf8JsonReader reader)
    {
        string stringValue = reader.GetString()!;

        if (System.Text.RegularExpressions.Regex.IsMatch(stringValue, DateTimeOffsetPattern) &&
            DateTimeOffset.TryParse(stringValue, out var dto))
            return dto;

        return stringValue;
    }

    private object ReadArray(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var firstElement = doc.RootElement.EnumerateArray().FirstOrDefault();

            if (firstElement.ValueKind != JsonValueKind.Undefined)
            {
                Type elementType = GetArrayElementType(firstElement, doc);

                var array = CopyArray(elementType, doc, options);

                return array;
            }
        }

        return JsonSerializer.Deserialize(ref reader, typeToConvert, options);
    }

    private Type GetArrayElementType(JsonElement firstElement, JsonDocument doc)
    {
        Type elementType = firstElement.ValueKind switch
        {
            JsonValueKind.Number => firstElement.TryGetInt64(out _) ? typeof(int) : typeof(double),
            // If string matches Iso date format then try to parse it as DateTimeOffset
            JsonValueKind.String =>
                System.Text.RegularExpressions.Regex.IsMatch(firstElement.GetString()!, DateTimeOffsetPattern) &&
                DateTimeOffset.TryParse(firstElement.GetString(), out _)
                    ? typeof(DateTimeOffset)
                    : typeof(string),
            JsonValueKind.True => typeof(bool),
            JsonValueKind.False => typeof(bool),
            JsonValueKind.Null => typeof(object),
            _ => typeof(object),
        };

        bool hasNull = doc.RootElement.EnumerateArray().Any(e => e.ValueKind == JsonValueKind.Null);

        if (hasNull && elementType.IsValueType && Nullable.GetUnderlyingType(elementType) == null)
            elementType = typeof(Nullable<>).MakeGenericType(elementType);

        return elementType;
    }

    private Array CopyArray(Type elementType, JsonDocument doc, JsonSerializerOptions options)
    {
        var array = Array.CreateInstance(elementType, doc.RootElement.GetArrayLength());

        int index = 0;
        foreach (var item in doc.RootElement.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.Null)
            {
                array.SetValue(null, index++);
            }
            else
            {
                var obj = JsonSerializer.Deserialize(item.GetRawText(), elementType, options);
                array.SetValue(obj, index++);
            }
        }

        return array;
    }

    private object ReadObject(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        if (doc.RootElement.EnumerateObject().Any())
            return JsonSerializer.Deserialize(ref reader, typeToConvert, options);
        return new object();
    }
}