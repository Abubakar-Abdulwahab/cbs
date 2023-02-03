using System;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;


namespace Parkway.CBS.Police.Client.Controllers
{
    public class RequestConfirmationController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IPSSRequestConfirmationHandler _handler;
        private readonly IHandler _compHandler;

        public RequestConfirmationController(IHandler compHandler, IAuthenticationService authenticationService, IPSSRequestConfirmationHandler handler)
            : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
        }



        /// <summary>
        /// Route Name : P.Request.Confirm
        /// Handles confirmation of PSS extract
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfirmPSSRequest()
        {
            string errorMessage = string.Empty;
            try
            {
                TempData = null;
                CheckStageSessionValue(ref errorMessage);

                PSSRequestStageModel processStage = GetDeserializedSessionObj(ref errorMessage);

                if (!IsStageCorrect(PSSUserRequestGenerationStage.PSSRequestConfirmation, processStage.Stage))
                {
                    Logger.Information(string.Format("Stage mismatch in ExtractRequest. Stage is sess: " + processStage.Stage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }
                string sRequestFormDump = Util.LetsDecrypt(processStage.Token);

                RequestConfirmationVM viewModel = _handler.GetVMForRequestConfirmationPage(processStage.ServiceId, processStage.ServiceType, sRequestFormDump);
                UserDetailsModel userDetails = GetLoggedInUserDetails();
                viewModel.HeaderObj = HeaderFiller(userDetails);

                if(userDetails == null)
                {
                    userDetails = GetUserDetails(processStage.CBSUserProfileId);
                }

                viewModel.Address = (userDetails.CBSUserVM.IsAdministrator) ? userDetails.TaxPayerProfileVM.Address : _handler.GetCBSUserLocation(userDetails.CBSUserVM.Id).Address;
                viewModel.Name = userDetails.CBSUserVM.Name;
                viewModel.FlashObj = processStage.FlashObj ?? new FlashObj { };
                viewModel.HasMessage = processStage.FlashObj == null ? false : true;
                if (!string.IsNullOrEmpty(viewModel.CustomViewName)) { return View(viewModel.CustomViewName, viewModel); }
                return View(viewModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in ExtractRequest get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = ErrorLang.genericexception().ToString(); }
                TempData.Add("Error", errorMessage);
            }
            return RedirectToRoute("P.SelectService");
        }


        /// <summary>
        /// Confirm request
        /// </summary>
        [HttpPost]
        public ActionResult ConfirmPSSRequest(string holder)
        {
            string errorMessage = string.Empty;
            try
            {
                TempData = null;
                CheckStageSessionValue(ref errorMessage);

                PSSRequestStageModel processStage = GetDeserializedSessionObj(ref errorMessage);

                if (!IsStageCorrect(PSSUserRequestGenerationStage.PSSRequestConfirmation, processStage.Stage))
                {
                    Logger.Information(string.Format("Stage mismatch in ExtractRequest. Stage is sess: " + processStage.Stage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }
                string sRequestFormDump = Util.LetsDecrypt(processStage.Token);
                UserDetailsModel userDetails = GetLoggedInUserDetails();
                if (userDetails == null)
                {
                    userDetails = GetUserDetails(processStage.CBSUserProfileId);
                }
                InvoiceGenerationResponse response = _handler.SaveRequestDetails(processStage, sRequestFormDump, userDetails.TaxPayerProfileVM);
                if (Session["PSSRequestStage"] != null)
                {
                    try
                    {
                        Session.Remove("PSSRequestStage");
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, string.Format("Exception getting removing PSSRequestStage from session value {0}", exception.Message));
                    }
                }
                return RedirectToRoute("P.Make.Payment", new { invoiceNumber = response.InvoiceNumber });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in ExtractRequest get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = ErrorLang.genericexception().ToString(); }
                TempData.Add("Error", errorMessage);
            }
            return RedirectToRoute("P.SelectService");
        }


    }
}