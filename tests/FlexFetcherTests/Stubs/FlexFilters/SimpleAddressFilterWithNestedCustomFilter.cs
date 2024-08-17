using FlexFetcher;
using FlexFetcherTests.Stubs.CustomFilters;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class SimpleAddressFilterWithNestedCustomFilter : FlexFilter<AddressEntity>
{
    public SimpleAddressFilterWithNestedCustomFilter(AddressLocationCustomFilter customFilter)
    {
        Options.AddCustomField(customFilter);
    }
}