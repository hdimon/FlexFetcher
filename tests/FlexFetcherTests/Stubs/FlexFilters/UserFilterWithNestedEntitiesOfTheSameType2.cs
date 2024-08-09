using FlexFetcher;
using FlexFetcherTests.Stubs.CustomFilters;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class UserFilterWithNestedEntitiesOfTheSameType2 : FlexFilter<UserEntity>
{
    public UserFilterWithNestedEntitiesOfTheSameType2(UserFullNameCustomFilter customFilter)
    {
        AddCustomField(customFilter);
    }

    protected override string MapField(string field)
    {
        return field switch
        {
            "CreatorFullName" => "FullName",
            "UpdaterFullName" => "FullName",
            _ => field
        };
    }
}