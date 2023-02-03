using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Module.Web.Middleware.Filters
{

    /// <summary>
    /// This class is for first level request authecation of the request, you are required to
    /// check if this request user has a CBS profile.
    /// <para>
    /// The r query param is used to indicate the return path if the request user is not authenticated,
    /// this r value is the second segment value of the request URL. The convention for Collection controller URL is domain.com/c/wuwuwu
    /// therefore seg[2] is wuwuwu. So if you are using this middleware make sure the action is in the collection controller and follows
    /// pretty URL convention.
    /// </para>
    /// </summary>
    public class CBSCollectionAuthorized : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Collection",
                            action = "SignIn",
                            r = filterContext.HttpContext.Request.Url.Segments[2]
                        })
                    );
                filterContext.Controller.TempData.Add("Error", ErrorLang.signinrequired().ToString());
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }


    public class CBSCollectionAJAXAuthorized : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                var jsonResponse = new JsonResult();
                jsonResponse.Data = new APIResponse { Error = true, ResponseObject = ErrorLang.usernotauthorized().ToString() };
                filterContext.Result = jsonResponse;
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }
}