// ReSharper disable once RedundantUsingDirective
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using FlexFetcher.Serialization.NewtonsoftJson.Converters;
using Newtonsoft.Json;

namespace FlexFetcher.Serialization.NewtonsoftJson;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
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
#if NET6_0_OR_GREATER
        settings.Converters.Add(new TimeOnlyJsonConverter());
#endif
        return settings;
    }
}