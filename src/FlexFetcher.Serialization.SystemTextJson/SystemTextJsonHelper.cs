using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using FlexFetcher.Serialization.SystemTextJson.Converters;

namespace FlexFetcher.Serialization.SystemTextJson;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class SystemTextJsonHelper
{
    public static JsonSerializerOptions GetSerializerSettings()
    {
        var settings = new JsonSerializerOptions();
        settings.Converters.Add(new GenericConverter());

        return settings;
    }
}