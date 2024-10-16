using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using FlexFetcher.Serialization.NewtonsoftJson;
using Newtonsoft.Json;

namespace WebApiSample.Framework48.Utils
{
    public class FlexFetcherModelBinder : IModelBinder
    {
        private static readonly JsonSerializerSettings _serializerSettings = NewtonsoftHelper.GetSerializerSettings();

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var type = bindingContext.ModelType;

            if (valueProviderResult == null)
                return false;

            var value = valueProviderResult.AttemptedValue;

            if (string.IsNullOrEmpty(value))
                return false;

            try
            {
                var result = JsonConvert.DeserializeObject(value, type, _serializerSettings);
                bindingContext.Model = result;
                return true;
            }
            catch (JsonException)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Invalid data format.");
                return false;
            }
        }
    }
}