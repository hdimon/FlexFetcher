using FlexFetcher;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;
using FlexFetcher.Utils;
using Microsoft.AspNetCore.Mvc;
using TestData;
using TestData.Database;

namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class FlexFetcherController : ControllerBase
{
    private readonly ILogger<FlexFetcherController> _logger;
    private readonly List<PeopleEntity> _people = InMemoryDataHelper.GetPeople();

    public FlexFetcherController(ILogger<FlexFetcherController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public List<PeopleEntity> Get([FromQuery] DataFilter filter, [FromQuery] DataSorters sorters, [FromQuery] DataPager pager)
    {
        var filtered = _people.FilterData(filter);
        var sorted = filtered.SortData(sorters);
        var paged = sorted.PageData(pager);
        return paged.ToList();
    }

    [HttpPost]
    public List<PeopleEntity> Post([FromServices] FlexFetcher<PeopleEntity> flexFetcher, [FromBody] DataQuery query)
    {
        var result = flexFetcher.FetchData(_people, query.Filter, query.Sorters, query.Pager);

        return result.ToList();
    }

    [HttpGet("Filter", Name = "GetFilter")]
    public List<PeopleEntity> GetFilter([FromQuery] DataFilter filter)
    {
        var addressFilterOptions = new FlexFilterOptions<AddressEntity>();
        addressFilterOptions.Field(entity => entity.City).Map("Town");
        FlexFilter<AddressEntity> addressFilter = new FlexFilter<AddressEntity>(addressFilterOptions);

        var peopleFilterOptions = new FlexFilterOptions<PeopleEntity>();
        peopleFilterOptions.AddNestedFlexFilter(addressFilter);
        FlexFilter<PeopleEntity> peopleFilter = new FlexFilter<PeopleEntity>(peopleFilterOptions);

        var filtered = peopleFilter.FilterData(_people, filter);
        return filtered.ToList();
    }

    [HttpPost("Filter", Name = "PostFilter")]
    public List<PeopleEntity> PostFilter([FromBody] DataQuery query)
    {
        var addressFilterOptions = new FlexFilterOptions<AddressEntity, AddressModel>();
        addressFilterOptions.Field(entity => entity.City).Map(model => model.Town);
        FlexFilter<AddressEntity> addressFilter = new FlexFilter<AddressEntity>(addressFilterOptions);

        var peopleFilterOptions = new FlexFilterOptions<PeopleEntity>();
        peopleFilterOptions.AddNestedFlexFilter(addressFilter);
        FlexFilter<PeopleEntity> peopleFilter = new FlexFilter<PeopleEntity>(peopleFilterOptions);

        var filtered = peopleFilter.FilterData(_people, query.Filter);
        return filtered.ToList();
    }

    [HttpGet("Sorter", Name = "GetSort")]
    public List<PeopleEntity> GetSort([FromQuery] DataSorters sorters)
    {
        FlexSorter<PeopleEntity> sorter = new FlexSorter<PeopleEntity>();
        var sorted = sorter.SortData(_people, sorters);

        return sorted.ToList();
    }

    [HttpPost("Sorter", Name = "PostSort")]
    public List<PeopleEntity> PostSort([FromBody] DataQuery query)
    {
        var sorted = _people.SortData(query.Sorters);
        return sorted.ToList();
    }

    [HttpGet("Pager", Name = "GetPager")]
    public List<PeopleEntity> GetPager([FromQuery] DataPager pager)
    {
        var sorted = _people.PageData(pager);
        return sorted.ToList();
    }

    [HttpPost("Pager", Name = "PostPager")]
    public List<PeopleEntity> PostPager([FromBody] DataQuery query)
    {
        var sorted = _people.PageData(query.Pager);
        return sorted.ToList();
    }

    public class AddressModel
    {
        public string Town { get; set; } = null!;
    }
}