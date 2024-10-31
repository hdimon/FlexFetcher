using System.Text.Json.Serialization;
using System.Text.Json;
using FlexFetcher.Models.Queries;

namespace FlexFetcher.Serialization.SystemTextJson.Converters;

public class FlexFetcherDataSorterConverter : JsonConverter<DataSorter>
{
    private readonly bool _readOnlyShortForm;

    public FlexFetcherDataSorterConverter(bool readOnlyShortForm = false)
    {
        _readOnlyShortForm = readOnlyShortForm;
    }

    public override void Write(Utf8JsonWriter writer, DataSorter? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteString("F", value.Field);
        writer.WriteString("D", value.Direction);
        writer.WriteEndObject();
    }

    public override DataSorter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null!;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        var dataSorter = new DataSorter();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return dataSorter;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "F":
                        dataSorter.Field = reader.GetString();
                        break;
                    case "Field":
                        if (!_readOnlyShortForm)
                            dataSorter.Field = reader.GetString();
                        else
                            reader.Skip();
                        break;
                    case "D":
                        dataSorter.Direction = reader.GetString();
                        break;
                    case "Direction":
                        if (!_readOnlyShortForm)
                            dataSorter.Direction = reader.GetString();
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