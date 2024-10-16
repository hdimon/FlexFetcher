using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using FlexFetcher.Serialization.NewtonsoftJson;
using WebApiSample.Framework48.Utils;

namespace WebApiSample.Framework48
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var jsonSettings = NewtonsoftHelper.GetSerializerSettings();
            config.Formatters.JsonFormatter.SerializerSettings = jsonSettings;

            config.Services.Insert(typeof(ModelBinderProvider), 0, new FlexFetcherModelBinderProvider());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
