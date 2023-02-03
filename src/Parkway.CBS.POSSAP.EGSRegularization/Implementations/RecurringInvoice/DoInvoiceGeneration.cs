using InfoGRID.Pulse.SDK.DTO;
using InfoGRID.Pulse.SDK.Models;
using Newtonsoft.Json;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.HangFireInterface.Notification;
using Parkway.CBS.HangFireInterface.Notification.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModels;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations.RecurringInvoice
{
    public class DoInvoiceGeneration
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IPSSRegularizationUnknownOfficerRecurringInvoiceSettingsDAOManager _regularizationUnknownOfficerRecurringInvoiceSettingsDAOManager { get; set; }

        public IGenerateRequestWithoutOfficersUploadBatchItemsDAOManager _generateRequestWithoutOfficersUploadBatchItemsStagingDAOManager { get; set; }

        public IEscortAmountChartSheetDAOManager _escortAmountChartSheetDAOManager { get; set; }

        public IPSSProposedRegularizationUnknownPoliceOfficerDeploymentLogDAOManager _proposedRegularizationUnknownPoliceOfficerDeploymentLogDAOManager { get; set; }

        public IRevenueHeadDAOManager _revenueHeadDAOManager { get; set; }

        public IInvoiceItemsDAOManager _invoiceItemsDAOManager { get; set; }

        public ITransactionLogDAOManager _transactionLogDAOManager { get; set; }

        public IInvoiceDAOManager _invoiceDAOManager { get; set; }

        public IPSSRequestDAOManager _requestDAOManager { get; set; }

        public IPSSEscortDetailsDAOManager _escortDetailsDAOManager { get; set; }

        public IPSSRequestInvoiceDAOManager _requestInvoiceDAOManager { get; set; }

        public IRequestStatusLogDAOManager _requestStatusLogDAOManager { get; set; }

        public IPoliceServiceRequestDAOManager _policeServiceRequestDAOManager { get; set; }

        public IPSServiceRevenueHeadDAOManager _serviceRevenueHeadDAOManager { get; set; }

        public IPSServiceDAOManager _serviceDAOManager { get; set; }

        public IServiceWorkflowDifferentialDAOManager _serviceWorkflowDifferentialDAOManager { get; set; }

        public IExpertSystemSettingsDAOManager _expertSystemSettingsDAOManager { get; set; }

        public IPSServiceRequestFlowDefinitionLevelDAOManager _serviceRequestFlowDefinitionLevelDAOManager { get; set; }

        public IInvoicingService _invoicingService { get; set; }

        public ISMSNotification _smsNotification { get; set; }

        public IEmailNotification _emailNotification { get; set; }

        private const int weeklyDay = 7;

        private const int monthlyDay = 30;

        const string defaultCode = "+234";

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSSettlementJob");
            }
        }

        private void SetPSSRegularizationUnknownOfficerRecurringInvoiceSettingsDAOManager()
        {
            if (_regularizationUnknownOfficerRecurringInvoiceSettingsDAOManager == null) { _regularizationUnknownOfficerRecurringInvoiceSettingsDAOManager = new PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDAOManager(UoW); }
        }

        private void SetGenerateRequestWithoutOfficersUploadBatchItemsDAOManager()
        {
            if (_generateRequestWithoutOfficersUploadBatchItemsStagingDAOManager == null) { _generateRequestWithoutOfficersUploadBatchItemsStagingDAOManager = new GenerateRequestWithoutOfficersUploadBatchItemsDAOManager(UoW); }
        }

        private void SetEscortAmountChartSheetDAOManager()
        {
            if (_escortAmountChartSheetDAOManager == null) { _escortAmountChartSheetDAOManager = new EscortAmountChartSheetDAOManager(UoW); }
        }

        private void SetPSSProposedRegularizationUnknownPoliceOfficerDeploymentLogDAOManager()
        {
            if (_proposedRegularizationUnknownPoliceOfficerDeploymentLogDAOManager == null) { _proposedRegularizationUnknownPoliceOfficerDeploymentLogDAOManager = new PSSProposedRegularizationUnknownPoliceOfficerDeploymentLogDAOManager(UoW); }
        }

        private void SetRevenueHeadDAOManager()
        {
            if (_revenueHeadDAOManager == null) { _revenueHeadDAOManager = new RevenueHeadDAOManager(UoW); }
        }

        private void SetInvoiceItemsDAOManager()
        {
            if (_invoiceItemsDAOManager == null) { _invoiceItemsDAOManager = new InvoiceItemsDAOManager(UoW); }
        }

        private void SetTransactionLogDAOManager()
        {
            if (_transactionLogDAOManager == null) { _transactionLogDAOManager = new TransactionLogDAOManager(UoW); }
        }

        private void SetIInvoiceDAOManager()
        {
            if (_invoiceDAOManager == null) { _invoiceDAOManager = new InvoiceDAOManager(UoW); }
        }

        private void SetPSSRequestDAOManager()
        {
            if (_requestDAOManager == null) { _requestDAOManager = new PSSRequestDAOManager(UoW); }
        }

        private void SetPSSEscortDetailsDAOManager()
        {
            if (_escortDetailsDAOManager == null) { _escortDetailsDAOManager = new PSSEscortDetailsDAOManager(UoW); }
        }

        private void SetPSSRequestInvoiceDAOManager()
        {
            if (_requestInvoiceDAOManager == null) { _requestInvoiceDAOManager = new PSSRequestInvoiceDAOManager(UoW); }
        }

        private void SetRequestStatusLogDAOManager()
        {
            if (_requestStatusLogDAOManager == null) { _requestStatusLogDAOManager = new RequestStatusLogDAOManager(UoW); }
        }

        private void SetPoliceServiceRequestDAOManager()
        {
            if (_policeServiceRequestDAOManager == null) { _policeServiceRequestDAOManager = new PoliceServiceRequestDAOManager(UoW); }
        }

        private void SetPSServiceRevenueHeadDAOManager()
        {
            if (_serviceRevenueHeadDAOManager == null) { _serviceRevenueHeadDAOManager = new PSServiceRevenueHeadDAOManager(UoW); }
        }

        private void SetPSServiceDAOManager()
        {
            if (_serviceDAOManager == null) { _serviceDAOManager = new PSServiceDAOManager(UoW); }
        }

        private void SetServiceWorkflowDifferentialDAOManager()
        {
            if (_serviceWorkflowDifferentialDAOManager == null) { _serviceWorkflowDifferentialDAOManager = new ServiceWorkflowDifferentialDAOManager(UoW); }
        }

        private void SetExpertSystemSettingsDAOManager()
        {
            if (_expertSystemSettingsDAOManager == null) { _expertSystemSettingsDAOManager = new ExpertSystemSettingsDAOManager(UoW); }
        }

        private void SetPSServiceRequestFlowDefinitionLevelDAOManager()
        {
            if (_serviceRequestFlowDefinitionLevelDAOManager == null) { _serviceRequestFlowDefinitionLevelDAOManager = new PSServiceRequestFlowDefinitionLevelDAOManager(UoW); }
        }

        public void InstantiateInvoicingService()
        {
            if (_invoicingService == null) { _invoicingService = new InvoicingService(); }
        }

        public void SetSMSNotificationInstance()
        {
            if (_smsNotification == null) { _smsNotification = new SMSNotification(); }
        }

        public void SetEmailNotificationInstance()
        {
            if (_emailNotification == null) { _emailNotification = new EmailNotification(); }
        }


        /// <summary>
        /// Generate regularization recurring invoice for a specified request 
        /// </para>
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="invoiceSetting"></param>
        public void GenerateInvoice(string tenantName, PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDTO invoiceSetting)
        {
            log.Info($"About to start generate regularization recurring invoice for request Id {invoiceSetting.RequestId}");

            try
            {
                SetUnitofWork(tenantName);
                SetPSSRegularizationUnknownOfficerRecurringInvoiceSettingsDAOManager();
                SetGenerateRequestWithoutOfficersUploadBatchItemsDAOManager();
                SetEscortAmountChartSheetDAOManager();
                SetPSSProposedRegularizationUnknownPoliceOfficerDeploymentLogDAOManager();
                SetRevenueHeadDAOManager();
                SetInvoiceItemsDAOManager();
                SetTransactionLogDAOManager();
                SetIInvoiceDAOManager();
                SetPSSRequestDAOManager();
                SetPSSEscortDetailsDAOManager();
                SetPSSRequestInvoiceDAOManager();
                SetRequestStatusLogDAOManager();
                SetPoliceServiceRequestDAOManager();
                SetPSServiceRevenueHeadDAOManager();
                SetPSServiceDAOManager();
                SetServiceWorkflowDifferentialDAOManager();
                SetExpertSystemSettingsDAOManager();
                SetPSServiceRequestFlowDefinitionLevelDAOManager();
                InstantiateInvoicingService();
                SetSMSNotificationInstance();
                SetEmailNotificationInstance();

                log.Info($"Getting PSSRequest with id {invoiceSetting.RequestId} for recurring invoice generation with settings id {invoiceSetting.Id}");
                PSSRequestTaxEntityViewVM request = _requestDAOManager.GetPSSRequest(invoiceSetting.RequestId);
                if (request == null) { throw new Exception($"Could not get pssRequest with id {invoiceSetting.RequestId} for recurring invoice generation with settings id {invoiceSetting.Id}"); }

                log.Info($"Getting Escort Details with id {invoiceSetting.EscortDetailId} for recurring invoice generation with settings id {invoiceSetting.Id}");
                EscortDetailsDTO escortRequestDetails = _escortDetailsDAOManager.GetEscortDetails(invoiceSetting.EscortDetailId);
                if (escortRequestDetails == null) { throw new Exception($"Could not get PSSEscortDetails with id {invoiceSetting.EscortDetailId} for recurring invoice generation with settings id {invoiceSetting.Id}"); }

                log.Info($"Fetching the last flow definition level with workflow action value GenerateInvoice for PSService with id {request.ServiceId} for recurring invoice generation with settings id {invoiceSetting.Id}");
                PSServiceRequestFlowDefinitionLevelDTO serviceFeeInvoiceGenerationLevel = GetLastFlowLevelWithWorkflowActionValue(request.ServiceId, false, new ServiceWorkFlowDifferentialDataParam { DifferentialModelName = nameof(CommandType), DifferentialValue = escortRequestDetails.CommandTypeId }, RequestDirection.GenerateInvoice);
                if (serviceFeeInvoiceGenerationLevel == null) { throw new Exception($"Could not get last flow definition level with Generate Invoice workflow action value for psservice with id {request.ServiceId}"); }

                log.Info($"Fetching revenue heads attached to PSService with id {request.ServiceId} at flow definition level with id {serviceFeeInvoiceGenerationLevel.Id} for recurring invoice generation with settings id {invoiceSetting.Id}");
                IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeads = _serviceRevenueHeadDAOManager.GetRevenueHead(request.ServiceId, serviceFeeInvoiceGenerationLevel.Id);
                if (serviceRevenueHeads == null || !serviceRevenueHeads.Any())
                {
                    throw new Exception($"No billing info found for service Id {request.ServiceId}");
                }

                decimal rate = 0.00m;

                //compute amount for unknown officers
                log.Info($"Computing amount for unknown police officers for recurring invoice generation with settings id {invoiceSetting.Id}");
                List<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog> proposedDeploymentLogs = new List<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog>();
                ComputeAmountForUnknownOfficers(invoiceSetting.GenerateRequestWithoutOfficersUploadBatchStagingId, request.PSSRequestId, invoiceSetting, ref rate, ref proposedDeploymentLogs);

                PSServiceRevenueHeadVM parentServicerevenueHead = serviceRevenueHeads.Count() > 1 ? serviceRevenueHeads.Where(r => r.IsGroupHead).Single() : serviceRevenueHeads.ElementAt(0);

                //get generate invoice input model
                log.Info($"Building Create Invoice User Input Model for recurring invoice generation with settings id {invoiceSetting.Id}");
                CreateInvoiceUserInputModel inputModel = GetInvoiceUserInputModel(parentServicerevenueHead, escortRequestDetails, serviceRevenueHeads, request.TaxEntity.CategoryId, request.FileRefNumber, request.PSSRequestId.ToString(), rate, invoiceSetting);

                //generate invoice
                log.Info($"Attemping to generate invoice for recurring invoice generation with settings id {invoiceSetting.Id}");
                var expertSystem = _expertSystemSettingsDAOManager.GetRootExpertSystem().ElementAt(0);
                RegularizationInvoiceGenerationResponse response = TryGenerateInvoice(inputModel, expertSystem, request.TaxEntity);
                log.Info($"Invoice generated successfully for recurring invoice with settings id {invoiceSetting.Id}");

                //Start transaction
                UoW.BeginTransaction();

                Invoice invoice = SaveInvoice(response.InvoiceHelper, inputModel, response.InvoiceResponse, expertSystem, request.TaxEntity, response.GroupDetails, null);

                //save invoice items
                List<InvoiceItems> invoiceItems = SaveInvoiceItems(invoice, response.RequestModel);
                SaveTransactionLog(invoice, new GenerateInvoiceModel { Entity = new TaxEntity { Id = request.TaxEntity.Id }, AdminUser = null }, invoiceItems);

                //save request and invoice to PSSRequestInvoice
                log.Info($"Creating request invoice record for request with id {request.PSSRequestId} and newly generated invoice with id {invoice.Id} for recurring invoice generation with settings id {invoiceSetting.Id}");
                _requestInvoiceDAOManager.Add(new PSSRequestInvoice { Request = new PSSRequest { Id = request.PSSRequestId }, Invoice = new Invoice { Id = invoice.Id } });

                log.Info($"Adding logs to proposed regularization unknown police officer deployment log for recurring invoice generation with settings id {invoiceSetting.Id}");
                foreach (var log in proposedDeploymentLogs)
                {
                    log.Invoice = new Invoice { Id = invoice.Id };
                    _proposedRegularizationUnknownPoliceOfficerDeploymentLogDAOManager.Add(log);
                }

                //add to request status log
                log.Info($"Creating Request Status Log record for request with id {request.PSSRequestId} and invoice with id {invoice.Id} for recurring invoice generation with settings id {invoiceSetting.Id}");
                RequestStatusLog statusLog = new RequestStatusLog
                {
                    Invoice = new Invoice { Id = invoice.Id },
                    StatusDescription = serviceFeeInvoiceGenerationLevel.PositionDescription,
                    Request = new PSSRequest { Id = request.PSSRequestId },
                    FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = serviceFeeInvoiceGenerationLevel.Id },
                    UserActionRequired = true,
                    Status = (int)PSSRequestStatus.PendingInvoicePayment
                };

                _requestStatusLogDAOManager.Add(statusLog);

                log.Info($"Getting updated next invoice generation date for recurring invoice generation with settings id {invoiceSetting.Id}");
                string updatedCronExpression = GenerateCronExp((PSBillingType)invoiceSetting.PaymentBillingType, invoiceSetting.NextInvoiceGenerationDate);
                DateTime UpdatedNextInvoiceGenerationDate = invoiceSetting.PaymentBillingType == (int)PSBillingType.Weekly ? Core.Utilities.Util.GetNextDate(updatedCronExpression, invoiceSetting.NextInvoiceGenerationDate).Value : invoiceSetting.NextInvoiceGenerationDate.AddDays(monthlyDay);

                log.Info($"Updating next invoice generation date for recurring invoice generation with settings id {invoiceSetting.Id}");
                _regularizationUnknownOfficerRecurringInvoiceSettingsDAOManager.UpdateNextInvoiceGenerationDateAndCronExpForInvoiceSettings(invoiceSetting.Id, UpdatedNextInvoiceGenerationDate, updatedCronExpression);

                UoW.Commit();

                //send sms and email notifications
                log.Info($"Sending sms and email notifications for recurring invoice generation with settings id {invoiceSetting.Id}");
                SendNofications(response.InvoiceResponse, request.CBSUser, invoiceSetting.PaymentBillingType, tenantName);

            }
            catch (Exception exception)
            {
                log.Error($"Error processing POSSAP regularization recurring invoice");
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); }
                _regularizationUnknownOfficerRecurringInvoiceSettingsDAOManager = null;
                _generateRequestWithoutOfficersUploadBatchItemsStagingDAOManager = null;
                _escortAmountChartSheetDAOManager = null;
                _proposedRegularizationUnknownPoliceOfficerDeploymentLogDAOManager = null;
                _revenueHeadDAOManager = null;
                _invoiceItemsDAOManager = null;
                _transactionLogDAOManager = null;
                _invoiceDAOManager = null;
                _requestDAOManager = null;
                _escortDetailsDAOManager = null;
                _requestInvoiceDAOManager = null;
                _requestStatusLogDAOManager = null;
                _policeServiceRequestDAOManager = null;
                _serviceRevenueHeadDAOManager = null;
                _serviceDAOManager = null;
                _serviceWorkflowDifferentialDAOManager = null;
                _expertSystemSettingsDAOManager = null;
                _serviceRequestFlowDefinitionLevelDAOManager = null;
            }
        }


        /// <summary>
        /// Generates invoice
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <param name="expertSystemVM"></param>
        /// <returns>InvoiceGenerationResponse</returns>
        private RegularizationInvoiceGenerationResponse TryGenerateInvoice(CreateInvoiceUserInputModel model, ExpertSystemVM expertSystemVM, TaxEntityViewModel entity)
        {
            try
            {
                DateTime invoiceDate = DateTime.Now.ToLocalTime();

                log.Info(string.Format("Generating invoice {0}, Date {1}", JsonConvert.SerializeObject(model), invoiceDate));

                //get group id
                //if this bundle of revenue heads does not contain a group Id
                //we are assumming that this is a bunch of revenue heads
                //that are of the same Id
                //or
                //we need to check that the group Id is not part of the revenue head ids
                GenerateInvoiceRequestModel groupDetails = GetGroupRevenueHeadDetails(model);

                log.Info(string.Format("Group Id has been found {0}. Continuing processing..", model.GroupId));

                //get the revenue head details from the database
                List<RevenueHeadCombinedHelper> rhHelperModels = GetRevenueHeadDetailsHelper(model.RevenueHeadModels);
                List<GenerateInvoiceRequestModel> requestModel = ValidateRevenueHeadDetailsFromDB(rhHelperModels, entity.CategoryId);
                rhHelperModels = null;

                List<CashFlowCreateInvoice.CashFlowProductModel> invoiceItemsCF = DoValidationForRevenueHeadsAndGetInvoiceItems(requestModel);
                //what happens here is that if this request is not attached to a group, we take the first element as the gropu leader
                if (groupDetails == null) { groupDetails = requestModel.ElementAt(0); }

                CreateInvoiceHelper invoiceHelper = GetHelperModel(groupDetails, model, invoiceDate);

                invoiceHelper.Items = invoiceItemsCF;
                //get helper model for invoice generation
                CashFlowCreateCustomerAndInvoice cashFlowRequestModel = GetCashFlowRequestModel(expertSystemVM.StateId, entity, invoiceHelper);
                //add invoice items

                CashFlowCreateCustomerAndInvoiceResponse resultfromCF = CreateInvoiceOnCashflow(groupDetails.MDAVM.SMEKey, cashFlowRequestModel);

                return new RegularizationInvoiceGenerationResponse
                {
                    InvoiceResponse = resultfromCF,
                    InvoiceHelper = invoiceHelper,
                    GroupDetails = groupDetails,
                    RequestModel = requestModel
                };
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error in TryGenerateInvoice {0}", exception), exception);
                throw;
            }
        }


        /// <summary>
        /// Builds input model required for generating invoice
        /// </summary>
        /// <param name="parentServiceRevenueHead"></param>
        /// <param name="requestId"></param>
        /// <param name="requestVM"></param>
        /// <param name="result"></param>
        /// <param name="categoryId"></param>
        /// <param name="fileRefNumber"></param>
        /// <param name="requestId"></param>
        /// <param name="escortFee"></param>
        /// <param name="invoiceSetting"></param>
        /// <returns></returns>
        private CreateInvoiceUserInputModel GetInvoiceUserInputModel(PSServiceRevenueHeadVM parentServiceRevenueHead, EscortDetailsDTO requestVM, IEnumerable<PSServiceRevenueHeadVM> result, int categoryId, string fileRefNumber, string requestId, decimal escortFee, PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDTO invoiceSetting)
        {
            return new CreateInvoiceUserInputModel
            {
                GroupId = parentServiceRevenueHead.IsGroupHead ? parentServiceRevenueHead.RevenueHeadId : 0,
                InvoiceTitle = parentServiceRevenueHead.FeeDescription,
                InvoiceDescription = string.Format("{0} for {1} {2}. Duration {3}. Number of Officers {4}.", parentServiceRevenueHead.FeeDescription, parentServiceRevenueHead.ServiceName, fileRefNumber, string.Format("{0} - {1}", invoiceSetting.NextInvoiceGenerationDate.ToString("dd'/'MM'/'yyyy"), (invoiceSetting.NextInvoiceGenerationDate.AddDays((invoiceSetting.PaymentBillingType == (int)PSBillingType.Weekly) ? weeklyDay : monthlyDay).AddDays(-1)).ToString("dd'/'MM'/'yyyy")), requestVM.NumberOfOfficers),
                CallBackURL = $"{ConfigurationManager.AppSettings["PSSDeploymentAllowanceRecurringInvoiceAPICallBack"]}{requestId}",
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
        /// Compuutes amount for unknown officers
        /// </summary>
        /// <param name="requestVM"></param>
        /// <param name="batchId"></param>
        /// <param name="sumTotalRate"></param>
        private void ComputeAmountForUnknownOfficers(long batchId, long requestId, PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDTO invoiceSetting, ref decimal sumTotalRate, ref List<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog> deploymentLogs)
        {
            log.Info($"Getting generate request without officers upload batch items staging in batch with id {batchId} for recurring invoice generation with settings id {invoiceSetting.Id}");
            IEnumerable<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO> items = _generateRequestWithoutOfficersUploadBatchItemsStagingDAOManager.GetItems(batchId);
            if (items == null || !items.Any())
            {
                throw new Exception($"No GenerateRequestWithoutOfficersUploadBatchItemsStaging found for batch with id {batchId}");
            }

            foreach (var item in items)
            {
                log.Info($"Getting rate from chart sheet for officer with command type id {item.CommandType} and day type id {item.DayType} for recurring invoice generation with settings id {invoiceSetting.Id}");
                decimal rate = _escortAmountChartSheetDAOManager.GetRateForUnknownOfficer(item.CommandType, item.DayType);
                if (rate == 0) { throw new Exception($"Escort rank rate not configured in chart sheet for day type {item.DayType} and command type with id {item.CommandType}"); }
                sumTotalRate += GetDuration(invoiceSetting.PaymentBillingType, invoiceSetting.WeekDayNumber) * item.NumberOfOfficers * rate;
                //here we use next invoice generate date as the start date and in computing the end date because it has not yet been updated and represents the date that the job will run
                deploymentLogs.Add(new PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog { Request = new PSSRequest { Id = requestId }, DeploymentRate = rate, GenerateRequestWithoutOfficersUploadBatchItemsStaging = new GenerateRequestWithoutOfficersUploadBatchItemsStaging { Id = item.Id }, IsActive = true, StartDate = invoiceSetting.NextInvoiceGenerationDate, EndDate = invoiceSetting.NextInvoiceGenerationDate.AddDays((invoiceSetting.PaymentBillingType == (int)PSBillingType.Weekly) ? weeklyDay : monthlyDay).AddDays(-1) });
            }
        }


        /// <summary>
        /// Returns cron expressions based on value of <paramref name="billingType"/>
        /// </summary>
        /// <param name="billingType"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        private string GenerateCronExp(PSBillingType billingType, DateTime startDate)
        {
            //cron expression for weekly every {DateTime.Now.DayOfWeek} of the week
            string weeklyCron = $"0 0 0 ? * {startDate.DayOfWeek.ToString().Substring(0, 3)} *";
            //cron expression for monthly every first day of the month
            string monthlyCron = $"0 0 0 {startDate.Day} 1/1 ? *";
            return (billingType == PSBillingType.Weekly) ? weeklyCron : monthlyCron;
        }


        /// <summary>
        /// Returns billing duration for request
        /// </summary>
        /// <param name="requestVM"></param>
        /// <returns></returns>
        private int GetDuration(int paymentPeriodType, int weekDayNumber)
        {
            return (paymentPeriodType == (int)PSBillingType.Weekly) ? weekDayNumber : monthlyDay;
        }


        /// <summary>
        /// Get the group details for this
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>RevenueHeadForInvoiceGenerationHelper</returns>
        private GenerateInvoiceRequestModel GetGroupRevenueHeadDetails(CreateInvoiceUserInputModel model)
        {
            if (model.GroupId < 1)
            {
                //here the revenue head bundle is not attached to any group
                //this could mean that the revenue heads here are of the same revenue head Id
                //so we do a check to make sure all revenue heads are the same
                CheckIfRevenueHeadAreallTheSameIfNoGroupIdFound(model);
                return null;
            }

            //we need to check that the group Id is not part of the revenue head ids
            CheckGroupIdIsPartOfRevenueHead(model);

            //here we get the group details
            GenerateInvoiceRequestModel groupDetails = _revenueHeadDAOManager.GetGroupRevenueHeadVMForInvoiceGeneration(model.GroupId);

            if (groupDetails == null)
            {
                throw new Exception($"Group Revenue head {model.GroupId} not found");
            }

            //check that the revenue heads are in the group
            //here we confirm that the revenue heads in the user input are indeed part of the group
            CheckThatTheRevenueHeadsProvidedArePartOfGroup(model.RevenueHeadModels, groupDetails);
            return groupDetails;
        }


        /// <summary>
        /// We are checking if all the revenue heads in the user input are of the same revenue head Id
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        private void CheckIfRevenueHeadAreallTheSameIfNoGroupIdFound(CreateInvoiceUserInputModel model)
        {
            //check that this either contains only one revenue head model or there is only one distinct revenue head model
            if (model.RevenueHeadModels.Count > 1)
            {
                if (model.RevenueHeadModels.GroupBy(v => v.RevenueHeadId).Count() > 1)
                {
                    throw new Exception($"Group Revenue head Id is required for this operation {model.GroupId}");
                }
            }
        }


        /// <summary>
        /// Here we check that the group Id is not part of the revenue head item
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        private void CheckGroupIdIsPartOfRevenueHead(CreateInvoiceUserInputModel model)
        {
            if (model.RevenueHeadModels.Where(m => m.RevenueHeadId == model.GroupId).Any())
            {
                throw new Exception($"Revenue head {model.GroupId} does not belong to this group");
            }
        }


        /// <summary>
        /// Check if revenue heads provided belong to a group
        /// </summary>
        /// <param name="revenueHeadModels"></param>
        /// <param name="groupDetails"></param>
        /// <param name="errors"></param>
        private void CheckThatTheRevenueHeadsProvidedArePartOfGroup(List<RevenueHeadUserInputModel> revenueHeadModels, GenerateInvoiceRequestModel groupDetails)
        {
            Dictionary<int, RevenueHeadGroupVM> groupRevenueHeads = groupDetails.RevenueHeadGroupVM.ToDictionary(k => k.RevenueHeadsInGroup);

            foreach (var item in revenueHeadModels)
            {
                if (!groupRevenueHeads.ContainsKey(item.RevenueHeadId))
                {
                    throw new Exception($"Revenue head {item.RevenueHeadId} does not belong to this group");
                }
            }
        }


        /// <summary>
        /// Get helper details for revenue heads
        /// <para>This method makes database calls to get the needed models for invoice generation.
        /// The provided list is grouped by revenue head. 
        /// It groups the revenue heads by revenue head id, do a database call,
        /// then adds the result to the list in each group. So at the end of the day
        /// the number of revenue head items would be returned instead of the group count.
        /// Example: id of revenue heads 1, 2, 3, 4, 3. the grouping would have 3 elements
        /// with key 1, 2, 3, 4. Element at key 3 would have a list of two revenue heads model,
        /// while the remaining would have one element each.
        /// </para>
        /// </summary>
        /// <param name="revenueHeadModels"></param>
        /// <returns>List{RevenueHeadCombinedHelper}</returns>
        private List<RevenueHeadCombinedHelper> GetRevenueHeadDetailsHelper(List<RevenueHeadUserInputModel> revenueHeadModels)
        {
            StringBuilder revList = new StringBuilder();
            try
            {
                var grp = revenueHeadModels.GroupBy(r => r.RevenueHeadId, r => r);
                List<RevenueHeadCombinedHelper> rhList = new List<RevenueHeadCombinedHelper>();

                foreach (var revenueHeadModel in grp)
                {
                    //this gets the revenue head details from the database
                    IEnumerable<RevenueHeadForInvoiceGenerationHelper> result = _revenueHeadDAOManager.GetRevenueHeadVMForInvoiceGeneration(revenueHeadModel.Key);
                    //this adds to the list, where each item has a combiantion for the revenue head model from the user and the corresponding revenue head details from the database
                    //this allows us to do what ever comaprison or addtional validation requirements
                    rhList.AddRange(revenueHeadModel.Select(r => new RevenueHeadCombinedHelper { RequestModel = r, RevenueHeadEssentialsFromDB = result }));
                    revList.AppendFormat(revenueHeadModel.Key + ", ");
                }
                //lets fire one, just triggering the future query
                rhList.ElementAt(0).RevenueHeadDBModel.RevenueHeadVM.Id.ToString();
                //
                log.Info(string.Format("Revenue head details gotten from DB values {0}", revList.ToString().Trim(", ".ToCharArray())));
                return rhList;
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Exception getting revenue head models from DB values {0}. Exception {1}", revList.ToString().Trim(", ".ToCharArray()), exception), exception);
                throw;
            }
        }


        /// <summary>
        /// Validates revenue head details
        /// </summary>
        /// <param name="rhHelperModels"></param>
        /// <param name="categoryId"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private List<GenerateInvoiceRequestModel> ValidateRevenueHeadDetailsFromDB(List<RevenueHeadCombinedHelper> rhHelperModels, int categoryId)
        {
            if (rhHelperModels == null || rhHelperModels.Count < 1)
            {
                throw new Exception("Revenue head not founds");
            }
            List<GenerateInvoiceRequestModel> requestModel = new List<GenerateInvoiceRequestModel> { };

            //check if each revenue head in the list has a details from database
            //this might not be neccessary
            foreach (RevenueHeadCombinedHelper x in rhHelperModels)
            {
                if (x.RevenueHeadDBModel == null)
                {
                    throw new Exception($"Revenue head not found {x.RequestModel.RevenueHeadId}");
                }

                requestModel.Add(new GenerateInvoiceRequestModel
                {
                    MDAVM = x.RevenueHeadDBModel.MDAVM,
                    RevenueHeadVM = x.RevenueHeadDBModel.RevenueHeadVM,
                    BillingModelVM = x.RevenueHeadDBModel.BillingModelVM,
                    AdditionalDescription = x.RequestModel.AdditionalDescription,
                    Amount = x.RequestModel.ApplySurcharge ? x.RequestModel.Surcharge + x.RequestModel.Amount : x.RequestModel.Amount,
                    AmountCanVary = x.RequestModel.AmountCanVary,
                    Quantity = x.RequestModel.Quantity
                });
            }

            return requestModel;
        }


        /// <summary>
        /// Do validation for revenue heads
        /// <para>This method groups the revenue heads by revenue head id, 
        /// then does the neccessary validation for each revenue head.
        /// A RevenueHeadRequestValidationObject object is returned for the validation bit
        /// This would in turn contain the CashFlowProductModel for invoice generation or
        /// would contain and error if the revenue head model fails validation
        /// </para>
        /// </summary>
        /// <param name="reqModels">List{GenerateInvoiceRequestModel}</param>
        /// <returns>List{CashFlowCreateInvoice.CashFlowProductModel}</returns>
        private List<CashFlowCreateInvoice.CashFlowProductModel> DoValidationForRevenueHeadsAndGetInvoiceItems(List<GenerateInvoiceRequestModel> reqModels)
        {
            var grp = reqModels.GroupBy(r => r.RevenueHeadVM.Id, r => r, (key, values) => new
            {
                Key = key,
                values.ElementAt(0).BillingModelVM.BillingType,
                RequestModelsList = values.ToList(),
            });

            ConcurrentStack<ConcurrentStack<CashFlowCreateInvoice.CashFlowProductModel>> invoiceItems = new ConcurrentStack<ConcurrentStack<CashFlowCreateInvoice.CashFlowProductModel>> { };

            Parallel.ForEach(grp, grpItem =>
            {
                invoiceItems.Push(ValidateRequestModel(grpItem.RequestModelsList).PartInvoiceItems);
            });

            return invoiceItems.SelectMany(invItms => invItms).ToList();
        }


        /// <summary>
        /// Do a validation on the requst for invoice generation for this billing type
        /// Validate the request model agianst the rules for invoice generation for this revenue head
        /// </summary>
        /// <param name="requestModels">List{GenerateInvoiceRequestModel}, list of model to be validated</param>
        /// <returns>RevenueHeadRequestValidationObject</returns>
        private RevenueHeadRequestValidationObject ValidateRequestModel(List<GenerateInvoiceRequestModel> requestModels)
        {
            ConcurrentStack<CashFlowCreateInvoice.CashFlowProductModel> partInvoiceItems = new ConcurrentStack<CashFlowCreateInvoice.CashFlowProductModel> { };

            Parallel.ForEach(requestModels, item =>
            {
                if (item.Amount <= 0) { throw new Exception($"Amount {item.Amount} for Revenue head {item.RevenueHeadVM.Id} is invalid"); }

                if (!item.AmountCanVary)
                {
                    if (item.Amount < item.BillingModelVM.Amount)
                    {
                        throw new Exception($"Amount {item.Amount} is less than the expected amount {item.BillingModelVM.Amount}");
                    }
                }

                partInvoiceItems.Push(new CashFlowCreateInvoice.CashFlowProductModel
                {
                    Pos = 1,
                    Price = Math.Round(item.Amount, 2),
                    ProductId = item.RevenueHeadVM.CashflowProductId,
                    ProductName = $"{item.RevenueHeadVM.Name}-{item.RevenueHeadVM.Code}: {item.AdditionalDescription}",
                    Qty = item.Quantity,
                });
            });

            return new RevenueHeadRequestValidationObject { PartInvoiceItems = partInvoiceItems };
        }


        /// <summary>
        /// Save Invoice Items
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="rhHelperModels"></param>
        private List<InvoiceItems> SaveInvoiceItems(Invoice invoice, List<GenerateInvoiceRequestModel> requestModel)
        {
            List<InvoiceItems> invoiceItems = new List<InvoiceItems> { };
            invoiceItems.AddRange(requestModel.Select(item => new InvoiceItems
            {
                Invoice = invoice,
                Mda = new MDA { Id = item.MDAVM.Id },
                RevenueHead = new RevenueHead { Id = item.RevenueHeadVM.Id },
                TaxEntity = invoice.TaxPayer,
                InvoiceNumber = invoice.InvoiceNumber,
                Quantity = item.Quantity,
                UnitAmount = item.Amount,
                InvoicingUniqueIdentifier = item.CashflowUniqueIdentifier,
                TaxEntityCategory = invoice.TaxPayerCategory,
            }));

            foreach (var invoiceItem in invoiceItems)
            {
                _invoiceItemsDAOManager.Add(invoiceItem);
            }

            return invoiceItems;
        }


        /// <summary>
        /// Builds Invoice Helper Model
        /// </summary>
        /// <param name="groupDetails"></param>
        /// <param name="model"></param>
        /// <param name="invoiceDate"></param>
        /// <returns></returns>
        private CreateInvoiceHelper GetHelperModel(GenerateInvoiceRequestModel groupDetails, CreateInvoiceUserInputModel model, DateTime invoiceDate)
        {
            CreateInvoiceHelper helperModel = new CreateInvoiceHelper { };
            ////we need an amount for that invoice
            //get due date
            helperModel.DueDate = GetDueDate(groupDetails.BillingModelVM, invoiceDate);
            //get title
            helperModel.Title = model.InvoiceTitle;
            //get type
            helperModel.Type = "Single";
            //invoice description
            helperModel.InvoiceDescription = model.InvoiceDescription;
            //get footnotes for discounts and penalties that are to be applied
            helperModel.FootNotes = GetFootNotes(groupDetails.BillingModelVM);
            //invoice date
            helperModel.InvoiceDate = invoiceDate;
            helperModel.VAT = model.VAT;

            return helperModel;
        }


        /// <summary>
        /// Get due date on invoice date
        /// </summary>
        /// <param name="billingVM"></param>
        /// <param name="invoiceDate"></param>
        /// <returns>DateTime</returns>
        private DateTime GetDueDate(BillingModelVM billingVM, DateTime invoiceDate)
        {
            if (string.IsNullOrEmpty(billingVM.DueDate))
            { throw new NoDueDateTypeFoundException("No due date found for billing " + billingVM.Id); }

            DueDateModel dueDate = JsonConvert.DeserializeObject<DueDateModel>(billingVM.DueDate);
            if (dueDate == null)
            { throw new NoDueDateTypeFoundException("No due date found for billing. Due date is null for billing Id " + billingVM.Id); }

            //if the due date is due on the next billing date
            if (dueDate.DueOnNextBillingDate) { return billingVM.NextBillingDate.Value; }

            switch (dueDate.DueDateAfter)
            {
                case Core.Models.Enums.DueDateAfter.Days:
                    return invoiceDate.AddDays(dueDate.DueDateInterval);
                case Core.Models.Enums.DueDateAfter.Weeks:
                    return invoiceDate.AddDays(7 * dueDate.DueDateInterval);
                case Core.Models.Enums.DueDateAfter.Months:
                    return invoiceDate.AddMonths(dueDate.DueDateInterval);
                case Core.Models.Enums.DueDateAfter.Years:
                    return invoiceDate.AddYears(dueDate.DueDateInterval);
                default:
                    throw new NoDueDateTypeFoundException("No due date found for billing ");
            }
        }


        /// <summary>
        /// Build foot notes text
        /// </summary>
        /// <param name="billingModelVM"></param>
        /// <returns>string</returns>
        private string GetFootNotes(BillingModelVM billingVM)
        {
            string discountConcat = ""; string penaltyConcat = "";
            if (!string.IsNullOrEmpty(billingVM.DiscountJSONModel))
            {
                List<DiscountModel> discounts = JsonConvert.DeserializeObject<List<DiscountModel>>(billingVM.DiscountJSONModel);
                if (discounts.Any())
                {
                    discountConcat = "Discounts:\r\n";
                    foreach (var item in discounts)
                    {
                        var rate = item.BillingDiscountType == Core.Models.Enums.BillingDiscountType.Flat ? "Naira flat rate" : "% percent";
                        discountConcat += string.Format("\u2022 {0} {1} discount is applicable {2} {3} after invoice generation \r\n", item.Discount, rate, item.EffectiveFrom, item.EffectiveFromType.ToString().ToLower());
                    }
                }
            }

            if (!string.IsNullOrEmpty(billingVM.PenaltyJSONModel))
            {
                List<PenaltyModel> penalties = JsonConvert.DeserializeObject<List<PenaltyModel>>(billingVM.PenaltyJSONModel);
                if (penalties.Any())
                {
                    penaltyConcat = "Penalties:\r\n";
                    foreach (var item in penalties)
                    {
                        var rate = item.PenaltyValueType == Core.Models.Enums.PenaltyValueType.FlatRate ? "Naira flat rate" : "% percent";
                        penaltyConcat += string.Format("\u2022 penalty is applicable {2} {3} after due date \r\n", item.Value, item.PenaltyValueType, item.EffectiveFrom, item.EffectiveFromType.ToString().ToLower());
                    }
                }
            }
            return discountConcat + penaltyConcat;
        }


        /// <summary>
        /// Get request model we are sending to cashflow for invoice generation
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="entity"></param>
        /// <param name="invoiceHelper"></param>
        /// <returns></returns>
        private CashFlowCreateCustomerAndInvoice GetCashFlowRequestModel(int stateId, TaxEntityViewModel entity, CreateInvoiceHelper invoiceHelper)
        {
            return new CashFlowCreateCustomerAndInvoice
            {
                CreateCustomer = Core.Utilities.InvoiceUtil.CreateCashflowCustomer(stateId, new TaxEntity { Id = entity.Id, CashflowCustomerId = entity.CashflowCustomerId, Recipient = entity.Recipient, StateLGA = new LGA { State = new StateModel { Id = entity.SelectedState } }, TaxEntityCategory = new TaxEntityCategory { Id = entity.CategoryId }, Email = entity.Email }),
                CreateInvoice = Core.Utilities.InvoiceUtil.CreateCashflowCustomerInvoice(invoiceHelper),
                InvoiceUniqueKey = invoiceHelper.UniqueInvoiceIdentifier,
                PropertyTitle = "CentralBillingSystem"
            };
        }


        /// <summary>
        /// Creates invoice on cashflow
        /// </summary>
        /// <param name="smeKey"></param>
        /// <param name="cashFlowRequestModel"></param>
        /// <returns></returns>
        private CashFlowCreateCustomerAndInvoiceResponse CreateInvoiceOnCashflow(string smeKey, CashFlowCreateCustomerAndInvoice cashFlowRequestModel)
        {
            try
            {
                log.Info("Calling cashflow");
                var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", smeKey } });
                var invoiceService = _invoicingService.InvoiceService(context);
                IntegrationResponseModel response = invoiceService.CreateCustomerAndInvoice(cashFlowRequestModel);
                if (response.HasErrors)
                {
                    log.Error("Error on invoice request on cashflow ");
                    string errors = response.ResponseObject;
                    string message = string.Format("ErrorCode : {0} Error: {1}", response.ErrorCode, errors);
                    log.Error(message);
                    throw new CannotConnectToCashFlowException(message);
                }
                var objjson = JsonConvert.SerializeObject(response.ResponseObject);
                return JsonConvert.DeserializeObject<CashFlowCreateCustomerAndInvoiceResponse>(objjson);
            }
            catch (Exception exception)
            {
                log.Error(exception.Message, exception);
                throw new CannotConnectToCashFlowException(exception.Message + exception.StackTrace);
            }
        }


        /// <summary>
        /// create invoice details
        /// </summary>
        /// <param name="invoiceHelper"></param>
        /// <param name="model"></param>
        /// <param name="resultfromCF"></param>
        /// <returns>Invoice</returns>
        /// <exception cref="CouldNotSaveInvoiceOnCentralBillingException"></exception>
        private Invoice SaveInvoice(CreateInvoiceHelper invoiceHelper, CreateInvoiceUserInputModel model, CashFlowCreateCustomerAndInvoiceResponse resultfromCF, ExpertSystemVM expertSettingsVM, TaxEntityViewModel entity, GenerateInvoiceRequestModel revenueHeadDetails, Orchard.Users.Models.UserPartRecord adminUser = null)
        {
            string smodel = JsonConvert.SerializeObject(invoiceHelper);
            log.Info(string.Format("saving invoice model: {0}, invoice number {1}", smodel, resultfromCF.Invoice.Number));

            Invoice invoice = new Invoice
            {
                Amount = resultfromCF.Invoice.AmountDue,
                CashflowInvoiceIdentifier = invoiceHelper.UniqueInvoiceIdentifier,
                CallBackURL = $"{model.CallBackURL}&invoiceNumber={resultfromCF.Invoice.Number}",
                DueDate = invoiceHelper.DueDate,
                ExpertSystemSettings = new ExpertSystemSettings { Id = expertSettingsVM.Id },
                InvoiceDescription = invoiceHelper.InvoiceDescription,
                InvoiceNumber = resultfromCF.Invoice.Number,
                InvoiceType = (int)Core.Models.Enums.InvoiceType.Standard,
                ExternalRefNumber = model.ExternalRefNumber,
                InvoiceURL = resultfromCF.Invoice.IntegrationPreviewUrl,
                InvoiceTitle = resultfromCF.Invoice.Title,
                TaxPayer = new TaxEntity { Id = entity.Id },
                Status = (int)Core.Models.Enums.InvoiceStatus.Unpaid,
                InvoiceModel = smodel,
                GeneratedByAdminUser = adminUser,
                Mda = new MDA { Id = revenueHeadDetails.MDAVM.Id },
                RevenueHead = new RevenueHead { Id = revenueHeadDetails.RevenueHeadVM.Id },
                TaxPayerCategory = new TaxEntityCategory { Id = entity.CategoryId },
                Quantity = invoiceHelper.Items.Count
            };

            _invoiceDAOManager.Add(invoice);

            return invoice;
        }


        /// <summary>
        /// Save transaction log for invoice generation
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="invoice"></param>
        /// <param name="model"></param>
        /// <returns>List<TransactionLog></returns>
        private List<TransactionLog> SaveTransactionLog(Invoice invoice, GenerateInvoiceModel model, List<InvoiceItems> items)
        {
            List<TransactionLog> tranItems = new List<TransactionLog> { };
            tranItems.AddRange(items.Select(item => new TransactionLog
            {
                PaymentReference = string.Format("{0}", invoice.InvoiceNumber),
                AmountPaid = Math.Round(item.Quantity * item.UnitAmount, 2),
                Invoice = invoice,
                PaymentDate = invoice.CreatedAtUtc,
                Status = Core.Models.Enums.PaymentStatus.Pending,
                TaxEntity = model.Entity,
                TypeID = (int)Core.Models.Enums.PaymentType.Bill,
                RevenueHead = item.RevenueHead,
                MDA = item.Mda,
                TaxEntityCategory = item.TaxEntityCategory,
                UpdatedByAdmin = model.AdminUser == null ? false : true,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceItem = item,
            }));

            foreach (var tranItem in tranItems)
            {
                _transactionLogDAOManager.Add(tranItem);
            }

            return tranItems;
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
        private void SaveServiceRequest(PSSRequestVM request, IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeads, int serviceId, Int64 invoiceId, int definitionLevelId, PSSRequestStatus status = PSSRequestStatus.Pending)
        {
            foreach (var item in serviceRevenueHeads.Where(sr => !sr.IsGroupHead))
            {
                PoliceServiceRequest serviceRequest = new PoliceServiceRequest
                {
                    Request = new PSSRequest { Id = request.Id },
                    Service = new PSService { Id = serviceId },
                    RevenueHead = new RevenueHead { Id = item.RevenueHeadId },
                    Invoice = new Invoice { Id = invoiceId },
                    FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = definitionLevelId },
                    Status = (int)status,
                };

                _policeServiceRequestDAOManager.Add(serviceRequest);
            }
        }


        /// <summary>
        /// Get the last level definition with specified workflow action value
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="hasDifferentialWorkFlow">show if the workflow assigned to the service has other factor that determine the direction to follow</param>
        /// <param name="dataParams">this would hold the additional data param for differential workflow if any</param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO</returns>
        private PSServiceRequestFlowDefinitionLevelDTO GetLastFlowLevelWithWorkflowActionValue(int serviceId, bool hasDifferentialWorkFlow, ServiceWorkFlowDifferentialDataParam dataParams, RequestDirection actionValue)
        {
            int flowDefinitionId = hasDifferentialWorkFlow ? _serviceWorkflowDifferentialDAOManager.GetFlowDefinitionIdAttachedToServiceWithDifferentialValue(serviceId, dataParams) : _serviceDAOManager.GetFlowDefinitionForService(serviceId);
            return _serviceRequestFlowDefinitionLevelDAOManager.GetLastLevelDefinitionWithWorkflowActionValue(flowDefinitionId, actionValue);
        }


        /// <summary>
        /// Sends SMS and Email notification to user credentials specified in <paramref name="invoiceGenResponse"/>
        /// </summary>
        /// <param name="requestDeets"></param>
        private void SendNofications(CashFlowCreateCustomerAndInvoiceResponse invoiceGenResponse, CBSUserVM cbsUser, int paymentBillingType, string tenantName)
        {
            //send sms notification
            string message = string.Format("Dear {0}, your {1} invoice {2} of NGN{3:N2} for ESCORT AND GUARD SERVICES has been generated successfully and its due for payment", cbsUser.Name, ((PSBillingType)paymentBillingType).GetDescription(), invoiceGenResponse.Invoice.Number, invoiceGenResponse.Invoice.AmountDue);
            SendGenericSMSNotification(new List<string> { cbsUser.PhoneNumber }, message, tenantName);

            //send email notification
            EmailNotificationModel notificationModel = new EmailNotificationModel
            {
                Sender = PSSPulseTemplateFileNames.Sender.GetDescription(),
                Subject = $"{((PSBillingType)paymentBillingType).GetDescription()} Escort And Guards Invoice Notification",
                CBSUser = new CBSUserVM { Email = cbsUser.Email },
                TemplateFileName = PSSPulseTemplateFileNames.EGSRegularizationInvoiceNotification.GetDescription(),
                Params = new Dictionary<string, string> { { "Name", cbsUser.Name }, { "InvoiceNumber", invoiceGenResponse.Invoice.Number }, { "AmountDue", invoiceGenResponse.Invoice.AmountDue.ToString("N2") }, { "PaymentBillingType", ((PSBillingType)paymentBillingType).GetDescription() }, { "BaseUrl", ConfigurationManager.AppSettings["POSSAPBaseURL"] } }
            };

            SendGenericEmailNotification(notificationModel, tenantName);
        }


        /// <summary>
        /// Send any generic SMS notification
        /// </summary>
        /// <param name="smsDetails"></param>
        public void SendGenericSMSNotification(List<string> phoneNumbers, string message, string tenantName)
        {
            try
            {
                string isSMSEnabledConfig = ConfigurationManager.AppSettings["IsSMSEnabled"];
                if (!string.IsNullOrEmpty(isSMSEnabledConfig))
                {
                    bool isSMSEnabled = false;
                    bool.TryParse(isSMSEnabledConfig, out isSMSEnabled);
                    if (isSMSEnabled)
                    {
                        foreach (string phoneNumber in phoneNumbers)
                        {
                            if (phoneNumbers.Count == 0 || string.IsNullOrEmpty(message))
                            {
                                throw new Exception($"Phone number or message is empty");
                            }

                            List<RecipientInfo> recipients = new List<RecipientInfo>();

                            RecipientInfo recipientInfo = new RecipientInfo
                            {
                                Value = phoneNumber,
                                Type = RecipientType.SMS,
                                Channel = NotificationChannel.Unknown
                            };
                            recipients.Add(recipientInfo);

                            Dictionary<string, string> smsDetails = new Dictionary<string, string>();
                            smsDetails.Add("Code", defaultCode);
                            smsDetails.Add("Message", message);

                            PulseData pulseData = new PulseData();
                            pulseData.Recipients = recipients;
                            pulseData.Params = smsDetails;

                            Pulse pulse = new Pulse();
                            pulse.Data = pulseData;
                            pulse.Name = ConfigurationManager.AppSettings["PulseSMSTemplateName"];

                            PulseHeader pulseHeader = new PulseHeader();
                            pulseHeader.Type = RecipientType.SMS.ToString();

                            _smsNotification.SendNotificationPulse(pulse, pulseHeader);
                        }
                    }
                }
            }
            catch (Exception exception) { log.Error(exception.Message); }
        }


        /// <summary>
        /// Send email notification
        /// </summary>
        /// <param name="emailDetails"></param>
        public void SendGenericEmailNotification(EmailNotificationModel emailModel, string tenantName)
        {
            try
            {
                string isEmailEnabledConfig = ConfigurationManager.AppSettings["IsEmailEnabled"];
                if (!string.IsNullOrEmpty(isEmailEnabledConfig))
                {
                    bool isEmailEnabled = false;
                    bool.TryParse(isEmailEnabledConfig, out isEmailEnabled);
                    if (isEmailEnabled && !string.IsNullOrEmpty(emailModel.CBSUser.Email))
                    {
                        int providerId = 0;
                        bool result = int.TryParse(ConfigurationManager.AppSettings["EmailProvider"], out providerId);
                        if (!result)
                        {
                            providerId = (int)EmailProvider.Pulse;
                        }

                        switch (providerId)
                        {
                            case (int)EmailProvider.Gmail:
                                string sModel = JsonConvert.SerializeObject(new { Recipient = emailModel.CBSUser.Email, Subject = emailModel.Subject });
                                _emailNotification.SendNotificationGmail(sModel, emailModel.Params, $"{emailModel.TemplateFileName}.html");
                                break;

                            case (int)EmailProvider.Pulse:
                                Dictionary<string, string> emailDetails = new Dictionary<string, string>();

                                string sDataParams = JsonConvert.SerializeObject(emailModel.Params);
                                emailDetails.Add("Subject", emailModel.Subject);
                                emailDetails.Add("Sender", emailModel.Sender);
                                emailDetails.Add("Params", sDataParams);

                                PulseData pulseData = new PulseData();
                                pulseData.Recipients = new List<RecipientInfo>
                                {
                                    new RecipientInfo
                                    {
                                        Value = emailModel.CBSUser.Email,
                                        Type = RecipientType.Email,
                                        Channel = NotificationChannel.Unknown
                                    }
                                };

                                pulseData.Params = emailDetails;

                                Pulse pulse = new Pulse();
                                pulse.Data = pulseData;
                                pulse.Name = emailModel.TemplateFileName;

                                PulseHeader pulseHeader = new PulseHeader();
                                pulseHeader.Type = nameof(RecipientType.Email);

                                _emailNotification.SendNotificationPulse(pulse, pulseHeader);
                                break;
                        }
                    }
                }
            }
            catch (Exception exception) { log.Error(exception.Message); }
        }
    }
}
