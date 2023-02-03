using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Exceptions;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class GenerateEscortRequestWithoutOfficersHandler : IGenerateEscortRequestWithoutOfficersHandler
    {
        private readonly ICommandTypeManager<CommandType> _commandTypeManager;
        private readonly Lazy<ICoreCommand> _coreCommandService;
        private readonly ICoreHelperService _corehelper;
        private readonly ICorePSSEscortServiceCategory _corePSSEscortServiceCategory;
        private readonly ICoreStateAndLGA _coreStateLGAService;
        private readonly ITaxEntityProfileLocationManager<TaxEntityProfileLocation> _entityProfileLocationManager;
        private readonly IHandlerComposition _handlerComposition;
        private readonly IOrchardServices _orchardServices;
        private readonly IPSSReasonManager<PSSReason> _pssReasonRepo;
        private readonly ITaxEntitySubCategoryManager<TaxEntitySubCategory> _taxEntitySubCategoryRepository;
        private readonly ITaxEntitySubSubCategoryManager<TaxEntitySubSubCategory> _taxEntitySubSubCategoryRepository;
        private readonly IGenerateRequestWithoutOfficersUploadBatchStagingManager<GenerateRequestWithoutOfficersUploadBatchStaging> _generateRequestWithoutOfficersUploadBatchStagingManager;
        private readonly ICoreGenerateEscortRequestWithoutOfficersService _coreGenerateEscortRequestWithoutOfficersService;
        ILogger Logger { get; set; }

        public GenerateEscortRequestWithoutOfficersHandler(IHandlerComposition handlerComposition, IOrchardServices orchardServices, ICoreHelperService corehelper, ITaxEntityProfileLocationManager<TaxEntityProfileLocation> entityProfileLocationManager, ICoreStateAndLGA coreStateLGAService, ICommandTypeManager<CommandType> commandTypeManager, IPSSReasonManager<PSSReason> pssReasonRepo, ICorePSSEscortServiceCategory corePSSEscortServiceCategory, ITaxEntitySubCategoryManager<TaxEntitySubCategory> taxEntitySubCategoryRepository,Lazy<ICoreCommand> coreCommandService, ITaxEntitySubSubCategoryManager<TaxEntitySubSubCategory> taxEntitySubSubCategoryRepository, IGenerateRequestWithoutOfficersUploadBatchStagingManager<GenerateRequestWithoutOfficersUploadBatchStaging> generateRequestWithoutOfficersUploadBatchStagingManager, ICoreGenerateEscortRequestWithoutOfficersService coreGenerateEscortRequestWithoutOfficersService)
        {
            _handlerComposition = handlerComposition;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _corehelper = corehelper;
            _entityProfileLocationManager = entityProfileLocationManager;
            _coreStateLGAService = coreStateLGAService;
            _commandTypeManager = commandTypeManager;
            _pssReasonRepo = pssReasonRepo;
            _corePSSEscortServiceCategory = corePSSEscortServiceCategory;
            _taxEntitySubCategoryRepository = taxEntitySubCategoryRepository;
            _taxEntitySubSubCategoryRepository = taxEntitySubSubCategoryRepository;
            _generateRequestWithoutOfficersUploadBatchStagingManager = generateRequestWithoutOfficersUploadBatchStagingManager;
            _coreCommandService = coreCommandService;
            _coreGenerateEscortRequestWithoutOfficersService = coreGenerateEscortRequestWithoutOfficersService;
        }


        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="permission"></param>
        public void CheckForPermission(Orchard.Security.Permissions.Permission permission)
        {
            _handlerComposition.IsAuthorized(permission);
        }


        /// <summary>
        /// Gets GenerateEscortRequestForWithoutOfficersVM
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public GenerateEscortRequestForWithoutOfficersVM GetGenerateEscortRequestVM(long batchId)
        {
            try
            {
                if (_generateRequestWithoutOfficersUploadBatchStagingManager.Count(x => x.Id == batchId && x.Status == (int)GenerateRequestWithoutOfficersUploadStatus.BatchValidated) == 0) { throw new UserNotAuthorizedForThisActionException($"Batch with id {batchId} is not at validation stage"); }
                GenerateRequestWithoutOfficersUploadBatchStagingDTO batchInfo = _generateRequestWithoutOfficersUploadBatchStagingManager.GetGenerateRequestWithoutOfficersUploadBatchWithId(batchId);
                if (batchInfo == null)
                {
                    throw new Exception($"No GenerateRequestWithoutOfficersUploadBatchStaging with id {batchId} found");
                }

                if (_generateRequestWithoutOfficersUploadBatchStagingManager.Count(x => x.TaxEntityProfileLocation.Id == batchInfo.TaxEntityProfileLocation.Id && x.HasGeneratedInvoice) > 0)
                {
                    throw new PSSRequestAlreadyExistsException();
                }

                return new GenerateEscortRequestForWithoutOfficersVM
                {
                    BranchDetails = batchInfo.TaxEntityProfileLocation,
                    EscortDetails = new EscortRequestVM
                    {
                        StateLGAs = _coreStateLGAService.GetStates(),
                        Reasons = _pssReasonRepo.GetReasonsVM(),
                        EscortServiceCategories = _corePSSEscortServiceCategory.GetEscortServiceCategories(),
                        SelectedEscortServiceCategories = new List<int>(),
                        CommandTypes = _commandTypeManager.GetCommandTypes(),
                    },
                    Sectors = _taxEntitySubCategoryRepository.GetActiveTaxEntitySubCategoryByCategoryId(batchInfo.TaxEntityProfileLocation.TaxEntity.CategoryId),
                    BatchId = batchId
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Validates and Generate escort request for default branch
        /// </summary>
        /// <param name="userInputModel"></param>
        /// <param name="errors"></param>
        public InvoiceGenerationResponse GenerateEscortRequestForDefaultBranch(GenerateEscortRequestForWithoutOfficersVM userInputModel, ref List<ErrorModel> errors)
        {
            var command = ValidateUserInput(userInputModel, errors);

            EscortRequestVM escortRequestVM = BuildEscortRequest(userInputModel, command);

            //Generates request
            InvoiceGenerationResponse response = _coreGenerateEscortRequestWithoutOfficersService.GenerateRequestForUnknownOfficers(userInputModel.EscortDetails, userInputModel.BatchId, true);

            //update batch invoice generation status
            _generateRequestWithoutOfficersUploadBatchStagingManager.UpdateInvoiceGenerationStatusForBatchWithId(userInputModel.BatchId);

            return response;
        }


        /// <summary>
        /// Validates and Generate escort request for branch
        /// </summary>
        /// <param name="userInputModel"></param>
        /// <param name="errors"></param>
        public InvoiceGenerationResponse GenerateEscortRequestForBranch(GenerateEscortRequestForWithoutOfficersVM userInputModel, ref List<ErrorModel> errors)
        {
            var command = ValidateUserInput(userInputModel, errors);

            EscortRequestVM escortRequestVM = BuildEscortRequest(userInputModel, command);

            //Generates request
            InvoiceGenerationResponse response = _coreGenerateEscortRequestWithoutOfficersService.GenerateRequestForUnknownOfficers(userInputModel.EscortDetails, userInputModel.BatchId);

            //update batch invoice generation status
            _generateRequestWithoutOfficersUploadBatchStagingManager.UpdateInvoiceGenerationStatusForBatchWithId(userInputModel.BatchId);

            return response;
        }


        /// <summary>
        /// populates <paramref name="vm"/> for postback
        /// </summary>
        /// <param name="vm"></param>
        public void PopulateGenerateEscortRequestVMForPostback(GenerateEscortRequestForWithoutOfficersVM vm)
        {
            try
            {
                var escortRequestModel = GetGenerateEscortRequestVM(vm.BatchId);
                vm.BranchDetails = escortRequestModel.BranchDetails;
                vm.EscortDetails.StateLGAs = escortRequestModel.EscortDetails.StateLGAs;
                vm.EscortDetails.ListLGAs = (vm.EscortDetails.SelectedState > 0) ? escortRequestModel.EscortDetails.StateLGAs.Where(x => x.Id == vm.EscortDetails.SelectedState).SingleOrDefault().LGAs.ToList() : null;
                vm.EscortDetails.Reasons = escortRequestModel.EscortDetails.Reasons;
                vm.EscortDetails.EscortServiceCategories = escortRequestModel.EscortDetails.EscortServiceCategories;
                vm.EscortDetails.CommandTypes = escortRequestModel.EscortDetails.CommandTypes;
                vm.Sectors = escortRequestModel.Sectors;
                vm.EscortDetails.EscortCategoryTypes = (vm.EscortDetails.SelectedEscortServiceCategories.ElementAtOrDefault(0) > 0) ? _corePSSEscortServiceCategory.GetCategoryTypesForServiceCategoryWithId(vm.EscortDetails.SelectedEscortServiceCategories.ElementAtOrDefault(0)).ToList() : null;
                vm.EscortDetails.OriginLGAs = (vm.EscortDetails.SelectedOriginState > 0) ? vm.EscortDetails.StateLGAs.Where(s => s.Id == vm.EscortDetails.SelectedOriginState).SingleOrDefault().LGAs.ToList() : null;
                vm.EscortDetails.TacticalSquads = (vm.EscortDetails.SelectedCommandType > 0) ? _coreCommandService.Value.GetCommandsForCommandTypeWithId(vm.EscortDetails.SelectedCommandType) : null;
                CommandVM tacticalSquad = _coreCommandService.Value.GetCommandDetails(vm.EscortDetails.SelectedTacticalSquad);
                vm.EscortDetails.Formations = (tacticalSquad != null) ? _coreCommandService.Value.GetNextLevelCommandsWithCode(tacticalSquad.Code) : (vm.EscortDetails.SelectedOriginState > 0) ? _coreCommandService.Value.GetCommandsByState(vm.EscortDetails.SelectedOriginState) : _coreCommandService.Value.GetCommandsByState(vm.EscortDetails.SelectedState);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Builds escort request model
        /// </summary>
        /// <param name="userInputModel"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private EscortRequestVM BuildEscortRequest(GenerateEscortRequestForWithoutOfficersVM userInputModel, CommandVM command)
        {
            var defaultSubSubCategory = _taxEntitySubSubCategoryRepository.GetActiveDefaultTaxEntitySubSubCategoryById(userInputModel.EscortDetails.SubCategoryId);
            return new EscortRequestVM
            {
                SelectedStateLGA = userInputModel.EscortDetails.SelectedStateLGA,
                SelectedState = userInputModel.EscortDetails.SelectedState,
                SelectedCommand = command.Id,
                Reason = userInputModel.EscortDetails.Reason,
                LGAName = command.LGAName,
                StateName = command.StateName,
                CommandName = command.Name,
                CommandAddress = command.Address,
                Address = userInputModel.EscortDetails.Address,
                NumberOfOfficers = userInputModel.EscortDetails.NumberOfOfficers,
                PSBillingType = userInputModel.EscortDetails.PSBillingType,
                SelectedOriginState = userInputModel.EscortDetails.SelectedOriginState,
                SelectedOriginLGA = userInputModel.EscortDetails.SelectedOriginLGA,
                AddressOfOriginLocation = userInputModel.EscortDetails.AddressOfOriginLocation,
                OriginStateName = userInputModel.EscortDetails.OriginStateName,
                OriginLGAName = userInputModel.EscortDetails.OriginLGAName,
                ShowExtraFieldsForServiceCategoryType = userInputModel.EscortDetails.ShowExtraFieldsForServiceCategoryType,
                SelectedEscortServiceCategories = userInputModel.EscortDetails.SelectedEscortServiceCategories,
                SelectedCommandType = userInputModel.EscortDetails.SelectedCommandType,
                SubCategoryId = userInputModel.EscortDetails.SubCategoryId,
                SubSubCategoryId = (defaultSubSubCategory != null) ? defaultSubSubCategory.Id : 0
            };
        }


        /// <summary>
        /// validates user input
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private CommandVM ValidateUserInput(GenerateEscortRequestForWithoutOfficersVM model, List<ErrorModel> errors)
        {
            ValidateServiceCategoryDetails(model.EscortDetails, ref errors);

            if (model.EscortDetails.SelectedState == 0)
            {
                errors.Add(new ErrorModel { ErrorMessage = $"State is required", FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(model.EscortDetails.SelectedState)}" });
            }

            if (model.EscortDetails.SelectedStateLGA == 0)
            {
                errors.Add(new ErrorModel { ErrorMessage = $"LGA is required", FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(model.EscortDetails.SelectedStateLGA)}" });
            }

            if (errors.Count > 0) { throw new DirtyFormDataException(); }

            CommandVM command = ValidateSelectedCommand(model.EscortDetails, ref errors);

            if (string.IsNullOrEmpty(model.EscortDetails.Address?.Trim()))
            {
                errors.Add(new ErrorModel { ErrorMessage = $"Address is required", FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(model.EscortDetails.Address)}" });
            }
            else
            {
                if (model.EscortDetails.Address.Trim().Length > 100 || model.EscortDetails.Address.Trim().Length < 5)
                { errors.Add(new ErrorModel { FieldName = $"{nameof(model.EscortDetails)}.{nameof(model.EscortDetails.Address)}", ErrorMessage = "Address field must be between 5 to 100 characters long." }); }
            }

            if (model.EscortDetails.SubCategoryId == 0)
            {
                errors.Add(new ErrorModel { ErrorMessage = $"Sector is required", FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(model.EscortDetails.SubCategoryId)}" });
            }
            else
            {
                if (_taxEntitySubCategoryRepository.Count(x => x.Id == model.EscortDetails.SubCategoryId && x.IsActive) == 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Sector value is not valid", FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(model.EscortDetails.SubCategoryId)}" });
                }
            }

            if(Enum.GetName(typeof(PSBillingType),model.EscortDetails.PSBillingType) == null)
            {
                errors.Add(new ErrorModel { ErrorMessage = $"Selected payment method billing type is not valid", FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(model.EscortDetails.PSBillingType)}" });
            }
            else
            {
                if(Enum.GetName(typeof(PSBillingType), model.EscortDetails.PSBillingType) == nameof(PSBillingType.Weekly) && model.EscortDetails.PSBillingTypeDurationNumber <= 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Selected payment method duration number is not valid", FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(model.EscortDetails.PSBillingTypeDurationNumber)}" });
                }
            }

            ValidateStartAndEndDate(model.EscortDetails, ref errors);

            if (errors.Count > 0) { throw new DirtyFormDataException(); }

            return command;
        }


        /// <summary>
        /// Validates service category, category type and extra fields such as origin state, origin LGA
        /// </summary>
        /// <param name="userInput"></param>
        private void ValidateServiceCategoryDetails(EscortRequestVM userInput, ref List<ErrorModel> errors)
        {
            PSSEscortServiceCategoryVM serviceCategory = _corePSSEscortServiceCategory.GetEscortServiceCategoryWithId(userInput.SelectedEscortServiceCategories.ElementAtOrDefault(0));
            if (serviceCategory == null)
            {
                errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.SelectedEscortServiceCategory", ErrorMessage = "Selected Service Category value is not valid" });
            }

            PSSEscortServiceCategoryVM categoryType = null;
            if (userInput.SelectedEscortServiceCategories.ElementAtOrDefault(1) > 0 && serviceCategory != null)
            {
                if (!_corePSSEscortServiceCategory.CheckIfCategoryTypeInServiceCategory(userInput.SelectedEscortServiceCategories.ElementAtOrDefault(0), userInput.SelectedEscortServiceCategories.ElementAtOrDefault(1)))
                {
                    errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.SelectedEscortServiceCategoryType", ErrorMessage = "Selected Service Category Type value is not valid" });
                }
                else
                {
                    categoryType = _corePSSEscortServiceCategory.GetEscortServiceCategoryWithId(userInput.SelectedEscortServiceCategories.ElementAtOrDefault(1));
                }
            }
            else
            {
                //getting for children category type for service category
                if (serviceCategory != null)
                {
                    IEnumerable<PSSEscortServiceCategoryVM> categoryTypes = _corePSSEscortServiceCategory.GetCategoryTypesForServiceCategoryWithId(userInput.SelectedEscortServiceCategories.ElementAtOrDefault(0)).ToList();
                    if (categoryTypes.Count() > 0)
                    {
                        errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.SelectedEscortServiceCategoryType", ErrorMessage = "Selected Service Category Type value is not valid" });
                    }
                }
            }

            //extra form fields validation for category types that have extra form fields
            if (categoryType != null && categoryType.ShowExtraFields)
            {
                userInput.ShowExtraFieldsForServiceCategoryType = categoryType.ShowExtraFields;
                if (userInput.SelectedOriginState < 1) { errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(userInput.SelectedOriginState)}", ErrorMessage = "Origin State is required." }); }
                if (userInput.SelectedOriginLGA < 1) { errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(userInput.SelectedOriginLGA)}", ErrorMessage = "Origin LGA is required." }); }

                if (userInput.SelectedOriginLGA > 0)
                {
                    var originLGA = _coreStateLGAService.GetLGAWithId(userInput.SelectedOriginLGA);

                    if (originLGA == null)
                    {
                        errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(userInput.SelectedOriginLGA)}", ErrorMessage = "Origin LGA value is not valid." });
                    }
                    else
                    {
                        if (originLGA.StateId != userInput.SelectedOriginState)
                        {
                            errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(userInput.SelectedOriginState)}", ErrorMessage = "Origin state value is not valid." });
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
                    errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(userInput.AddressOfOriginLocation)}", ErrorMessage = "Address Of Origin Location field is required" });
                }
                else
                {
                    userInput.AddressOfOriginLocation = userInput.AddressOfOriginLocation.Trim();
                    if (userInput.AddressOfOriginLocation.Length > 100 || userInput.AddressOfOriginLocation.Length < 5)
                    { errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(userInput.AddressOfOriginLocation)}", ErrorMessage = "Address Of Origin Location field must be between 5 to 100 characters long." }); }
                }
            }
        }


        /// <summary>
        /// validates selected team, tactical squad if any and next level commands if any
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private CommandVM ValidateSelectedCommand(EscortRequestVM userInput, ref List<ErrorModel> errors)
        {
            CommandTypeVM commandType = _commandTypeManager.GetCommandType(userInput.SelectedCommandType);
            if (commandType != null)
            {
                //checks if the selected command type i.e Tactical, Conventional has commands
                if (_coreCommandService.Value.CheckIfCommandTypeHasCommands(commandType.Id))
                {
                    //if the command type has commands it will fetch the commands i.e for tactical the commands retrieved could be PMF, CTU, SPU
                    CommandVM command = _coreCommandService.Value.GetCommandForCommandTypeWithId(commandType.Id, userInput.SelectedTacticalSquad);
                    if (command != null)
                    {
                        if (userInput.SelectedCommand == 0) { return command; }
                        CommandVM nextLevelCommand = _coreCommandService.Value.GetCommandDetails(userInput.SelectedCommand);
                        if (nextLevelCommand != null)
                        {
                            if (!nextLevelCommand.Code.Contains(command.Code))
                            {
                                errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(EscortRequestVM.SelectedCommand)}", ErrorMessage = "Selected command/formation value is not valid for the selected tactical squad" });
                                return null;
                            }
                            return nextLevelCommand;
                        }
                        else
                        {
                            errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(EscortRequestVM.SelectedCommand)}", ErrorMessage = "Selected command/formation value is not valid" });
                            return null;
                        }
                    }
                    else
                    {
                        errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(EscortRequestVM.SelectedTacticalSquad)}", ErrorMessage = "Selected tactical squad value is not valid" });
                        return null;
                    }
                }
                else
                {
                    CommandVM selectedCommand = (userInput.SelectedCommand > 0) ? _coreCommandService.Value.GetCommandDetails(userInput.SelectedCommand) : (userInput.SelectedOriginState > 0) ? _coreCommandService.Value.GetStateCommand(userInput.SelectedOriginState) : _coreCommandService.Value.GetStateCommand(userInput.SelectedState);
                    if (selectedCommand != null) { return selectedCommand; }
                    errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(EscortRequestVM.SelectedCommand)}", ErrorMessage = "Selected command not found." });
                    return null;
                }
            }
            else
            {
                errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(EscortRequestVM.SelectedCommandType)}", ErrorMessage = "Selected team value is not valid" });
                return null;
            }
        }


        /// <summary>
        /// validates and parses start date and end date
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        private void ValidateStartAndEndDate(EscortRequestVM userInput, ref List<ErrorModel> errors)
        {
            if (!string.IsNullOrEmpty(userInput.StartDate) && !string.IsNullOrEmpty(userInput.EndDate))
            {
                try
                {
                    userInput.StartDate = userInput.StartDate.Trim();
                    userInput.EndDate = userInput.EndDate.Trim();

                    DateTime startDate = DateTime.ParseExact(userInput.StartDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime endDate = DateTime.ParseExact(userInput.EndDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    if (startDate > endDate)
                    {
                        errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(EscortRequestVM.StartDate)}", ErrorMessage = "Please input a valid date. Start date cannot be ahead of the end date." });
                        errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(EscortRequestVM.EndDate)}", ErrorMessage = "Please input a valid date. Start date cannot be ahead of the end date." });
                        throw new DirtyFormDataException { };
                    }

                    if (startDate <= DateTime.Now.ToLocalTime())
                    {
                        errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(EscortRequestVM.StartDate)}", ErrorMessage = "Please input a valid date. Start date must be ahead of today." });
                    }

                    userInput.DurationNumber = (endDate - startDate).Days + 1; //1 is added because both start and end date are inclusive i.e 02/01/2020 - 01/01/2020 difference will be 1 without an addition of inclusive 1 day
                    userInput.ParsedStartDate = startDate;
                    userInput.ParsedEndDate = endDate;
                }
                catch (Exception)
                {
                    errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(EscortRequestVM.StartDate)}", ErrorMessage = $"Please input a valid date. Expected date format dd/MM/yyyy i.e. {DateTime.Now.ToString("dd/MM/yyyy")}." });
                    errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(EscortRequestVM.EndDate)}", ErrorMessage = $"Please input a valid date. Expected date format dd/MM/yyyy i.e. {DateTime.Now.ToString("dd/MM/yyyy")}." });
                }
            }
            else
            {
                errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(EscortRequestVM.StartDate)}", ErrorMessage = $"Please input a valid date. Expected date format dd/MM/yyyy i.e. {DateTime.Now.ToString("dd/MM/yyyy")}." });
                errors.Add(new ErrorModel { FieldName = $"{nameof(GenerateEscortRequestForWithoutOfficersVM.EscortDetails)}.{nameof(EscortRequestVM.EndDate)}", ErrorMessage = $"Please input a valid date. Expected date format dd/MM/yyyy i.e. {DateTime.Now.ToString("dd/MM/yyyy")}." });
            }
        }
    }
}