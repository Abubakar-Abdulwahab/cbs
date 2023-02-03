using System.Web.Mvc;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Lang;

namespace Parkway.CBS.Police.Client.Middleware
{
    public class PSSAuthorizedAJAX : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Controller.TempData.Add("Error", PoliceErrorLang.usernotauthorized().ToString());
                JsonResult returnResult = new JsonResult();
                returnResult.Data = new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() };
                filterContext.Result = returnResult;
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }
}