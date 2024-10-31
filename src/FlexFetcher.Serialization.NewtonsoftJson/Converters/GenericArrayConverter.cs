using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FlexFetcher.Serialization.NewtonsoftJson.Converters;

public class GenericArrayConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(object);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);

        if (token.Type == JTokenType.Array)
        {
            var firstElement = token.First;

            if (firstElement != null)
            {
                var elementType = firstElement.Type switch
                {
                    JTokenType.Integer => typeof(int),
                    JTokenType.Float => typeof(double),
                    JTokenType.String => typeof(string),
                    JTokenType.Boolean => typeof(bool),
                    JTokenType.Date => typeof(DateTimeOffset),
                    JTokenType.Guid => typeof(Guid),
                    JTokenType.TimeSpan => typeof(TimeSpan),
                    _ => typeof(object)
                };

                bool hasNull = token.Any(t => t.Type == JTokenType.Null);
                if (hasNull && elementType.IsValueType)
                {
                    elementType = typeof(Nullable<>).MakeGenericType(elementType);
                }

                var arrayType = elementType.MakeArrayType();
                return token.ToObject(arrayType);
            }
        }

        return token.ToObject<object>();
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        JToken.FromObject(value!).WriteTo(writer);
    }
}