using Hangfire;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl;
using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Entities.VMs;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.Services.Implementations.Contracts;
using Parkway.CBS.Services.Logger;
using Parkway.CBS.Services.Logger.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace Parkway.CBS.Services.Implementations
{
    public class BatchInvoiceResponseProcessor : IBatchInvoiceResponseProcessor
    {
        private static readonly ILogger log = new ServicesLog4netLogger();
        public IUoW UoW { get; set; }
        public IReferenceDataBatchDAOManager BatchDAO { get; set; }

        public IReferenceDataRecordsInvoiceDAOManager RecordsInvoiceDAO { get; set; }

        public IInvoiceDAOManager InvoiceDAO { get; set; }

        public IBatchInvoiceResponseDAOManager BatchInvoiceResponseDAO { get; set; }

        public ITransactionLogDAOManager TransactionLogDAO { get; set; }

        public IReferenceDataTaxEntityStagingDAOManager TaxEntityStagingRecordsDAO { get; set; }

        public IInvoicingService InvoicingService;

        protected void SetBatchDAO()
        { if (BatchDAO == null) { BatchDAO = new ReferenceDataBatchDAOManager(UoW); } }

        private void SetRecordsInvoiceDAO()
        { if (RecordsInvoiceDAO == null) { RecordsInvoiceDAO = new ReferenceDataRecordsInvoiceDAOManager(UoW); } }

        private void SetInvoiceDAO()
        { if (InvoiceDAO == null) { InvoiceDAO = new InvoiceDAOManager(UoW); } }

        private void SetTransactionLogDAO()
        { if (TransactionLogDAO == null) { TransactionLogDAO = new TransactionLogDAOManager(UoW); } }

        protected void SetTaxEntityStagingRecordsDAO()
        { if (TaxEntityStagingRecordsDAO == null) { TaxEntityStagingRecordsDAO = new ReferenceDataTaxEntityStagingDAOManager(UoW); } }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName + "_SessionFactory", "ClientFileServices");
            }
        }

        public BatchInvoiceResponseProcessor()
        {

        }

        public void InstantiateInvoicingService()
        { if (InvoicingService == null) { InvoicingService = new InvoicingService(); } }

        private void SetBatchInvoiceResponseDAO()
        { if (BatchInvoiceResponseDAO == null) { BatchInvoiceResponseDAO = new BatchInvoiceResponseDAOManager(UoW); } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void UpdateInvoiceStagingWithCashFlowResponse(string tenantName, long batchId)
        {
            try
            {
                log.Info($"Inside updating batch invoice for batch {batchId}");

                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);

                CheckProcessStage(batchRecord, ReferenceDataProcessingStages.SavedCashflowBatchInvoiceResponse);

                UoW.BeginTransaction();
                SetRecordsInvoiceDAO();

                RecordsInvoiceDAO.UpdateInvoiceStagingWithCashFlowResponse(batchId);
                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.UpdateInvoiceStagingWithCashFlowResponse;
                UoW.Commit();
                log.Info($"After updating batch invoice for batch {batchId}");

                StartHangfireServer(tenantName);
                BackgroundJob.Enqueue(() => CreateInvoices(tenantName, batchId));
            }
            catch (Exception exception)
            {
                log.Error($"Errooorr");

                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                RecordsInvoiceDAO = null;
                BatchDAO = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void CreateInvoices(string tenantName, long batchId)
        {
            try
            {
                log.Info($"Starting Create Invoices processing for Batch Invoice Response for Stage {ReferenceDataProcessingStages.MoveInvoiceStagingRecordsToInvoiceTable}");

                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);
                CheckProcessStage(batchRecord, ReferenceDataProcessingStages.UpdateInvoiceStagingWithCashFlowResponse);
                SetInvoiceDAO();
                UoW.BeginTransaction();
                InvoiceDAO.CreateBatchInvoices(batchId, batchRecord.RevenueHead);
                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.MoveInvoiceStagingRecordsToInvoiceTable;
                UoW.Commit();

                log.Info($"Done Create Invoices for batch Id {batchId}. Batch Invoice Response for Stage {ReferenceDataProcessingStages.MoveInvoiceStagingRecordsToInvoiceTable}");

                //Call the Hangfire storage
                StartHangfireServer(tenantName);
                BackgroundJob.Enqueue(() => CreateTransactionLog(tenantName, batchRecord.Id));
                log.Info($"Queued the job successfully. Stage {ReferenceDataProcessingStages.MoveInvoiceStagingRecordsToInvoiceTable}");
            }
            catch (Exception exception)
            {
                log.Error($"Error Processing for stage {ReferenceDataProcessingStages.MoveInvoiceStagingRecordsToInvoiceTable}. WithHolding Tax on Rent. Batch ID {batchId}");
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchDAO = null;
                InvoiceDAO = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void CreateTransactionLog(string tenantName, long batchId)
        {
            try
            {
                log.Info($"Starting Create Transaction logs for Batch Invoice Response for Stage {ReferenceDataProcessingStages.MoveInvoiceStagingRecordsToTransactionsLogTable}");

                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);
                CheckProcessStage(batchRecord, ReferenceDataProcessingStages.MoveInvoiceStagingRecordsToInvoiceTable);
                SetTransactionLogDAO();
                UoW.BeginTransaction();
                TransactionLogDAO.CreateBatchInvoiceTransactionLog(batchId, batchRecord.RevenueHead);
                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.MoveInvoiceStagingRecordsToTransactionsLogTable;
                UoW.Commit();

                log.Info($"Done Create Invoices for batch Id {batchId}. Batch Invoice Response for Stage {ReferenceDataProcessingStages.MoveInvoiceStagingRecordsToTransactionsLogTable}");

                //Call the Hangfire storage
                StartHangfireServer(tenantName);
                BackgroundJob.Enqueue(() => UpdateTaxEntityWithCashflowCustomerDetails(tenantName, batchRecord.Id));
                log.Info($"Queued the job successfully. Stage {ReferenceDataProcessingStages.MoveInvoiceStagingRecordsToTransactionsLogTable}");
            }
            catch (Exception exception)
            {
                log.Error($"Error Processing for stage {ReferenceDataProcessingStages.MoveInvoiceStagingRecordsToTransactionsLogTable}. WithHolding Tax on Rent. Batch ID {batchId}");
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchDAO = null;
                InvoiceDAO = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void UpdateTaxEntityWithCashflowCustomerDetails(string tenantName, long batchId)
        {
            try
            {
                log.Info($"Starting updating Tax Entity customer details for Batch Id {batchId}. Batch Invoice Response for Stage {ReferenceDataProcessingStages.MoveInvoiceStagingRecordsToTransactionsLogTable}");

                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);
                CheckProcessStage(batchRecord, ReferenceDataProcessingStages.MoveInvoiceStagingRecordsToTransactionsLogTable);
                SetTaxEntityStagingRecordsDAO();
                UoW.BeginTransaction();
                TaxEntityStagingRecordsDAO.UpdateTaxEntityWithCashflowInvoiceResponse(batchId);

                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.UpdateTaxEntityWithCashFlowCustomerDetails;
                if (batchRecord.Page == batchRecord.TotalPage)
                {
                    batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.Completed;
                }
                UoW.Commit();

                log.Info($"Done updating the Tax Entity for batch Id {batchId}. Batch Invoice Response for Stage {ReferenceDataProcessingStages.UpdateTaxEntityWithCashFlowCustomerDetails}");
            }
            catch (Exception exception)
            {
                log.Error($"Error Processing for stage {ReferenceDataProcessingStages.UpdateTaxEntityWithCashFlowCustomerDetails}. Batch ID {batchId}");
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchDAO = null;
                InvoiceDAO = null;
            }
        }

        /// <summary>
        /// Check if the process stage is valid
        /// </summary>
        /// <param name="batchRecord"></param>
        /// <param name="expectedStage"></param>
        private void CheckProcessStage(ReferenceDataBatch batchRecord, ReferenceDataProcessingStages expectedStage)
        {
            if (batchRecord != null)
            {
                //lets check what stage it is in
                //for this method the expected stage is FileValidationProcessed
                if (batchRecord.ProccessStage != (int)expectedStage)
                { throw new InvalidOperationException($"ReferenceDataBatch processing has already passed this stage. Current stage {(ReferenceDataProcessingStages)batchRecord.ProccessStage}, Expected stage {expectedStage}, Batch Id {batchRecord.Id}"); }
            }
        }

        /// <summary>
        /// Call CashFlow to get details of batch Invoice 
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void GetCashFlowBatchInvoiceResponse(string tenantName, long batchId, string batchFileName)
        {
            try
            {
                //Get the pageSize
                int pageSize = 500;
                string configPageSize = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.ChunkSizeForBatchInvoiceGenerationResponse);
                if (!string.IsNullOrEmpty(configPageSize))
                {
                    Int32.TryParse(configPageSize, out pageSize);
                }

                string cashFlowBatchInvoiceURL = ConfigurationManager.AppSettings["CashFlowBaseFileLocationForBatchInvoice"];
                if (string.IsNullOrEmpty(cashFlowBatchInvoiceURL))
                {
                    throw new Exception("Unable to get config Batch Invoice PDF URL");
                }

                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchDetails(batchId);
                CheckProcessStage(batchRecord, ReferenceDataProcessingStages.SentBulkInvoiceRequestToCashFlow);

                //get the pages
                int pages = Util.Pages(pageSize, batchRecord.NumberOfRecordSentToCashFlow);
                int stopper = 0;
                List<CashFlowBatchInvoiceResponse> listOfProcessedInvoices = new List<CashFlowBatchInvoiceResponse> { };
                InstantiateInvoicingService();
                SetBatchInvoiceResponseDAO();
                UoW.BeginTransaction();
                batchRecord.TotalPage = pages;
                batchRecord.BatchInvoiceFileName = $"{cashFlowBatchInvoiceURL}\\{batchFileName}";

                while (stopper < pages)
                {
                    stopper++;
                    log.Info($"Starting pulling the Invoice response details for batch Id {batchId}. Page {stopper} of {pages}");

                    CashFlowRequestContext context = InvoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", batchRecord.RevenueHead.Mda.SMEKey } });

                    var webCallResult = InvoicingService.InvoiceService(context).BatchInvoicePagedResult(new BatchResultPageModel { BatchRef = batchRecord.GeneralBatchReference.Id.ToString(), Page = stopper, PageSize = pageSize });

                    if(webCallResult.HasErrors == true)
                    {
                        throw new Exception("Erorr retrieving the records from CashFlow Batch Invoice");
                    }

                    log.Info($"Done pulling the Invoice response for batch Id {batchId}. Page {stopper} of {pages}");
                    var response = new CashFlowBatchInvoiceResponse { ResultModel = webCallResult.ResponseObject.ResultModel, BatchIdentifier = webCallResult.ResponseObject.BatchIdentifier, DoneProcessing = webCallResult.ResponseObject.DoneProcessing };
                    BatchInvoiceResponseDAO.SaveBundle(response.ResultModel, batchRecord.GeneralBatchReference.Id.ToString());
                }

                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.SavedCashflowBatchInvoiceResponse;
                batchRecord.Page = stopper;
                UoW.Commit();

                StartHangfireServer(tenantName);
                BackgroundJob.Enqueue(() => UpdateInvoiceStagingWithCashFlowResponse(tenantName, batchRecord.Id));
            }
            catch (Exception exception)
            {
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchDAO = null;
                BatchInvoiceResponseDAO = null;
                InvoicingService = null;
            }
        }

        private void StartHangfireServer(string tenantName)
        {
            //Get the connection string
            string dbConnectionString = HangFireScheduler.GetConnectionString(tenantName);
            //Call the Hangfire storage
            GlobalConfiguration.Configuration.UseSqlServerStorage(dbConnectionString);
        }

    }
}
