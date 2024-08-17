﻿using FlexFetcher;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs.FlexFilters;

public class SimpleNestedEntityPeopleFilterWithFieldAlias : FlexFilter<PeopleEntity>
{
    public SimpleNestedEntityPeopleFilterWithFieldAlias(SimpleNestedEntityAddressFilterWithFieldAlias addressFilter)
    {
        Options.AddNestedFlexFilter(addressFilter);
        Options.Field(entity => entity.Address).Map("Residence");
    }
}