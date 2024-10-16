using FlexFetcher;
using FlexFetcherTests.Stubs.CustomFilters;
using TestData.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class SimpleAddressFilterWithNestedCustomFilter : FlexFilter<AddressEntity>
{
    public SimpleAddressFilterWithNestedCustomFilter(AddressLocationCustomFilter customFilter)
    {
        Options.AddCustomField(customFilter);
    }
}