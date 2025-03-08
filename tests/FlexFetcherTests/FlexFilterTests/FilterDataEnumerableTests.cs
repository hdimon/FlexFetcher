using System.Globalization;
using FlexFetcher;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;
using FlexFetcher.Utils;
using FlexFetcherTests.Stubs.CustomFields;
using FlexFetcherTests.Stubs.CustomFilters;
using FlexFetcherTests.Stubs.FlexFetcherContexts;
using TestData;
using TestData.Database;

namespace FlexFetcherTests.FlexFilterTests;

public class FilterDataEnumerableTests : BaseFilterData
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
    public void SimpleContainsFilter()
    {
        SimpleContainsFilterTest(filters => _people.FilterData(filters).ToList());

        var flexFilter = new FlexFilter<PeopleEntity>();
        SimpleContainsFilterTest(filters => flexFilter.FilterData(_people, filters).ToList());
    }

    [Test]
    public void SimpleStartsWithFilter()
    {
        SimpleStartsWithFilterTest(filters => _people.FilterData(filters).ToList());

        var flexFilter = new FlexFilter<PeopleEntity>();
        SimpleStartsWithFilterTest(filters => flexFilter.FilterData(_people, filters).ToList());
    }

    [Test]
    public void SimpleEndsWithFilter()
    {
        SimpleEndsWithFilterTest(filters => _people.FilterData(filters).ToList());

        var flexFilter = new FlexFilter<PeopleEntity>();
        SimpleEndsWithFilterTest(filters => flexFilter.FilterData(_people, filters).ToList());
    }

    [Test]
    public void SimpleInArrayFilter()
    {
        SimpleInArrayFilterTest(filters => _people.FilterData(filters).ToList());

        var flexFilter = new FlexFilter<PeopleEntity>();
        SimpleInArrayFilterTest(filters => flexFilter.FilterData(_people, filters).ToList());
    }

    [Test]
    public void SimpleFilterWithCustomFilter()
    {
        var customExpressionFilter = new PeopleFullNameCustomExpressionFilter();
        var flexExpressionFilter = new SimplePeopleFilterWithCustomExpressionFilter(customExpressionFilter);
        SimpleFilterWithCustomFilterTest(filters => flexExpressionFilter.FilterData(_people, filters).ToList());
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
        SimpleFilterWithCustomFilterWithContextTest(filters => flexFilter.FilterData(_people, filters, context).ToList());
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
        SimpleFilterWithNestedCustomFilterTest((flexFilter, filters) => flexFilter.FilterData(_people, filters).ToList());
    }

    [Test]
    public void SimpleFilterWithNestedWithCustomFilterWithContext()
    {
        SimpleFilterWithNestedWithCustomFilterWithContextTest((flexFilter, filters, context) =>
            flexFilter.FilterData(_people, filters, context).ToList());
    }

    [Test]
    public void SimpleFilterWithFieldAlias()
    {
        SimpleFilterWithFieldAliasTest((filters, options) => _people.FilterData(filters, options).ToList());

        var flexFilter = new SimplePeopleFilterWithFieldAlias();
        SimpleFilterWithFieldAliasTest((filters, _) => flexFilter.FilterData(_people, filters).ToList());
    }

    [Test]
    public void SimpleValueObjectFilterWithFieldAlias()
    {
        SimpleValueObjectFilterWithFieldAliasTest((filters, options) => _people.FilterData(filters, options).ToList());

        var flexFilter = new SimplePeopleFilterWithFieldAlias();
        SimpleValueObjectFilterWithFieldAliasTest((filters, _) => flexFilter.FilterData(_people, filters).ToList());
    }

    private class SimplePeopleFilterWithFieldAlias : FlexFilter<PeopleEntity>
    {
        public SimplePeopleFilterWithFieldAlias()
        {
            Options.Field(x => x.Name).Map("FirstName");
        }
    }

    [Test]
    public void SimpleFilterWithDefaultAndLogic()
    {
        SimpleFilterWithDefaultAndLogicTest(filters => _people.FilterData(filters).ToList());
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
        var filter = new DataFilter
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
        SimpleNestedEntityFilterWithFieldAliasTest((flexFilter, filters) => flexFilter.FilterData(_people, filters).ToList());
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

    [Test]
    public void SimpleFilterWithHiddenField()
    {
        var options = new FlexFilterOptions<PeopleEntity>();
        options.Field(x => x.CreatedByUserId).Hide();
        var flexFilter = new FlexFilter<PeopleEntity>(options);
        SimpleFilterWithHiddenFieldTest(sorters => flexFilter.FilterData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleFilterWithAllHiddenOriginalFields()
    {
        var options = new FlexFilterOptions<PeopleEntity>();
        options.HideOriginalFields();
        var flexFilter = new FlexFilter<PeopleEntity>(options);
        SimpleFilterWithHiddenFieldTest(sorters => flexFilter.FilterData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleFilterWithNotFoundField()
    {
        var options = new FlexFilterOptions<PeopleEntity>();
        var flexFilter = new FlexFilter<PeopleEntity>(options);
        SimpleFilterWithNotFoundFieldTest(sorters => flexFilter.FilterData(_people, sorters).ToList());
    }
}