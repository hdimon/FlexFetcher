using System.Text.Json;
using FlexFetcher.Serialization.SystemTextJson;
using FlexFetcher.Serialization.SystemTextJson.Converters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using JsonException = System.Text.Json.JsonException;

namespace WebApiSample.Utils;

public class FlexFetcherModelBinder : IModelBinder
{
    private readonly JsonSerializerOptions _serializerSettings = SystemTextJsonHelper.GetSerializerSettings();

    public FlexFetcherModelBinder()
    {
        _serializerSettings.Converters.Add(new FlexFetcherDataFilterConverter());
        _serializerSettings.Converters.Add(new FlexFetcherDataSortersConverter());
        _serializerSettings.Converters.Add(new FlexFetcherDataSorterConverter());
        _serializerSettings.Converters.Add(new FlexFetcherDataPagerConverter());
        _serializerSettings.Converters.Add(new FlexFetcherDataQueryConverter());
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        var type = bindingContext.ModelType;

        if (valueProviderResult == ValueProviderResult.None) 
            return Task.CompletedTask;

        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(value)) 
            return Task.CompletedTask;

        try
        {
            var result = JsonSerializer.Deserialize(value, type, _serializerSettings);

            bindingContext.Result = ModelBindingResult.Success(result);
        }
        catch (JsonException)
        {
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Invalid data format.");
        }
        return Task.CompletedTask;
    }
}