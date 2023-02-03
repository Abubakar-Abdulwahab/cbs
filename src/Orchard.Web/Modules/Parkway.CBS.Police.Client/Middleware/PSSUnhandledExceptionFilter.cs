using System;
using Orchard;
using System.Net;
using Orchard.Mvc;
using System.Linq;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Exceptions;
using Orchard.Mvc.Filters;
using System.Collections.Generic;
using IFilterProvider = Orchard.Mvc.Filters.IFilterProvider;
using System.Web.Routing;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;

namespace Parkway.CBS.Police.Client.Middleware
{
    public class PSSUnhandledExceptionFilter : FilterProvider, IActionFilter, IResultFilter
    {
        private readonly IExceptionPolicy _exceptionPolicy;
        private readonly IOrchardServices _orchardServices;
        private readonly Lazy<IEnumerable<IFilterProvider>> _filterProviders;

        public PSSUnhandledExceptionFilter(
            IExceptionPolicy exceptionPolicy,
            IOrchardServices orchardServices,
            Lazy<IEnumerable<IFilterProvider>> filters)
        {
            _exceptionPolicy = exceptionPolicy;
            _orchardServices = orchardServices;
            _filterProviders = filters;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // don't provide custom errors if the action has some custom code to handle exceptions
            if (!filterContext.ActionDescriptor.GetCustomAttributes(typeof(HandleErrorAttribute), false).Any())
            {
                if (!filterContext.ExceptionHandled && filterContext.Exception != null)
                {
                    if (_exceptionPolicy.HandleException(this, filterContext.Exception))
                    {
                        filterContext.ExceptionHandled = true;

                        // inform exception filters of the exception that was suppressed
                        var exceptionContext = new ExceptionContext(filterContext.Controller.ControllerContext, filterContext.Exception);
                        foreach (var exceptionFilter in _filterProviders.Value.OfType<IExceptionFilter>())
                        {
                            exceptionFilter.OnException(exceptionContext);
                        }

                        if (exceptionContext.ExceptionHandled)
                        {
                            filterContext.Result = exceptionContext.Result;
                        }
                        else
                        {
                            filterContext.Controller.TempData.Add("Error", ErrorLang.genericexception().ToString());
                            filterContext.Result = new RedirectToRouteResult("P.SelectService", new RouteValueDictionary { });
                            filterContext.RequestContext.HttpContext.Response.StatusCode = 500;
                            // prevent IIS 7.0 classic mode from handling the 404/500 itself
                            filterContext.RequestContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                        }
                    }
                }
            }

            if (filterContext.Result is HttpNotFoundResult)
            {
                var model = _orchardServices.New.PSSNotFound();
                var request = filterContext.RequestContext.HttpContext.Request;
                var url = request.RawUrl;

                // If the url is relative then replace with Requested path
                model.RequestedUrl = request.Url.OriginalString.Contains(url) & request.Url.OriginalString != url ?
                    request.Url.OriginalString : url;

                // Dont get the user stuck in a 'retry loop' by
                // allowing the Referrer to be the same as the Request
                model.ReferrerUrl = request.UrlReferrer != null &&
                                    request.UrlReferrer.OriginalString != model.RequestedUrl ?
                    request.UrlReferrer.OriginalString : null;
                //
                filterContext.Result = new ShapeResult(filterContext.Controller, model);

                filterContext.RequestContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                filterContext.ExceptionHandled = true;

                // prevent IIS 7.0 classic mode from handling the 404/500 itself
                filterContext.RequestContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {

        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            // for exceptions which occurred during the action execution

            // don't provide custom errors if the action has some custom code to handle exceptions
            if (!filterContext.ExceptionHandled && filterContext.Exception != null)
            {
                if (_exceptionPolicy.HandleException(this, filterContext.Exception))
                {
                    // inform exception filters of the exception that was suppressed
                    var exceptionContext = new ExceptionContext(filterContext.Controller.ControllerContext, filterContext.Exception);
                    foreach (var exceptionFilter in _filterProviders.Value.OfType<IExceptionFilter>())
                    {
                        exceptionFilter.OnException(exceptionContext);
                    }
                }
            }
        }
    }
}

//public class VerifyAccountGate :  HandleErrorAttribute
//{
//    public override void OnException(ExceptionContext filterContext)
//    {
//        //if (filterContext.ExceptionHandled || filterContext.HttpContext.IsCustomErrorEnabled)
//        //{
//        //    return;
//        //}
//        var fsd = filterContext.HttpContext.Response.StatusCode;
//        var e = filterContext.Exception;
//        var fd = e.GetType();
//        if (e.GetType() is UserNotVerifiedException) { return; }
//        filterContext.ExceptionHandled = true;
//        //e.HelpLink = 
//        var dsd = new RouteValueDictionary { };
//        filterContext.Result = new RedirectToRouteResult("P.SelectService", new RouteValueDictionary { { "token", e.Message } });
//    }
//}


