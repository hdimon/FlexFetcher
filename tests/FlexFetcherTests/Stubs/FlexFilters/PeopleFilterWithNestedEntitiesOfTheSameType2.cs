using FlexFetcher;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class PeopleFilterWithNestedEntitiesOfTheSameType2 : FlexFilter<PeopleEntity>
{
    public PeopleFilterWithNestedEntitiesOfTheSameType2(UserFilterWithNestedEntitiesOfTheSameType2 userFlexFilter) : base(userFlexFilter)
    {
    }

    protected override string MapField(string field)
    {
        return field switch
        {
            "Creator" => "CreatedByUser",
            "Updater" => "UpdatedByUser",
            _ => field
        };
    }
}