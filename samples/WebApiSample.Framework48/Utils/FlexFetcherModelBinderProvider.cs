using System;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using FlexFetcher.Models.Queries;

namespace WebApiSample.Framework48.Utils
{
    public class FlexFetcherModelBinderProvider : ModelBinderProvider
    {
        private readonly IModelBinder _binder = new FlexFetcherModelBinder();

        public override IModelBinder GetBinder(HttpConfiguration configuration, Type modelType)
        {
            if (modelType == typeof(DataQuery) ||
                modelType == typeof(DataFilters) ||
                modelType == typeof(DataSorters) ||
                modelType == typeof(DataPager))
            {
                return _binder;
            }

            return null;
        }
    }
}