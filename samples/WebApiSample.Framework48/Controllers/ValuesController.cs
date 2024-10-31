using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using FlexFetcher;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;
using FlexFetcher.Utils;
using TestData;
using TestData.Database;

namespace WebApiSample.Framework48.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly List<PeopleEntity> _people = InMemoryDataHelper.GetPeople();

        // GET api/values
        /*public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }*/
        [HttpGet]
        [Route("api/FlexFetcher")]
        public List<PeopleEntity> Get([FromUri] DataFilter filter, [FromUri] DataSorters sorters, [FromUri] DataPager pager)
        {
            var filtered = _people.FilterData(filter);
            var sorted = filtered.SortData(sorters);
            var paged = sorted.PageData(pager);
            return paged.ToList();
        }

        [HttpPost]
        [Route("api/FlexFetcher")]
        public List<PeopleEntity> Post([FromBody] DataQuery query)
        {
            var addressFetcherOptions = new FlexFetcherOptions<AddressEntity>();
            addressFetcherOptions.Field(entity => entity.City).Map("Town");
            FlexFetcher<AddressEntity> addressFilter = new FlexFetcher<AddressEntity>(addressFetcherOptions);

            var peopleFetcherOptions = new FlexFetcherOptions<PeopleEntity>();
            peopleFetcherOptions.AddNestedFlexFetcher(addressFilter);
            FlexFetcher<PeopleEntity> peopleFilter = new FlexFetcher<PeopleEntity>(peopleFetcherOptions);

            var result = peopleFilter.FetchData(_people, query.Filter, query.Sorters, query.Pager);

            return result.ToList();
        }

        [HttpGet]
        [Route("api/FlexFetcher/GetFilter")]
        public List<PeopleEntity> GetFilter([FromUri] DataFilter filter)
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

        [HttpPost]
        [Route("api/FlexFetcher/PostFilter")]
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

        [HttpGet]
        [Route("api/FlexFetcher/GetSort")]
        public List<PeopleEntity> GetSort([FromUri] DataSorters sorters)
        {
            FlexSorter<PeopleEntity> sorter = new FlexSorter<PeopleEntity>();
            var sorted = sorter.SortData(_people, sorters);

            return sorted.ToList();
        }

        [HttpPost]
        [Route("api/FlexFetcher/PostSort")]
        public List<PeopleEntity> PostSort([FromBody] DataQuery query)
        {
            var sorted = _people.SortData(query.Sorters);
            return sorted.ToList();
        }

        [HttpGet]
        [Route("api/FlexFetcher/GetPager")]
        public List<PeopleEntity> GetPager([FromUri] DataPager pager)
        {
            var sorted = _people.PageData(pager);
            return sorted.ToList();
        }

        [HttpPost]
        [Route("api/FlexFetcher/PostPager")]
        public List<PeopleEntity> PostPager([FromBody] DataQuery query)
        {
            var sorted = _people.PageData(query.Pager);
            return sorted.ToList();
        }

        public class AddressModel
        {
            public string Town { get; set; }
        }
    }
}
