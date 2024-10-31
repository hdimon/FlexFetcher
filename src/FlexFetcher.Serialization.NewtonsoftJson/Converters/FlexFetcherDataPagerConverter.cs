using FlexFetcher.Models.Queries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlexFetcher.Serialization.NewtonsoftJson.Converters;

public class FlexFetcherDataPagerConverter : JsonConverter<DataPager>
{
    private readonly bool _readOnlyShortForm;

    public FlexFetcherDataPagerConverter(bool readOnlyShortForm = false)
    {
        _readOnlyShortForm = readOnlyShortForm;
    }

    public override void WriteJson(JsonWriter writer, DataPager? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var jo = new JObject();

        if (value.PageSize > 0)
        {
            jo["P"] = value.Page;
            jo["Ps"] = value.PageSize;
        }
        else
        {
            jo["S"] = value.Skip;
            jo["T"] = value.Take;
        }

        jo.WriteTo(writer);
    }

    public override DataPager? ReadJson(JsonReader reader, Type objectType, DataPager? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var jo = JObject.Load(reader);

        var dataPager = new DataPager();

        if (jo["P"] != null)
            dataPager.Page = jo["P"]!.ToObject<int>(serializer);
        else if (!_readOnlyShortForm && jo["Page"] != null)
            dataPager.Page = jo["Page"]!.ToObject<int>(serializer);

        if (jo["Ps"] != null)
            dataPager.PageSize = jo["Ps"]!.ToObject<int>(serializer);
        else if (!_readOnlyShortForm && jo["PageSize"] != null)
            dataPager.PageSize = jo["PageSize"]!.ToObject<int>(serializer);

        if (jo["S"] != null)
            dataPager.Skip = jo["S"]!.ToObject<int>(serializer);
        else if (!_readOnlyShortForm && jo["Skip"] != null)
            dataPager.Skip = jo["Skip"]!.ToObject<int>(serializer);

        if (jo["T"] != null)
            dataPager.Take = jo["T"]!.ToObject<int>(serializer);
        else if (!_readOnlyShortForm && jo["Take"] != null)
            dataPager.Take = jo["Take"]!.ToObject<int>(serializer);

        return dataPager;
    }
}