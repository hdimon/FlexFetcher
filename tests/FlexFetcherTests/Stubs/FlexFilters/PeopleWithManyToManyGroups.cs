using FlexFetcher;
using FlexFetcherTests.Stubs.CustomFilters;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class PeopleWithManyToManyGroups : FlexFilter<PeopleEntity>
{
    public PeopleWithManyToManyGroups(PeopleWithManyToManyGroupsCustomFilter customFilter)
    {
        AddCustomFilter(customFilter);
    }
}