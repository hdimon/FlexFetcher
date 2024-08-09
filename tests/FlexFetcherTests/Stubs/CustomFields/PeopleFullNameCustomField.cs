using FlexFetcher;
using FlexFetcherTests.Stubs.Database;
using System.Linq.Expressions;

namespace FlexFetcherTests.Stubs.CustomFields;

public class PeopleFullNameCustomField : BaseFlexCustomField<PeopleEntity, string>
{
    public override string Field => "FullName";

    protected override Expression<Func<PeopleEntity, string>> BuildFieldExpression()
    {
        return p => p.Surname + " " + p.Name;
    }
}