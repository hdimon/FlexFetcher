using FlexFetcher;
using System.Linq.Expressions;
using TestData.Database;

namespace FlexFetcherTests.Stubs.CustomFields;

public class PeopleFullNameCustomField : BaseFlexCustomField<PeopleEntity, string>
{
    public override string Field => "FullName";

    protected override Expression<Func<PeopleEntity, string>> BuildFieldExpression(IFlexFetcherContext? context = null)
    {
        return p => p.Surname + " " + p.Name;
    }
}