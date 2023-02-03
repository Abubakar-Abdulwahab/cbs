using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using System;
using System.IO;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    public class TCCValidationController : BaseController
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ICommonBaseHandler _baseHandler;
        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;
        private readonly ITCCValidationHandler _tccValidationHandler;

        public TCCValidationController(ICommonBaseHandler baseHandler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ITCCValidationHandler tccValidationHandler) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, baseHandler)
        {
            _baseHandler = baseHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _userService = userService;
            _membershipService = membershipService;
            _cbsUserService = cbsUserService;
            _authenticationService = authenticationService;
            _membershipValidationService = membershipValidationService;
            _userEventHandler = userEventHandler;
            _tccValidationHandler = tccValidationHandler;
        }

        [HttpGet]
        public ActionResult ValidateTCC()
        {
            try
            {
                ValidateTCCVM vm = new ValidateTCCVM { };
                try
                {
                    if (TempData.ContainsKey("Error"))
                    {
                        vm.HasErrors = true;
                        vm.ErrorMessage = TempData["Error"].ToString();
                        TempData = null;
                    }
                }
                catch (Exception exception)
                { Logger.Error(exception, exception.Message); }
                
                vm.HeaderObj = HeaderFiller(GetLoggedInUserDetails());

                return View(vm);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return RedirectToRoute(Module.Web.RouteName.Collection.GenerateInvoice);
            }
        }


        [HttpPost]
        public ActionResult ValidateTCC(ValidateTCCVM userInput)
        {
            try
            {
                if (!string.IsNullOrEmpty(userInput.ApplicationNumber))
                {
                    return RedirectToRouteX(RouteName.TCCValidation.TCCDetails, new { applicationNumber = userInput.ApplicationNumber });
                }
                else { TempData.Add("Error", "Application number not specified."); return RedirectToAction(RouteName.TCCValidation.ValidateTCC); }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return RedirectToRouteX(Module.Web.RouteName.Collection.GenerateInvoice);
            }
        }


        [HttpGet]
        public ActionResult TCCDetails(string applicationNumber)
        {
            try
            {
                if (!string.IsNullOrEmpty(applicationNumber))
                {
                    if (_tccValidationHandler.ValidateApplicationNumberFormat(applicationNumber))
                    {
                        TCCRequestDetailVM vm = new TCCRequestDetailVM { };
                        vm = _tccValidationHandler.GetRequestDetail(applicationNumber);
                        vm.HeaderObj = HeaderFiller(GetLoggedInUserDetails());
                        return View(vm);
                    }
                    else { TempData.Add("Error", "Invalid Application Number Format"); return RedirectToRouteX(RouteName.TCCValidation.ValidateTCC); }
                }
                else { TempData.Add("Error", "Application Number not specified"); return RedirectToRouteX(RouteName.TCCValidation.ValidateTCC); }
            }
            catch(NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", "Record not found");
                return RedirectToRouteX(RouteName.TCCValidation.ValidateTCC);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return RedirectToRouteX(Module.Web.RouteName.Collection.GenerateInvoice);
            }
        }


        /// <summary>
        /// Download a particular using the filename, path and content type
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TCCAttachmentDownload(string fileName, string path, string contentType)
        {
            try
            {
                System.Web.HttpContext.Current.Response.ClearHeaders();
                System.Web.HttpContext.Current.Response.ContentType = contentType;
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName.Replace(" ", "_") + Path.GetExtension(path));
                System.Web.HttpContext.Current.Response.TransmitFile(path);
                System.Web.HttpContext.Current.Response.End();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return RedirectToRoute(Module.Web.RouteName.Collection.GenerateInvoice);
            }
            return null;
        }
    }
}