using System.Linq.Expressions;
using FlexFetcher;
using FlexFetcherTests.Stubs.FlexFetcherContexts;
using TestData.Database;

namespace FlexFetcherTests.Stubs.CustomFields;

public class AddressCountryCustomField : BaseFlexCustomField<AddressEntity, string?>
{
    public override string Field => "Country";

    protected override Expression<Func<AddressEntity, string?>> BuildFieldExpression(IFlexFetcherContext? context = null)
    {
        if (context is not CustomContext customContext)
        {
            throw new NotSupportedException("Invalid context type");
        }

        if (customContext.Culture.Name == "de-DE")
        {
            return entity => entity.CountryDe;
        }

        return entity => entity.CountryEn;
    }
}