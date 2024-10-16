using FlexFetcher;
using FlexFetcherTests.Stubs.CustomFilters;
using TestData.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class UserFilterWithNestedEntitiesOfTheSameType1 : FlexFilter<UserEntity>
{
    public UserFilterWithNestedEntitiesOfTheSameType1(UserFullNameCustomFilter customFilter)
    {
        Options.AddCustomField(customFilter).Map("CreatorFullName").Map("UpdaterFullName");
    }
}