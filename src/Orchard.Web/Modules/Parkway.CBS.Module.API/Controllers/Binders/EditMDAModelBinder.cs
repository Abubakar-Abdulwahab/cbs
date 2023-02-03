using Newtonsoft.Json;
using Orchard.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Parkway.CBS.Module.API.Controllers.Binders
{
    public class EditMDAModelBinder : IModelBinder
    {
        public ILogger Logger { get; set; }

        public EditMDAModelBinder()
        {
            Logger = NullLogger.Instance;
        }

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            try
            {
                EditMDAModel createMDAObject = null;
                String modelJSONString = null;
                HttpContent requestContent = actionContext.Request.Content;
                if (!actionContext.Request.Content.IsMimeMultipartContent())
                {
                    modelJSONString = requestContent.ReadAsStringAsync().Result;
                }
                else
                {
                    MultipartMemoryStreamProvider multiPartRequest = null;
                    Task.Factory
                        .StartNew(() => multiPartRequest = requestContent.ReadAsMultipartAsync().Result,
                            CancellationToken.None, TaskCreationOptions.LongRunning, /* runs on a separate thread*/ TaskScheduler.Default).Wait();

                    var sCreateMDAObject = multiPartRequest.Contents.Where(c => ((c.Headers.ContentType != null) && (c.Headers.ContentType.MediaType.Contains("application/json")))).Single();
                    modelJSONString = sCreateMDAObject.ReadAsStringAsync().Result;
                }
                createMDAObject = JsonConvert.DeserializeObject<EditMDAModel>(modelJSONString);
                bindingContext.Model = createMDAObject;
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Could not find/convert CreateMDAModel json. Exception: " + exception.Message);
            }
            return false;
        }
    }
}