using Orchard;
using Orchard.Logging;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class PSSCharacterCertificateHandler : IPSSCharacterCertificateHandler
    {
        private readonly ICoreStateAndLGA _coreStateLGAService;
        private readonly Lazy<ICoreCommand> _coreCommandService;
        private readonly ICoreServiceStateCommand _coreServiceStateCommand;
        private readonly ICoreCharacterCertificateReasonForInquiry _coreCharacterCertificateReasonForInquiryService;
        private readonly ICoreCountryService _coreCountryService;
        private readonly IPSSCharacterCertificateRequestTypeManager<PSSCharacterCertificateRequestType> _requestTypeManager;
        private readonly IPSServiceCaveatManager<PSServiceCaveat> _caveatRepo;
        private readonly IOrchardServices _orchardServices;

        public ILogger Logger { get; set; }

        public PSSCharacterCertificateHandler(ICoreStateAndLGA coreStateLGAService, Lazy<ICoreCommand> coreCommandService, ICoreCharacterCertificateReasonForInquiry coreCharacterCertificateReasonForInquiryService, ICoreCountryService coreCountryService, IPSSCharacterCertificateRequestTypeManager<PSSCharacterCertificateRequestType> requestTypeManager, IPSServiceCaveatManager<PSServiceCaveat> caveatRepo, IOrchardServices orchardServices, ICoreServiceStateCommand coreServiceStateCommand)
        {
            _coreStateLGAService = coreStateLGAService;
            _coreCommandService = coreCommandService;
            _coreCharacterCertificateReasonForInquiryService = coreCharacterCertificateReasonForInquiryService;
            _coreCountryService = coreCountryService;
            Logger = NullLogger.Instance;
            _requestTypeManager = requestTypeManager;
            _caveatRepo = caveatRepo;
            _orchardServices = orchardServices;
            _coreServiceStateCommand = coreServiceStateCommand;
        }

        /// <summary>
        /// Get character certificate request VM
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public CharacterCertificateRequestVM GetVMForCharacterCertificate(int serviceId)
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            PSServiceCaveatVM caveat = ObjectCacheProvider.GetCachedObject<PSServiceCaveatVM>(tenant, $"{nameof(POSSAPCachePrefix.Caveat)}-{serviceId}");

            if (caveat == null)
            {
                caveat = _caveatRepo.GetServiceCaveat(serviceId);

                if (caveat != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.Caveat)}-{serviceId}", caveat);
                }
            }

            return new CharacterCertificateRequestVM
            {
                HeaderObj = new HeaderObj { },
                StateLGAs = _coreStateLGAService.GetStates(),
                CharacterCertificateReasonsForInquiry = _coreCharacterCertificateReasonForInquiryService.GetReasonsForInquiry(),
                Countries = _coreCountryService.GetCountries(),
                RequestTypes = _requestTypeManager.GetRequestTypes(),
                Caveat = caveat
            };
        }


        /// <summary>
        /// Validate  and get the command details
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="selectedState"></param>
        /// <param name="selectedStateLGA"></param>
        /// <param name="selectedCommand"></param>
        /// <returns>CommandVM</returns>
        public CommandVM ValidateSelectedCommand(PSSCharacterCertificateController callback, int selectedState, int selectedStateLGA, int selectedCommand, ref List<ErrorModel> errors)
        {
            CommandVM command = _coreCommandService.Value.GetCommandDetails(selectedCommand);
            if (command == null || command.StateId != selectedState || command.LGAId != selectedStateLGA)
            {
                errors.Add(new ErrorModel { FieldName = "Command", ErrorMessage = "Select a valid Command." });
            }
            return command;
        }


        /// <summary>
        /// Validate that the selected state, selected command and service Id match
        /// that would provide the capture location
        /// </summary>
        /// <param name="selectedState"></param>
        /// <param name="selectedCommand"></param>
        /// <param name="serviceId"></param>
        /// <param name="errors"></param>
        /// <returns>CommandVM</returns>
        public CommandVM GetSelectedCommand(int selectedState, int selectedCommand, int serviceId, ref List<ErrorModel> errors)
        {
            CommandVM command = null;
            if (selectedCommand > 0 && selectedState > 0 && serviceId > 0)
                command = _coreServiceStateCommand.GetActiveCommand(selectedState, selectedCommand, serviceId);

            if (command == null)
            {
                errors.Add(new ErrorModel { FieldName = "SelectedCommand", ErrorMessage = PoliceErrorLang.selected_command_404().ToString() });
            }
            return command;
        }


        /// <summary>
        /// Validates character certificate request vm
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        public void ValidateCharacterCertificateRequest(CharacterCertificateRequestVM userInput, out List<ErrorModel> errors)
        {
            errors = new List<ErrorModel>();
            try
            {
                bool isNigeria = _coreCountryService.checkIfCountryIsNigeria(userInput.SelectedCountryOfOrigin);
                if (!(userInput.SelectedState > 0)) { errors.Add(new ErrorModel { FieldName = nameof(userInput.SelectedState), ErrorMessage = "Selected state value is not valid" }); }
                if (!(userInput.SelectedStateOfOrigin > 0) && isNigeria) { errors.Add(new ErrorModel { FieldName = nameof(userInput.SelectedStateOfOrigin), ErrorMessage = "Selected state of origin value is not valid" }); }

                if (!_coreCountryService.ValidateCountry(userInput.SelectedCountryOfOrigin))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Selected country of origin is not valid", FieldName = nameof(CharacterCertificateRequestVM.SelectedCountryOfOrigin) });
                }

                var certificateRequestType = _requestTypeManager.GetRequestType(userInput.RequestType);
                if (certificateRequestType == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Selected request type is not valid", FieldName = nameof(CharacterCertificateRequestVM.RequestType) });
                }

                var certificateReasonForInquiry = _coreCharacterCertificateReasonForInquiryService.GetReasonForInquiry(userInput.CharacterCertificateReasonForInquiry).SingleOrDefault();
                if (certificateReasonForInquiry == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Selected reason for inquiry is not valid", FieldName = nameof(CharacterCertificateRequestVM.CharacterCertificateReasonForInquiry) });
                }
                else
                {
                    if (certificateReasonForInquiry.ShowFreeForm)
                    {
                        if (!string.IsNullOrEmpty(userInput.ReasonForInquiryValue))
                        {
                            userInput.ReasonForInquiryValue = userInput.ReasonForInquiryValue.Trim();
                            if (userInput.ReasonForInquiryValue.Length < 5 || userInput.ReasonForInquiryValue.Length > 20)
                            {
                                errors.Add(new ErrorModel { ErrorMessage = "Reason for inquiry is required. Must be between 5 - 20 characters", FieldName = nameof(CharacterCertificateRequestVM.ReasonForInquiryValue) });
                                userInput.ShowReasonForInquiryFreeForm = true;
                            }
                        }
                        else
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "Reason for inquiry is required. Must be between 5 - 20 characters", FieldName = nameof(CharacterCertificateRequestVM.ReasonForInquiryValue) });
                            userInput.ShowReasonForInquiryFreeForm = true;
                        }
                    }
                    else { userInput.ReasonForInquiryValue = certificateReasonForInquiry.Name; }
                }

                if (!string.IsNullOrEmpty(userInput.PlaceOfBirth))
                {
                    userInput.PlaceOfBirth = userInput.PlaceOfBirth.Trim();
                    if (userInput.PlaceOfBirth.Length < 3 || userInput.PlaceOfBirth.Length > 50)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Place of birth is required. Must be between 3 - 50 characters", FieldName = nameof(CharacterCertificateRequestVM.PlaceOfBirth) });
                    }
                }
                else
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Place of birth is required. Must be between 3 - 50 characters", FieldName = nameof(CharacterCertificateRequestVM.PlaceOfBirth) });
                }

                if (!string.IsNullOrEmpty(userInput.DateOfBirth))
                {
                    try
                    {
                        userInput.DateOfBirthParsed = DateTime.ParseExact(userInput.DateOfBirth.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if (userInput.DateOfBirthParsed > DateTime.Now.AddYears(-14))
                        {
                            errors.Add(new ErrorModel { FieldName = nameof(CharacterCertificateRequestVM.DateOfBirth), ErrorMessage = $"Please input a valid date. Date of birth cannot be more than {DateTime.Now.AddYears(-14).ToString("dd/MM/yyyy")}." });
                        }
                    }
                    catch (Exception)
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(CharacterCertificateRequestVM.DateOfBirth), ErrorMessage = "Please input a valid date. Expected date format dd/MM/yyyy i.e. 31/09/2020." });
                    }
                }
                else { errors.Add(new ErrorModel { ErrorMessage = "Date of birth is required", FieldName = nameof(CharacterCertificateRequestVM.DateOfBirth) }); }

                if (userInput.RequestType == (int)PCCRequestType.International)
                {
                    if (string.IsNullOrEmpty(userInput.PassportNumber))
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "International passport number is required", FieldName = nameof(CharacterCertificateRequestVM.PassportNumber) });
                    }
                    else if (_coreCountryService.checkIfCountryIsNigeria(userInput.SelectedCountryOfPassport))
                    {
                        if (userInput.PassportNumber.Trim().Length != 9)
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "International passport number must be 9 characters(a letter and 8 digits)", FieldName = nameof(CharacterCertificateRequestVM.PassportNumber) });
                        }
                    }
                    else
                    {
                        if (userInput.PassportNumber.Trim().Length < 7 || userInput.PassportNumber.Trim().Length > 9)
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "International passport number must be at least 7 characters and 9 characters at most", FieldName = nameof(CharacterCertificateRequestVM.PassportNumber) });
                        }
                    }

                    if (string.IsNullOrEmpty(userInput.PlaceOfIssuance))
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Place of issuance is required", FieldName = nameof(CharacterCertificateRequestVM.PlaceOfIssuance) });
                    }
                    else
                    {
                        if (userInput.PlaceOfIssuance.Trim().Length < 3 || userInput.PlaceOfIssuance.Trim().Length > 50)
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "Place of issuance is required. Must be between 3 - 50 characters", FieldName = nameof(CharacterCertificateRequestVM.PlaceOfIssuance) });
                        }
                    }

                    if (!string.IsNullOrEmpty(userInput.DateOfIssuance))
                    {
                        try
                        {
                            userInput.DateOfIssuanceParsed = DateTime.ParseExact(userInput.DateOfIssuance.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            if (userInput.DateOfIssuanceParsed > DateTime.Now)
                            {
                                errors.Add(new ErrorModel { FieldName = nameof(CharacterCertificateRequestVM.DateOfIssuance), ErrorMessage = "Please input a valid date. Date of issuance cannot be a date in the future." });
                            }
                        }
                        catch (Exception)
                        {
                            errors.Add(new ErrorModel { FieldName = nameof(CharacterCertificateRequestVM.DateOfIssuance), ErrorMessage = "Please input a valid date. Expected date format dd/MM/yyyy i.e. 31/09/2020." });
                        }
                    }
                    else { errors.Add(new ErrorModel { ErrorMessage = "Date of issuance is required", FieldName = nameof(CharacterCertificateRequestVM.DateOfIssuance) }); }

                    if (!_coreCountryService.ValidateCountry(userInput.DestinationCountry))
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Selected destination country is not valid", FieldName = nameof(CharacterCertificateRequestVM.DestinationCountry) });
                    }

                    if (!_coreCountryService.ValidateCountry(userInput.SelectedCountryOfPassport))
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Selected country of passport is not valid", FieldName = nameof(CharacterCertificateRequestVM.SelectedCountryOfPassport) });
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(userInput.PassportNumber))
                    {
                        if (_coreCountryService.checkIfCountryIsNigeria(userInput.SelectedCountryOfPassport))
                        {
                            if (userInput.PassportNumber.Trim().Length != 9)
                            {
                                errors.Add(new ErrorModel { ErrorMessage = "International passport number must be 9 characters(a letter and 8 digits)", FieldName = nameof(CharacterCertificateRequestVM.PassportNumber) });
                            }
                        }else if (userInput.PassportNumber.Trim().Length < 7 || userInput.PassportNumber.Trim().Length > 9)
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "International passport number must be at least 7 characters and 9 characters at most", FieldName = nameof(CharacterCertificateRequestVM.PassportNumber) });
                        }
                    }

                    if (!string.IsNullOrEmpty(userInput.PlaceOfIssuance))
                    {
                        if (userInput.PlaceOfIssuance.Trim().Length < 3 || userInput.PlaceOfIssuance.Trim().Length > 50)
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "Place of issuance is required. Must be between 3 - 50 characters", FieldName = nameof(CharacterCertificateRequestVM.PlaceOfIssuance) });
                        }
                    }

                    if (!string.IsNullOrEmpty(userInput.DateOfIssuance))
                    {
                        try
                        {
                            userInput.DateOfIssuanceParsed = DateTime.ParseExact(userInput.DateOfIssuance.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            if (userInput.DateOfIssuanceParsed > DateTime.Now)
                            {
                                errors.Add(new ErrorModel { FieldName = nameof(CharacterCertificateRequestVM.DateOfIssuance), ErrorMessage = "Please input a valid date. Date of issuance cannot be a date in the future." });
                            }
                        }
                        catch (Exception)
                        {
                            errors.Add(new ErrorModel { FieldName = nameof(CharacterCertificateRequestVM.DateOfIssuance), ErrorMessage = "Please input a valid date. Expected date format dd/MM/yyyy i.e. 31/09/2020." });
                        }
                    }

                    if (userInput.DestinationCountry > 0)
                    {
                        if (!_coreCountryService.ValidateCountry(userInput.DestinationCountry))
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "Selected destination country is not valid", FieldName = nameof(CharacterCertificateRequestVM.DestinationCountry) });
                        }
                    }

                    if(userInput.SelectedCountryOfPassport > 0)
                    {
                        if (!_coreCountryService.ValidateCountry(userInput.SelectedCountryOfPassport))
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "Selected country of passport is not valid", FieldName = nameof(CharacterCertificateRequestVM.SelectedCountryOfPassport) });
                        }
                    }
                }

                if (userInput.PreviouslyConvicted)
                {
                    if (!string.IsNullOrEmpty(userInput.PreviousConvictionHistory))
                    {
                        userInput.PreviousConvictionHistory = userInput.PreviousConvictionHistory.Trim();
                        if (userInput.PreviousConvictionHistory.Length < 10 || userInput.PreviousConvictionHistory.Length > 100)
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "Previous conviction history is required. Must be between 10 - 100 characters", FieldName = nameof(CharacterCertificateRequestVM.PreviousConvictionHistory) });
                            userInput.ShowConvictionFreeForm = true;
                        }
                    }
                    else
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Previous conviction history is required. Must be between 10 - 100 characters", FieldName = nameof(CharacterCertificateRequestVM.PreviousConvictionHistory) });
                        userInput.ShowConvictionFreeForm = true;
                    }
                }

                if (!_coreStateLGAService.ValidateState(userInput.SelectedState))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Selected state value is invalid", FieldName = nameof(CharacterCertificateRequestVM.SelectedState) });
                }

                if (isNigeria)
                {
                    StateModelVM stateOfOrigin = _coreStateLGAService.GetState(userInput.SelectedStateOfOrigin).Select(x => new StateModelVM { Id = x.Id, Name = x.Name }).SingleOrDefault();
                    if (stateOfOrigin != null)
                    {
                        userInput.SelectedStateOfOriginValue = stateOfOrigin.Name;
                    }
                    else { errors.Add(new ErrorModel { ErrorMessage = "Selected state of origin value is invalid", FieldName = nameof(CharacterCertificateRequestVM.SelectedStateOfOrigin) }); }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Error when validating character certificate request. Exception message --- {exception.Message}");
                throw;
            }
        }


        /// <summary>
        /// Get next action direction for character certificate
        /// </summary>
        /// <returns></returns>
        public dynamic GetNextDirectionForConfirmation()
        {
            return new { RouteName = "P.Request.Confirm", Stage = PSSUserRequestGenerationStage.PSSRequestConfirmation };
        }


        /// <summary>
        /// Get the list of active commands for this state and service
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="serviceId"></param>
        /// <returns>IEnumerable{CommandVM}</returns>
        public IEnumerable<CommandVM> GetListOfCommandsForState(int stateId, int serviceId)
        {
            return _coreServiceStateCommand.GetActiveCommands(stateId, serviceId);
        }

    }
}