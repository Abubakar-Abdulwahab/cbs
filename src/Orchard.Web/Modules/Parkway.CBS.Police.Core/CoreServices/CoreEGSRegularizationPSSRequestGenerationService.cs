using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Core.StateConfig;
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
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreEGSRegularizationPSSRequestGenerationService : ICoreEGSRegularizationPSSRequestGenerationService
    {
        private readonly Lazy<ITypeImplComposer> _compositionHandler;
        private readonly Lazy<IPSServiceManager<PSService>> _iPSServiceManager;
        private readonly Lazy<ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation>> _iCBSUserTaxEntityProfileLocationManager;
        private readonly Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> _escortManager;
        private readonly Lazy<IPSSRequestManager<PSSRequest>> _requestManager;
        private readonly Lazy<IPSSEscortSettingsManager<PSSEscortSettings>> _escortSettings;
        private readonly Lazy<IEscortFormationOfficerManager<EscortFormationOfficer>> _escortFormationOfficerManager;
        private readonly Lazy<IEscortFormationAllocationManager<EscortFormationAllocation>> _escortFormationAllocationManager;
        private readonly Lazy<IEscortProcessFlowManager<EscortProcessFlow>> _escortProcessFlowManager;
        private readonly Lazy<IEscortSquadAllocationGroupManager<EscortSquadAllocationGroup>> _escortSquadAllocationGroupManager;
        private readonly Lazy<IEscortAmountChartSheetManager<EscortAmountChartSheet>> _escortAmountChartSheetManager;
        private readonly Lazy<IPSSBranchOfficersUploadBatchItemsStagingManager<PSSBranchOfficersUploadBatchItemsStaging>> _officersUploadBatchItemsStagingManager;
        private readonly Lazy<IEscortSquadAllocationManager<EscortSquadAllocation>> _escortSquadAllocationManager;
        private readonly Lazy<IPSSAdminUsersManager<PSSAdminUsers>> _adminUsersManager;
        private readonly Lazy<ICommandManager<Command>> _commandManager;
        private readonly Lazy<IPolicerOfficerLogManager<PolicerOfficerLog>> _policerOfficerLogManager;
        private readonly IOrchardServices _orchardServices;
        ILogger Logger { get; set; }

        public CoreEGSRegularizationPSSRequestGenerationService(Lazy<ITypeImplComposer> compositionHandler, Lazy<IPSServiceManager<PSService>> iPSServiceManager, Lazy<ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation>> iCBSUserTaxEntityProfileLocationManager, Lazy<IPSSRequestManager<PSSRequest>> requestManager, IOrchardServices orchardServices, Lazy<IPSSEscortSettingsManager<PSSEscortSettings>> escortSettings, Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> escortManager, Lazy<IEscortFormationOfficerManager<EscortFormationOfficer>> escortFormationOfficerManager, Lazy<IPSSAdminUsersManager<PSSAdminUsers>> adminUsersManager, Lazy<IEscortProcessFlowManager<EscortProcessFlow>> escortProcessFlowManager, Lazy<IEscortSquadAllocationGroupManager<EscortSquadAllocationGroup>> escortSquadAllocationGroupManager, Lazy<IEscortFormationAllocationManager<EscortFormationAllocation>> escortFormationAllocationManager, Lazy<IPSSBranchOfficersUploadBatchItemsStagingManager<PSSBranchOfficersUploadBatchItemsStaging>> officersUploadBatchItemsStagingManager, Lazy<ICommandManager<Command>> commandManager, Lazy<IEscortSquadAllocationManager<EscortSquadAllocation>> escortSquadAllocationManager, Lazy<IEscortAmountChartSheetManager<EscortAmountChartSheet>> escortAmountChartSheetManager, Lazy<IPolicerOfficerLogManager<PolicerOfficerLog>> policerOfficerLogManager)
        {
            _compositionHandler = compositionHandler;
            _iPSServiceManager = iPSServiceManager;
            _iCBSUserTaxEntityProfileLocationManager = iCBSUserTaxEntityProfileLocationManager;
            _requestManager = requestManager;
            _escortSettings = escortSettings;
            _escortManager = escortManager;
            _escortFormationOfficerManager = escortFormationOfficerManager;
            _escortFormationAllocationManager = escortFormationAllocationManager;
            _escortProcessFlowManager = escortProcessFlowManager;
            _escortSquadAllocationGroupManager = escortSquadAllocationGroupManager;
            _officersUploadBatchItemsStagingManager = officersUploadBatchItemsStagingManager;
            _escortSquadAllocationManager = escortSquadAllocationManager;
            _adminUsersManager = adminUsersManager;
            _escortAmountChartSheetManager = escortAmountChartSheetManager;
            _commandManager = commandManager;
            _policerOfficerLogManager = policerOfficerLogManager;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Generates a request using details in <paramref name="requestVM"/> for the configured duration and tax entity profile location <paramref name="taxEntityProfileLocationId"/>
        /// using officers in batch with id <paramref name="PSSBranchOfficersUploadBatchStagingId"/>
        /// </summary>
        /// <param name="requestVM"></param>
        /// <param name="taxEntityProfileLocationId"></param>
        /// <param name="PSSBranchOfficersUploadBatchStagingId"></param>
        /// <returns></returns>
        public InvoiceGenerationResponse GenerateTimeSpecificRequest(EscortRequestVM requestVM, int taxEntityProfileLocationId, long PSSBranchOfficersUploadBatchStagingId)
        {
            try
            {
                IEnumerable<ExpertSystemVM> expertSystem = _compositionHandler.Value.GetExpertSystem();
                PSServiceVM service = _iPSServiceManager.Value.GetServiceWithServiceType(PSSServiceTypeDefinition.Escort);
                requestVM.HasDifferentialWorkFlow = service.HasDifferentialWorkFlow;
                requestVM.NumberOfOfficers = _officersUploadBatchItemsStagingManager.Value.Count(x => x.PSSBranchOfficersUploadBatchStaging.Id == PSSBranchOfficersUploadBatchStagingId && !x.HasError);
                PSServiceRequestFlowDefinitionLevelDTO serviceFeeInvoiceGenerationLevel = _compositionHandler.Value.GetLastFlowLevelWithWorkflowActionValue(service.ServiceId, requestVM.HasDifferentialWorkFlow, new ServiceWorkFlowDifferentialDataParam { DifferentialModelName = nameof(CommandType), DifferentialValue = requestVM.SelectedCommandType }, RequestDirection.GenerateInvoice);

                requestVM.ExpectedHash = _compositionHandler.Value.GetExpectedHash(service.ServiceId, serviceFeeInvoiceGenerationLevel.Id);

                //get revenue head for definition level
                IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeads = _compositionHandler.Value.GetRevenueHeadDetails(service.ServiceId, serviceFeeInvoiceGenerationLevel.Id);
                if (serviceRevenueHeads == null || !serviceRevenueHeads.Any())
                {
                    throw new NoBillingInformationFoundException("No billing info found for service Id " + service.ServiceId);
                }

                CBSUserVM subUser = _iCBSUserTaxEntityProfileLocationManager.Value.GetSubUserInLocation(taxEntityProfileLocationId);

                //set parsed start date, parsed end date and number of days
                SetPSSRequestDuration(requestVM);

                //get pss request model
                PSSRequest request = GetPSSRequestModel(requestVM, service, subUser, serviceFeeInvoiceGenerationLevel, taxEntityProfileLocationId);

                //save pss request model
                if (!_requestManager.Value.Save(request)) { throw new CouldNotSaveRecord("Could not save request record"); }

                //get and save escort details model
                if (!_escortManager.Value.Save(GetPSSEscortDetailsModel(request, requestVM, serviceFeeInvoiceGenerationLevel.DefinitionId)))
                { throw new CouldNotSaveRecord("Could not save escort details"); }

                decimal rate = 0.00m;

                //perform escort formation officer allocations
                PerformEscortFormationOfficerAllocations(requestVM, request, _orchardServices.WorkContext.CurrentUser.Id, service, PSSBranchOfficersUploadBatchStagingId, ref rate);

                string fileRefNumber = _compositionHandler.Value.GetRequestFileRefNumber(request);

                PSServiceRevenueHeadVM parentServicerevenueHead = serviceRevenueHeads.Count() > 1 ? serviceRevenueHeads.Where(r => r.IsGroupHead).Single() : serviceRevenueHeads.ElementAt(0);

                string callbackParam = _compositionHandler.Value.GetURLRequestTokenString(request.Id, requestVM.ExpectedHash);

                //get generate invoice input model
                CreateInvoiceUserInputModel inputModel = GetInvoiceUserInputModel(parentServicerevenueHead, request.Id, requestVM, serviceRevenueHeads, subUser.TaxEntity.CategoryId, fileRefNumber, callbackParam, rate);

                //generate invoice
                InvoiceGenerationResponse response = _compositionHandler.Value.GenerateInvoice(inputModel, expertSystem.ElementAt(0), new TaxEntityViewModel { Id = subUser.TaxEntity.Id });

                //save request and invoice to PSSRequestInvoice
                _compositionHandler.Value.AddRequestAndInvoice(request, response.InvoiceId);

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
        /// Set request start date, end date and duration values
        /// </summary>
        /// <param name="requestVM"></param>
        private void SetPSSRequestDuration(EscortRequestVM requestVM)
        {
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            Node invoiceGenerationPeriodMonthNode = siteConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.PSSEGSRegularizationInvoiceGenerationPeriodMonthValue)).FirstOrDefault();
            Node invoiceGenerationPeriodYearNode = siteConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.PSSEGSRegularizationInvoiceGenerationPeriodYearValue)).FirstOrDefault();

            if (string.IsNullOrEmpty(invoiceGenerationPeriodMonthNode?.Value) || string.IsNullOrEmpty(invoiceGenerationPeriodYearNode?.Value) || !int.TryParse(invoiceGenerationPeriodMonthNode.Value, out int invoiceGenerationPeriodMonthValue) || !int.TryParse(invoiceGenerationPeriodYearNode.Value, out int invoiceGenerationPeriodYearValue))
            {
                Logger.Error("Unable to get EGS Regularization invoice generation period month and year values in config file");
                invoiceGenerationPeriodMonthValue = DateTime.Now.Month;
                invoiceGenerationPeriodYearValue = DateTime.Now.Year;

            }

            int numberOfDays = DateTime.DaysInMonth(invoiceGenerationPeriodYearValue, invoiceGenerationPeriodMonthValue);

            requestVM.DurationNumber = numberOfDays;
            requestVM.ParsedStartDate = new DateTime(invoiceGenerationPeriodYearValue, invoiceGenerationPeriodMonthValue, 1);
            requestVM.ParsedEndDate = new DateTime(invoiceGenerationPeriodYearValue, invoiceGenerationPeriodMonthValue, numberOfDays);
        }


        /// <summary>
        /// Create escort squad allocation group and perform squad allocations, formation allocations and formation officer allocations
        /// </summary>
        /// <param name="requestVM"></param>
        /// <param name="request"></param>
        /// <param name="userPartRecordId"></param>
        /// <param name="service"></param>
        /// <param name="batchId"></param>
        private void PerformEscortFormationOfficerAllocations(EscortRequestVM requestVM, PSSRequest request, int userPartRecordId, PSServiceVM service, long batchId, ref decimal sumTotalRate)
        {
            int adminUserId = _adminUsersManager.Value.GetAdminUserId(userPartRecordId);

            EscortProcessFlowDTO adminProcessFlowObj = _escortProcessFlowManager.Value.GetProcessFlowObject(userPartRecordId, requestVM.SelectedCommandType).FirstOrDefault();
            if (adminProcessFlowObj == null) { throw new Exception($"No escort process flow configured for admin with user part record id {userPartRecordId} for command type with id {requestVM.SelectedCommandType}"); }

            //create squad allocation group
            EscortSquadAllocationGroup squadAllocGroupModel = new EscortSquadAllocationGroup { AdminUser = new PSSAdminUsers { Id = adminUserId }, Comment = "EGS Regularization", Request = request, RequestLevel = new EscortProcessStageDefinition { Id = adminProcessFlowObj.LevelId }, Service = new PSService { Id = service.ServiceId }, StatusDescription = "EGS Regularization", Fulfilled = true };

            if (!_escortSquadAllocationGroupManager.Value.Save(squadAllocGroupModel))
            {
                throw new CouldNotSaveRecord($"Unable to create escort squad allocation for request with id {request.Id}");
            }

            IEnumerable<PSSBranchOfficersUploadBatchItemsStagingDTO> pssBranchOfficers = _officersUploadBatchItemsStagingManager.Value.GetItemsInBatchWithId(batchId);
            if (pssBranchOfficers == null || !pssBranchOfficers.Any()) { throw new NoRecordFoundException($"No PSS Branch Officers Upload Batch items found in batch with id {batchId}"); }
            //perform squad allocations
            foreach(var squadGroup in pssBranchOfficers.GroupBy(x => x.OfficerCommand.ParentCode))
            {
                CommandVM parentCommand = _commandManager.Value.GetCommandWithCode(squadGroup.Key);

                EscortSquadAllocation squad = new EscortSquadAllocation
                {
                    AllocationGroup = new EscortSquadAllocationGroup { Id = squadAllocGroupModel.Id },
                    Fulfilled = true,
                    StatusDescription = "Squad Allocated For EGS Regularization",
                    Command = new Command { Id = parentCommand.Id },
                    CommandType = new CommandType { Id = requestVM.SelectedCommandType },
                    NumberOfOfficers = squadGroup.Count()
                };

                if (!_escortSquadAllocationManager.Value.Save(squad))
                {
                    throw new CouldNotSaveRecord($"Unable to create escort squad allocation for command with id {parentCommand.Id}");
                }

                //perform formation allocations
                foreach (var formationGroup in squadGroup.GroupBy(x => x.OfficerCommand.Id))
                {
                    EscortFormationAllocation formation = new EscortFormationAllocation
                    {
                        Group = new EscortSquadAllocationGroup { Id = squadAllocGroupModel.Id },
                        EscortSquadAllocation = new EscortSquadAllocation { Id = squad.Id },
                        AllocatedByAdminUser = new PSSAdminUsers { Id = adminUserId },
                        State = new StateModel { Id = formationGroup.FirstOrDefault().OfficerCommand.StateId },
                        LGA = new LGA { Id = formationGroup.FirstOrDefault().OfficerCommand.LGAId },
                        Command = new Command { Id = formationGroup.Key },
                        NumberOfOfficers = formationGroup.Count(),
                        StatusDescription = "Formation Allocated For EGS Regularization",
                        Fulfilled = true,
                        NumberAssignedByCommander = formationGroup.Count()
                    };

                    if (!_escortFormationAllocationManager.Value.Save(formation))
                    {
                        throw new CouldNotSaveRecord($"Unable to create escort formation allocation for command with id {formationGroup.Key}");
                    }

                    //perform formation officer allocations
                    List<EscortFormationOfficer> formationOfficers = new List<EscortFormationOfficer>();

                    foreach(var officer in formationGroup)
                    {
                        PoliceOfficerLogVM policeOfficerLog = _policerOfficerLogManager.Value.GetPoliceOfficerDetails(officer.APNumber);
                        if (policeOfficerLog == null) 
                        {
                            throw new Exception($"Police Officer log for officer with AP Number {officer.APNumber} not found");
                        }

                        decimal rate = _escortAmountChartSheetManager.Value.GetRateSheetId(policeOfficerLog.RankId, requestVM.SelectedEscortServiceCategories.Last(), requestVM.SelectedState, requestVM.SelectedStateLGA, requestVM.SelectedCommandType);
                        if (rate == 0) { throw new Exception($"Escort rank rate not configured in chart sheet for rank with id {policeOfficerLog.RankId}, state with id {requestVM.SelectedState} and LGA with id {requestVM.SelectedStateLGA}"); }
                        sumTotalRate += (requestVM.DurationNumber * rate);
                        formationOfficers.Add(new EscortFormationOfficer
                        {
                            FormationAllocation = new EscortFormationAllocation { Id = formation.Id },
                            Group = new EscortSquadAllocationGroup { Id = squadAllocGroupModel.Id },
                            PoliceOfficerLog = new PolicerOfficerLog { Id = policeOfficerLog.Id },
                            EscortRankRate = rate
                        });

                    }

                    if (!_escortFormationOfficerManager.Value.SaveBundle(formationOfficers))
                    {
                        throw new CouldNotSaveRecord($"Unable to save escort formation officers for command with id {formationGroup.Key}");
                    }
                }
            }
        }
    }
}