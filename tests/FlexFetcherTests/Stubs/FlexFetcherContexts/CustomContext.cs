using System.Globalization;
using FlexFetcher;

namespace FlexFetcherTests.Stubs.FlexFetcherContexts;

public class CustomContext : IFlexFetcherContext
{
    public CultureInfo Culture { get; set; } = null!;
}