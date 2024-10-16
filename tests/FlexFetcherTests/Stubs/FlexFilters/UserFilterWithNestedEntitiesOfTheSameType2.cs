using FlexFetcher;
using FlexFetcherTests.Stubs.CustomFilters;
using TestData.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class UserFilterWithNestedEntitiesOfTheSameType2 : FlexFilter<UserEntity>
{
    public UserFilterWithNestedEntitiesOfTheSameType2(UserFullNameCustomFilter customFilter)
    {
        Options.AddCustomField(customFilter).Map("CreatorFullName").Map("UpdaterFullName");
    }
}