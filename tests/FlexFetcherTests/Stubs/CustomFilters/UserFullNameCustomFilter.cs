using System.Linq.Expressions;
using FlexFetcher;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs.CustomFilters;

public class UserFullNameCustomFilter : BaseFlexCustomFilter<UserEntity, string>
{
    public override string Field => "FullName";

    protected override Expression<Func<UserEntity, string>> BuildFilterExpression()
    {
        return p => p.Name + " " + p.Surname;
    }
}