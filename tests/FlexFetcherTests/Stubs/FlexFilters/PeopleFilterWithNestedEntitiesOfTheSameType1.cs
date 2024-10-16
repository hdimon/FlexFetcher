using FlexFetcher;
using TestData.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class PeopleFilterWithNestedEntitiesOfTheSameType1 : FlexFilter<PeopleEntity>
{
    public PeopleFilterWithNestedEntitiesOfTheSameType1(UserFilterWithNestedEntitiesOfTheSameType1 userFlexFilter)
    {
        Options.AddNestedFlexFilter(userFlexFilter);
        Options.Field(entity => entity.CreatedByUser).Map("Creator");
        Options.Field(entity => entity.UpdatedByUser).Map("Updater");
    }
}