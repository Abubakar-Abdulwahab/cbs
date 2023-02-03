using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Orchard.Logging;
using Orchard.Security;
using System.Globalization;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class PSSEscortController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IPSSEscortHandler _handler;
        private readonly IHandler _compHandler;
        private int minimumRequiredOfficers;

        public PSSEscortController(IHandler compHandler, IAuthenticationService authenticationService, IPSSEscortHandler handler)
            : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
        }


        /// <summary>
        /// Route Name: P.ExtractRequest
        /// </summary>
        public ActionResult EscortRequest()
        {
            string errorMessage = string.Empty;
            string categoryType = string.Empty;
            EscortRequestVM viewModel = new EscortRequestVM();
            try
            {
                TempData = null;
                CheckStageSessionValue(ref errorMessage);

                PSSRequestStageModel processStage = GetDeserializedSessionObj(ref errorMessage);

                if (!IsStageCorrect(PSSUserRequestGenerationStage.PSSRequest, processStage.Stage))
                {
                    Logger.Error(string.Format("Stage mismatch in ExtractRequest. Stage is sess: " + processStage.Stage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }
                viewModel = _handler.GetVMForPoliceEscort(processStage.ServiceId);
                UserDetailsModel userDetails = GetLoggedInUserDetails();

                viewModel.HeaderObj = HeaderFiller(userDetails);
                viewModel.SubSubCategoryId = processStage.SubSubCategoryId;
                viewModel.FlashObj = processStage.FlashObj ?? new FlashObj { };
                viewModel.HasMessage = processStage.FlashObj == null ? false : true;
                viewModel.ServiceNote = processStage.ServiceNote;
                viewModel.ServiceName = processStage.ServiceName;
                return View(viewModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in ExtractRequest get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session."; }
                TempData = null;
                TempData.Add("Error", errorMessage);
            }
            return RedirectToRoute("P.SelectService");
        }


        [HttpPost]
        /// <summary>
        /// Route Name: P.ExtractRequest
        /// </summary>
        public ActionResult EscortRequest(EscortRequestVM userInput)
        {
            string errorMessage = string.Empty;
            UserDetailsModel userDetails = GetLoggedInUserDetails();
            short formErrorNumber = 1;
            PSSRequestStageModel processStage = null;
            List<ErrorModel> errors = new List<ErrorModel> { };

            try
            {
                TempData = null;
                CheckStageSessionValue(ref errorMessage);

                processStage = GetDeserializedSessionObj(ref errorMessage);

                if (!IsStageCorrect(PSSUserRequestGenerationStage.PSSRequest, processStage.Stage))
                {
                    Logger.Error(string.Format("Stage mismatch in ExtractRequest. Stage is sess: " + processStage.Stage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }

                //remove errors
                foreach (var item in this.ModelState)
                {
                    this.ModelState[item.Key].Errors.Clear();
                }
                //do validation
                //service category, category type, additional field details validation
                int lastErrorsCount = 0;

                validateServiceCategoryDetails(userInput, ref errors);

                //state, LGA validation for service delivery
                if (userInput.SelectedState <= 0) { errors.Add(new ErrorModel { FieldName = "SelectedState", ErrorMessage = "State is required." }); }
                if (userInput.SelectedStateLGA <= 0) { errors.Add(new ErrorModel { FieldName = "SelectedStateLGA", ErrorMessage = "State LGA is required." }); }

                CommandVM command = null;
                DateTime startDate = new DateTime { };
                DateTime endDate = new DateTime { };
                int escortDuration = 0;
                try
                {
                    if (userInput.SelectedState > 0)
                    { command = _handler.ValidateSelectedCommand(userInput, ref errors); }
                }
                catch (Exception) { }

                if (string.IsNullOrEmpty(userInput.Address))
                {
                    errors.Add(new ErrorModel { FieldName = "Address", ErrorMessage = "Address field is required" });
                }
                else
                {
                    userInput.Address = userInput.Address.Trim();
                    if (userInput.Address.Length > 100 || userInput.Address.Length < 5)
                    { errors.Add(new ErrorModel { FieldName = "Address", ErrorMessage = "Address field must be between 5 to 100 characters long." }); }
                }

                if (string.IsNullOrEmpty(userInput.StartDate))
                {
                    errors.Add(new ErrorModel { FieldName = "StartDate", ErrorMessage = "Start date field is required." });
                }

                if (string.IsNullOrEmpty(userInput.EndDate))
                {
                    errors.Add(new ErrorModel { FieldName = "EndDate", ErrorMessage = "End date field is required." });
                }

                //
                if(!string.IsNullOrEmpty(userInput.StartDate) && !string.IsNullOrEmpty(userInput.EndDate))
                {
                   
                    try
                    {
                        userInput.StartDate = userInput.StartDate.Trim();
                        userInput.EndDate = userInput.EndDate.Trim();

                        startDate = DateTime.ParseExact(userInput.StartDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(userInput.EndDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        if (startDate > endDate)
                        {
                            errors.Add(new ErrorModel { FieldName = "StartDate", ErrorMessage = "Please input a valid date. Start date cannot be ahead of the end date." });
                            errors.Add(new ErrorModel { FieldName = "EndDate", ErrorMessage = "Please input a valid date. Start date cannot be ahead of the end date." });
                            throw new DirtyFormDataException { };
                        }

                        if (startDate <= DateTime.Now.ToLocalTime())
                        {
                            errors.Add(new ErrorModel { FieldName = "StartDate", ErrorMessage = "Please input a valid date. Start date must be ahead of today." });
                        }

                        escortDuration = (endDate - startDate).Days + 1; //1 is added because both start and end date are inclusive i.e 02/01/2020 - 01/01/2020 difference will be 1 without an addition of inclusive 1 day
                    }
                    catch (Exception)
                    {
                        errors.Add(new ErrorModel { FieldName = "StartDate", ErrorMessage = "Please input a valid date. Expected date format dd/MM/yyyy i.e. 31/09/2020." });
                        errors.Add(new ErrorModel { FieldName = "EndDate", ErrorMessage = "Please input a valid date. Expected date format dd/MM/yyyy i.e. 31/09/2020." });
                    }                   
                }

                if (errors.Count > 0)
                {
                    formErrorNumber = 1;
                    lastErrorsCount = errors.Count;
                }

                //form 2
                //formErrorNumber = 2;

                if (userInput.NumberOfOfficers < minimumRequiredOfficers)
                { errors.Add(new ErrorModel { FieldName = "NumberOfOfficers", ErrorMessage = $"Please provide a valid number of police officers. Minimum is {minimumRequiredOfficers}" }); }

                //if (userInput.PSBillingType == PSBillingType.None)
                //{ errors.Add(new ErrorModel { FieldName = "PSBillingType", ErrorMessage = "Please select a prefered payment method" }); }

                if (errors.Count > 0)
                {
                    if (lastErrorsCount == 0) formErrorNumber = 2;
                    throw new DirtyFormDataException { };
                }

                EscortRequestVM pssRequestTokenObj = new EscortRequestVM { SelectedStateLGA = userInput.SelectedStateLGA, SelectedState = userInput.SelectedState, SelectedCommand = command.Id, Reason = userInput.Reason, LGAName = command.LGAName, StateName = command.StateName, CommandName = command.Name, CommandAddress = command.Address, ParsedStartDate = startDate, ParsedEndDate = endDate, Address = userInput.Address, NumberOfOfficers = userInput.NumberOfOfficers, PSBillingType = userInput.PSBillingType, SelectedOriginState = userInput.SelectedOriginState, SelectedOriginLGA = userInput.SelectedOriginLGA, AddressOfOriginLocation = userInput.AddressOfOriginLocation, OriginStateName = userInput.OriginStateName, OriginLGAName = userInput.OriginLGAName, ShowExtraFieldsForServiceCategoryType = userInput.ShowExtraFieldsForServiceCategoryType, SelectedEscortServiceCategories = userInput.SelectedEscortServiceCategories, DurationNumber = escortDuration, HasDifferentialWorkFlow = processStage.HasDifferentialWorkFlow, SelectedCommandType = userInput.SelectedCommandType };

                string pssEscortToken = Util.LetsEncrypt(JsonConvert.SerializeObject(pssRequestTokenObj));

                processStage.Token = pssEscortToken;

                dynamic routeAndStage = _handler.GetNextDirectionForConfirmation();

                processStage.Stage = routeAndStage.Stage;

                Session.Add("PSSRequestStage", JsonConvert.SerializeObject(processStage));

                return RedirectToRoute(routeAndStage.RouteName);
            }
            catch (DirtyFormDataException)
            {
                foreach (var item in this.ModelState.Keys)
                {
                    var modelState = this.ModelState[item];
                    if (modelState.Errors.Count > 1 && modelState.Value != null)
                    {
                        modelState.Errors.RemoveAt(0);
                    }
                }

                foreach (var error in errors)
                {
                    this.ModelState.AddModelError(error.FieldName, error.ErrorMessage);
                }

                EscortRequestVM vm = _handler.GetVMForPoliceEscort(processStage.ServiceId);
                if (userInput.SelectedState > 0)
                {
                    var state = vm.StateLGAs.Where(s => s.Id == userInput.SelectedState).SingleOrDefault();
                    userInput.ListLGAs = state != null ? state.LGAs.ToList() : null;
                    userInput.StateLGAs = vm.StateLGAs;
                }
                else
                {
                    userInput.StateLGAs = vm.StateLGAs;
                }
                userInput.HeaderObj = HeaderFiller(userDetails);
                userInput.FormErrorNumber = formErrorNumber;
                userInput.SelectedReason = (userInput.SelectedReason != 0) ? userInput.SelectedReason : 0;
                userInput.Reasons = vm.Reasons;
                userInput.ServiceNote = processStage.ServiceNote;
                userInput.ServiceName = processStage.ServiceName;
                userInput.ViewedTermsAndConditionsModal = true;
                userInput.EscortServiceCategories = vm.EscortServiceCategories;
                userInput.EscortCategoryTypes = (userInput.SelectedEscortServiceCategories.ElementAtOrDefault(0) > 0) ? _handler.GetCategoryTypesForServiceCategoryWithId(userInput.SelectedEscortServiceCategories.ElementAtOrDefault(0)) : null;
                userInput.OriginLGAs = (userInput.SelectedOriginState > 0) ? vm.StateLGAs.Where(s => s.Id == userInput.SelectedOriginState).SingleOrDefault().LGAs.ToList() : null;
                userInput.CommandTypes = vm.CommandTypes;
                userInput.TacticalSquads = (userInput.SelectedCommandType > 0) ? _handler.GetCommandsForCommandTypeWithId(userInput.SelectedCommandType) : null;
                CommandVM tacticalSquad = _handler.GetCommandDetails(userInput.SelectedTacticalSquad);
                userInput.Formations = (tacticalSquad != null) ? _handler.GetNextLevelCommandsWithCode(tacticalSquad.Code) : (userInput.SelectedOriginState > 0) ? _handler.GetCommandsByState(userInput.SelectedOriginState) : _handler.GetCommandsByState(userInput.SelectedState);
                userInput.SelectedFormationName = (userInput.SelectedCommand > 0) ? _handler.GetCommandDetails(userInput.SelectedCommand).Name : string.Empty;
                return View(userInput);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in ExtractRequest get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = ErrorLang.genericexception().ToString(); }
                TempData.Add("Error", errorMessage);
            }
            return RedirectToRoute("P.SelectService");
        }

        public virtual JsonResult CalculateEstimate(int officerQty, string startDate, string endDate, int stateId, int lgaId, int subSubTaxCategoryId)
        {

            try
            {
                return Json(_compHandler.GetEscortBillEstimate(officerQty, startDate, endDate, stateId, lgaId, subSubTaxCategoryId));
            }
            catch (Exception excep)
            {
                Logger.Debug(excep, excep.Message);
                return Json(new APIResponse { Error = true });
            }
        }


        /// <summary>
        /// Validates service category, category type and extra fields such as origin state, origin LGA
        /// </summary>
        /// <param name="userInput"></param>
        private void validateServiceCategoryDetails(EscortRequestVM userInput, ref List<ErrorModel> errors)
        {
            PSSEscortServiceCategoryVM serviceCategory = _handler.GetEscortServiceCategoryWithId(userInput.SelectedEscortServiceCategories.ElementAtOrDefault(0));
            if (serviceCategory == null)
            {
                errors.Add(new ErrorModel { FieldName = "SelectedEscortServiceCategory", ErrorMessage = "Selected Service Category value is not valid" });
            }

            PSSEscortServiceCategoryVM categoryType = null;
            if (userInput.SelectedEscortServiceCategories.ElementAtOrDefault(1) > 0 && serviceCategory != null)
            {
                if (!_handler.ValidateCategoryTypeForServiceCategory(userInput.SelectedEscortServiceCategories.ElementAtOrDefault(0), userInput.SelectedEscortServiceCategories.ElementAtOrDefault(1)))
                {
                    errors.Add(new ErrorModel { FieldName = "SelectedEscortServiceCategoryType", ErrorMessage = "Selected Service Category Type value is not valid" });
                }
                else
                {
                    categoryType = _handler.GetEscortServiceCategoryWithId(userInput.SelectedEscortServiceCategories.ElementAtOrDefault(1));
                    minimumRequiredOfficers = categoryType.MinimumRequiredOfficers;
                }
            }
            else
            {
                //getting for children category type for service category
                if(serviceCategory != null)
                {
                    IEnumerable<PSSEscortServiceCategoryVM> categoryTypes = _handler.GetCategoryTypesForServiceCategoryWithId(userInput.SelectedEscortServiceCategories.ElementAtOrDefault(0));
                    if (categoryTypes.Count() > 0)
                    {
                        errors.Add(new ErrorModel { FieldName = "SelectedEscortServiceCategoryType", ErrorMessage = "Selected Service Category Type value is not valid" });
                    }
                    minimumRequiredOfficers = serviceCategory.MinimumRequiredOfficers;
                }
            }

            //extra form fields validation for category types that have extra form fields
            if (categoryType != null && categoryType.ShowExtraFields)
            {
                userInput.ShowExtraFieldsForServiceCategoryType = categoryType.ShowExtraFields;
                if (userInput.SelectedOriginState < 1) { errors.Add(new ErrorModel { FieldName = "SelectedOriginState", ErrorMessage = "Origin State is required." }); }
                if (userInput.SelectedOriginLGA < 1) { errors.Add(new ErrorModel { FieldName = "SelectedOriginLGA", ErrorMessage = "Origin LGA is required." }); }

                if(userInput.SelectedOriginLGA > 0)
                {
                    var originLGA = _handler.GetLGAWithId(userInput.SelectedOriginLGA);

                    if (originLGA == null)
                    {
                        errors.Add(new ErrorModel { FieldName = "SelectedOriginLGA", ErrorMessage = "Origin LGA value is not valid." });
                    }
                    else
                    {
                        if (originLGA.StateId != userInput.SelectedOriginState)
                        {
                            errors.Add(new ErrorModel { FieldName = "SelectedOriginState", ErrorMessage = "Origin state value is not valid." });
                        }
                        else
                        {
                            userInput.OriginStateName = originLGA.StateName;
                            userInput.OriginLGAName = originLGA.Name;
                        }
                    }
                }

                //validate extra input
                if (string.IsNullOrEmpty(userInput.AddressOfOriginLocation))
                {
                    errors.Add(new ErrorModel { FieldName = "AddressOfOriginLocation", ErrorMessage = "Address Of Origin Location field is required" });
                }
                else
                {
                    userInput.AddressOfOriginLocation = userInput.AddressOfOriginLocation.Trim();
                    if (userInput.AddressOfOriginLocation.Length > 100 || userInput.AddressOfOriginLocation.Length < 5)
                    { errors.Add(new ErrorModel { FieldName = "AddressOfOriginLocation", ErrorMessage = "Address Of Origin Location field must be between 5 to 100 characters long." }); }
                }
            }
        }

    }
}