using FlexFetcher;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class SimpleNestedEntityAddressFilterWithFieldAlias : FlexFilter<AddressEntity>
{
    protected override string MapField(string field)
    {
        return field switch
        {
            "Town" => "City",
            _ => field
        };
    }
}