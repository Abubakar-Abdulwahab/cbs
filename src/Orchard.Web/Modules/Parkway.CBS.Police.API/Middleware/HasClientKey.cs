using Newtonsoft.Json;
using Parkway.CBS.Core;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Xml.Linq;

namespace Parkway.CBS.Police.API.Middleware
{
    public class HasClientKey : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            if (!IsExclude(filterContext.Request.RequestUri.AbsolutePath))
            {
                IEnumerable<string> values;
                if (!filterContext.Request.Headers.TryGetValues("CLIENTID", out values))
                {
                    var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    response.Content = new StringContent(JsonConvert.SerializeObject(
                       new APIResponse
                       {
                           Error = true,
                           ErrorCode = ErrorCode.PPCK.ToString(),
                           ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.noclientidinheader().ToString(), FieldName = "ClientId" } } }
                       }
                   ), Encoding.UTF8, "application/json");
                    filterContext.Response = response;
                }
            }
            base.OnActionExecuting(filterContext);
        }


        /// <summary>
        /// Check if this path has been exclude from having a CLIENTID in the header
        /// <para>Returns true is it has been exclude from CLIENTID check</para>
        /// </summary>
        /// <param name="path">request path</param>
        /// <returns>bool</returns>
        private bool IsExclude(string path)
        {
            List<string> pathAndValue = new List<string>();
            try
            {
                var remotePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                foreach (XElement excludeElement in XElement.Load($"{remotePath}\\App.xml").Elements("clientkeypathexclude"))
                {
                    foreach (XElement pathElement in excludeElement.Elements("path"))
                    {
                        if (pathElement.Attribute("value").Value == path) { return true; }
                    }
                }
                return false;
            }
            catch (Exception) { throw new Exception("Could not validate App xml"); }
        }
    }
}