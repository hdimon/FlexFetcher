using System.Linq.Expressions;
using FlexFetcher;
using FlexFetcher.ExpressionBuilders;
using FlexFetcher.ExpressionBuilders.FilterExpressionHandlers;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;
using FlexFetcherTests.Stubs;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.FlexFilterTests;

public class CustomFilterExpressionBuilderTests
{
    private List<PeopleEntity> _people = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _people = InMemoryDataHelper.GetPeople();
    }

    [Test]
    public void FilterWithValue()
    {
        var customExpressionBuilder = new CustomExpressionBuilderWithValueTest();
        var options = new FlexFilterOptions<PeopleEntity>(customExpressionBuilder);
        var flexFilter = new FlexFilter<PeopleEntity>(options);

        var filter = new DataFilters
        {
            Logic = DataFilterLogic.And,
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "Age",
                    Operator = "Module",
                    Value = 15
                }
            }
        };

        var result = flexFilter.FilterData(_people, filter).ToList();

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 3, 6, 9 }));
    }

    [Test]
    public void FilterWithoutValue()
    {
        var customExpressionBuilder = new CustomExpressionBuilderWithoutValueTest();
        var options = new FlexFilterOptions<PeopleEntity>(customExpressionBuilder);
        var flexFilter = new FlexFilter<PeopleEntity>(options);

        var filter = new DataFilters
        {
            Logic = DataFilterLogic.And,
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "Age",
                    Operator = "Even",
                    Value = null
                }
            }
        };

        var result = flexFilter.FilterData(_people, filter).ToList();

        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 3, 5, 7, 9 }));
    }

    private class CustomExpressionBuilderWithValueTest : FilterExpressionBuilder<PeopleEntity>
    {
        protected override void AddCustomExpressionHandlers(List<IFilterExpressionHandler> handlers)
        {
            handlers.Add(new ModuleFilterExpressionHandler());
        }

        private class ModuleFilterExpressionHandler : FilterExpressionHandlerAbstract
        {
            public override string Operator => "MODULE";

            public override Expression BuildExpression(Expression property, DataFilter filter)
            {
                var value = BuildValueExpression(filter);
                return Expression.Equal(Expression.Modulo(property, value), Expression.Constant(0));
            }
        }
    }

    private class CustomExpressionBuilderWithoutValueTest : FilterExpressionBuilder<PeopleEntity>
    {
        protected override void AddCustomExpressionHandlers(List<IFilterExpressionHandler> handlers)
        {
            handlers.Add(new EvenNumberFilterExpressionHandler());
        }

        private class EvenNumberFilterExpressionHandler : FilterExpressionHandlerAbstract
        {
            public override string Operator => "EVEN";

            public override Expression BuildExpression(Expression property, DataFilter filter)
            {
                return Expression.Equal(Expression.Modulo(property, Expression.Constant(2, property.Type)), Expression.Constant(0));
            }
        }
    }
}