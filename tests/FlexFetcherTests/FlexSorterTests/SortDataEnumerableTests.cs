using FlexFetcher;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Utils;
using FlexFetcherTests.Stubs.CustomFields;
using FlexFetcherTests.Stubs.FlexFetcherContexts;
using System.Globalization;
using TestData;
using TestData.Database;

namespace FlexFetcherTests.FlexSorterTests;

public class SortDataEnumerableTests : BaseSortData
{
    private List<PeopleEntity> _people = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _people = InMemoryDataHelper.GetPeople();
    }

    [Test]
    public void SimpleSort()
    {
        SimpleSortTest(sorters => _people.SortData(sorters).ToList());

        var flexSorter = new FlexSorter<PeopleEntity>();
        SimpleSortTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleIdSort()
    {
        SimpleIdSortTest(sorters => _people.SortData(sorters).ToList());

        var flexSorter = new FlexSorter<PeopleEntity>();
        SimpleIdSortTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void TwoFieldsSurnameAndIdSort()
    {
        TwoFieldsSurnameAndIdSortTest(sorters => _people.SortData(sorters).ToList());

        var flexSorter = new FlexSorter<PeopleEntity>();
        TwoFieldsSurnameAndIdSortTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void TwoFieldsSurnameAndNameSort()
    {
        TwoFieldsSurnameAndNameSortTest(sorters => _people.SortData(sorters).ToList());

        var flexSorter = new FlexSorter<PeopleEntity>();
        TwoFieldsSurnameAndNameSortTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void TwoFieldsSurnameAndValueObjectNameSort()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            TwoFieldsSurnameAndValueObjectNameSortTest(sorters => _people.SortData(sorters).ToList());
        });

        var flexSorterOptions = new FlexSorterOptions<PeopleEntity>();
        flexSorterOptions.Field(x => x.PeopleName).CastTo<string>();
        var flexSorter = new FlexSorter<PeopleEntity>(flexSorterOptions);
        TwoFieldsSurnameAndValueObjectNameSortTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleNestedCitySort()
    {
        var nullCities = _people.Where(p => p.Address == null).ToList();
        nullCities.ForEach(p => p.Address = new AddressEntity { City = "A" });

        SimpleNestedCitySortTest(sorters => _people.SortData(sorters).ToList());

        var flexSorter = new FlexSorter<PeopleEntity>();
        SimpleNestedCitySortTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleSorterWithFieldAlias()
    {
        SimpleSorterWithFieldAliasTest((sorters, options) => _people.SortData(sorters, options).ToList());

        var options = new FlexSorterOptions<PeopleEntity>(); 
        options.Field(x => x.Surname).Map("SecondName");
        var flexSorter = new FlexSorter<PeopleEntity>(options);
        SimpleSorterWithFieldAliasTest((sorters, _) => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleNestedEntitySorterWithFieldAlias()
    {
        var nullCities = _people.Where(p => p.Address == null).ToList();
        nullCities.ForEach(p => p.Address = new AddressEntity { City = "A" });

        var addressOptions = new FlexSorterOptions<AddressEntity>();
        addressOptions.Field(x => x.City).Map("Town");
        var addressSorter = new FlexSorter<AddressEntity>(addressOptions);
        var peopleOptions = new FlexSorterOptions<PeopleEntity>();
        peopleOptions.AddNestedFlexSorter(addressSorter);
        peopleOptions.Field(x => x.Address).Map("Residence");
        var flexSorter = new FlexSorter<PeopleEntity>(peopleOptions);
        SimpleNestedEntitySorterWithFieldAliasTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleSorterWithCustomSorter()
    {
        var flexSorter = new SimplePeopleSorterWithCustomSorter();
        SimpleSorterWithCustomSorterTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleSorterWithCustomSorterWithContext()
    {
        var customExpressionFilter = new PeopleOriginCountryCustomField();
        var options = new FlexSorterOptions<PeopleEntity>();
        options.AddCustomField(customExpressionFilter);
        var flexSorter = new FlexSorter<PeopleEntity>(options);
        var context = new CustomContext
        {
            Culture = new CultureInfo("de-DE")
        };
        SimpleSorterWithCustomSorterWithContextTest(sorters => flexSorter.SortData(_people, sorters, context).ToList());
    }

    [Test]
    public void SimpleSorterWithCustomSorterWithAlias()
    {
        var flexSorterCustom = new SimplePeopleSorterWithCustomSorter();
        SimpleSorterWithCustomSorterWithAliasTest(sorters => flexSorterCustom.SortData(_people, sorters).ToList());

        var customField = new PeopleFullNameCustomField();
        var options = new FlexSorterOptions<PeopleEntity>();
        options.AddCustomField(customField).Map("Title");
        var flexSorter = new FlexSorter<PeopleEntity>(options);
        SimpleSorterWithCustomSorterTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    private class SimplePeopleSorterWithCustomSorter : FlexSorter<PeopleEntity>
    {
        public SimplePeopleSorterWithCustomSorter()
        {
            Options.AddCustomField(new PeopleFullNameCustomField()).Map("Title");
        }
    }

    [Test]
    public void SimpleSorterWithHiddenField()
    {
        var options = new FlexSorterOptions<PeopleEntity>();
        options.Field(x => x.CreatedByUserId).Hide();
        var flexSorter = new FlexSorter<PeopleEntity>(options);
        SimpleSorterWithHiddenFieldTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleSorterWithAllHiddenOriginalFields()
    {
        var options = new FlexSorterOptions<PeopleEntity>();
        options.HideOriginalFields();
        var flexSorter = new FlexSorter<PeopleEntity>(options);
        SimpleSorterWithHiddenFieldTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleSorterWithNotFoundField()
    {
        var options = new FlexSorterOptions<PeopleEntity>();
        var flexSorter = new FlexSorter<PeopleEntity>(options);
        SimpleSorterWithNotFoundFieldTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }
}