﻿using System.Linq.Expressions;
using FlexFetcher;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs.CustomFilters;

public class PeopleFullNameCustomFilter : BaseFlexCustomFilter<PeopleEntity, string>
{
    public override string Field => "FullName";

    protected override Expression<Func<PeopleEntity, string>> BuildFilterExpression()
    {
        Expression<Func<PeopleEntity, string>> expressionLambda = p => p.Name + " " + p.Surname;
        return expressionLambda;
    }
}