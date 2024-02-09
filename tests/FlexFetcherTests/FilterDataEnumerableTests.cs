using FlexFetcher;
using FlexFetcher.Models.Queries;
using FlexFetcher.Utils;
using FlexFetcherTests.Stubs;
using FlexFetcherTests.Stubs.CustomFilters;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests;

public class FilterDataEnumerableTests : FilterDataBase
{
    private List<PeopleEntity> _people = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _people = InMemoryDataHelper.GetPeople();
    }

    [Test]
    public void NullFilter()
    {
        var result1 = _people.FilterData(null);
        Assert.That(result1.Count(), Is.EqualTo(_people.Count));

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result2 = flexFilter.FilterData(_people, null);
        Assert.That(result2.Count(), Is.EqualTo(_people.Count));
    }

    [Test]
    public void SimpleFilter()
    {
        SimpleFilterTest(filters => _people.FilterData(filters).ToList());

        var flexFilter = new FlexFilter<PeopleEntity>();
        SimpleFilterTest(filters => flexFilter.FilterData(_people, filters).ToList());
    }

    [Test]
    public void SimpleInArrayFilter()
    {
        SimpleInArrayFilterTest(filters => _people.FilterData(filters).ToList());

        var flexFilter = new FlexFilter<PeopleEntity>();
        SimpleInArrayFilterTest(filters => flexFilter.FilterData(_people, filters).ToList());
    }

    [Test]
    public void SimpleInCommaDelimitedStringFilter()
    {
        SimpleInCommaDelimitedStringFilterTest(filters => _people.FilterData(filters).ToList());

        var flexFilter = new FlexFilter<PeopleEntity>();
        SimpleInCommaDelimitedStringFilterTest(filters => flexFilter.FilterData(_people, filters).ToList());
    }

    [Test]
    public void SimpleFilterWithCustomFilter()
    {
        var customExpressionFilter = new PeopleFullNameCustomExpressionFilter();
        var flexExpressionFilter = new SimplePeopleFilterWithCustomExpressionFilter(customExpressionFilter);
        SimpleFilterWithCustomFilterTest(filters => flexExpressionFilter.FilterData(_people, filters).ToList());
    }

    private class SimplePeopleFilterWithCustomExpressionFilter : FlexFilter<PeopleEntity>
    {
        public SimplePeopleFilterWithCustomExpressionFilter(PeopleFullNameCustomExpressionFilter customFilter)
        {
            AddCustomFilter(customFilter);
        }
    }

    [Test]
    public void SimpleFilterWithNestedCustomFilter()
    {
        SimpleFilterWithNestedCustomFilterTest((flexFilter, filters) => flexFilter.FilterData(_people, filters).ToList());
    }

    [Test]
    public void SimpleFilterWithFieldAlias()
    {
        SimpleFilterWithFieldAliasTest((filters, mapField) => _people.FilterData(filters, mapField).ToList());

        var flexFilter = new SimplePeopleFilterWithFieldAlias();
        SimpleFilterWithFieldAliasTest((filters, _) => flexFilter.FilterData(_people, filters).ToList());
    }

    private class SimplePeopleFilterWithFieldAlias : FlexFilter<PeopleEntity>
    {
        protected override string MapField(string field)
        {
            return field switch
            {
                "FirstName" => "Name",
                _ => field
            };
        }
    }

    [Test]
    public void SimpleFilterWithAndLogic()
    {
        SimpleFilterWithAndLogicTest(filters => _people.FilterData(filters).ToList());
    }

    [Test]
    public void SimpleFilterWithOrLogic()
    {
        SimpleFilterWithOrLogicTest(filters => _people.FilterData(filters).ToList());
    }

    [Test]
    public void FilterWithOrLogicAndNestedAndFilters()
    {
        FilterWithOrLogicAndNestedAndFiltersTest(filters => _people.FilterData(filters).ToList());
    }

    [Test]
    public void FilterWithAndLogicAndNestedOrFilters()
    {
        FilterWithAndLogicAndNestedOrFiltersTest(filters => _people.FilterData(filters).ToList());
    }

    [Test]
    public void SimpleNestedEntityFilter()
    {
        var filter = new DataFilters
        {
            Logic = DataFilterLogic.And,
            Filters = new List<DataFilter>
            {
                new()
                {
                    Field = "Address",
                    Operator = DataFilterOperator.NotEqual,
                    Value = null
                },
                new()
                {
                    Field = "Address.City",
                    Operator = DataFilterOperator.Equal,
                    Value = "New York"
                }
            }
        };

        var result = _people.FilterData(filter).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1 }));
    }

    [Test]
    public void SimpleNestedEntityFilterWithFieldAlias()
    {
        SimpleNestedEntityFilterWithFieldAliasTest((filters, mapField) => _people.FilterData(filters, mapField).ToList());
    }

    [Test]
    public void SimpleNestedEntityFilterWithFieldAliasByFlexFilter()
    {
        SimpleNestedEntityFilterWithFieldAliasByFlexFilterTest((flexFilter, filter) =>
            flexFilter.FilterData(_people, filter).ToList());
    }

    [Test]
    public void FilterWithNestedEntitiesOfTheSameType()
    {
        FilterWithNestedEntitiesOfTheSameTypeTest((flexFilter, filters) => flexFilter.FilterData(_people, filters).ToList());
    }

    [Test]
    public void FilterWithNestedManyToManyEntities()
    {
        FilterWithNestedManyToManyEntitiesTest((flexFilter, filters) => flexFilter.FilterData(_people, filters).ToList());
    }
}