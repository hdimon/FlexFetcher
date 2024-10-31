using FlexFetcher.Models.Queries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlexFetcher.Serialization.NewtonsoftJson.Converters;

public class FlexFetcherDataFilterConverter : JsonConverter<DataFilter>
{
    private readonly bool _readOnlyShortForm;

    public FlexFetcherDataFilterConverter(bool readOnlyShortForm = false)
    {
        _readOnlyShortForm = readOnlyShortForm;
    }

    public override void WriteJson(JsonWriter writer, DataFilter? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var jo = new JObject();

        if (value.Logic != null)
        {
            jo["L"] = value.Logic;
            jo["Fs"] = value.Filters != null ? JArray.FromObject(value.Filters, serializer) : new JArray();
        }
        else
        {
            jo["O"] = value.Operator;
            jo["F"] = value.Field;
            jo["V"] = value.Value != null ? JToken.FromObject(value.Value, serializer) : null;
        }

        jo.WriteTo(writer);
    }

    public override DataFilter? ReadJson(JsonReader reader, Type objectType, DataFilter? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var jo = JObject.Load(reader);
        var dataFilters = new DataFilter();

        if (jo["L"] != null) 
            dataFilters.Logic = jo["L"]!.ToString();
        else if (!_readOnlyShortForm && jo["Logic"] != null)
            dataFilters.Logic = jo["Logic"]!.ToString();

        if (jo["Fs"] != null)
            dataFilters.Filters = jo["Fs"]!.ToObject<List<DataFilter>>(serializer);
        else if (!_readOnlyShortForm && jo["Filters"] != null)
            dataFilters.Filters = jo["Filters"]!.ToObject<List<DataFilter>>(serializer);

        if (jo["O"] != null)
            dataFilters.Operator = jo["O"]!.ToString();
        else if (!_readOnlyShortForm && jo["Operator"] != null)
            dataFilters.Operator = jo["Operator"]!.ToString();

        if (jo["F"] != null)
            dataFilters.Field = jo["F"]!.ToString();
        else if (!_readOnlyShortForm && jo["Field"] != null)
            dataFilters.Field = jo["Field"]!.ToString();

        if (jo["V"] != null)
            dataFilters.Value = jo["V"]!.ToObject<object>(serializer);
        else if (!_readOnlyShortForm && jo["Value"] != null)
            dataFilters.Value = jo["Value"]!.ToObject<object>(serializer);

        return dataFilters;
    }
}