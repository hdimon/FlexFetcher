using FlexFetcher;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class SimpleNestedEntityPeopleFilterWithFieldAlias
    (SimpleNestedEntityAddressFilterWithFieldAlias addressFilter) : FlexFilter<PeopleEntity>(addressFilter)
{
    protected override string MapField(string field)
    {
        return field switch
        {
            "Residence" => "Address",
            _ => field
        };
    }
}