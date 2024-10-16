using System.Linq.Expressions;
using FlexFetcher;
using TestData.Database;

namespace FlexFetcherTests.Stubs.CustomFilters;

public class UserFullNameCustomFilter : BaseFlexCustomField<UserEntity, string>
{
    public override string Field => "FullName";

    protected override Expression<Func<UserEntity, string>> BuildFieldExpression()
    {
        return p => p.Name + " " + p.Surname;
    }
}