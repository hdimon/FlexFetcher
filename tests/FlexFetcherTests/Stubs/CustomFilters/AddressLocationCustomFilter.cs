using System.Linq.Expressions;
using FlexFetcher;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs.CustomFilters;

public class AddressLocationCustomFilter : BaseFlexCustomFilter<AddressEntity, string>
{
    public override string Field => "Location";

    protected override Expression<Func<AddressEntity, string>> BuildFilterExpression()
    {
        return address => address.City + ", " + address.State;
    }
}