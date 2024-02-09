using FlexFetcher;
using FlexFetcherTests.Stubs.CustomFilters;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class UserFilterWithNestedEntitiesOfTheSameType1 : FlexFilter<UserEntity>
{
    public UserFilterWithNestedEntitiesOfTheSameType1(UserFullNameCustomFilter customFilter)
    {
        AddCustomFilter(customFilter);
    }
}