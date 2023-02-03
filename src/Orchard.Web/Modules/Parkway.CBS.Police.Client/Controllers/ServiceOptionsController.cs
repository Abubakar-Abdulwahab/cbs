using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Orchard.Logging;
using Orchard.Security;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using System.Linq;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class ServiceOptionsController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IServiceOptionHandler _handler;

        public ServiceOptionsController(IHandler compHandler, IAuthenticationService authenticationService, IServiceOptionHandler handler) : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
        }


        /// <summary>
        /// Select options for Service
        /// </summary>
        public ActionResult SelectOption()
        {
            string errorMessage = string.Empty;
            try
            {
                TempData = null;
                CheckStageSessionValue(ref errorMessage);

                PSSRequestStageModel processStage = GetDeserializedSessionObj(ref errorMessage);

                return View(GetViewModelForOptions(processStage));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in CharacterCertificateRequest get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session."; }
                TempData.Add("Error", errorMessage);
            }
            return RedirectToRoute("P.SelectService");
        }


        /// <summary>
        /// Get model for building the view for option select for police
        /// </summary>
        /// <param name="processStage"></param>
        /// <returns>PSServiceOptionsPageVM</returns>
        private PSServiceOptionsPageVM GetViewModelForOptions(PSSRequestStageModel processStage)
        {
            PSServiceOptionsPageVM viewModel = _handler.GetOptionsVM(processStage.ParentServiceOptionId == 0 ? processStage.ServiceId : processStage.ParentServiceOptionId);

            if (!viewModel.Options.Any()) { throw new Exception("No options found for this service " + processStage.ServiceId); }

            viewModel.HeaderObj = HeaderFiller(GetLoggedInUserDetails());
            viewModel.FlashObj = processStage.FlashObj ?? new FlashObj { };
            viewModel.HasMessage = processStage.FlashObj == null ? false : true;
            viewModel.ServiceName = processStage.ParentServiceName??processStage.ServiceName;
            viewModel.ServiceNote = processStage.ServiceNote;
            return viewModel;
        }



        [HttpPost]
        public ActionResult SelectOption(int? selectedOption)
        {
            string errorMessage = string.Empty;
            List<ErrorModel> errors = new List<ErrorModel>();
            try
            {
                TempData = null;
                CheckStageSessionValue(ref errorMessage);

                PSSRequestStageModel processStage = GetDeserializedSessionObj(ref errorMessage);

                PSServiceOptionsVM option = _handler.GetSelectedOption(processStage.ParentServiceOptionId == 0 ? processStage.ServiceId : processStage.ParentServiceOptionId, selectedOption);
                if(option == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = PoliceErrorLang.selected_option_404().ToString(), FieldName = "selectedOption" });
                    return View(GetOptionsPostBackModel(processStage, errors));
                }

                RouteNameAndStage routeAndStage = _handler.GetNextOptionDirection(option);

                processStage.ParentServiceOptionId = processStage.ParentServiceOptionId == 0 ? processStage.ServiceId : processStage.ParentServiceOptionId;
                processStage.ParentServiceName = processStage.ParentServiceName ?? processStage.ServiceName;

                processStage.ServiceId = option.ServiceOptionId;
                processStage.OptionType = option.OptionType;
                processStage.Stage = routeAndStage.Stage;
                processStage.ServiceName = option.Name;
                Session.Add("PSSRequestStage", JsonConvert.SerializeObject(processStage));

                return RedirectToRoute(routeAndStage.RouteName);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in CharacterCertificateRequest get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session."; }
                TempData.Add("Error", errorMessage);
            }
            return RedirectToRoute("P.SelectService");
        }



        /// <summary>
        /// Load up the view model and return the VM for building the view for police
        /// character ceritifcate view
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="errors"></param>
        /// <returns>PSServiceOptionsPageVM</returns>
        private PSServiceOptionsPageVM GetOptionsPostBackModel(PSSRequestStageModel processStage, List<ErrorModel> errors)
        {
            foreach (var error in errors)
            {
                this.ModelState.AddModelError(error.FieldName, error.ErrorMessage);
            }
            return GetViewModelForOptions(processStage);
        }

    }
}