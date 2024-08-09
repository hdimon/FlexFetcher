using FlexFetcher.Models.Queries;

namespace FlexFetcherTests.FlexFilterTests;

public class DataFilterLogicExtendingTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Base()
    {
        var test1 = DataFilterLogic.And;
        Assert.That(test1, Is.EqualTo("And"));

        // ReSharper disable once AccessToStaticMemberViaDerivedType
        var test2 = DataFilterLogicExtended.Or;
        Assert.That(test2, Is.EqualTo("Or"));

        var test3 = DataFilterLogicExtended.Xor;
        Assert.That(test3, Is.EqualTo("Xor"));
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class DataFilterLogicExtended : DataFilterLogic
    {
        public const string Xor = "Xor";
    }
}