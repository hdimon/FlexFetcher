using Newtonsoft.Json;

namespace FlexFetcherTests;

public class NewtonsoftHelper
{
    public static JsonSerializerSettings DeserializationSettings = new()
    {
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
        DateParseHandling = DateParseHandling.DateTimeOffset
    };
}