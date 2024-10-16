using FlexFetcher;
using FlexFetcher.Models.Queries;
using FlexFetcher.Utils;
using TestData;
using TestData.Database;

namespace FlexFetcherTests.FlexPagerTests;

public class PageDataTests
{
    private List<PeopleEntity> _people = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _people = InMemoryDataHelper.GetPeople();
    }

    [Test]
    public void Validation()
    {
        var flexPager = new FlexPager<PeopleEntity>();

        var pager = new DataPager { PageSize = 0, Take = 0 };
        var result = flexPager.PagerIsValid(pager);
        Assert.That(result, Is.False);

        pager = new DataPager { PageSize = 10, Page = 0 };
        result = flexPager.PagerIsValid(pager);
        Assert.That(result, Is.False);

        pager = new DataPager { Take = 10, Skip = -1 };
        result = flexPager.PagerIsValid(pager);
        Assert.That(result, Is.False);

        pager = new DataPager { PageSize = 10, Page = 1 };
        result = flexPager.PagerIsValid(pager);
        Assert.That(result, Is.True);

        pager = new DataPager { Take = 10, Skip = 0 };
        result = flexPager.PagerIsValid(pager);
        Assert.That(result, Is.True);
    }

    [Test]
    public void PageDataWithPageNumber()
    {
        var flexPager = new FlexPager<PeopleEntity>();

        var pager = new DataPager { PageSize = 3, Page = 1 };
        var result = flexPager.PageData(_people, pager).ToList();
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 2, 3 }));
        // Extension method
        result = _people.PageData(pager).ToList();
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 2, 3 }));

        pager = new DataPager { PageSize = 3, Page = 2 };
        result = flexPager.PageData(_people, pager).ToList();
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 4, 5, 6 }));

        pager = new DataPager { PageSize = 3, Page = 3 };
        result = flexPager.PageData(_people, pager).ToList();
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 7, 8, 9 }));

        pager = new DataPager { PageSize = 3, Page = 4 };
        result = flexPager.PageData(_people, pager).ToList();
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 10 }));

        pager = new DataPager { PageSize = 3, Page = 5 };
        result = flexPager.PageData(_people, pager).ToList();
        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public void PageDataWithNullPager()
    {
        var flexPager = new FlexPager<PeopleEntity>();

        var result = flexPager.PageData(_people, null).ToList();
        Assert.That(result.Count, Is.EqualTo(10));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }));
    }

    [Test]
    public void PageDataWithSkip()
    {
        var flexPager = new FlexPager<PeopleEntity>();

        var pager = new DataPager { Skip = 0, Take = 3 };
        var result = flexPager.PageData(_people, pager).ToList();
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 2, 3 }));
        // Extension method
        result = _people.PageData(pager).ToList();
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 2, 3 }));

        pager = new DataPager { Skip = 3, Take = 3 };
        result = flexPager.PageData(_people, pager).ToList();
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 4, 5, 6 }));

        pager = new DataPager { Skip = 6, Take = 3 };
        result = flexPager.PageData(_people, pager).ToList();
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 7, 8, 9 }));

        pager = new DataPager { Skip = 9, Take = 3 };
        result = flexPager.PageData(_people, pager).ToList();
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 10 }));

        pager = new DataPager { Skip = 12, Take = 3 };
        result = flexPager.PageData(_people, pager).ToList();
        Assert.That(result.Count, Is.EqualTo(0));
    }
}