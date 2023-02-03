using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.UI.Notify;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.Middleware.Filters;
using System;
using System.Collections.Generic;
using System.Web.Mvc;


namespace Parkway.CBS.Module.Web.Controllers
{
    public abstract class BaseInvoiceConfirmationController : BaseController
    {
        private readonly IModuleInvoiceConfirmationHandler _handler;

        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;


        public BaseInvoiceConfirmationController(IModuleInvoiceConfirmationHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ICommonBaseHandler commonHandler) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, commonHandler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _userService = userService;
            _membershipService = membershipService;
            _cbsUserService = cbsUserService;
            _authenticationService = authenticationService;
            _membershipValidationService = membershipValidationService;
            _userEventHandler = userEventHandler;
        }


        // GET: BaseInvoiceConfirmation
        public ActionResult ConfirmInvoice()
        {
            TempData = null;
            string revenueHeadIdentifier = string.Empty;
            string taxCategory = string.Empty;
            string errorMessage = string.Empty;

            try
            {
                UserDetailsModel userDetails = GetLoggedInUserDetails();
                GenerateInvoiceStepsModel processStage = GetDeserializedSessionObj(ref errorMessage);
                //check that this is the correct stage
                if (!IsStageCorrect(InvoiceGenerationStage.GenerateInvoice, processStage.InvoiceGenerationStage))
                {
                    Logger.Error(string.Format("Stage mismatch in ConfirmInvoice. Stage is sess: " + processStage.InvoiceGenerationStage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }

                revenueHeadIdentifier = processStage.RevenueHeadId.ToString();
                taxCategory = processStage.CategoryId.ToString();
                
                if (userDetails == null && userDetails.Entity == null)
                {
                    if (processStage.ProceedWithInvoiceGenerationVM != null)
                    {
                        if (processStage.ProceedWithInvoiceGenerationVM.FromTaxProfileSetup)
                        { userDetails.Entity = processStage.ProceedWithInvoiceGenerationVM.Entity; }
                    }
                }

                if (userDetails.Entity == null) { throw new AuthorizedUserNotFoundException(); }
                //get invoice details
                CreateInvoiceModel createInvoiceModel = _handler.GetCreateInvoiceModel(processStage, userDetails.Entity);

                if (createInvoiceModel.ExternalRedirect != null && createInvoiceModel.ExternalRedirect.Redirecting)
                {
                    {
                        if (Session["InvoiceGenerationStage"] != null) { Session.Remove("InvoiceGenerationStage"); }
                        Session.Add("ExternalRedirect", JsonConvert.SerializeObject(new ExternalRedirect { URL = createInvoiceModel.ExternalRedirect.URL, Stage = InvoiceGenerationStage.ExternalRedirect }));
                        return RedirectToRouteX(RouteName.Collection.ThirdPartyRedirect, new { url = createInvoiceModel.ExternalRedirect.URL });
                    }
                }

                createInvoiceModel.HeaderObj = HeaderFiller(userDetails);
                //Session.Add("InvoiceGenerationStage", JsonConvert.SerializeObject(processStage));
                return View(ViewName.InvoiceConfirmationViewPath, createInvoiceModel);
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.usernotfound().ToString());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in ConfirmInvoice {0}", exception.Message));
                Logger.Error(exception, exception.Message);
                TempData["Error"] = string.IsNullOrEmpty(errorMessage) ? ErrorLang.genericexception().ToString() : errorMessage;
            }
            return RedirectToRouteX(RouteName.Collection.GenerateInvoice);
        }



        [HttpPost]
        [BrowserHeaderFilter]
        public ActionResult ConfirmInvoice(string placeholder)
        {
            TempData = null;
            string revenueHeadIdentifier = string.Empty;
            string taxCategory = string.Empty;
            string errorMessage = string.Empty;

            try
            {
                UserDetailsModel userDetails = GetLoggedInUserDetails();
                GenerateInvoiceStepsModel processStage = GetDeserializedSessionObj(ref errorMessage);
                //check that this is the correct stage
                if (!IsStageCorrect(InvoiceGenerationStage.GenerateInvoice, processStage.InvoiceGenerationStage))
                {
                    Logger.Error(string.Format("Stage mismatch in ConfirmInvoice. Stage is sess: " + processStage.InvoiceGenerationStage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }
                revenueHeadIdentifier = processStage.RevenueHeadId.ToString();
                taxCategory = processStage.CategoryId.ToString();

                if (userDetails == null && userDetails.Entity == null)
                {
                    if (processStage.ProceedWithInvoiceGenerationVM != null)
                    {
                        if (processStage.ProceedWithInvoiceGenerationVM.FromTaxProfileSetup)
                        { userDetails.Entity = processStage.ProceedWithInvoiceGenerationVM.Entity; }
                    }
                }

                if (userDetails.Entity == null) { throw new AuthorizedUserNotFoundException(); }

                //get invoice details
                string invoiceNumber = _handler.TryGenerateInvoice(processStage, userDetails.Entity);
                Session.Remove("InvoiceGenerationStage");
                return RedirectToRouteX(RouteName.Collection.MakePayment, new { invoiceNumber });
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.usernotfound().ToString());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in ConfirmInvoice {0}", exception.Message));
                Logger.Error(exception, exception.Message);
                TempData["Error"] = string.IsNullOrEmpty(errorMessage) ? ErrorLang.genericexception().ToString() : errorMessage;
            }

            return RedirectToRouteX(RouteName.Collection.GenerateInvoice);
        }

    }
}