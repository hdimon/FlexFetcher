using System.Text.Json.Serialization;
using System.Text.Json;
using FlexFetcher.Models.Queries;

namespace FlexFetcher.Serialization.SystemTextJson.Converters;

public class FlexFetcherDataPagerConverter : JsonConverter<DataPager>
{
    private readonly bool _readOnlyShortForm;

    public FlexFetcherDataPagerConverter(bool readOnlyShortForm = false)
    {
        _readOnlyShortForm = readOnlyShortForm;
    }

    public override void Write(Utf8JsonWriter writer, DataPager? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();

        if (value.PageSize > 0)
        {
            writer.WriteNumber("P", value.Page);
            writer.WriteNumber("Ps", value.PageSize);
        }
        else
        {
            writer.WriteNumber("S", value.Skip);
            writer.WriteNumber("T", value.Take);
        }

        writer.WriteEndObject();
    }

    public override DataPager Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null!;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        var dataPager = new DataPager();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return dataPager;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "P":
                        dataPager.Page = reader.GetInt32();
                        break;
                    case "Page":
                        if (!_readOnlyShortForm)
                            dataPager.Page = reader.GetInt32();
                        else
                            reader.Skip();
                        break;
                    case "Ps":
                        dataPager.PageSize = reader.GetInt32();
                        break;
                    case "PageSize":
                        if (!_readOnlyShortForm)
                            dataPager.PageSize = reader.GetInt32();
                        else
                            reader.Skip();
                        break;
                    case "S":
                        dataPager.Skip = reader.GetInt32();
                        break;
                    case "Skip":
                        if (!_readOnlyShortForm)
                            dataPager.Skip = reader.GetInt32();
                        else
                            reader.Skip();
                        break;
                    case "T":
                        dataPager.Take = reader.GetInt32();
                        break;
                    case "Take":
                        if (!_readOnlyShortForm)
                            dataPager.Take = reader.GetInt32();
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