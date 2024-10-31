using System.Text.Json.Serialization;
using System.Text.Json;
using FlexFetcher.Models.Queries;

namespace FlexFetcher.Serialization.SystemTextJson.Converters;

public class FlexFetcherDataQueryConverter : JsonConverter<DataQuery>
{
    private readonly bool _readOnlyShortForm;

    public FlexFetcherDataQueryConverter(bool readOnlyShortForm = false)
    {
        _readOnlyShortForm = readOnlyShortForm;
    }

    public override void Write(Utf8JsonWriter writer, DataQuery? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();

        writer.WritePropertyName("F");
        JsonSerializer.Serialize(writer, value.Filter, options);

        writer.WritePropertyName("S");
        JsonSerializer.Serialize(writer, value.Sorters, options);

        writer.WritePropertyName("P");
        JsonSerializer.Serialize(writer, value.Pager, options);

        writer.WriteEndObject();
    }

    public override DataQuery Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null!;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        var dataQuery = new DataQuery();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return dataQuery;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "F":
                        dataQuery.Filter = JsonSerializer.Deserialize<DataFilter>(ref reader, options);
                        break;
                    case "Filter":
                        if (!_readOnlyShortForm)
                            dataQuery.Filter = JsonSerializer.Deserialize<DataFilter>(ref reader, options);
                        else
                            reader.Skip();
                        break;
                    case "S":
                        dataQuery.Sorters = JsonSerializer.Deserialize<DataSorters>(ref reader, options);
                        break;
                    case "Sorters":
                        if (!_readOnlyShortForm)
                            dataQuery.Sorters = JsonSerializer.Deserialize<DataSorters>(ref reader, options);
                        else
                            reader.Skip();
                        break;
                    case "P":
                        dataQuery.Pager = JsonSerializer.Deserialize<DataPager>(ref reader, options);
                        break;
                    case "Pager":
                        if (!_readOnlyShortForm)
                            dataQuery.Pager = JsonSerializer.Deserialize<DataPager>(ref reader, options);
                        else
                            reader.Skip();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        throw new JsonException("Unexpected end of JSON input");
    }
}