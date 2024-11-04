using System.Linq.Expressions;
using FlexFetcher;
using TestData.Database;

namespace FlexFetcherTests.Stubs.CustomFilters;

public class PeopleFullNameCustomFilter : BaseFlexCustomField<PeopleEntity, string>
{
    public override string Field => "FullName";

    protected override Expression<Func<PeopleEntity, string>> BuildFieldExpression(IFlexFetcherContext? context = null)
    {
        Expression<Func<PeopleEntity, string>> expressionLambda = p => p.Name + " " + p.Surname;
        return expressionLambda;
    }
}