using System.Linq.Expressions;
using FlexFetcher;
using FlexFetcher.Models.Queries;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs.CustomFilters;

public class PeopleFullNameCustomExpressionFilter : BaseFlexCustomFieldFilter<PeopleEntity>
{
    public override string Field => "FullName";

    protected override Expression<Func<PeopleEntity, bool>> BuildFilterExpression(string filterOperator, object? filterValue)
    {
        string value = (string)filterValue!;
        return filterOperator switch
        {
            DataFilterOperator.Equal => entity => entity.Name + " " + entity.Surname == value,
            DataFilterOperator.NotEqual => entity => entity.Name + " " + entity.Surname != value,
            _ => throw new NotSupportedException($"Operator {filterOperator} is not supported for field {Field}")
        };
    }
}