using FlexFetcher;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class PeopleFilterWithNestedEntitiesOfTheSameType1 : FlexFilter<PeopleEntity>
{
    public PeopleFilterWithNestedEntitiesOfTheSameType1(UserFilterWithNestedEntitiesOfTheSameType1 userFlexFilter) : base(userFlexFilter)
    {
    }

    protected override string MapField(string field)
    {
        return field switch
        {
            "Creator" => "CreatedByUser",
            "Creator.CreatorFullName" => "CreatedByUser.FullName",
            "Updater" => "UpdatedByUser",
            "Updater.UpdaterFullName" => "UpdatedByUser.FullName",
            _ => field
        };
    }
}