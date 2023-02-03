using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.UI.Notify;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
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
using System.Web.Mvc;


namespace Parkway.CBS.Client.Web.Controllers
{
    public class PAYEWithScheduleController : BaseController
    {
        private readonly IPAYEWithScheduleHandler _handler;
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;


        public PAYEWithScheduleController(IPAYEWithScheduleHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ICommonBaseHandler commonHandler) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, commonHandler)
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



        [CBSCollectionAuthorized]
        // GET: WithScheduleOption
        /// <summary>
        /// route name C.WithScheduleOption
        /// </summary>
        /// <returns></returns>
        public ActionResult WithScheduleOption()
        {
            string revenueHeadIdentifier = string.Empty;
            string taxCategory = string.Empty;
            string errorMessage = null;

            try
            {
                if (TempData.ContainsKey("Error"))
                {
                    errorMessage = TempData["Error"].ToString();
                }
            }
            catch (Exception exception) { Logger.Error(exception, "Error getting error value from temp data " + exception.Message); }
            TempData = null;

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
                InvoiceProceedVMForPayeAssessment viewModel = _handler.GetDirectAssessmentBillVM(processStage, userDetails.Entity);
                viewModel.InvoiceProceedVM.HeaderObj = HeaderFiller(userDetails);
                viewModel.InvoiceProceedVM.ErrorMessage = errorMessage;
                return View(viewModel);
            }
            #region catch region
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.usernotfound().ToString());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in SelectPAYEOption get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = ErrorLang.genericexception().ToString(); }
                TempData.Add("Error", errorMessage);
            }
            #endregion

            if (!string.IsNullOrEmpty(revenueHeadIdentifier)) { TempData.Add("RevenueHeadIdentifier", revenueHeadIdentifier); }
            if (!string.IsNullOrEmpty(taxCategory)) { TempData.Add("TaxCategory", taxCategory); }

            return RedirectToRouteX(Module.Web.RouteName.Collection.GenerateInvoice);
        }



        [CBSCollectionAuthorized]
        public ActionResult ShowScheduleResult()
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

                return View(_handler.GetResultsViewForPAYEAssessment(processStage, userDetails).ViewModel);
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.sessionended().ToString());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                TempData["Error"] = string.IsNullOrEmpty(errorMessage) ? ErrorLang.genericexception().ToString() : errorMessage;
            }
            return RedirectToRouteX(Module.Web.RouteName.Collection.GenerateInvoice);
        }



        [HttpPost]
        [CBSCollectionAuthorized]
        public ActionResult ShowScheduleResult(string placeholder)
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
                    Logger.Error(string.Format("Stage mismatch in ShowScheduleResult. Stage is sess: " + processStage.InvoiceGenerationStage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }
                _handler.DoProcessingScheduleResultConfirmation(processStage, userDetails.TaxPayerProfileVM.Id);
                Session.Add("InvoiceGenerationStage", JsonConvert.SerializeObject(processStage));
                return RedirectToRouteX(Module.Web.RouteName.InvoiceConfirmation.ConfirmInvoice);
            }
            #region catch region
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.usernotfound().ToString());
            }
            catch (NoRecordFoundException)
            {
                TempData.Add("Error", ErrorLang.norecord404().ToString());
                return RedirectToRouteX(RouteName.PAYEWithSchedule.WithScheduleOption);
            }
            catch (AmountTooSmallException)
            {
                TempData.Add("Error", ErrorLang.amounttoosmall().ToString());
                return RedirectToRouteX(RouteName.PAYEWithSchedule.WithScheduleOption);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in SelectPAYEOption get {0}", exception.Message));
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