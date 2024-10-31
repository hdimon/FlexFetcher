using System.Text.Json.Serialization;
using System.Text.Json;
using FlexFetcher.Models.Queries;

namespace FlexFetcher.Serialization.SystemTextJson.Converters;

public class FlexFetcherDataSortersConverter : JsonConverter<DataSorters>
{
    private readonly bool _readOnlyShortForm;

    public FlexFetcherDataSortersConverter(bool readOnlyShortForm = false)
    {
        _readOnlyShortForm = readOnlyShortForm;
    }

    public override void Write(Utf8JsonWriter writer, DataSorters? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("S");
        JsonSerializer.Serialize(writer, value.Sorters ?? new List<DataSorter>(), options);
        writer.WriteEndObject();
    }

    public override DataSorters Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null!;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        var dataSorters = new DataSorters();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return dataSorters;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read();

                if (propertyName == "S")
                {
                    dataSorters.Sorters = JsonSerializer.Deserialize<List<DataSorter>>(ref reader, options);
                }
                else if (propertyName == "Sorters")
                {
                    if (!_readOnlyShortForm)
                        dataSorters.Sorters = JsonSerializer.Deserialize<List<DataSorter>>(ref reader, options);
                    else
                        reader.Skip();
                }
                else
                    reader.Skip();
            }
        }

        throw new JsonException("Unexpected end of JSON input");
    }
}