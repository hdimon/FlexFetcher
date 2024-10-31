using FlexFetcher.Models.Queries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlexFetcher.Serialization.NewtonsoftJson.Converters;

public class FlexFetcherDataSortersConverter : JsonConverter<DataSorters>
{
    private readonly bool _readOnlyShortForm;

    public FlexFetcherDataSortersConverter(bool readOnlyShortForm = false)
    {
        _readOnlyShortForm = readOnlyShortForm;
    }

    public override void WriteJson(JsonWriter writer, DataSorters? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var jo = new JObject();
        jo["S"] = value.Sorters != null ? JArray.FromObject(value.Sorters, serializer) : new JArray();

        jo.WriteTo(writer);
    }

    public override DataSorters? ReadJson(JsonReader reader, Type objectType, DataSorters? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var jo = JObject.Load(reader);
        var dataSorters = new DataSorters();

        if (jo["S"] != null)
            dataSorters.Sorters = jo["S"]!.ToObject<List<DataSorter>>(serializer);
        else if (!_readOnlyShortForm && jo["Sorters"] != null)
            dataSorters.Sorters = jo["Sorters"]!.ToObject<List<DataSorter>>(serializer);

        return dataSorters;
    }
}