﻿using System.Linq.Expressions;
using FlexFetcher;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs.CustomFilters;

public class PeopleWithManyToManyGroupsCustomFilter : BaseFlexCustomFilter<PeopleEntity>
{
    public override string Field => "PeopleGroups";

    protected override Expression<Func<PeopleEntity, bool>> BuildFilterExpression(string filterOperator, object? filterValue)
    {
        string value = (string)filterValue!;

        return filterOperator switch
        {
            //"Neq" => p => p.PeopleGroups.All(p => p.Group.Name != value),
            "AnyGroup" => p => p.PeopleGroups.Any(pg => pg.Group.Name == value),
            //"all" => (PeopleEntity p) => p.PeopleGroups.All(),
            _ => throw new NotSupportedException($"Invalid filter operator: {filterOperator}")
        };
    }
}