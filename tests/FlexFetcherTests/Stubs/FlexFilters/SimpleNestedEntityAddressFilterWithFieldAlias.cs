using FlexFetcher;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class SimpleNestedEntityAddressFilterWithFieldAlias : FlexFilter<AddressEntity>
{
    public SimpleNestedEntityAddressFilterWithFieldAlias()
    {
        Options.Field(entity => entity.City).Map("Town");
    }
}