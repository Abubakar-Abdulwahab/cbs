using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    [PSSAuthorized]
    public class RequestDetailsController : BaseController
    {
        private readonly IRequestListHandler _policeRequestHandler;

        public RequestDetailsController(IHandler compHandler, IAuthenticationService authenticationService, IRequestListHandler policeRequestHandler)
            : base(authenticationService, compHandler)
        {
            _policeRequestHandler = policeRequestHandler;
        }


        public ActionResult RequestDetails(string fileRefNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(fileRefNumber) || fileRefNumber.Trim().Length == 0)
                {
                    TempData = null;
                    TempData["Error"] = "File number not specified";
                    return RedirectToRoute(RouteName.SelectService.ShowSelectService);
                }

                UserDetailsModel userModel = GetLoggedInUserDetails();
                var detailsModel = _policeRequestHandler.GetRequestViewDetails(fileRefNumber, userModel.Entity.Id);
                detailsModel.HeaderObj = HeaderFiller(userModel);
                return View(detailsModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return RedirectToRoute(RouteName.RequestList.ShowRequestList);
            }
        }

        public ActionResult RequestBranchDetails(string fileRefNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(fileRefNumber) || fileRefNumber.Trim().Length == 0)
                {
                    TempData = null;
                    TempData["Error"] = "File number not specified";
                    return RedirectToRoute(RouteName.SelectService.ShowSelectService);
                }

                var detailsModel = _policeRequestHandler.GetRequestBranchViewDetails(fileRefNumber);
                UserDetailsModel userModel = GetLoggedInUserDetails();
                detailsModel.HeaderObj = HeaderFiller(userModel);
                return View(detailsModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return RedirectToRoute(Client.RouteName.RequestList.ShowRequestList);
            }
        }

        public ActionResult ViewServiceDocument(string fileRefNumber)
        {
            try
            {
                CreateCertificateDocumentVM result = _policeRequestHandler.CreateServiceDocumentByteFile(fileRefNumber, GetLoggedInUserDetails().TaxPayerProfileVM.Id);
                System.Web.HttpContext.Current.Response.ClearHeaders();
                System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + result.FileName);
                System.Web.HttpContext.Current.Response.TransmitFile(result.SavedPath);
                System.Web.HttpContext.Current.Response.End();
                return null;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.genericexception().Text);
            }
            return RedirectToRoute("P.SelectService");
        }
    }
}