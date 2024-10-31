using FlexFetcher.Models.Queries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlexFetcher.Serialization.NewtonsoftJson.Converters;

public class FlexFetcherDataQueryConverter : JsonConverter<DataQuery>
{
    private readonly bool _readOnlyShortForm;

    public FlexFetcherDataQueryConverter(bool readOnlyShortForm = false)
    {
        _readOnlyShortForm = readOnlyShortForm;
    }

    public override void WriteJson(JsonWriter writer, DataQuery? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var jo = new JObject();

        jo["F"] = value.Filter != null ? JToken.FromObject(value.Filter, serializer) : null;
        jo["S"] = value.Sorters != null ? JToken.FromObject(value.Sorters, serializer) : null;
        jo["P"] = value.Pager != null ? JToken.FromObject(value.Pager, serializer) : null;

        jo.WriteTo(writer);
    }

    public override DataQuery? ReadJson(JsonReader reader, Type objectType, DataQuery? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var jo = JObject.Load(reader);

        var dataQuery = new DataQuery();

        if (jo["F"] != null)
            dataQuery.Filter = jo["F"]!.ToObject<DataFilter>(serializer);
        else if (!_readOnlyShortForm && jo["Filter"] != null)
            dataQuery.Filter = jo["Filter"]!.ToObject<DataFilter>(serializer);

        if (jo["S"] != null)
            dataQuery.Sorters = jo["S"]!.ToObject<DataSorters>(serializer);
        else if (!_readOnlyShortForm && jo["Sorters"] != null)
            dataQuery.Sorters = jo["Sorters"]!.ToObject<DataSorters>(serializer);

        if (jo["P"] != null)
            dataQuery.Pager = jo["P"]!.ToObject<DataPager>(serializer);
        else if (!_readOnlyShortForm && jo["Pager"] != null)
            dataQuery.Pager = jo["Pager"]!.ToObject<DataPager>(serializer);

        return dataQuery;
    }
}