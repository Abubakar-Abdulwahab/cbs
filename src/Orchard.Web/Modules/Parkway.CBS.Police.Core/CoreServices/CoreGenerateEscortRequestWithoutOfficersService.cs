using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.Utilities;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreGenerateEscortRequestWithoutOfficersService : ICoreGenerateEscortRequestWithoutOfficersService
    {
        private readonly Lazy<ITypeImplComposer> _compositionHandler;
        private readonly Lazy<IPSServiceManager<PSService>> _iPSServiceManager;
        private readonly Lazy<ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation>> _iCBSUserTaxEntityProfileLocationManager;
        private readonly Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> _escortManager;
        private readonly Lazy<IPSSRequestManager<PSSRequest>> _requestManager;
        private readonly Lazy<IPSSEscortSettingsManager<PSSEscortSettings>> _escortSettings;
        private readonly Lazy<IEscortAmountChartSheetManager<EscortAmountChartSheet>> _escortAmountChartSheetManager;
        private readonly Lazy<IPSSAdminUsersManager<PSSAdminUsers>> _adminUsersManager;
        private readonly Lazy<ICommandManager<Command>> _commandManager;
        private readonly IOrchardServices _orchardServices;
        private readonly IGenerateRequestWithoutOfficersUploadBatchStagingManager<GenerateRequestWithoutOfficersUploadBatchStaging> _generateRequestWithoutOfficersUploadBatchStagingManager;
        private readonly ICBSUserManager<CBSUser> _cbsUserManager;
        private readonly IGenerateRequestWithoutOfficersUploadBatchItemsStagingManager<GenerateRequestWithoutOfficersUploadBatchItemsStaging> _generateRequestWithoutOfficersUploadBatchItemsStagingManager;
        private readonly IPSSRegularizationUnknownOfficerRecurringInvoiceSettingsManager<PSSRegularizationUnknownOfficerRecurringInvoiceSettings> _unknownOfficerRecurringInvoiceSettingsManager;
        private readonly IPSSProposedRegularizationUnknownPoliceOfficerDeploymentLogManager<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog> _proposedRegularizationUnknownPoliceOfficerDeploymentLogManager;
        private readonly ICronGenerator _cronBaker;
        ILogger Logger { get; set; }
        private const int weeklyDay = 7;
        private const int monthlyDay = 30;


        public CoreGenerateEscortRequestWithoutOfficersService(Lazy<ITypeImplComposer> compositionHandler, Lazy<IPSServiceManager<PSService>> iPSServiceManager, Lazy<ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation>> iCBSUserTaxEntityProfileLocationManager, Lazy<IPSSRequestManager<PSSRequest>> requestManager, IOrchardServices orchardServices, Lazy<IPSSEscortSettingsManager<PSSEscortSettings>> escortSettings, Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> escortManager, Lazy<IPSSAdminUsersManager<PSSAdminUsers>> adminUsersManager, Lazy<ICommandManager<Command>> commandManager,  Lazy<IEscortAmountChartSheetManager<EscortAmountChartSheet>> escortAmountChartSheetManager, IGenerateRequestWithoutOfficersUploadBatchStagingManager<GenerateRequestWithoutOfficersUploadBatchStaging> generateRequestWithoutOfficersUploadBatchStagingManager, ICBSUserManager<CBSUser> cbsUserManager, IGenerateRequestWithoutOfficersUploadBatchItemsStagingManager<GenerateRequestWithoutOfficersUploadBatchItemsStaging> generateRequestWithoutOfficersUploadBatchItemsStagingManager, ICronGenerator cronBaker, IPSSRegularizationUnknownOfficerRecurringInvoiceSettingsManager<PSSRegularizationUnknownOfficerRecurringInvoiceSettings> unknownOfficerRecurringInvoiceSettingsManager, IPSSProposedRegularizationUnknownPoliceOfficerDeploymentLogManager<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog> proposedRegularizationUnknownPoliceOfficerDeploymentLogManager)
        {
            _compositionHandler = compositionHandler;
            _iPSServiceManager = iPSServiceManager;
            _iCBSUserTaxEntityProfileLocationManager = iCBSUserTaxEntityProfileLocationManager;
            _requestManager = requestManager;
            _escortSettings = escortSettings;
            _escortManager = escortManager;
            _adminUsersManager = adminUsersManager;
            _escortAmountChartSheetManager = escortAmountChartSheetManager;
            _commandManager = commandManager;
            _generateRequestWithoutOfficersUploadBatchStagingManager = generateRequestWithoutOfficersUploadBatchStagingManager;
            _generateRequestWithoutOfficersUploadBatchItemsStagingManager = generateRequestWithoutOfficersUploadBatchItemsStagingManager;
            _cbsUserManager = cbsUserManager;
            _orchardServices = orchardServices;
            _unknownOfficerRecurringInvoiceSettingsManager = unknownOfficerRecurringInvoiceSettingsManager;
            _proposedRegularizationUnknownPoliceOfficerDeploymentLogManager = proposedRegularizationUnknownPoliceOfficerDeploymentLogManager;
            _cronBaker = cronBaker;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Generates request for unknown officers for default branch if <paramref name="isDefaultBranch"/> is true else a branch
        /// </summary>
        /// <param name="requestVM"></param>
        /// <param name="batchId"></param>
        /// <param name="isDefaultBranch"></param>
        /// <returns></returns>
        public InvoiceGenerationResponse GenerateRequestForUnknownOfficers(EscortRequestVM requestVM, long batchId, bool isDefaultBranch = false)
        {
            try
            {
                //get expert system
                IEnumerable<ExpertSystemVM> expertSystem = _compositionHandler.Value.GetExpertSystem();

                //get service
                PSServiceVM service = _iPSServiceManager.Value.GetServiceWithServiceType(PSSServiceTypeDefinition.EscortRegularization);
                requestVM.HasDifferentialWorkFlow = service.HasDifferentialWorkFlow;

                //set number of officers
                requestVM.NumberOfOfficers = _generateRequestWithoutOfficersUploadBatchItemsStagingManager.GetTotalNumberOfRequestedOfficersInBatch(batchId);

                //get last invoice generation workflow level
                PSServiceRequestFlowDefinitionLevelDTO serviceFeeInvoiceGenerationLevel = _compositionHandler.Value.GetLastFlowLevelWithWorkflowActionValue(service.ServiceId, requestVM.HasDifferentialWorkFlow, new ServiceWorkFlowDifferentialDataParam { DifferentialModelName = nameof(CommandType), DifferentialValue = requestVM.SelectedCommandType }, RequestDirection.GenerateInvoice);

                //get expected hash
                requestVM.ExpectedHash = _compositionHandler.Value.GetExpectedHash(service.ServiceId, serviceFeeInvoiceGenerationLevel.Id);

                //get revenue head for definition level
                IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeads = _compositionHandler.Value.GetRevenueHeadDetails(service.ServiceId, serviceFeeInvoiceGenerationLevel.Id);
                if (serviceRevenueHeads == null || !serviceRevenueHeads.Any())
                {
                    throw new NoBillingInformationFoundException("No billing info found for service Id " + service.ServiceId);
                }

                var batchTaxEntityProfileLocation = _generateRequestWithoutOfficersUploadBatchStagingManager.GetTaxEntityProfileLocationIdAndTaxEntityIdForBatch(batchId);

                CBSUserVM cbsUser = null;
                if (isDefaultBranch)
                {
                    //get admin user
                    cbsUser = _cbsUserManager.GetAdminCBSUserWithTaxEntityId(batchTaxEntityProfileLocation.TaxEntityProfileLocation.TaxEntity.Id);
                }
                else
                {
                    //get sub user
                    cbsUser = _iCBSUserTaxEntityProfileLocationManager.Value.GetSubUserInLocation(batchTaxEntityProfileLocation.TaxEntityProfileLocation.Id);
                }

                if(cbsUser == null)
                {
                    throw new Exception($"No cbs user found for tax entity profile location with id {batchTaxEntityProfileLocation.TaxEntityProfileLocation.Id}");
                }

                //get pss request model
                PSSRequest request = GetPSSRequestModel(requestVM, service, cbsUser, serviceFeeInvoiceGenerationLevel, batchTaxEntityProfileLocation.TaxEntityProfileLocation.Id);

                //save pss request model
                if (!_requestManager.Value.Save(request)) { throw new CouldNotSaveRecord("Could not save request record"); }

                //get escort details model
                PSSEscortDetails escortDetails = GetPSSEscortDetailsModel(request, requestVM, serviceFeeInvoiceGenerationLevel.DefinitionId);

                //get and save escort details model
                if (!_escortManager.Value.Save(escortDetails)){ throw new CouldNotSaveRecord("Could not save escort details"); }

                if(!_unknownOfficerRecurringInvoiceSettingsManager.Save(GetPSSRegularizationUnknownOfficerRecurringInvoiceSettings(request, escortDetails, requestVM.PSBillingTypeDurationNumber, batchId))) 
                { throw new CouldNotSaveRecord($"Could not save pss regularization unknown officer recurring invoice settings record for GenerateRequestWithoutOfficersUploadBatchStaging with id {batchId}"); }

                decimal rate = 0.00m;

                //compute amount for unknown officers
                List<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog> proposedDeploymentLogs = new List<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog>();
                ComputeAmountForUnknownOfficers(requestVM, batchId, request, ref rate, ref proposedDeploymentLogs);

                string fileRefNumber = _compositionHandler.Value.GetRequestFileRefNumber(request);

                PSServiceRevenueHeadVM parentServicerevenueHead = serviceRevenueHeads.Count() > 1 ? serviceRevenueHeads.Where(r => r.IsGroupHead).Single() : serviceRevenueHeads.ElementAt(0);

                string callbackParam = _compositionHandler.Value.GetURLRequestTokenString(request.Id, requestVM.ExpectedHash);

                //get generate invoice input model
                CreateInvoiceUserInputModel inputModel = GetInvoiceUserInputModel(parentServicerevenueHead, request.Id, requestVM, serviceRevenueHeads, cbsUser.TaxEntity.CategoryId, fileRefNumber, callbackParam, rate);

                //generate invoice
                InvoiceGenerationResponse response = _compositionHandler.Value.GenerateInvoice(inputModel, expertSystem.ElementAt(0), new TaxEntityViewModel { Id = cbsUser.TaxEntity.Id });

                //save request and invoice to PSSRequestInvoice
                _compositionHandler.Value.AddRequestAndInvoice(request, response.InvoiceId);

                //add invoice id to proposed deployment logs
                proposedDeploymentLogs.ForEach(log => log.Invoice = new Invoice { Id = response.InvoiceId });

                //save proposed deployment logs
                if (!_proposedRegularizationUnknownPoliceOfficerDeploymentLogManager.SaveBundleUnCommit(proposedDeploymentLogs))
                {
                    throw new CouldNotSaveRecord("Could not save regularization deployment records for batch Id " + batchId);
                }

                //add to request status log
                RequestStatusLog statusLog = new RequestStatusLog
                {
                    Invoice = new Invoice { Id = response.InvoiceId },
                    StatusDescription = serviceFeeInvoiceGenerationLevel.PositionDescription,
                    Request = new PSSRequest { Id = request.Id },
                    FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = serviceFeeInvoiceGenerationLevel.Id },
                    UserActionRequired = true,
                    Status = (int)PSSRequestStatus.PendingInvoicePayment
                };

                _compositionHandler.Value.AddRequestStatusLog(statusLog);

                //add to request command workflow log
                _compositionHandler.Value.AddRequestCommandWorkFlowLog(new RequestCommandWorkFlowLog { Request = new PSSRequest { Id = request.Id }, Command = new Command { Id = requestVM.SelectedCommand }, DefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = serviceFeeInvoiceGenerationLevel.Id }, IsActive = false, RequestPhaseId = (int)RequestPhase.New, RequestPhaseName = nameof(RequestPhase.New) });

                //save service request
                _compositionHandler.Value.SaveServiceRequest(request, serviceRevenueHeads, service.ServiceId, response.InvoiceId, serviceFeeInvoiceGenerationLevel.Id, PSSRequestStatus.PendingInvoicePayment);

                return response;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _requestManager.Value.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Gets invoice user input model
        /// </summary>
        /// <param name="parentServiceRevenueHead"></param>
        /// <param name="requestId"></param>
        /// <param name="requestVM"></param>
        /// <param name="result"></param>
        /// <param name="categoryId"></param>
        /// <param name="fileRefNumber"></param>
        /// <param name="callbackParam"></param>
        /// <returns></returns>
        private CreateInvoiceUserInputModel GetInvoiceUserInputModel(PSServiceRevenueHeadVM parentServiceRevenueHead, long requestId, EscortRequestVM requestVM, IEnumerable<PSServiceRevenueHeadVM> result, int categoryId, string fileRefNumber, string callbackParam, decimal escortFee)
        {
            return new CreateInvoiceUserInputModel
            {
                GroupId = parentServiceRevenueHead.IsGroupHead ? parentServiceRevenueHead.RevenueHeadId : 0,
                InvoiceTitle = parentServiceRevenueHead.FeeDescription,
                InvoiceDescription = string.Format("{0} for {1} {2}. Duration {3}. Number of Officers {4}.", parentServiceRevenueHead.FeeDescription, parentServiceRevenueHead.ServiceName, fileRefNumber, string.Format("{0} - {1}", requestVM.ParsedStartDate.ToString("dd'/'MM'/'yyyy"), requestVM.ParsedEndDate.ToString("dd'/'MM'/'yyyy")), requestVM.NumberOfOfficers),
                CallBackURL = PSSUtil.GetURLForFeeConfirmation(_orchardServices.WorkContext.CurrentSite.SiteName, callbackParam),
                TaxEntityCategoryId = categoryId,
                AddSurcharge = true,
                RevenueHeadModels = result.Where(r => !r.IsGroupHead).Select(r =>
                new RevenueHeadUserInputModel
                {
                    AdditionalDescription = string.Format("{0} for {1} {2}", r.FeeDescription, r.ServiceName, fileRefNumber),
                    Amount = escortFee,
                    Quantity = 1,
                    RevenueHeadId = r.RevenueHeadId
                }).ToList()
            };
        }


        /// <summary>
        /// Gets PSSRequest model
        /// </summary>
        /// <param name="requestVM"></param>
        /// <param name="service"></param>
        /// <param name="subUser"></param>
        /// <param name="serviceFeeInvoiceGenerationLevel"></param>
        /// <param name="taxEntityProfileLocationId"></param>
        /// <returns></returns>
        private PSSRequest GetPSSRequestModel(EscortRequestVM requestVM, PSServiceVM service, CBSUserVM subUser, PSServiceRequestFlowDefinitionLevelDTO serviceFeeInvoiceGenerationLevel, int taxEntityProfileLocationId)
        {
            return new PSSRequest
            {
                Command = new Command { Id = requestVM.SelectedCommand },
                TaxEntity = new TaxEntity { Id = subUser.TaxEntity.Id },
                Service = new PSService { Id = service.ServiceId },
                Status = (int)PSSRequestStatus.PendingInvoicePayment,
                ServicePrefix = service.ServicePrefix,
                FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = serviceFeeInvoiceGenerationLevel.Id },
                ExpectedHash = requestVM.ExpectedHash,
                CBSUser = new CBSUser { Id = subUser.Id },
                TaxEntityProfileLocation = new TaxEntityProfileLocation { Id = taxEntityProfileLocationId }
            };
        }


        /// <summary>
        /// Gets PSSEscortDetails model
        /// </summary>
        /// <param name="request"></param>
        /// <param name="requestVM"></param>
        /// <param name="workFlowDefinitionId"></param>
        /// <returns></returns>
        private PSSEscortDetails GetPSSEscortDetailsModel(PSSRequest request, EscortRequestVM requestVM, int workFlowDefinitionId)
        {
            //get escort settings
            int escortSettingsId = _escortSettings.Value.GetEscortSettingsId(workFlowDefinitionId);
            if (escortSettingsId < 1) { throw new NoRecordFoundException("No escort settings found"); }

            return
                    new PSSEscortDetails
                    {
                        Request = new PSSRequest { Id = request.Id },
                        NumberOfOfficers = requestVM.NumberOfOfficers,
                        DurationNumber = requestVM.DurationNumber,
                        Address = requestVM.Address,
                        StartDate = requestVM.ParsedStartDate,
                        EndDate = requestVM.ParsedEndDate,
                        LGA = new LGA { Id = requestVM.SelectedStateLGA },
                        State = new StateModel { Id = requestVM.SelectedState },
                        PaymentPeriodType = (int)requestVM.PSBillingType,
                        TaxEntitySubCategory = new TaxEntitySubCategory { Id = requestVM.SubCategoryId },
                        TaxEntitySubSubCategory = requestVM.SubSubCategoryId == 0 ? null : new TaxEntitySubSubCategory { Id = requestVM.SubSubCategoryId },
                        Settings = new PSSEscortSettings { Id = escortSettingsId },
                        ServiceCategory = new PSSEscortServiceCategory { Id = requestVM.SelectedEscortServiceCategories.ElementAtOrDefault(0) },
                        CategoryType = (requestVM.SelectedEscortServiceCategories.ElementAtOrDefault(1) > 0) ? new PSSEscortServiceCategory { Id = requestVM.SelectedEscortServiceCategories.ElementAtOrDefault(1) } : null,
                        OriginState = (requestVM.ShowExtraFieldsForServiceCategoryType) ? new StateModel { Id = requestVM.SelectedOriginState } : null,
                        OriginLGA = (requestVM.ShowExtraFieldsForServiceCategoryType) ? new LGA { Id = requestVM.SelectedOriginLGA } : null,
                        OriginAddress = (requestVM.ShowExtraFieldsForServiceCategoryType) ? requestVM.AddressOfOriginLocation : null,
                        CommandType = new CommandType { Id = requestVM.SelectedCommandType },
                        OfficersHaveBeenAssigned = true
                    };
        }


        /// <summary>
        /// Gets PSSRegularizationUnknownOfficerRecurringInvoiceSettings model
        /// </summary>
        /// <param name="request"></param>
        /// <param name="escortDetails"></param>
        /// <param name="weekDayNumber"></param>
        /// <param name="batchId"></param>
        /// <returns></returns>
        private PSSRegularizationUnknownOfficerRecurringInvoiceSettings GetPSSRegularizationUnknownOfficerRecurringInvoiceSettings(PSSRequest request, PSSEscortDetails escortDetails, int weekDayNumber, long batchId)
        {
            return new PSSRegularizationUnknownOfficerRecurringInvoiceSettings
            {
                GenerateRequestWithoutOfficersUploadBatchStaging = new GenerateRequestWithoutOfficersUploadBatchStaging { Id = batchId },
                EscortDetails = escortDetails,
                Request = request,
                WeekDayNumber = (escortDetails.PaymentPeriodType == (int)PSBillingType.Weekly) ? weekDayNumber : 30,
                OffSet = (escortDetails.PaymentPeriodType == (int)PSBillingType.Weekly) ? (7 - weekDayNumber) : 0,
                PaymentBillingType = escortDetails.PaymentPeriodType,
                CronExpression = GenerateCronExp((PSBillingType)escortDetails.PaymentPeriodType, escortDetails.StartDate),
                NextInvoiceGenerationDate = escortDetails.PaymentPeriodType == (int)PSBillingType.Weekly? Util.GetNextDate(GenerateCronExp((PSBillingType)escortDetails.PaymentPeriodType, escortDetails.StartDate), escortDetails.StartDate).Value : escortDetails.StartDate.AddDays(monthlyDay)
            };
        }


        /// <summary>
        /// Compuutes amount for unknown officers
        /// </summary>
        /// <param name="requestVM"></param>
        /// <param name="batchId"></param>
        /// <param name="sumTotalRate"></param>
        private void ComputeAmountForUnknownOfficers(EscortRequestVM requestVM, long batchId, PSSRequest request, ref decimal sumTotalRate, ref List<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog> deploymentLogs)
        {
            IEnumerable<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO> items = _generateRequestWithoutOfficersUploadBatchItemsStagingManager.GetItems(batchId);
            if(items == null || !items.Any())
            {
                throw new Exception($"No GenerateRequestWithoutOfficersUploadBatchItemsStaging found for batch with id {batchId}");
            }

            foreach(var item in items)
            {
                decimal rate = _escortAmountChartSheetManager.Value.GetRateForUnknownOfficer(item.CommandType, item.DayType);
                if (rate == 0) { throw new Exception($"Escort rank rate not configured in chart sheet for day type {item.DayType} and command type with id {item.CommandType}"); }
                sumTotalRate += GetDuration(requestVM) * item.NumberOfOfficers * rate;
                deploymentLogs.Add(new PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog { Request = request, DeploymentRate = rate, GenerateRequestWithoutOfficersUploadBatchItemsStaging = new GenerateRequestWithoutOfficersUploadBatchItemsStaging { Id = item.Id }, IsActive = true, StartDate = requestVM.ParsedStartDate, EndDate = requestVM.ParsedStartDate.AddDays((requestVM.PSBillingType == PSBillingType.Weekly) ? weeklyDay : monthlyDay).AddDays(-1) });
            }
        }


        private string GenerateCronExp(PSBillingType billingType, DateTime startDate)
        {
            //cron expression for weekly every {DateTime.Now.DayOfWeek} of the week
            string weeklyCron = $"0 0 0 ? * {startDate.DayOfWeek.ToString().Substring(0, 3)} *";
            //cron expression for monthly every first day of the month
            string monthlyCron = $"0 0 0 {startDate.Day} 1/1 ? *";
            return (billingType == PSBillingType.Weekly) ? weeklyCron : monthlyCron;
        }


        private int GetDuration(EscortRequestVM requestVM)
        {
            return (requestVM.PSBillingType == PSBillingType.Weekly) ? requestVM.PSBillingTypeDurationNumber : monthlyDay;
        }
    }
}