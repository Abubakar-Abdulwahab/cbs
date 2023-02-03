using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DataFilters.PSSBranchOfficersUploadBatchStagingReport.Contracts;
using Parkway.CBS.Police.Core.Exceptions;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class BranchOfficerHandler : IBranchOfficerHandler
    {
        private readonly IPSSBranchOfficersUploadBatchItemsStagingFilter _batchItemsStagingFilter;
        private readonly ICommandTypeManager<CommandType> _commandTypeManager;
        private readonly Lazy<ICoreCommand> _coreCommandService;
        private readonly ICoreHelperService _corehelper;
        private readonly ICorePSSEscortServiceCategory _corePSSEscortServiceCategory;
        private readonly ICoreStateAndLGA _coreStateLGAService;
        private readonly ITaxEntityProfileLocationManager<TaxEntityProfileLocation> _entityProfileLocationManager;
        private readonly IHandlerComposition _handlerComposition;
        private readonly IOrchardServices _orchardServices;
        private readonly IPSSReasonManager<PSSReason> _pssReasonRepo;
        private readonly ICoreEGSRegularizationPSSRequestGenerationService _pssRequestGenerationService;
        private readonly ITaxEntitySubCategoryManager<TaxEntitySubCategory> _taxEntitySubCategoryRepository;
        private readonly ITaxEntitySubSubCategoryManager<TaxEntitySubSubCategory> _taxEntitySubSubCategoryRepository;
        private readonly IPSSBranchOfficersUploadBatchStagingFilter _uploadBatchStagingFilter;
        private readonly IPSSBranchOfficersUploadBatchStagingManager<PSSBranchOfficersUploadBatchStaging> _uploadBatchStagingManager;
        ILogger Logger { get; set; }


        public BranchOfficerHandler(IHandlerComposition handlerComposition, IOrchardServices orchardServices, ICoreHelperService corehelper, IPSSBranchOfficersUploadBatchStagingManager<PSSBranchOfficersUploadBatchStaging> uploadBatchStagingManager, ITaxEntityProfileLocationManager<TaxEntityProfileLocation> entityProfileLocationManager, IPSSBranchOfficersUploadBatchStagingFilter uploadBatchStagingFilter, ICoreStateAndLGA coreStateLGAService, ICommandTypeManager<CommandType> commandTypeManager, IPSSReasonManager<PSSReason> pssReasonRepo, ICorePSSEscortServiceCategory corePSSEscortServiceCategory, ITaxEntitySubCategoryManager<TaxEntitySubCategory> taxEntitySubCategoryRepository, ICoreEGSRegularizationPSSRequestGenerationService pssRequestGenerationService, IPSSBranchOfficersUploadBatchItemsStagingFilter batchItemsStagingFilter, Lazy<ICoreCommand> coreCommandService, ITaxEntitySubSubCategoryManager<TaxEntitySubSubCategory> taxEntitySubSubCategoryRepository)
        {
            _handlerComposition = handlerComposition;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _corehelper = corehelper;
            _uploadBatchStagingManager = uploadBatchStagingManager;
            _entityProfileLocationManager = entityProfileLocationManager;
            _uploadBatchStagingFilter = uploadBatchStagingFilter;
            _coreStateLGAService = coreStateLGAService;
            _commandTypeManager = commandTypeManager;
            _pssReasonRepo = pssReasonRepo;
            _corePSSEscortServiceCategory = corePSSEscortServiceCategory;
            _taxEntitySubCategoryRepository = taxEntitySubCategoryRepository;
            _taxEntitySubSubCategoryRepository = taxEntitySubSubCategoryRepository;
            _pssRequestGenerationService = pssRequestGenerationService;
            _batchItemsStagingFilter = batchItemsStagingFilter;
            _coreCommandService = coreCommandService;
        }


        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canAddBranchOfficer"></param>
        public void CheckForPermission(Permission canAddBranchOfficer)
        {
            _handlerComposition.IsAuthorized(canAddBranchOfficer);
        }

        /// <summary>
        /// Checks if branch officer file batch with id embedded in specified batch token has been successfully uploaded and validated
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns></returns>
        public APIResponse CheckIfBatchUploadValidationCompleted(string batchToken)
        {
            try
            {
                if (string.IsNullOrEmpty(batchToken))
                {
                    Logger.Error("Batch token not specified");
                    return new APIResponse { ResponseObject = "Batch token not specified", Error = true };
                }

                var branchOfficerUploadBatchStagingDTO = _uploadBatchStagingManager.GetBatchByBatchId(GetFileProcessModel(batchToken).Id);

                if (branchOfficerUploadBatchStagingDTO.Status == (int)PSSBranchOfficersUploadStatus.BatchValidated)
                {
                    return new APIResponse { ResponseObject = new { Completed = true } };
                }
                else if (branchOfficerUploadBatchStagingDTO.Status == (int)PSSBranchOfficersUploadStatus.Fail)
                {
                    return new APIResponse { ResponseObject = branchOfficerUploadBatchStagingDTO.ErrorMessage, Error = true };
                }
                else if (branchOfficerUploadBatchStagingDTO.HasError)
                {
                    return new APIResponse { ResponseObject = "Error occured while processing uploaded file, please upload a new file and try again", Error = true };
                }
                else
                {
                    return new APIResponse { ResponseObject = new { Completed = false } };
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { ResponseObject = ErrorLang.genericexception().Text };
            }
        }

        /// <summary>
        /// Validates and Generate escort
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="userInputModel"></param>
        public InvoiceGenerationResponse GenerateEscortRequest(ref List<ErrorModel> errors, PSSBranchGenerateEscortRequestVM userInputModel)
        {
            if (!_uploadBatchStagingManager.IsBatchProcessed(userInputModel.BatchId))
            {
                throw new UserNotAuthorizedForThisActionException($"Batch with id {userInputModel.BatchId} is not at validation stage");
            }

            var command = ValidateUserInput(errors, userInputModel);

            EscortRequestVM escortRequestVM = BuildEscortRequest(userInputModel, command);

            InvoiceGenerationResponse response = _pssRequestGenerationService.GenerateTimeSpecificRequest(escortRequestVM, userInputModel.BranchDetails.Id, userInputModel.BatchId);

            _uploadBatchStagingManager.UpdateInvoiceGenerationStatusForBatchWithId(userInputModel.BatchId);

            return response;
        }

        /// <summary>
        /// Gets add officer vm
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public PSSBranchGenerateEscortRequestVM GetGenerateEscortRequestVM(long batchId)
        {
            try
            {
                if (!_uploadBatchStagingManager.IsBatchProcessed(batchId)) { throw new Exception($"Batch with id {batchId} is not at validation stage"); }
                var taxEntityProfileLocation = _uploadBatchStagingManager.GetTaxEntityProfileLocationAttachedToBatchWithId(batchId);
                if (taxEntityProfileLocation == null)
                {
                    throw new Exception($"No Tax entity profile location attached to branch officers upload batch staging with id {batchId} found");
                }

                if (_uploadBatchStagingManager.Count(x => x.TaxEntityProfileLocation.Id == taxEntityProfileLocation.Id && x.HasGeneratedInvoice) > 0)
                {
                    throw new PSSRequestAlreadyExistsException();
                }

                return new PSSBranchGenerateEscortRequestVM
                {
                    BranchDetails = taxEntityProfileLocation,
                    EscortDetails = new EscortRequestVM
                    {
                        StateLGAs = _coreStateLGAService.GetStates(),
                        Reasons = _pssReasonRepo.GetReasonsVM(),
                        EscortServiceCategories = _corePSSEscortServiceCategory.GetEscortServiceCategories(),
                        SelectedEscortServiceCategories = new List<int>(),
                        CommandTypes = _commandTypeManager.GetCommandTypes(),
                    },
                    Sectors = _taxEntitySubCategoryRepository.GetActiveTaxEntitySubCategoryByCategoryId(taxEntityProfileLocation.TaxEntity.CategoryId),
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
        /// Get view model for branch officer report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalRecordCount }</returns>
        public PSSBranchProfileDetailVM GetBranchProfileDetailVM(PSSBranchOfficersUploadBatchStagingReportSearchParams searchParams)
        {
            try
            {
                dynamic recordsAndAggregate = _uploadBatchStagingFilter.GetReportViewModel(searchParams);
                IEnumerable<PSSBranchOfficersUploadBatchStagingVM> records = (IEnumerable<PSSBranchOfficersUploadBatchStagingVM>)recordsAndAggregate.ReportRecords;

                var taxEntityProfileLocationVM = _entityProfileLocationManager.GetTaxEntityLocationWithId(searchParams.ProfileLocationId);

                return new PSSBranchProfileDetailVM
                {
                    DefaultBranchTaxEntityProfileLocation = _entityProfileLocationManager.GetDefaultTaxEntityLocation(taxEntityProfileLocationVM.TaxEntity.Id),
                    BranchTaxEntityProfileLocation = taxEntityProfileLocationVM,
                    BranchOfficerBatches = (records == null || !records.Any()) ? Enumerable.Empty<PSSBranchOfficersUploadBatchStagingVM>() : records,
                    TotalRecordCount = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount,
                    ProfileId = searchParams.ProfileLocationId
                };

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        public EscortRequestDetailVM GetEscortRequestDetailVM(PSSBranchOfficersUploadBatchItemsStagingReportSearchParams searchParams)
        {
            var taxEntityProfileLocationVM = _entityProfileLocationManager.GetTaxEntityLocationWithId(searchParams.ProfileLocationId);

            dynamic recordsAndAggregate = _batchItemsStagingFilter.GetReportViewModel(searchParams);
            IEnumerable<PSSBranchOfficersUploadBatchItemsStagingVM> records = (IEnumerable<PSSBranchOfficersUploadBatchItemsStagingVM>)recordsAndAggregate.ReportRecords;

            FileUploadReport uploadReport = ((IEnumerable<FileUploadReport>)recordsAndAggregate.ValidItemsAggregate).First();
            uploadReport.NumberOfRecords = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount;

            return new EscortRequestDetailVM
            {
                BranchTaxEntityProfileLocation = _entityProfileLocationManager.GetDefaultTaxEntityLocation(taxEntityProfileLocationVM.TaxEntity.Id),
                BranchOfficers = (records == null || !records.Any()) ? Enumerable.Empty<PSSBranchOfficersUploadBatchItemsStagingVM>() : records,
                TotalRecordCount = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount,
                ProfileId = searchParams.ProfileLocationId,
                BatchId = searchParams.BatchId,
                PSSBranchOfficerUploadBatchItemsReport = uploadReport,
                Status = _uploadBatchStagingManager.GetBatchByBatchId(searchParams.BatchId).Status
            };
        }

        public FileProcessModel GetFileProcessModel(string batchToken)
        {
            return JsonConvert.DeserializeObject<FileProcessModel>(Util.LetsDecrypt(batchToken, System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]));
        }

        public OfficerValidationResultVM GetOfficerValidationResultVM(PSSBranchOfficersUploadBatchItemsStagingReportSearchParams searchParams)
        {
            var taxEntityProfileLocationVM = _entityProfileLocationManager.GetTaxEntityLocationWithId(searchParams.ProfileLocationId);

            dynamic recordsAndAggregate = _batchItemsStagingFilter.GetReportViewModel(searchParams);
            IEnumerable<PSSBranchOfficersUploadBatchItemsStagingVM> records = (IEnumerable<PSSBranchOfficersUploadBatchItemsStagingVM>)recordsAndAggregate.ReportRecords;

            FileUploadReport uploadReport = ((IEnumerable<FileUploadReport>)recordsAndAggregate.ValidItemsAggregate).First();
            uploadReport.NumberOfRecords = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount;

            return new OfficerValidationResultVM
            {
                BranchTaxEntityProfileLocation = taxEntityProfileLocationVM,
                BranchOfficers = (records == null || !records.Any()) ? Enumerable.Empty<PSSBranchOfficersUploadBatchItemsStagingVM>() : records,
                TotalRecordCount = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount,
                ProfileId = searchParams.ProfileLocationId,
                PSSBranchOfficerUploadBatchItemsReport = uploadReport,
                Status = _uploadBatchStagingManager.GetBatchByBatchId(searchParams.BatchId).Status
            };
        }


        /// <summary>
        /// Handles the processing of the <paramref name="branchOfficerFile"/> which includes saving and batch creation
        /// </summary>
        /// <param name="branchOfficerFile"></param>
        /// <param name="profileId"></param>
        /// <returns>A serialized and encrpted <see cref="FileProcessModel"/></returns>
        public string ProcessBranchOfficerUpload(HttpPostedFileBase branchOfficerFile, int profileId)
        {
            try
            {
                string fileName = Util.StrongRandomNoSpecailCharacters() + DateTime.Now.Ticks.ToString() + profileId + Path.GetExtension(branchOfficerFile.FileName);

                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node node = siteConfig.Node.FirstOrDefault(x => x.Key == nameof(PSSTenantConfigKeys.PSSAdminBranchOfficerFilePath));
                if (string.IsNullOrEmpty(node?.Value))
                {
                    Logger.Error(string.Format("Unable to get pss admin branch officer file path in config file"));
                    throw new Exception();
                }

                DirectoryInfo basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + node.Value);
                string filePath = Path.Combine(basePath.FullName, fileName);

                //create batch model
                PSSBranchOfficersUploadBatchStaging batch = new PSSBranchOfficersUploadBatchStaging
                {
                    TaxEntityProfileLocation = new TaxEntityProfileLocation { Id = profileId },
                    Status = (int)PSSBranchOfficersUploadStatus.BatchInitialized,
                    FilePath = filePath,
                    AddedBy = new Orchard.Users.Models.UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id }
                };

                //save file
                branchOfficerFile.SaveAs(filePath);

                //Save batch
                if (!_uploadBatchStagingManager.Save(batch))
                {
                    Logger.Error($"Unable to create PSSBranchOfficersUploadBatchStaging batch for entity with profile id {profileId}");
                    _uploadBatchStagingManager.RollBackAllTransactions();
                    throw new Exception($"Unable to create PSSBranchOfficersUploadBatchStaging batch for entity with profile id {profileId}");
                }

                //Pass to hangfire to extract the items from the file as a background job
                IPSSBranchOfficersFileUpload branchOfficersFileUpload = new PSSBranchOfficersFileUpload();
                branchOfficersFileUpload.ProcessPSSBranchOfficersFileUpload(batch.Id, _orchardServices.WorkContext.CurrentSite.SiteName.Trim());

                FileProcessModel processedBranchOfficerBatchId = new FileProcessModel { Id = batch.Id };
                return Util.LetsEncrypt(JsonConvert.SerializeObject(processedBranchOfficerBatchId), AppSettingsConfigurations.EncryptionSecret);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Validates branch sub users file size and type
        /// </summary>
        /// <param name="branchOfficerFile"></param>
        /// <param name="errors"></param>
        /// <returns>returns true if there are errors</returns>
        public bool ValidateBranchOfficerFile(HttpPostedFileBase branchOfficerFile, ref List<ErrorModel> errors)
        {
            List<UploadedFileAndName> filesAndFileNames = new List<UploadedFileAndName>();
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            Node node = siteConfig.Node.FirstOrDefault(x => x.Key == nameof(PSSTenantConfigKeys.PSSAdminBranchOfficerFileSize));

            if (string.IsNullOrEmpty(node?.Value) || !int.TryParse(node.Value, out int fileSizeCap))
            {
                Logger.Error(string.Format("Unable to get pss admin branch officer file size in config file"));
                errors.Add(new ErrorModel { ErrorMessage = "Error uploading branch officer file.", FieldName = "branchOfficerFile" });
                return true;
            }

            filesAndFileNames.Add(new UploadedFileAndName { Upload = branchOfficerFile, UploadName = "branchOfficerFile" });
            _corehelper.CheckFileSize(filesAndFileNames, errors, fileSizeCap);

            return errors.Any();
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
                                errors.Add(new ErrorModel { FieldName = nameof(EscortRequestVM.SelectedCommand), ErrorMessage = "Selected command/formation value is not valid for the selected tactical squad" });
                                return null;
                            }
                            return nextLevelCommand;
                        }
                        else
                        {
                            errors.Add(new ErrorModel { FieldName = nameof(EscortRequestVM.SelectedCommand), ErrorMessage = "Selected command/formation value is not valid" });
                            return null;
                        }
                    }
                    else
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(EscortRequestVM.SelectedTacticalSquad), ErrorMessage = "Selected tactical squad value is not valid" });
                        return null;
                    }
                }
                else
                {
                    CommandVM selectedCommand = (userInput.SelectedCommand > 0) ? _coreCommandService.Value.GetCommandDetails(userInput.SelectedCommand) : (userInput.SelectedOriginState > 0) ? _coreCommandService.Value.GetStateCommand(userInput.SelectedOriginState) : _coreCommandService.Value.GetStateCommand(userInput.SelectedState);
                    if (selectedCommand != null) { return selectedCommand; }
                    errors.Add(new ErrorModel { FieldName = nameof(EscortRequestVM.SelectedCommand), ErrorMessage = "Selected command not found." });
                    return null;
                }
            }
            else
            {
                errors.Add(new ErrorModel { FieldName = nameof(EscortRequestVM.SelectedCommandType), ErrorMessage = "Selected team value is not valid" });
                return null;
            }
        }


        /// <summary>
        /// Builds escort request model
        /// </summary>
        /// <param name="userInputModel"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private EscortRequestVM BuildEscortRequest(PSSBranchGenerateEscortRequestVM userInputModel, CommandVM command)
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
        /// Gets category types for service category with specified id
        /// </summary>
        /// <param name="serviceCategoryId"></param>
        /// <returns></returns>
        public APIResponse GetCategoryTypesForServiceCategoryWithId(string serviceCategoryId)
        {
            try
            {
                int serviceCategoryIdParsed = 0;
                if (!string.IsNullOrEmpty(serviceCategoryId) && int.TryParse(serviceCategoryId, out serviceCategoryIdParsed))
                {
                    return new APIResponse { ResponseObject = _corePSSEscortServiceCategory.GetCategoryTypesForServiceCategoryWithId(serviceCategoryIdParsed).ToList() };
                }
                Logger.Error("Service category id not specified");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text };
        }


        /// <summary>
        /// Gets escort service category with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private PSSEscortServiceCategoryVM GetEscortServiceCategoryWithId(int id)
        {
            return _corePSSEscortServiceCategory.GetEscortServiceCategoryWithId(id);
        }


        /// <summary>
        /// Validates service category, category type and extra fields such as origin state, origin LGA
        /// </summary>
        /// <param name="userInput"></param>
        private void ValidateServiceCategoryDetails(EscortRequestVM userInput, ref List<ErrorModel> errors)
        {
            PSSEscortServiceCategoryVM serviceCategory = GetEscortServiceCategoryWithId(userInput.SelectedEscortServiceCategories.ElementAtOrDefault(0));
            if (serviceCategory == null)
            {
                errors.Add(new ErrorModel { FieldName = "EscortDetails.SelectedEscortServiceCategory", ErrorMessage = "Selected Service Category value is not valid" });
            }

            PSSEscortServiceCategoryVM categoryType = null;
            if (userInput.SelectedEscortServiceCategories.ElementAtOrDefault(1) > 0 && serviceCategory != null)
            {
                if (!_corePSSEscortServiceCategory.CheckIfCategoryTypeInServiceCategory(userInput.SelectedEscortServiceCategories.ElementAtOrDefault(0), userInput.SelectedEscortServiceCategories.ElementAtOrDefault(1)))
                {
                    errors.Add(new ErrorModel { FieldName = "EscortDetails.SelectedEscortServiceCategoryType", ErrorMessage = "Selected Service Category Type value is not valid" });
                }
                else
                {
                    categoryType = GetEscortServiceCategoryWithId(userInput.SelectedEscortServiceCategories.ElementAtOrDefault(1));
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
                        errors.Add(new ErrorModel { FieldName = "EscortDetails.SelectedEscortServiceCategoryType", ErrorMessage = "Selected Service Category Type value is not valid" });
                    }
                }
            }

            //extra form fields validation for category types that have extra form fields
            if (categoryType != null && categoryType.ShowExtraFields)
            {
                userInput.ShowExtraFieldsForServiceCategoryType = categoryType.ShowExtraFields;
                if (userInput.SelectedOriginState < 1) { errors.Add(new ErrorModel { FieldName = $"EscortDetails.{nameof(userInput.SelectedOriginState)}", ErrorMessage = "Origin State is required." }); }
                if (userInput.SelectedOriginLGA < 1) { errors.Add(new ErrorModel { FieldName = $"EscortDetails.{nameof(userInput.SelectedOriginLGA)}", ErrorMessage = "Origin LGA is required." }); }

                if (userInput.SelectedOriginLGA > 0)
                {
                    var originLGA = _coreStateLGAService.GetLGAWithId(userInput.SelectedOriginLGA);

                    if (originLGA == null)
                    {
                        errors.Add(new ErrorModel { FieldName = $"EscortDetails.{nameof(userInput.SelectedOriginLGA)}", ErrorMessage = "Origin LGA value is not valid." });
                    }
                    else
                    {
                        if (originLGA.StateId != userInput.SelectedOriginState)
                        {
                            errors.Add(new ErrorModel { FieldName = $"EscortDetails.{nameof(userInput.SelectedOriginState)}", ErrorMessage = "Origin state value is not valid." });
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
                    errors.Add(new ErrorModel { FieldName = $"EscortDetails.{nameof(userInput.AddressOfOriginLocation)}", ErrorMessage = "Address Of Origin Location field is required" });
                }
                else
                {
                    userInput.AddressOfOriginLocation = userInput.AddressOfOriginLocation.Trim();
                    if (userInput.AddressOfOriginLocation.Length > 100 || userInput.AddressOfOriginLocation.Length < 5)
                    { errors.Add(new ErrorModel { FieldName = $"EscortDetails.{nameof(userInput.AddressOfOriginLocation)}", ErrorMessage = "Address Of Origin Location field must be between 5 to 100 characters long." }); }
                }
            }
        }


        /// <summary>
        /// validates user input
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private CommandVM ValidateUserInput(List<ErrorModel> errors, PSSBranchGenerateEscortRequestVM model)
        {
            ValidateServiceCategoryDetails(model.EscortDetails, ref errors);

            if (model.EscortDetails.SelectedState == 0)
            {
                errors.Add(new ErrorModel { ErrorMessage = $"State is required", FieldName = $"{nameof(model.EscortDetails)}.{nameof(model.EscortDetails.SelectedState)}" });
            }

            if (model.EscortDetails.SelectedStateLGA == 0)
            {
                errors.Add(new ErrorModel { ErrorMessage = $"LGA is required", FieldName = $"{nameof(model.EscortDetails)}.{nameof(model.EscortDetails.SelectedStateLGA)}" });
            }

            if (errors.Count > 0) { throw new DirtyFormDataException(); }

            CommandVM command = ValidateSelectedCommand(model.EscortDetails, ref errors);

            if (string.IsNullOrEmpty(model.EscortDetails.Address?.Trim()))
            {
                errors.Add(new ErrorModel { ErrorMessage = $"Address is required", FieldName = $"{nameof(model.EscortDetails)}.{nameof(model.EscortDetails.Address)}" });
            }
            else
            {
                if (model.EscortDetails.Address.Trim().Length > 100 || model.EscortDetails.Address.Trim().Length < 5)
                { errors.Add(new ErrorModel { FieldName = $"{nameof(model.EscortDetails)}.{nameof(model.EscortDetails.Address)}", ErrorMessage = "Address field must be between 5 to 100 characters long." }); }
            }

            if (model.EscortDetails.SubCategoryId == 0)
            {
                errors.Add(new ErrorModel { ErrorMessage = $"Sector is required", FieldName = $"{nameof(model.EscortDetails)}.{nameof(model.EscortDetails.SubCategoryId)}" });
            }
            else
            {
                if (_taxEntitySubCategoryRepository.Count(x => x.Id == model.EscortDetails.SubCategoryId && x.IsActive) == 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Sector value is not valid", FieldName = $"{nameof(model.EscortDetails)}.{nameof(model.EscortDetails.SubCategoryId)}" });
                }
            }

            if (errors.Count > 0) { throw new DirtyFormDataException(); }

            return command;
        }


        /// <summary>
        /// populates <paramref name="vm"/> for postback
        /// </summary>
        /// <param name="vm"></param>
        public void PopulateGenerateEscortRequestVMForPostback(PSSBranchGenerateEscortRequestVM vm)
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
        /// Gets commands for command type with specified id
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        public APIResponse GetCommandsForCommandTypeWithId(string commandTypeId)
        {
            try
            {
                int commandTypeIdParsed = 0;
                if (!string.IsNullOrEmpty(commandTypeId) && int.TryParse(commandTypeId, out commandTypeIdParsed))
                {
                    return new APIResponse { ResponseObject = _coreCommandService.Value.GetCommandsForCommandTypeWithId(commandTypeIdParsed) };
                }
                Logger.Error("Command type Id not specified");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text };
        }


        /// <summary>
        /// Gets next level commands using specified code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public APIResponse GetNextLevelCommandsWithCode(string code)
        {
            try
            {
                if (!string.IsNullOrEmpty(code))
                {
                    return new APIResponse { ResponseObject = _coreCommandService.Value.GetNextLevelCommandsWithCode(code) };
                }
                Logger.Error("Code not specified");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text };
        }


        /// <summary>
        /// Gets area and divisional commands using stateId and optionally specified lgaId
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        public APIResponse GetAreaAndDivisionalCommandsByStateAndLGA(string stateId, string lgaId)
        {
            try
            {
                int stateIdParsed = 0;
                int lgaIdParsed = 0;
                if (!string.IsNullOrEmpty(stateId) && int.TryParse(stateId, out stateIdParsed))
                {
                    if (!string.IsNullOrEmpty(lgaId)) { int.TryParse(lgaId, out lgaIdParsed); }
                    return new APIResponse { ResponseObject = _coreCommandService.Value.GetAreaAndDivisionalCommandsByStateAndLGA(stateIdParsed, lgaIdParsed) };
                }
                Logger.Error("State Id not specified");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text };
        }
    }
}