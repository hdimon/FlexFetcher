using FlexFetcher;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;
using FlexFetcher.Utils;
using FlexFetcherTests.Stubs.CustomFields;
using FlexFetcherTests.Stubs.CustomFilters;
using FlexFetcherTests.Stubs.Database;
using FlexFetcherTests.Stubs.FlexFetcherContexts;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TestData.Database;

namespace FlexFetcherTests.FlexFilterTests;

public class FilterDataQueryableTests : BaseFilterData
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
    public void SimpleFilterWithCustomFilter()
    {
        var customFilter = new PeopleFullNameCustomFilter();
        var flexFilter = new SimplePeopleFilterWithCustomFilter(customFilter);
        SimpleFilterWithCustomFilterTest(filters => flexFilter.FilterData(_ctx.People, filters).ToList());

        var customExpressionFilter = new PeopleFullNameCustomExpressionFilter();
        var flexExpressionFilter = new SimplePeopleFilterWithCustomExpressionFilter(customExpressionFilter);
        SimpleFilterWithCustomFilterTest(filters => flexExpressionFilter.FilterData(_ctx.People, filters).ToList());
    }

    [Test]
    public void SimpleFilterWithCustomFilterWithContext()
    {
        var customExpressionFilter = new PeopleOriginCountryCustomField();
        var options = new FlexFilterOptions<PeopleEntity>();
        options.AddCustomField(customExpressionFilter);
        var flexFilter = new FlexFilter<PeopleEntity>(options);
        var context = new CustomContext
        {
            Culture = new CultureInfo("de-DE")
        };
        SimpleFilterWithCustomFilterWithContextTest(filters => flexFilter.FilterData(_ctx.People, filters, context).ToList());
    }

    private class SimplePeopleFilterWithCustomFilter : FlexFilter<PeopleEntity>
    {
        public SimplePeopleFilterWithCustomFilter(PeopleFullNameCustomFilter customFilter)
        {
            Options.AddCustomField(customFilter);
        }
    }

    private class SimplePeopleFilterWithCustomExpressionFilter : FlexFilter<PeopleEntity>
    {
        public SimplePeopleFilterWithCustomExpressionFilter(PeopleFullNameCustomExpressionFilter customFilter)
        {
            Options.AddCustomField(customFilter);
        }
    }

    [Test]
    public void SimpleFilterWithNestedCustomFilter()
    {
        SimpleFilterWithNestedCustomFilterTest((flexFilter, filters) => flexFilter.FilterData(_ctx.People, filters).ToList());
    }

    [Test]
    public void SimpleFilterWithNestedWithCustomFilterWithContext()
    {
        SimpleFilterWithNestedWithCustomFilterWithContextTest((flexFilter, filters, context) =>
            flexFilter.FilterData(_ctx.People, filters, context).ToList());
    }

    [Test]
    public void SimpleFilterWithFieldAlias()
    {
        SimpleFilterWithFieldAliasTest((filters, options) => _ctx.People.FilterData(filters, options).ToList());

        var flexFilter = new PeopleFilter();
        SimpleFilterWithFieldAliasTest((filters, _) => flexFilter.FilterData(_ctx.People, filters).ToList());
    }

    [Test]
    public void SimpleValueObjectFilterWithFieldAlias()
    {
        SimpleValueObjectFilterWithFieldAliasTest((filters, options) => _ctx.People.FilterData(filters, options).ToList());

        var flexFilter = new PeopleFilter();
        SimpleValueObjectFilterWithFieldAliasTest((filters, _) => flexFilter.FilterData(_ctx.People, filters).ToList());
    }

    private class PeopleFilter : FlexFilter<PeopleEntity>
    {
        public PeopleFilter()
        {
            Options.Field(x => x.Name).Map("FirstName");
        }
    }

    [Test]
    public void SimpleFilterWithDefaultAndLogic()
    {
        SimpleFilterWithDefaultAndLogicTest(filters => _ctx.People.FilterData(filters).ToList());
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
        var filter = new DataFilter
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
        SimpleNestedEntityFilterWithFieldAliasTest((flexFilter, filters) =>
            flexFilter.FilterData(_ctx.People.Include(p => p.Address), filters).ToList());

        // With model
        var addressOptionsModel = new FlexFilterOptions<AddressEntity, AddressModel>();
        addressOptionsModel.Field(x => x.City).Map(model => model.Town).Map("Town");
        var addressFilterModel = new FlexFilter<AddressEntity>(addressOptionsModel);
        var peopleOptionsModel = new FlexFilterOptions<PeopleEntity, PeopleModel>();
        peopleOptionsModel.AddNestedFlexFilter(addressFilterModel);
        peopleOptionsModel.Field(x => x.Address).Map(model => model.Residence).Map("Residence");
        var peopleFilterModel = new FlexFilter<PeopleEntity>(peopleOptionsModel);
        SimpleNestedEntityFilterWithFieldAliasTest((_, filters) =>
            peopleFilterModel.FilterData(_ctx.People.Include(p => p.Address), filters).ToList());
    }

    private class PeopleModel
    {
        public AddressModel Residence { get; set; } = null!;
    }

    private class AddressModel
    {
        public string Town { get; set; } = null!;
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