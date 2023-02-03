using Autofac;
using Newtonsoft.Json;
using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace Parkway.CBS.Module.API
{
    public class WebApiConfig : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //https://stackoverflow.com/questions/19969228/ensure-that-httpconfiguration-ensureinitialized
            //GlobalConfiguration.DefaultServer.Configuration.EnsureInitialized();
            //GlobalConfiguration.DefaultServer.Configuration.Formatters.Insert(0, new CustomJsonFormatter());
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));

            //var jsonFormatter = GlobalConfiguration.Configuration.Formatters.OfType<JsonMediaTypeFormatter>().FirstOrDefault();

            //if (jsonFormatter != null)
            //{
            //    jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //}

            //https://stackoverflow.com/questions/12008686/webapi-batching-and-delegating-handlers/31445355#31445355
            //GlobalConfiguration.DefaultServer.Configuration.MessageHandlers.Insert(0, new ClientKeyHandler(GlobalConfiguration.DefaultServer.Configuration));
        }
    }

    public class CustomJsonFormatter : JsonMediaTypeFormatter
    {
        public CustomJsonFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
            //this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            this.SerializerSettings.Formatting = Formatting.Indented;
        }

        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
    }
}