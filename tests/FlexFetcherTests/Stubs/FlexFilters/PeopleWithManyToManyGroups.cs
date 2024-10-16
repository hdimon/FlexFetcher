using FlexFetcher;
using FlexFetcherTests.Stubs.CustomFilters;
using TestData.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class PeopleWithManyToManyGroups : FlexFilter<PeopleEntity>
{
    public PeopleWithManyToManyGroups(PeopleWithManyToManyGroupsCustomFilter customFilter)
    {
        Options.AddCustomField(customFilter);
    }
}