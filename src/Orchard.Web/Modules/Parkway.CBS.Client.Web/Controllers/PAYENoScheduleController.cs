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
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.Middleware.Filters;
using System;
using System.Collections.Generic;
using System.Web.Mvc;


namespace Parkway.CBS.Client.Web.Controllers
{
    public class PAYENoScheduleController : BaseController
    {
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;


        public PAYENoScheduleController(IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ICommonBaseHandler commonHandler) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, commonHandler)
        {
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _userService = userService;
            _membershipService = membershipService;
            _cbsUserService = cbsUserService;
            _authenticationService = authenticationService;
            _membershipValidationService = membershipValidationService;
            _userEventHandler = userEventHandler;
        }


        [CBSCollectionAuthorized]
        public ActionResult NoScheduleOption()
        {
            TempData = null;
            string revenueHeadIdentifier = string.Empty;
            string taxCategory = string.Empty;
            string errorMessage = null;

            try
            {
                //check if the user is logged in
                UserDetailsModel userDetails = GetLoggedInUserDetails();
                if (userDetails == null || userDetails.Entity == null) { throw new AuthorizedUserNotFoundException { }; }

                GenerateInvoiceStepsModel processStage = GetDeserializedSessionObj(ref errorMessage);
                revenueHeadIdentifier = processStage.RevenueHeadId.ToString();
                taxCategory = processStage.CategoryId.ToString();

                if (!IsStageCorrect(InvoiceGenerationStage.PAYEProcess, processStage.InvoiceGenerationStage))
                {
                    Logger.Error(string.Format("Stage mismatch in NoScheduleOption. Stage is sess: " + processStage.InvoiceGenerationStage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }

                return View(new PAYENoSchedule { SAmount = 00.00m.ToString(), HeaderObj = HeaderFiller(userDetails) });
            }
            #region catch region
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.usernotfound().ToString());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in NoScheduleOption get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = ErrorLang.genericexception().ToString(); }
                TempData.Add("Error", errorMessage);
            }
            #endregion

            if (!string.IsNullOrEmpty(revenueHeadIdentifier)) { TempData.Add("RevenueHeadIdentifier", revenueHeadIdentifier); }
            if (!string.IsNullOrEmpty(taxCategory)) { TempData.Add("TaxCategory", taxCategory); }

            return RedirectToRouteX(Module.Web.RouteName.Collection.GenerateInvoice);
        }



        [HttpPost]
        [CBSCollectionAuthorized]
        public ActionResult NoScheduleOption(PAYENoSchedule model)
        {
            TempData = null;
            string revenueHeadIdentifier = string.Empty;
            string taxCategory = string.Empty;
            string errorMessage = null;
            UserDetailsModel userDetails = null;

            try
            {
                //check if the user is logged in
                userDetails = GetLoggedInUserDetails();
                if (userDetails == null || userDetails.Entity == null) { throw new AuthorizedUserNotFoundException { }; }

                GenerateInvoiceStepsModel processStage = GetDeserializedSessionObj(ref errorMessage);
                revenueHeadIdentifier = processStage.RevenueHeadId.ToString();
                taxCategory = processStage.CategoryId.ToString();
                if (!IsStageCorrect(InvoiceGenerationStage.PAYEProcess, processStage.InvoiceGenerationStage))
                {
                    Logger.Error(string.Format("Stage mismatch in NoScheduleOption. Stage is sess: " + processStage.InvoiceGenerationStage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }
                decimal amountVal = 00.00m;
                if(!decimal.TryParse(model.SAmount.Replace(",", ""), out amountVal))
                {
                    AddValidationErrorsToCallback(this, new List<ErrorModel>{ { new ErrorModel { ErrorMessage = ErrorLang.addvalidamount().ToString(), FieldName = "SAmount" } } });
                }
                if(amountVal <= 0.00m)
                {
                    AddValidationErrorsToCallback(this, new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.amounttoosmall().ToString(), FieldName = "SAmount" } } });
                }
                //
                processStage.InvoiceConfirmedModel = new InvoiceConfirmedModel { Amount = amountVal, Token = Util.LetsEncrypt(JsonConvert.SerializeObject(new FileProcessModel { NoSchedulePresent = true }), AppSettingsConfigurations.EncryptionSecret) };
                processStage.InvoiceGenerationStage = InvoiceGenerationStage.GenerateInvoice;
                Session.Add("InvoiceGenerationStage", JsonConvert.SerializeObject(processStage));

                return RedirectToRouteX(Module.Web.RouteName.InvoiceConfirmation.ConfirmInvoice);
            }
            #region catch region
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.usernotfound().ToString());
            }
            catch (DirtyFormDataException)
            {
                model.HeaderObj = HeaderFiller(userDetails);
                return View(model);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in NoScheduleOption get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = ErrorLang.genericexception().ToString(); }
                TempData.Add("Error", errorMessage);
            }
            #endregion

            if (!string.IsNullOrEmpty(revenueHeadIdentifier)) { TempData.Add("RevenueHeadIdentifier", revenueHeadIdentifier); }
            if (!string.IsNullOrEmpty(taxCategory)) { TempData.Add("TaxCategory", taxCategory); }

            return RedirectToRouteX(Module.Web.RouteName.Collection.GenerateInvoice);
        }



    }
}