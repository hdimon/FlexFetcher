using System.Text.Json.Serialization;
using System.Text.Json;
using FlexFetcher.Models.Queries;

namespace FlexFetcher.Serialization.SystemTextJson.Converters;

public class FlexFetcherDataFilterConverter : JsonConverter<DataFilter>
{
    private readonly bool _readOnlyShortForm;

    public FlexFetcherDataFilterConverter(bool readOnlyShortForm = false)
    {
        _readOnlyShortForm = readOnlyShortForm;
    }

    public override void Write(Utf8JsonWriter writer, DataFilter? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();

        if (value.Logic != null)
        {
            writer.WriteString("L", value.Logic);
            writer.WritePropertyName("Fs");
            JsonSerializer.Serialize(writer, value.Filters ?? new List<DataFilter>(), options);
        }
        else
        {
            writer.WriteString("O", value.Operator);
            writer.WriteString("F", value.Field);
            writer.WritePropertyName("V");
            JsonSerializer.Serialize(writer, value.Value, options);
        }

        writer.WriteEndObject();
    }

    public override DataFilter? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        var dataFilter = new DataFilter();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return dataFilter;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "L":
                        dataFilter.Logic = reader.GetString();
                        break;
                    case "Logic":
                        if (!_readOnlyShortForm)
                            dataFilter.Logic = reader.GetString();
                        else
                            reader.Skip();
                        break;
                    case "Fs":
                        dataFilter.Filters = JsonSerializer.Deserialize<List<DataFilter>>(ref reader, options);
                        break;
                    case "Filters":
                        if (!_readOnlyShortForm)
                            dataFilter.Filters = JsonSerializer.Deserialize<List<DataFilter>>(ref reader, options);
                        else
                            reader.Skip();
                        break;
                    case "O":
                        dataFilter.Operator = reader.GetString();
                        break;
                    case "Operator":
                        if (!_readOnlyShortForm)
                            dataFilter.Operator = reader.GetString();
                        else
                            reader.Skip();
                        break;
                    case "F":
                        dataFilter.Field = reader.GetString();
                        break;
                    case "Field":
                        if (!_readOnlyShortForm)
                            dataFilter.Field = reader.GetString();
                        else
                            reader.Skip();
                        break;
                    case "V":
                        dataFilter.Value = JsonSerializer.Deserialize<object>(ref reader, options);
                        break;
                    case "Value":
                        if (!_readOnlyShortForm)
                            dataFilter.Value = JsonSerializer.Deserialize<object>(ref reader, options);
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