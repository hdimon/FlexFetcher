using FlexFetcher.Models.Queries;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApiSample.Utils;

public class FlexFetcherModelBinderProvider : IModelBinderProvider
{
    private readonly IModelBinder _binder = new FlexFetcherModelBinder();

    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        var bindingInfo = context.BindingInfo;
        var modelType = context.Metadata.ModelType;

        if (bindingInfo.BindingSource == BindingSource.Query && (modelType == typeof(DataQuery) ||
                                                                 modelType == typeof(DataFilter) ||
                                                                 modelType == typeof(DataSorters) ||
                                                                 modelType == typeof(DataPager)))
        {
            return _binder;
        }

        return null;
    }
}