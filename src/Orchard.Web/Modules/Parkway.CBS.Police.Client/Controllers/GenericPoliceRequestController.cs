using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Lang;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class GenericPoliceRequestController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IGenericRequestHandler _handler;

        public GenericPoliceRequestController(IHandler compHandler, IAuthenticationService authenticationService, IGenericRequestHandler handler)
            : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService; 
            _handler = handler;
        }


        /// <summary>
        /// Route Name: P.Generic.Police.Request
        /// </summary>
        public ActionResult GenericRequest()
        {
            string errorMessage = string.Empty;
            string categoryType = string.Empty;
            GenericPoliceRequest viewModel = new GenericPoliceRequest();

            try
            {
                TempData = null;
                CheckStageSessionValue(ref errorMessage);

                PSSRequestStageModel processStage = GetDeserializedSessionObj(ref errorMessage);

                if (!IsStageCorrect(PSSUserRequestGenerationStage.PSSRequest, processStage.Stage))
                {
                    Logger.Information(string.Format("Stage mismatch in GenericRequest. Stage is sess: " + processStage.Stage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }

                viewModel = _handler.GetVMForGenericPoliceRequest(processStage.ServiceId, processStage.CategoryId);

                UserDetailsModel userDetails = GetLoggedInUserDetails();
                viewModel.HeaderObj = HeaderFiller(userDetails);

                viewModel.FlashObj = processStage.FlashObj ?? new FlashObj { };
                viewModel.HasMessage = processStage.FlashObj == null ? false : true;
                viewModel.ServiceNote = processStage.ServiceNote;

                return View(viewModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in GenericRequest get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session."; }
                TempData.Add("Error", errorMessage);
            }
            return RedirectToRoute("P.SelectService");
        }


        [HttpPost]
        /// <summary>
        /// Route Name: P.ExtractRequest
        /// </summary>
        public ActionResult GenericRequest(GenericPoliceRequest userInput, ICollection<FormControlViewModel> controlCollectionFromUserInput)
        {
            string errorMessage = string.Empty;
            UserDetailsModel userDetails = GetLoggedInUserDetails();
            PSSRequestStageModel processStage = null;
            GenericPoliceRequest serverSideVM = null;

            try
            {
                TempData = null;
                CheckStageSessionValue(ref errorMessage);

                processStage = GetDeserializedSessionObj(ref errorMessage);

                if (!IsStageCorrect(PSSUserRequestGenerationStage.PSSRequest, processStage.Stage))
                {
                    Logger.Information(string.Format("Stage mismatch in PSSRequest. Stage is sess: " + processStage.Stage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }

                //get the request details such as the form fields from the DB
                serverSideVM = _handler.GetVMForGenericPoliceRequest(processStage.ServiceId, processStage.CategoryId);
                //do the validation here
                _handler.GetVMForGenericPoliceRequest(this, serverSideVM.Forms, controlCollectionFromUserInput);

                dynamic routeAndStage = _handler.GetNextDirectionForConfirmation();
                processStage.Stage = routeAndStage.Stage;
                //lets remove the things we dont need to reduce the string size
                IEnumerable<UserFormDetails> formReduce = serverSideVM.Forms.Select(rd => new UserFormDetails { FriendlyName = rd.FriendlyName, ControlIdentifier = rd.ControlIdentifier, RevenueHeadId = rd.RevenueHeadId, FormValue = rd.FormValue } );
                processStage.Token = Util.LetsEncrypt(JsonConvert.SerializeObject(formReduce));
                Session.Add("PSSRequestStage", JsonConvert.SerializeObject(processStage));

                return RedirectToRoute(routeAndStage.RouteName);
            }
            catch (DirtyFormDataException)
            {
                List<string> errorArray = new List<string>();
                foreach (var item in this.ModelState.Keys)
                {
                    var modelState = this.ModelState[item];
                    if (modelState.Errors.Count > 0 && modelState.Value != null) { errorArray.Add(modelState.Errors.FirstOrDefault().ErrorMessage); }
                }
                serverSideVM.HeaderObj = HeaderFiller(userDetails);
                serverSideVM.ServiceNote = processStage.ServiceNote;
                serverSideVM.ViewedTermsAndConditionsModal = true;
                return View(serverSideVM);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in GenericRequest get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = ErrorLang.genericexception().ToString(); }
                TempData.Add("Error", errorMessage);
            }
            return RedirectToRoute("P.SelectService");
        }


    }
}