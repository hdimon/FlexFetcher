// ReSharper disable RedundantUsingDirective
using Newtonsoft.Json;
using System.Globalization;

namespace FlexFetcher.Serialization.NewtonsoftJson.Converters;

#if NET6_0_OR_GREATER
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
#endif