using System.Text.Json;
using FlexFetcher.Serialization.SystemTextJson;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using JsonException = System.Text.Json.JsonException;

namespace WebApiSample.Utils;

public class FlexFetcherModelBinder : IModelBinder
{
    private static readonly JsonSerializerOptions _serializerSettings = SystemTextJsonHelper.GetSerializerSettings();

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