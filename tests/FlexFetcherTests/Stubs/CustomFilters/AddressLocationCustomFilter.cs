using System.Linq.Expressions;
using FlexFetcher;
using TestData.Database;

namespace FlexFetcherTests.Stubs.CustomFilters;

public class AddressLocationCustomFilter : BaseFlexCustomField<AddressEntity, string>
{
    public override string Field => "Location";

    protected override Expression<Func<AddressEntity, string>> BuildFieldExpression()
    {
        return address => address.City + ", " + address.State;
    }
}