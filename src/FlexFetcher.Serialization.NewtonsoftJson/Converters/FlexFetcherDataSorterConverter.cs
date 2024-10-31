using FlexFetcher.Models.Queries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlexFetcher.Serialization.NewtonsoftJson.Converters;

public class FlexFetcherDataSorterConverter : JsonConverter<DataSorter>
{
    private readonly bool _readOnlyShortForm;

    public FlexFetcherDataSorterConverter(bool readOnlyShortForm = false)
    {
        _readOnlyShortForm = readOnlyShortForm;
    }

    public override void WriteJson(JsonWriter writer, DataSorter? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var jo = new JObject();
        jo["F"] = value.Field;
        jo["D"] = value.Direction;

        jo.WriteTo(writer);
    }

    public override DataSorter? ReadJson(JsonReader reader, Type objectType, DataSorter? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var jo = JObject.Load(reader);

        var dataSorter = new DataSorter();

        if (jo["F"] != null)
            dataSorter.Field = jo["F"]!.ToString();
        else if (!_readOnlyShortForm && jo["Field"] != null)
            dataSorter.Field = jo["Field"]!.ToString();

        if (jo["D"] != null)
            dataSorter.Direction = jo["D"]!.ToString();
        else if (!_readOnlyShortForm && jo["Direction"] != null)
            dataSorter.Direction = jo["Direction"]!.ToString();

        return dataSorter;
    }
}