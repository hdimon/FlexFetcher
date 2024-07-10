using FlexFetcher;
using FlexFetcher.Models.Queries;
using FlexFetcher.Utils;
using FlexFetcherTests.Stubs.CustomFilters;
using FlexFetcherTests.Stubs.Database;
using Microsoft.EntityFrameworkCore;

namespace FlexFetcherTests;

public class FilterDataQueryableTests : FilterDataBase
{
    private TestDbContext _ctx = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _ctx = new TestDbContext();
        _ctx.Database.EnsureDeleted();
        _ctx.Database.EnsureCreated();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _ctx.Database.EnsureDeleted();
        _ctx.Dispose();
    }

    [Test]
    public void NullFilter()
    {
        var result1 = _ctx.People.FilterData(null);
        Assert.That(result1.Count(), Is.EqualTo(_ctx.People.Count()));

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result2 = flexFilter.FilterData(_ctx.People, null);
        Assert.That(result2.Count(), Is.EqualTo(_ctx.People.Count()));
    }

    [Test]
    public void SimpleFilter()
    {
        SimpleFilterTest(filters => _ctx.People.FilterData(filters).ToList());

        var flexFilter = new FlexFilter<PeopleEntity>();
        SimpleFilterTest(filters => flexFilter.FilterData(_ctx.People, filters).ToList());
    }

    [Test]
    public void SimpleContainsFilter()
    {
        SimpleContainsFilterTest(filters => _ctx.People.FilterData(filters).ToList());

        var flexFilter = new FlexFilter<PeopleEntity>();
        SimpleContainsFilterTest(filters => flexFilter.FilterData(_ctx.People, filters).ToList());
    }

    [Test]
    public void SimpleStartsWithFilter()
    {
        SimpleStartsWithFilterTest(filters => _ctx.People.FilterData(filters).ToList());

        var flexFilter = new FlexFilter<PeopleEntity>();
        SimpleStartsWithFilterTest(filters => flexFilter.FilterData(_ctx.People, filters).ToList());
    }

    [Test]
    public void SimpleEndsWithFilter()
    {
        SimpleEndsWithFilterTest(filters => _ctx.People.FilterData(filters).ToList());

        var flexFilter = new FlexFilter<PeopleEntity>();
        SimpleEndsWithFilterTest(filters => flexFilter.FilterData(_ctx.People, filters).ToList());
    }

    [Test]
    public void SimpleInArrayFilter()
    {
        SimpleInArrayFilterTest(filters => _ctx.People.FilterData(filters).ToList());

        var flexFilter = new FlexFilter<PeopleEntity>();
        SimpleInArrayFilterTest(filters => flexFilter.FilterData(_ctx.People, filters).ToList());
    }

    [Test]
    public void SimpleInCommaDelimitedStringFilter()
    {
        SimpleInCommaDelimitedStringFilterTest(filters => _ctx.People.FilterData(filters).ToList());

        var flexFilter = new FlexFilter<PeopleEntity>();
        SimpleInCommaDelimitedStringFilterTest(filters => flexFilter.FilterData(_ctx.People, filters).ToList());
    }

    [Test]
    public void SimpleFilterWithCustomFilter()
    {
        var customFilter = new PeopleFullNameCustomFilter();
        var flexFilter = new SimplePeopleFilterWithCustomFilter(customFilter);
        SimpleFilterWithCustomFilterTest(filters => flexFilter.FilterData(_ctx.People, filters).ToList());

        var customExpressionFilter = new PeopleFullNameCustomExpressionFilter();
        var flexExpressionFilter = new SimplePeopleFilterWithCustomExpressionFilter(customExpressionFilter);
        SimpleFilterWithCustomFilterTest(filters => flexExpressionFilter.FilterData(_ctx.People, filters).ToList());
    }

    private class SimplePeopleFilterWithCustomFilter : FlexFilter<PeopleEntity>
    {
        public SimplePeopleFilterWithCustomFilter(PeopleFullNameCustomFilter customFilter)
        {
            AddCustomFilter(customFilter);
        }
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
        SimpleFilterWithNestedCustomFilterTest((flexFilter, filters) => flexFilter.FilterData(_ctx.People, filters).ToList());
    }

    [Test]
    public void SimpleFilterWithFieldAlias()
    {
        SimpleFilterWithFieldAliasTest((filters, mapField) => _ctx.People.FilterData(filters, mapField).ToList());

        var flexFilter = new PeopleFilter();
        SimpleFilterWithFieldAliasTest((filters, _) => flexFilter.FilterData(_ctx.People, filters).ToList());
    }

    private class PeopleFilter : FlexFilter<PeopleEntity>
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
        SimpleFilterWithAndLogicTest(filters => _ctx.People.FilterData(filters).ToList());
    }

    [Test]
    public void SimpleFilterWithOrLogic()
    {
        SimpleFilterWithOrLogicTest(filters => _ctx.People.FilterData(filters).ToList());
    }

    [Test]
    public void FilterWithOrLogicAndNestedAndFilters()
    {
        FilterWithOrLogicAndNestedAndFiltersTest(filters => _ctx.People.FilterData(filters).ToList());
    }

    [Test]
    public void FilterWithAndLogicAndNestedOrFilters()
    {
        FilterWithAndLogicAndNestedOrFiltersTest(filters => _ctx.People.FilterData(filters).ToList());
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
                    Field = "Address.City",
                    Operator = DataFilterOperator.Equal,
                    Value = "New York"
                }
            }
        };

        var result = _ctx.People.Include(p => p.Address).FilterData(filter).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1 }));
    }

    [Test]
    public void SimpleNestedEntityFilterWithFieldAlias()
    {
        SimpleNestedEntityFilterWithFieldAliasTest((filters, mapField) =>
            _ctx.People.Include(p => p.Address).FilterData(filters, mapField).ToList());
    }

    [Test]
    public void SimpleNestedEntityFilterWithFieldAliasByFlexFilter()
    {
        SimpleNestedEntityFilterWithFieldAliasByFlexFilterTest((flexFilter, filter) =>
            flexFilter.FilterData(_ctx.People.Include(p => p.Address), filter).ToList());
    }

    [Test]
    public void FilterWithNestedEntitiesOfTheSameType()
    {
        FilterWithNestedEntitiesOfTheSameTypeTest((flexFilter, filters) =>
            flexFilter.FilterData(_ctx.People.Include(p => p.CreatedByUser).Include(p => p.UpdatedByUser), filters).ToList());
    }

    [Test]
    public void FilterWithNestedManyToManyEntities()
    {
        FilterWithNestedManyToManyEntitiesTest((flexFilter, filters) => flexFilter.FilterData(_ctx.People, filters).ToList());
    }
}