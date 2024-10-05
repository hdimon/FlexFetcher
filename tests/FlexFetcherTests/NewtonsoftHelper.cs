using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace FlexFetcherTests;

public class NewtonsoftHelper
{
    public static JsonSerializerSettings GetSerializerSettings()
    {
        var settings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
            DateParseHandling = DateParseHandling.DateTimeOffset
        };
        settings.Converters.Add(new GenericArrayConverter());
        settings.Converters.Add(new TimeOnlyJsonConverter());
        return settings;
    }

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

    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
    {
        private const string TimeFormat = "HH:mm:ss.FFFFFFF";

        public override TimeOnly ReadJson(JsonReader reader, Type objectType, TimeOnly existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return TimeOnly.ParseExact((string)reader.Value!, TimeFormat, CultureInfo.InvariantCulture);
        }

        public override void WriteJson(JsonWriter writer, TimeOnly value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(TimeFormat, CultureInfo.InvariantCulture));
        }
    }
}