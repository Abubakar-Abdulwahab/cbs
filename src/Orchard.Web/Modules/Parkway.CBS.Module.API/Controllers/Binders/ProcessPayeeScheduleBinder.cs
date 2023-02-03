using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Parkway.CBS.Module.API.Controllers.Binders
{
    public class ProcessPayeeScheduleBinder : IModelBinder
    {
        public ILogger Logger { get; set; }

        public ProcessPayeeScheduleBinder()
        {
            Logger = NullLogger.Instance;
        }

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            try
            {
                ProcessPayeModel payeeScheduleObject = null;
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

                    var sPayeeScheduleObject = multiPartRequest.Contents.Where(c => ((c.Headers.ContentType != null) && (c.Headers.ContentType.MediaType.Contains("application/json")))).Single();
                    modelJSONString = sPayeeScheduleObject.ReadAsStringAsync().Result;
                }
                payeeScheduleObject = JsonConvert.DeserializeObject<ProcessPayeModel>(modelJSONString);
                bindingContext.Model = payeeScheduleObject;
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Could not find/convert ProcessPayeModel json. Exception: " + exception.Message);
            }
            return false;
        }
    }
}