using FlexFetcher.Models.Queries;
using System.Text.Json;

namespace FlexFetcherTests;

public class SystemTextJsonHelper
{
    public static DataFilters ProcessFilter(DataFilters? filter)
    {
        if (filter == null) throw new ArgumentNullException(nameof(filter));
        // If filter was deserialized from JSON via System.Text.Json
        // then we need to convert value objects to normal .net types
        foreach (var f in filter.Filters!)
        {
            if (f.Value is JsonElement jsonElement)
            {
                f.Value = jsonElement.ValueKind switch
                {
                    JsonValueKind.String => jsonElement.GetString(),
                    JsonValueKind.Number => jsonElement.GetDecimal(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => f.Value
                };
            }
        }

        return filter;
    }
}