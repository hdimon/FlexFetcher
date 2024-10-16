using FlexFetcher;
using TestData.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class PeopleFilterWithNestedEntitiesOfTheSameType2 : FlexFilter<PeopleEntity>
{
    public PeopleFilterWithNestedEntitiesOfTheSameType2(UserFilterWithNestedEntitiesOfTheSameType2 userFlexFilter)
    {
        Options.AddNestedFlexFilter(userFlexFilter);
        Options.Field(entity => entity.CreatedByUser).Map("Creator");
        Options.Field(entity => entity.UpdatedByUser).Map("Updater");
    }
}