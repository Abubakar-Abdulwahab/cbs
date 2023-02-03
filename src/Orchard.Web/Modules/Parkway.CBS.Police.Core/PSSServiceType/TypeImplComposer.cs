using Orchard;
using Orchard.Logging;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.SMS.Provider.Contracts;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Mail.Provider.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.PSSSMS.Provider.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.Utilities;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.PSSServiceType
{
    public class TypeImplComposer : ITypeImplComposer
    {
        private readonly Lazy<IPSServiceManager<PSService>> _serviceRepo;
        private readonly Lazy<IServiceWorkflowDifferentialManager<ServiceWorkflowDifferential>> _serviceDiffWorkFlow;
        private readonly Lazy<IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel>> _workFlowDef;

        private readonly Lazy<IPSServiceRevenueHeadManager<PSServiceRevenueHead>> _revenueServiceMan;
        private readonly Lazy<IRequestCommandManager<RequestCommand>> _reqCommandMan;
        private readonly Lazy<ICoreInvoiceService> _coreInvoiceService;
        private readonly Lazy<IPSSRequestManager<PSSRequest>> _requestManager;
        private readonly Lazy<IPoliceServiceRequestManager<PoliceServiceRequest>> _serviceRequestManager;
        private readonly Lazy<IAdminSettingManager<ExpertSystemSettings>> _settingsRepository;
        private readonly IEnumerable<Lazy<IPSSEmailProvider>> _pssEmailProvider;
        private readonly IEnumerable<Lazy<ISMSProvider>> _smsProvider;
        private readonly IEnumerable<Lazy<IPSSSMSProvider>> _pssSMSProvider;
        private readonly Lazy<IPSSRequestInvoiceManager<PSSRequestInvoice>> _requestInvoiceService;
        private readonly Lazy<IRequestStatusLogManager<RequestStatusLog>> _requestLogRepo;
        private readonly ITaxEntityProfileLocationManager<TaxEntityProfileLocation> _taxEntityProfileLocationManager;
        private readonly ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation> _cbsUserTaxEntityProfileLocationManager;
        private readonly IOrchardServices _orchardServices;
        private readonly IRequestCommandWorkFlowLogManager<RequestCommandWorkFlowLog> _requestCommandWorkFlowLogManager;
        private readonly Lazy<ICorePoliceOfficerService> _corePoliceOfficerService;
        public ILogger Logger { get; set; }



        public TypeImplComposer(Lazy<IPSServiceRevenueHeadManager<PSServiceRevenueHead>> revenueServiceMan, Lazy<ICoreInvoiceService> coreInvoiceService, Lazy<IPSSRequestManager<PSSRequest>> requestManager, Lazy<IPoliceServiceRequestManager<PoliceServiceRequest>> serviceRequestManager, Lazy<IAdminSettingManager<ExpertSystemSettings>> settingsRepository, Lazy<IPSServiceManager<PSService>> serviceRepo, Lazy<IPSSRequestInvoiceManager<PSSRequestInvoice>> requestInvoiceService, IEnumerable<Lazy<ISMSProvider>> smsProvider, IEnumerable<Lazy<IPSSSMSProvider>> pssSMSProvider, Lazy<IRequestStatusLogManager<RequestStatusLog>> requestLogRepo, IOrchardServices orchardServices, IEnumerable<Lazy<IPSSEmailProvider>> pssEmailProvider, ITaxEntityProfileLocationManager<TaxEntityProfileLocation> taxEntityProfileLocationManager, ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation> cbsUserTaxEntityProfileLocationManager, Lazy<IServiceWorkflowDifferentialManager<ServiceWorkflowDifferential>> serviceDiffWorkFlow, Lazy<IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel>> workFlowDef, Lazy<IRequestCommandManager<RequestCommand>> reqCommandMan, IRequestCommandWorkFlowLogManager<RequestCommandWorkFlowLog> requestCommandWorkFlowLogManager, Lazy<ICorePoliceOfficerService> corePoliceOfficerService)
        {
            _revenueServiceMan = revenueServiceMan;
            _coreInvoiceService = coreInvoiceService;
            _requestManager = requestManager;
            _serviceRequestManager = serviceRequestManager;
            _settingsRepository = settingsRepository;
            _serviceRepo = serviceRepo;
            _requestInvoiceService = requestInvoiceService;
            _smsProvider = smsProvider;
            _orchardServices = orchardServices;
            _requestLogRepo = requestLogRepo;
            _pssEmailProvider = pssEmailProvider;
            _pssSMSProvider = pssSMSProvider;
            _taxEntityProfileLocationManager = taxEntityProfileLocationManager;
            _cbsUserTaxEntityProfileLocationManager = cbsUserTaxEntityProfileLocationManager;
            Logger = NullLogger.Instance;
            _serviceDiffWorkFlow = serviceDiffWorkFlow;
            _workFlowDef = workFlowDef;
            _reqCommandMan = reqCommandMan;
            _requestCommandWorkFlowLogManager = requestCommandWorkFlowLogManager;
            _corePoliceOfficerService = corePoliceOfficerService;
        }


        /// <summary>
        /// Get details for the revenue head associated with this service Id at the give process stage
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="processingFee"></param>
        /// <returns>IEnumerable{PSServiceRevenueHeadVM}</returns>
        public IEnumerable<PSServiceRevenueHeadVM> GetRevenueHeadDetails(int serviceId, int stage)
        {
            return _revenueServiceMan.Value.GetRevenueHead(serviceId, stage);
        }


        /// <summary>
        /// Generate invoice
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="expertSystem"></param>
        /// <param name="entityVM"></param>
        /// <returns>InvoiceGenerationResponse</returns>
        public InvoiceGenerationResponse GenerateInvoice(CreateInvoiceUserInputModel inputModel, ExpertSystemVM expertSystem, TaxEntityViewModel entityVM)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            return _coreInvoiceService.Value.TryGenerateInvoice(inputModel, ref errors, expertSystem, entityVM, null);

        }


        /// <summary>
        /// Get file ref number for reuest
        /// </summary>
        /// <param name="request"></param>
        /// <returns>string</returns>
        public string GetRequestFileRefNumber(PSSRequest request)
        {
            _requestManager.Value.Evict(request);
            string fileRefNumber = _requestManager.Value.GetFileRefNumber(request.Id);
            if (string.IsNullOrEmpty(fileRefNumber)) { throw new NoRecordFoundException("No record for for this request for request " + request.Id); }
            return fileRefNumber;
        }


        /// <summary>
        /// Get root expert system
        /// </summary>
        /// <returns>IEnumerable<ExpertSystemVM></returns>
        public IEnumerable<ExpertSystemVM> GetExpertSystem()
        {
            return _settingsRepository.Value.GetRootExpertSystem();
        }

        /// <summary>
        /// Send email notification
        /// </summary>
        /// <param name="emailDetails"></param>
        public void SendEmailNotification(dynamic emailDetails)
        {
            try
            {
                if (CheckSendEmailNotification((string)emailDetails.Email))
                {
                    int providerId = 0;
                    bool result = Int32.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.EmailProvider), out providerId);
                    if (!result)
                    {
                        providerId = (int)EmailProvider.Pulse;
                    }
                    foreach (var impl in _pssEmailProvider)
                    {
                        if ((EmailProvider)providerId == impl.Value.GetEmailNotificationProvider)
                        {
                            if ((int)emailDetails.ApprovalStatus == (int)PSSRequestStatus.Approved)
                            {
                                impl.Value.PSSRequestApproval(emailDetails);
                            }
                            else if ((int)emailDetails.ApprovalStatus == (int)PSSRequestStatus.Rejected)
                            {
                                impl.Value.PSSRequestRejection(emailDetails);
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }
        }


        /// <summary>
        /// Roll back transactions
        /// </summary>
        public void RollBackAllTransactions()
        {
            _requestManager.Value.RollBackAllTransactions();
        }


        /// <summary>
        /// Get the initialization level definition id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="hasDifferentialWorkFlow">show if the workflow assigned to the service has other factor that determine the direction to follow</param>
        /// <param name="dataParams">this would hold the additional data param for differential workflow if any</param>
        /// <returns>int</returns>
        public int GetInitFlow(int serviceId, bool hasDifferentialWorkFlow, ServiceWorkFlowDifferentialDataParam dataParams)
        {
            return hasDifferentialWorkFlow ?  GetDifferentialInitWorkFlowLevel(serviceId, dataParams) : GetInitFlow(serviceId);
        }

        /// <summary>
        /// Get the initialization level definition
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>int</returns>
        private int GetInitFlow(int serviceId)
        {
            return _serviceRepo.Value.GetFirstLevelDefinitionId(serviceId);
        }

        /// <summary>
        /// Get the flow definition this definition level ID
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>int</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        public int GetWorkFlowDefinition(int definitionLevelId)
        {
            return _workFlowDef.Value.GetWorkFlowDefinitionId(definitionLevelId);
        }


        /// <summary>
        /// Get the definition workflow level for differential workflow
        /// that is workflow that are different from what was configured on the service level
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="differentialValue"></param>
        /// <param name="differentialModelName"></param>
        /// <returns>int</returns>
        private int GetDifferentialInitWorkFlowLevel(int serviceId, ServiceWorkFlowDifferentialDataParam differential)
        {
            return _serviceDiffWorkFlow.Value.GetFirstLevelDefinitionId(serviceId, differential);
        }


        /// <summary>
        /// Get the initialization level definition
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="hasDifferentialWorkFlow">show if the workflow assigned to the service has other factor that determine the direction to follow</param>
        /// <param name="dataParams">this would hold the additional data param for differential workflow if any</param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        public PSServiceRequestFlowDefinitionLevelDTO GetInitFlowLevel(int serviceId, bool hasDifferentialWorkFlow, ServiceWorkFlowDifferentialDataParam dataParams)
        {
            return hasDifferentialWorkFlow ? GetDifferentialInitWorkFlowLevelObj(serviceId, dataParams) : GetInitFlowLevelObj(serviceId);
        }


        /// <summary>
        /// Get the last level definition with specified workflow action value
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="hasDifferentialWorkFlow">show if the workflow assigned to the service has other factor that determine the direction to follow</param>
        /// <param name="dataParams">this would hold the additional data param for differential workflow if any</param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO</returns>
        public PSServiceRequestFlowDefinitionLevelDTO GetLastFlowLevelWithWorkflowActionValue(int serviceId, bool hasDifferentialWorkFlow, ServiceWorkFlowDifferentialDataParam dataParams, RequestDirection actionValue)
        {
            return hasDifferentialWorkFlow ? GetDifferentialLastWorkFlowLevelObj(serviceId, dataParams, actionValue) : GetLastFlowLevelObjWithWorkflowActionValue(serviceId, actionValue);
        }


        /// <summary>
        /// Get the initialization level definition
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        private PSServiceRequestFlowDefinitionLevelDTO GetInitFlowLevelObj(int serviceId)
        {
            return _serviceRepo.Value.GetFirstLevelDefinition(serviceId);
        }


        /// <summary>
        /// Get the last level definition with specified workflow action value
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="actionValue"></param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        private PSServiceRequestFlowDefinitionLevelDTO GetLastFlowLevelObjWithWorkflowActionValue(int serviceId, RequestDirection actionValue)
        {
            return _serviceRepo.Value.GetLastLevelDefinitionWithWorkflowActionValue(serviceId, actionValue);
        }


        /// <summary>
        /// Get the definition workflow level for differential workflow
        /// that is workflow that are different from what was configured on the service level
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="differential">ServiceWorkFlowDifferentialDataParam</param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO | null</returns>
        private PSServiceRequestFlowDefinitionLevelDTO GetDifferentialInitWorkFlowLevelObj(int serviceId, ServiceWorkFlowDifferentialDataParam differential)
        {
            return _serviceDiffWorkFlow.Value.GetFirstLevelDefinitionObj(serviceId, differential);
        }


        /// <summary>
        /// Get the last definition level with specified workflow action value for differential workflow
        /// that is workflow that are different from what was configured on the service level
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="differential">ServiceWorkFlowDifferentialDataParam</param>
        /// <param name="actionValue"></param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO | null</returns>
        private PSServiceRequestFlowDefinitionLevelDTO GetDifferentialLastWorkFlowLevelObj(int serviceId, ServiceWorkFlowDifferentialDataParam differential, RequestDirection actionValue)
        {
            return _serviceDiffWorkFlow.Value.GetLastFlowDefinitionLevelObjWithWorkflowActionValue(serviceId, differential, actionValue);
        }


        /// <summary>
        /// Get the flow definition this service was assigned to
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="hasDifferentialWorkFlow">show if the workflow assigned to the service has other factor that determine the direction to follow</param>
        /// <param name="dataParams">this would hold the additional data param for differential workflow if any</param>
        /// <returns>int</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        public int GetWorkFlowDefinition(int serviceId, bool hasDifferentialWorkFlow, ServiceWorkFlowDifferentialDataParam dataParams)
        {
            return hasDifferentialWorkFlow ? GetDifferentialInitWorkFlowLevel(serviceId, dataParams) : GetInitFlow(serviceId);
        }


        /// <summary>
        /// Get the request token for this 
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="expectedHash"></param>
        /// <returns>string</returns>
        public string GetURLRequestTokenString(long requestId, string expectedHash)
        {
            return requestId.ToString();
        }


        /// <summary>
        /// Do a hash for this serviceId and it's initial levelId
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="levelId"></param>
        /// <returns>string</returns>
        public string GetExpectedHash(int serviceId, int levelId)
        {
            return Util.OnWayHashThis(serviceId.ToString() + "-" + levelId.ToString(), AppSettingsConfigurations.EncryptionSecret);
        }


        /// <summary>
        /// Save request
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="requestVM"></param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        public PSSRequest SaveRequest(PSSRequestStageModel processStage, RequestDumpVM requestVM, TaxEntityViewModel taxPayerProfileVM, int definitionLevelId, PSSRequestStatus status)
        {
            PSSRequest request = new PSSRequest
            {
                Command = new Command { Id = requestVM.SelectedCommand },
                TaxEntity = new TaxEntity { Id = taxPayerProfileVM.Id },
                Service = new PSService { Id = processStage.ServiceId },
                Status = (int)status,
                ServicePrefix = processStage.ServicePrefix,
                FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = definitionLevelId },
                ExpectedHash = requestVM.ExpectedHash,
                ContactPersonName = processStage.AlternativeContactPersonName,
                ContactPersonPhoneNumber = processStage.AlternativeContactPersonPhoneNumber,
                ContactPersonEmail = processStage.AlternativeContactPersonEmail,
                CBSUser = new CBSUser { Id = processStage.CBSUserProfileId },
                TaxEntityProfileLocation = new TaxEntityProfileLocation { Id = _cbsUserTaxEntityProfileLocationManager.GetCBSUserLocationId(processStage.CBSUserProfileId) }
            };

            if (!_requestManager.Value.Save(request)) { throw new CouldNotSaveRecord("Could not save request record"); }

            return request;
        }


        /// <summary>
        /// Save request command details
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        public void SaveCommandDetails(List<RequestCommand> reqCommands)
        {
            if (!_reqCommandMan.Value.SaveBundleUnCommit(reqCommands)) { throw new CouldNotSaveRecord("Could not save request command records"); }
        }


        /// <summary>
        /// Save request details
        /// </summary>
        /// <param name="processStage"></param>
        public InvoiceGenerationResponse SaveRequestDetails(PSSRequestStageModel processStage, RequestDumpVM requestVM, TaxEntityViewModel taxPayerProfileVM, PSSRequestStatus requestStatus, IEnumerable<UserFormDetails> formDetails)
        {
            try
            {
                //get expert system revenue head
                IEnumerable<ExpertSystemVM> expertSystem = GetExpertSystem();

                //get init level definition
                PSServiceRequestFlowDefinitionLevelDTO initLevel = GetInitFlowLevel(processStage.ServiceId, processStage.HasDifferentialWorkFlow, new ServiceWorkFlowDifferentialDataParam { });

                //get revenue heads assigned to this service
                IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeads = GetRevenueHeadDetails(processStage.ServiceId, initLevel.Id);
                if (serviceRevenueHeads == null || !serviceRevenueHeads.Any())
                {
                    throw new NoBillingInformationFoundException("No billing info found for service Id " + processStage.ServiceId);
                }

                //get the parent service revenue head
                PSServiceRevenueHeadVM parentServicerevenueHead = serviceRevenueHeads.Count() > 1 ? serviceRevenueHeads.Where(r => r.IsGroupHead).Single() : serviceRevenueHeads.ElementAt(0);

                //save the request
                PSSRequest request = SaveRequest(processStage, requestVM, taxPayerProfileVM, initLevel.Id, requestStatus);
                SaveCommandDetails(new List<RequestCommand> { { new RequestCommand { Command = new Command { Id = requestVM.SelectedCommand }, Request = request } } });

                requestVM.AlternativeContactName = request.ContactPersonName;
                requestVM.AlternativeContactPhoneNumber = request.ContactPersonPhoneNumber;
                requestVM.AlternativeContactEmail = request.ContactPersonEmail;
                //save file number
                string fileRefNumber = GetRequestFileRefNumber(request);

                //get model for invoice generation
                CreateInvoiceUserInputModel inputModel = GetInvoiceUserInputModel(parentServicerevenueHead, requestVM, request.Id, serviceRevenueHeads, processStage.CategoryId, fileRefNumber, formDetails);
                //se validation on this forms we set the flag to true
                inputModel.DontValidateFormControls = requestVM.DontValidateFormControls;

                //save invoice
                TaxEntityViewModel entityVM = new TaxEntityViewModel { Id = taxPayerProfileVM.Id };
                InvoiceGenerationResponse response = GenerateInvoice(inputModel, expertSystem.ElementAt(0), entityVM);

                //match request and invoice
                AddRequestAndInvoice(request, response.InvoiceId);

                //add to request status log
                RequestStatusLog statusLog = new RequestStatusLog
                {
                    Invoice = new Invoice { Id = response.InvoiceId },
                    StatusDescription = initLevel.PositionDescription,
                    Request = request,
                    FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = initLevel.Id },
                    UserActionRequired = true,
                    Status = (int)PSSRequestStatus.PendingInvoicePayment
                };
                AddRequestStatusLog(statusLog);

                //save service request
                SaveServiceRequest(request, serviceRevenueHeads.Where(sr => !sr.IsGroupHead), processStage.ServiceId, response.InvoiceId, initLevel.Id, requestStatus);

                //do notifications
                SendNotification(requestVM, taxPayerProfileVM, parentServicerevenueHead, response);

                return response;
            }
            catch (Exception)
            {
                _serviceRepo.Value.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Send user notification
        /// </summary>
        /// <param name="requestVM"></param>
        /// <param name="taxPayerProfileVM"></param>
        /// <param name="parentServicerevenueHead"></param>
        /// <param name="response"></param>
        private void SendNotification(RequestDumpVM requestVM, TaxEntityViewModel taxPayerProfileVM, PSServiceRevenueHeadVM parentServicerevenueHead, InvoiceGenerationResponse response)
        {
            //Send a sms notification to the payer
            SendInvoiceSMSNotification(new SMSDetailVM { RevenueHead = parentServicerevenueHead.ServiceName, Amount = response.AmountDue.ToString("F"), Name = taxPayerProfileVM.Recipient, PhoneNumber = (string.IsNullOrEmpty(requestVM.AlternativeContactPhoneNumber) ? taxPayerProfileVM.PhoneNumber : requestVM.AlternativeContactPhoneNumber), TaxEntityId = taxPayerProfileVM.Id, InvoiceNumber = response.InvoiceNumber });
        }


        /// <summary>
        /// Model for invoice generation
        /// </summary>
        /// <param name="parentServicerevenueHead"></param>
        /// <param name="id"></param>
        /// <param name="serviceRevenueHeads"></param>
        /// <param name="categoryId"></param>
        /// <param name="fileRefNumber"></param>
        /// <param name="formValues"></param>
        /// <returns>CreateInvoiceUserInputModel</returns>
        private CreateInvoiceUserInputModel GetInvoiceUserInputModel(PSServiceRevenueHeadVM parentServiceRevenueHead, RequestDumpVM requestVM, long requestId, IEnumerable<PSServiceRevenueHeadVM> result, int categoryId, string fileRefNumber, IEnumerable<UserFormDetails> formDetails)
        {
            return new CreateInvoiceUserInputModel
            {
                GroupId = parentServiceRevenueHead.IsGroupHead ? parentServiceRevenueHead.RevenueHeadId : 0,
                InvoiceTitle = parentServiceRevenueHead.FeeDescription,
                InvoiceDescription = string.Format("{0} for {1} {2}. {3}", parentServiceRevenueHead.FeeDescription, parentServiceRevenueHead.ServiceName, fileRefNumber, requestVM.InvoiceDescription),
                CallBackURL = PSSUtil.GetURLForFeeConfirmation(requestVM.SiteName, requestId.ToString()),
                TaxEntityCategoryId = categoryId,
                RevenueHeadModels = result.Where(r => !r.IsGroupHead).Select(r =>
                new RevenueHeadUserInputModel
                {
                    AdditionalDescription = string.Format("{0} for {1} {2}", r.FeeDescription, r.ServiceName, fileRefNumber),
                    Amount = r.AmountToPay,
                    Quantity = 1,
                    RevenueHeadId = r.RevenueHeadId,
                    FormValues = formDetails.Where(fm => fm.RevenueHeadId == r.RevenueHeadId).Select(fm => new FormControlViewModel { ControlIdentifier = fm.ControlIdentifier, FormValue = fm.FormValue, RevenueHeadId = fm.RevenueHeadId }).ToList()
                }).ToList()
            };
        }


        /// <summary>
        /// Save the service and request
        /// <para>Here the relationship between the service and request is defined</para>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="serviceRevenueHeads"></param>
        /// <param name="serviceId"></param>
        /// <param name="invoiceId"></param>
        /// <param name="definitionLevelId"></param>
        public void SaveServiceRequest(PSSRequest request, IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeads, int serviceId, Int64 invoiceId, int definitionLevelId, PSSRequestStatus status = PSSRequestStatus.Pending)
        {
            foreach (var item in serviceRevenueHeads.Where(sr => !sr.IsGroupHead))
            {
                PoliceServiceRequest serviceRequest = new PoliceServiceRequest
                {
                    Request = request,
                    Service = new PSService { Id = serviceId },
                    RevenueHead = new RevenueHead { Id = item.RevenueHeadId },
                    Invoice = new Invoice { Id = invoiceId },
                    FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = definitionLevelId },
                    Status = (int)status,
                };

                if (!_serviceRequestManager.Value.Save(serviceRequest)) { throw new CouldNotSaveRecord("Could not save service request record"); }
            }
        }


        /// <summary>
        /// Add the request and invoice joiner
        /// </summary>
        /// <param name="request"></param>
        /// <param name="invoiceId"></param>
        public void AddRequestAndInvoice(PSSRequest request, long invoiceId)
        {
            _requestInvoiceService.Value.Save(new PSSRequestInvoice { Request = request, Invoice = new Invoice { Id = invoiceId } });
        }

        public void SendInvoiceSMSNotification(SMSDetailVM model)
        {
            try
            {
                if (CheckSendSMSNotification(new List<string> { model.PhoneNumber }))
                {
                    int providerId = 0;
                    bool result = Int32.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.SMSProvider), out providerId);
                    if (!result)
                    {
                        providerId = (int)SMSProvider.Pulse;
                    }
                    foreach (var impl in _smsProvider)
                    {
                        if ((SMSProvider)providerId == impl.Value.GetSMSNotificationProvider)
                        {
                            string message = $"Dear {model.Name}, your invoice {model.InvoiceNumber} of NGN{model.Amount} for {model.RevenueHead} has been generated successfully.";
                            impl.Value.SendSMS(new List<string> { model.PhoneNumber }, message, _orchardServices.WorkContext.CurrentSite.SiteName);
                            break;
                        }
                    }
                }
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }
        }

        /// <summary>
        /// Send SMS notification to the next person to act on an approval request
        /// </summary>
        /// <param name="model"></param>
        /// <param name="phoneNumbers"></param>
        public void SendApproverSMSNotification(dynamic smsDetails, List<string> phoneNumbers)
        {
            try
            {
                if (CheckSendSMSNotification(phoneNumbers))
                {
                    int providerId = 0;
                    bool result = Int32.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.SMSProvider), out providerId);
                    if (!result)
                    {
                        providerId = (int)SMSProvider.Pulse;
                    }
                    foreach (var impl in _pssSMSProvider)
                    {
                        if ((SMSProvider)providerId == impl.Value.GetSMSNotificationProvider)
                        {
                            impl.Value.NotifyApprover(smsDetails, phoneNumbers, _orchardServices.WorkContext.CurrentSite.SiteName);
                            break;
                        }
                    }
                }
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }
        }

        public void SendPSSRequestApprovalSMSNotification(dynamic smsDetails)
        {
            try
            {
                if (CheckSendSMSNotification(new List<string> { smsDetails.PhoneNumber }))
                {
                    int providerId = 0;
                    bool result = Int32.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.SMSProvider), out providerId);
                    if (!result)
                    {
                        providerId = (int)SMSProvider.Pulse;
                    }
                    foreach (var impl in _pssSMSProvider)
                    {
                        if ((SMSProvider)providerId == impl.Value.GetSMSNotificationProvider)
                        {
                            impl.Value.PSSRequestApproval(smsDetails, _orchardServices.WorkContext.CurrentSite.SiteName);
                            break;
                        }
                    }
                }
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }
        }

        public void SendPSSRequestRejectionSMSNotification(dynamic smsDetails)
        {
            try
            {
                if (CheckSendSMSNotification(new List<string> { smsDetails.PhoneNumber }))
                {
                    int providerId = 0;
                    bool result = Int32.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.SMSProvider), out providerId);
                    if (!result)
                    {
                        providerId = (int)SMSProvider.Pulse;
                    }
                    foreach (var impl in _pssSMSProvider)
                    {
                        if ((SMSProvider)providerId == impl.Value.GetSMSNotificationProvider)
                        {
                            impl.Value.PSSRequestRejection(smsDetails, _orchardServices.WorkContext.CurrentSite.SiteName);
                            break;
                        }
                    }
                }
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }
        }

        /// <summary>
        /// Send any generic SMS notification
        /// </summary>
        /// <param name="smsDetails"></param>
        public void SendGenericSMSNotification(List<string> phoneNumbers, string message)
        {
            try
            {
                if (CheckSendSMSNotification(phoneNumbers))
                {
                    bool result = int.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.SMSProvider), out int providerId);
                    if (!result)
                    {
                        providerId = (int)SMSProvider.Pulse;
                    }
                    foreach (var impl in _pssSMSProvider)
                    {
                        if ((SMSProvider)providerId == impl.Value.GetSMSNotificationProvider)
                        {
                            impl.Value.SendSMS(phoneNumbers, message, _orchardServices.WorkContext.CurrentSite.SiteName);
                            break;
                        }
                    }
                }
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }
        }

        /// <summary>
        /// Check if we can send sms notification for a specified tenant
        /// </summary>
        /// <param name="phoneNumbers"></param>
        /// <returns></returns>
        private bool CheckSendSMSNotification(List<string> phoneNumbers)
        {
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.IsSMSEnabled.ToString()).FirstOrDefault();
            if (node != null && !string.IsNullOrEmpty(node.Value))
            {
                bool isSMSEnabled = false;
                bool.TryParse(node.Value, out isSMSEnabled);
                foreach(string phoneNumber in phoneNumbers)
                {
                    if (isSMSEnabled && !string.IsNullOrEmpty(phoneNumber))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Check if we can send email notification for a specified tenant
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="siteName"></param>
        /// <returns></returns>
        private bool CheckSendEmailNotification(string email)
        {
            bool canSendNotification = false;
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.IsEmailEnabled.ToString()).FirstOrDefault();
            if (node != null && !string.IsNullOrEmpty(node.Value))
            {
                bool isSMSEnabled = false;
                bool.TryParse(node.Value, out isSMSEnabled);
                if (isSMSEnabled && !string.IsNullOrEmpty(email))
                {
                    canSendNotification = true;
                }
            }

            return canSendNotification;
        }

        public void AddRequestStatusLog(RequestStatusLog requestStatus)
        {
            if (!_requestLogRepo.Value.Save(requestStatus)) { throw new CouldNotSaveRecord("Could not save status log record"); }
        }


        /// <summary>
        /// Creates request command workflow log
        /// </summary>
        /// <param name="log"></param>
        public void AddRequestCommandWorkFlowLog(RequestCommandWorkFlowLog log)
        {
            if (!_requestCommandWorkFlowLogManager.Save(log)) { throw new CouldNotSaveRecord("Could not save request command workflow log record"); }
        }


        /// <summary>
        /// Updates request command workflow log
        /// </summary>
        /// <param name="requestDeets"></param>
        public void UpdateRequestCommandWorkFlowLog(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets)
        {
            _requestCommandWorkFlowLogManager.UpdateRequestCommandWorkFlowLog(requestDeets.First().Request.Id, requestDeets.First().CommandId, requestDeets.First().DefinitionLevelId, false);
        }

        /// <summary>
        /// Compute deployment allowance
        /// </summary>
        /// <param name="invoiceContributedAmount"></param>
        /// <param name="deductionPercentage"></param>
        /// <param name="paymentPercentage"></param>
        /// <returns></returns>
        public decimal ComputeAllowanceFee(decimal invoiceContributedAmount, decimal deductionPercentage, decimal paymentPercentage)
        {
            //Get the percentage to be deducted for officer allowance
            decimal deductionAmount = Math.Round(((deductionPercentage / 100) * invoiceContributedAmount), 2);

            //Get the payment allowance
            return Math.Round(((paymentPercentage / 100) * deductionAmount), 2);
        }


        /// <summary>
        /// update the definition level for a rewuest wit the request Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="newDefinitionLevelId"></param>
        /// <param name="status"></param>
        /// <exception cref="Exception">Throws exception if update fails</exception>
        public void UpdateRequestDefinitionFlowLevel(long requestId, int newDefinitionLevelId, PSSRequestStatus status)
        {
            _requestManager.Value.UpdateRequestFlowId(requestId, newDefinitionLevelId, status);
        }


        ///// <summary>
        ///// Get the flow definition this service was assigned to
        ///// </summary>
        ///// <param name="serviceId"></param>
        ///// <returns>int</returns>
        ///// <exception cref="NoRecordFoundException"></exception>
        //public int GetWorkFlowDefinition(int serviceId)
        //{
        //    return _serviceRepo.Value.GetFlowDefinitionId(serviceId);
        //}


        /// <summary>
        /// Set approval number for request with specified Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="approvalNumber"></param>
        public void SetApprovalNumber(long requestId,string approvalNumber)
        {
            _requestManager.Value.SetApprovalNumber(requestId, approvalNumber);
        }

        /// <summary>
        /// Get request details using user input file number
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>PSSRequestVM</returns>
        public PSSRequestVM ConfirmFileNumber(string fileNumber)
        {
            try
            {
                string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;
                PSSRequestVM requestDet = null;
                requestDet = ObjectCacheProvider.GetCachedObject<PSSRequestVM>(tenant, $"USSD-{nameof(POSSAPCachePrefix.FileNumber)}-{fileNumber}");
                if (requestDet == null)
                {
                    requestDet = _requestManager.Value.GetRequestDetails(fileNumber);
                    if (requestDet != null)
                    {
                        ObjectCacheProvider.TryCache(tenant, $"USSD-{nameof(POSSAPCachePrefix.FileNumber)}-{fileNumber}", requestDet, DateTime.Now.AddMinutes(60));
                    }
                }

                return requestDet;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets police officer with specified service number
        /// </summary>
        /// <param name="serviceNumber"></param>
        /// <returns>PoliceOfficerVM</returns>
        public PoliceOfficerVM GetPoliceOfficer(string serviceNumber)
        {
            try
            {
                string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;
                PoliceOfficerVM officerVM = ObjectCacheProvider.GetCachedObject<PoliceOfficerVM>(tenant, $"USSD-{nameof(POSSAPCachePrefix.ServiceNumber)}-{serviceNumber.ToUpper()}");
                if (officerVM == null)
                {
                    APIResponse response = _corePoliceOfficerService.Value.GetPoliceOfficer(serviceNumber);
                    if (response.Error)
                    {
                        string error = Newtonsoft.Json.JsonConvert.SerializeObject(response.ResponseObject);
                        throw new Exception(error);
                    }

                    officerVM = Newtonsoft.Json.JsonConvert.DeserializeObject<PoliceOfficerVM>(response.ResponseObject);
                    ObjectCacheProvider.TryCache(tenant, $"USSD-{nameof(POSSAPCachePrefix.ServiceNumber)}-{serviceNumber.ToUpper()}", officerVM, DateTime.Now.AddMinutes(5));
                }
                return officerVM;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw new NoRecordFoundException("Unable to get officer information");
            }
        }

        /// <summary>
        /// Sets all request command workflow logs for request with the specified id to inactive
        /// </summary>
        /// <param name="requestId"></param>
        public void SetPreviousRequestCommandWorkflowLogsToInactive(long requestId)
        {
            _requestCommandWorkFlowLogManager.SetPreviousRequestCommandWorkflowLogsToInactive(requestId);
        }

    }
}