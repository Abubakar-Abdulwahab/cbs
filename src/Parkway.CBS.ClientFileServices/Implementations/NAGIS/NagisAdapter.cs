using Hangfire;
using Newtonsoft.Json;
using Parkway.Cashflow.Ng.API;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.ClientFileServices.Implementations.Contracts;
using Parkway.CBS.ClientFileServices.Implementations.IPPIS;
using Parkway.CBS.ClientFileServices.Implementations.Models;
using Parkway.CBS.ClientFileServices.Logger.Contracts;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.ClientRepository.Repositories.NAGIS;
using Parkway.CBS.ClientRepository.Repositories.NAGIS.Contracts;
using Parkway.CBS.ClientRepository.Repositories.NAGIS.Models;
using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl;
using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts;
using Parkway.CBS.ClientServices.Invoicing;
using Parkway.CBS.ClientServices.Invoicing.Contracts;
using Parkway.CBS.ClientServices.Services;
using Parkway.CBS.ClientServices.Services.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Entities.DTO;
using Parkway.CBS.Entities.VMs;
using Parkway.CBS.FileUpload.NAGISImplementation;
using Parkway.CBS.FileUpload.NAGISImplementation.Contracts;
using Parkway.CBS.FileUpload.NAGISImplementation.Models;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using Parkway.CBS.Payee.PayeeAdapters.ReferenceData;
using Parkway.CBS.Payee.ReferenceDataImplementation;
using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientFileServices.Implementations.NAGIS
{
    public class NAGISAdapter : IReferenceDataFileProcessor
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName + "_SessionFactory", "ClientFileServices");
            }
        }

        public INAGISDataBatchDAOManager BatchDAO { get; set; }

        protected void SetBatchDAO()
        { if (BatchDAO == null) { BatchDAO = new NAGISDataBatchDAOManager(UoW); } }

        public INAGISDataAdapter nagisDataAdapter { get; set; }

        private void SetNAGISDataAdapter()
        { if (nagisDataAdapter == null) { nagisDataAdapter = new NAGISDataAdapter(); } }

        public INagisOldInvoicesDAOManager NagisOldInvoicesDAO { get; set; }

        protected void SetOldInvoicesDAO()
        { if (NagisOldInvoicesDAO == null) { NagisOldInvoicesDAO = new NagisOldInvoicesDAOManager(UoW); } }

        public INagisOldInvoiceSummaryDAOManager NagisOldInvoiceSummaryDAO { get; set; }

        protected void SetOldInvoiceSummaryDAO()
        { if (NagisOldInvoiceSummaryDAO == null) { NagisOldInvoiceSummaryDAO = new NagisOldInvoiceSummaryDAOManager(UoW); } }

        public IRevenueHeadDAOManager RevenueHeadDAO { get; set; }

        private void SetRevenueHeadDAO()
        { if (RevenueHeadDAO == null) { RevenueHeadDAO = new RevenueHeadDAOManager(UoW); } }

        public IInvoicingService InvoicingService;

        public void InstantiateInvoicingService()
        { if (InvoicingService == null) { InvoicingService = new InvoicingService(); } }

        public IInvoiceGenerationType InvoiceGenerationType = new DirectAssessmentInvoiceGeneration { };

        public IBatchInvoiceResponseDAOManager BatchInvoiceResponseDAO { get; set; }

        private void SetBatchInvoiceResponseDAO()
        { if (BatchInvoiceResponseDAO == null) { BatchInvoiceResponseDAO = new BatchInvoiceResponseDAOManager(UoW); } }

        public INagisOldCustomersDAOManager CustomersDAO { get; set; }

        private void SetCustomersDAO()
        { if (CustomersDAO == null) { CustomersDAO = new NagisOldCustomersDAOManager(UoW); } }


        public IInvoiceDAOManager InvoiceDAO { get; set; }

        private void SetInvoiceDAO()
        { if (InvoiceDAO == null) { InvoiceDAO = new InvoiceDAOManager(UoW); } }

        public IInvoiceItemsDAOManager InvoiceItemsDAO { get; set; }

        private void SetInvoiceItemsDAO()
        { if (InvoiceItemsDAO == null) { InvoiceItemsDAO = new InvoiceItemsDAOManager(UoW); } }

        public INagisOldInvoiceCustomerResponseDAOManager CustomerResponseDAO { get; set; }

        private void SetCustomerResponseDAO()
        { if (CustomerResponseDAO == null) { CustomerResponseDAO = new NagisOldInvoiceCustomerResponseDAOManager(UoW); } }

        public ITransactionLogDAOManager TransactionLogDAO { get; set; }

        private void SetTransactionLogDAO()
        { if (TransactionLogDAO == null) { TransactionLogDAO = new TransactionLogDAOManager(UoW); } }

        public ITaxEntityDAOManager TaxEntityDAO { get; set; }

        private void SetTaxEntityDAO()
        { if (TaxEntityDAO == null) { TaxEntityDAO = new TaxEntityDAOManager(UoW); } }

        public ValidateFileResponse SaveFile(string tenantName, string filePath, long batchId)
        {
            try
            {
                log.Info($"Inside Nagis implementation {NagisDataProcessingStages.NotProcessed} {filePath}");


                //set unit of work
                SetUnitofWork(tenantName);
                //instantiate the ReferenceDataBatch repository
                SetBatchDAO();
                NagisDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);

                SetNAGISDataAdapter();
                //now lets validate and save the file contents
                NAGISDataResponse nagisDataResponse = nagisDataAdapter.GetNAGISDataResponseModels(filePath);
                if (nagisDataResponse.HeaderValidateObject.Error)
                {
                    batchRecord.ErrorOccurred = true;
                    batchRecord.ErrorMessage = nagisDataResponse.HeaderValidateObject.ErrorMessage;
                    return new ValidateFileResponse { BatchId = batchRecord.Id, ErrorMessage = "Error reading the file. Some header parameters are missing for batch Id: " + batchRecord.Id, ErrorOccurred = true };
                }
                //if no error occurred while reading the file, lets save the records to DB

                UoW.BeginTransaction();

                SetOldInvoicesDAO();

                int count = NagisOldInvoicesDAO.SaveRecords(tenantName, batchRecord.Id, nagisDataResponse.NAGISDataLineRecords);
                batchRecord.NumberOfRecords = count;
                //update the stage 
                batchRecord.ProccessStage = (int)NagisDataProcessingStages.FileSaved;
                batchRecord.FilePath = filePath;
                batchRecord.FileName = System.IO.Path.GetFileName(filePath);
                batchRecord.PercentageProgress = 100;
                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => UpdateNagisOldInvoicesStagingRecordsOperationType(tenantName, batchId));

                log.Info(string.Format("Processing for stage {1} completed. FilePath {0}", filePath, NagisDataProcessingStages.FileSaved));

                return new ValidateFileResponse { BatchId = batchRecord.Id };
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {1}. FilePath {0}", filePath, NagisDataProcessingStages.NotProcessed));
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchDAO = null;
                nagisDataAdapter = null;
                NagisOldInvoicesDAO = null;
            }
        }

        /// <summary>
        /// Update the operation Type for NagisOldInvoices table Records
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void UpdateNagisOldInvoicesStagingRecordsOperationType(string tenantName, long batchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                NagisDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);

                CheckProcessStage(batchRecord, NagisDataProcessingStages.FileSaved);

                UoW.BeginTransaction();
                SetOldInvoicesDAO();

                NagisOldInvoicesDAO.UpdateNagisOldInvoicesStagingRecordsOperationType(batchId);
                batchRecord.ProccessStage = (int)NagisDataProcessingStages.UpdateNagisOldInvoicesStagingOperationType;
                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => CategorizeByNAGISCustomerId(tenantName, batchId));
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
                NagisOldInvoicesDAO = null;
                BatchDAO = null;
            }
        }

        [ProlongExpirationTime]
        /// <summary>
        /// This method categorizes the records store into distinct NAGIS Invoice Number
        /// </summary>
        /// <param name="batchId"></param>
        public void CategorizeByNAGISCustomerId(string tenantName, long batchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                NagisDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);

                CheckProcessStage(batchRecord, NagisDataProcessingStages.UpdateNagisOldInvoicesStagingOperationType);

                UoW.BeginTransaction();

                SetCustomersDAO();
                CustomersDAO.CreateNAGISCustomers(batchId);

                UoW.Commit();

                //Call the Hangfire storage
                StartHangfireServer();
                BackgroundJob.Enqueue(() => CreateTaxEntityWithNAGISDataRecords(tenantName, batchId));
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
                CustomersDAO = null;
                BatchDAO = null;
            }
        }

        /// <summary>
        /// Create the Tax Entity for the payer that doesn't exist
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void CreateTaxEntityWithNAGISDataRecords(string tenantName, long batchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                NagisDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);

                CheckProcessStage(batchRecord, NagisDataProcessingStages.UpdateNagisOldInvoicesStagingOperationType);


                UoW.BeginTransaction();
                SetOldInvoicesDAO();

                NagisOldInvoicesDAO.CreateTaxEntityWithNAGISDataRecordsUsingStagingHelper(batchId);
                batchRecord.ProccessStage = (int)NagisDataProcessingStages.TaxPayerCreated;
                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => UpdateTaxEntityWithNAGISDataRecords(tenantName, batchId));
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
                NagisOldInvoicesDAO = null;
                BatchDAO = null;
            }
        }

        /// <summary>
        /// Update the Tax Entity records for the payers that already exist CBS
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void UpdateTaxEntityWithNAGISDataRecords(string tenantName, long batchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                NagisDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);

                CheckProcessStage(batchRecord, NagisDataProcessingStages.TaxPayerCreated);


                UoW.BeginTransaction();
                SetOldInvoicesDAO();

                NagisOldInvoicesDAO.UpdateTaxEntityWithNAGISDataRecords(batchId);
                batchRecord.ProccessStage = (int)NagisDataProcessingStages.UpdateExistingTaxPayerRecords;
                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => UpdateNAGISOldInvoicesStagingRecordsTaxEntityId(tenantName, batchId));
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
                NagisOldInvoicesDAO = null;
                BatchDAO = null;
            }
        }

        /// <summary>
        /// Update the TaxEntity Id on the NagisOldInvoices table after creation on the Tax Entity Table
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void UpdateNAGISOldInvoicesStagingRecordsTaxEntityId(string tenantName, long batchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                NagisDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);

                CheckProcessStage(batchRecord, NagisDataProcessingStages.UpdateExistingTaxPayerRecords);

                UoW.BeginTransaction();

                SetCustomersDAO();
                CustomersDAO.UpdateNAGISCustomerRecordsTaxEntityId(batchId);

                SetOldInvoicesDAO();
                NagisOldInvoicesDAO.UpdateNAGISDataStagingRecordsTaxEntityId(batchId);

                batchRecord.ProccessStage = (int)NagisDataProcessingStages.UpdateNagisDataTaxEntityStaging;

                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => CategorizeByNAGISInvoiceNumber(tenantName, batchId));
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
                NagisOldInvoicesDAO = null;
                BatchDAO = null;
                CustomersDAO = null;
            }
        }

        [ProlongExpirationTime]
        /// <summary>
        /// This method categorizes the records store into distinct NAGIS Invoice Number
        /// </summary>
        /// <param name="batchId"></param>
        public void CategorizeByNAGISInvoiceNumber(string tenantName, long batchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                NagisDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);

                CheckProcessStage(batchRecord, NagisDataProcessingStages.UpdateNagisDataTaxEntityStaging);

                UoW.BeginTransaction();
                SetOldInvoicesDAO();

                NagisOldInvoicesDAO.GroupRecordsFromNAGISOldInvoicesRecordsTableByNAGISInvoiceNumber(batchId);
                batchRecord.ProccessStage = (int)NagisDataProcessingStages.CategorizeByNAGISInvoiceNumber;
                UoW.Commit();

                //Call the Hangfire storage
                StartHangfireServer();
                BackgroundJob.Enqueue(() => UpdateNAGISOldInvoicesStagingRecordsInvoiceSummaryId(tenantName, batchId));
                log.Info(string.Format("Queued the job successfully. Stage {0} completed", NagisDataProcessingStages.CategorizeByNAGISInvoiceNumber));
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
                NagisOldInvoicesDAO = null;
                BatchDAO = null;
            }
        }

        /// <summary>
        /// Update the TaxEntity Id on the NagisOldInvoices table after creation on the Tax Entity Table
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void UpdateNAGISOldInvoicesStagingRecordsInvoiceSummaryId(string tenantName, long batchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                NagisDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);

                CheckProcessStage(batchRecord, NagisDataProcessingStages.CategorizeByNAGISInvoiceNumber);

                UoW.BeginTransaction();
                SetOldInvoicesDAO();

                NagisOldInvoicesDAO.UpdateNAGISDataStagingRecordsNagisOldInvoiceSummaryId(batchId);
                batchRecord.ProccessStage = (int)NagisDataProcessingStages.UpdateNagisDataInvoiceSummaryStaging;

                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => CreateCustomer(tenantName, batchId));
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
                NagisOldInvoicesDAO = null;
                BatchDAO = null;
            }
        }

        [ProlongExpirationTime]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        public void CreateCustomer(string tenantName, long batchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                NagisDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);
                CheckProcessStage(batchRecord, NagisDataProcessingStages.UpdateNagisDataInvoiceSummaryStaging);

                //get the records you want to generate invoices for
                //chunk these records
                //lets get chunk size
                int chunkSize = 500;
                string schunkSize = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.ChunkSizeForIPPISInvoiceGeneration);
                if (!string.IsNullOrEmpty(schunkSize))
                {
                    if (Int32.TryParse(schunkSize, out chunkSize)) { chunkSize = 500; }
                }

                SetCustomersDAO();
                Int64 recordCount = CustomersDAO.Count(r => r.NagisDataBatch == new NagisDataBatch { Id = batchId });
                if (recordCount < 1) return;

                //get the pages
                int pages = Util.Pages(chunkSize, recordCount);
                int stopper = 0;
                int skip = 0;

                List<ConcurrentStack<NAGISGenerateCustomerResult>> listOfProcessedCustomers = new List<ConcurrentStack<NAGISGenerateCustomerResult>> { };

                while (stopper < pages)
                {
                    log.Info(string.Format("Starting customer generation by chunk for batch Id {0}. Page {1} of {2}", batchId, stopper, pages));

                    List<NAGISDataGenerateInvoiceModel> chunkedRecords = CustomersDAO.GetChunkedBatchCustomer(batchId, chunkSize, skip);
                    //var result = ProcessCustomers(chunkedRecords, batchRecord.StateModel.Id).Result;
                    var result = ProcessBatchCustomers(chunkedRecords, batchRecord.StateModel.Id);

                    log.Info(string.Format("Done customer generation by chunk for batch Id {0}. Page {1} of {2}", batchId, stopper, pages));

                    listOfProcessedCustomers.Add(result);
                    skip += chunkSize;
                    stopper++;
                }
                log.Info(string.Format("Saving bundle for customer generation for batch Id {0}..", batchId));


                SetCustomerResponseDAO();
                UoW.BeginTransaction();

                CustomerResponseDAO.SaveBundle(listOfProcessedCustomers, batchRecord.Id);
                batchRecord.ProccessStage = (int)NagisDataProcessingStages.CreateCustomerOnCashflow;
                UoW.Commit();

                //Call the Hangfire storage
                StartHangfireServer();
                BackgroundJob.Enqueue(() => UpdateTaxEntityWithCashflowCustomerDetails(tenantName, batchId));
                log.Info(string.Format("Queued the job successfully. Stage {0} completed", NagisDataProcessingStages.CategorizeByNAGISInvoiceNumber));
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
                CustomerResponseDAO = null;
                CustomersDAO = null;
                BatchDAO = null;
            }
        }

        [ProlongExpirationTime]
        /// <summary>
        /// Generate invoices for the batch records
        /// </summary>
        /// <param name="batchId"></param>
        public void GenerateInvoices(string tenantName, long batchId)
        {
            try
            {
                log.Info($"About to send batch invoice request to cashflow for batch {batchId}");

                string callbackURL = ConfigurationManager.AppSettings["BatchInvoiceCallbackURL"];
                if (string.IsNullOrEmpty(callbackURL))
                {
                    throw new Exception("Unable to get config Batch Invoice Callback URL");
                }

                SetUnitofWork(tenantName);
                SetBatchDAO();
                NagisDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);

                CheckProcessStage(batchRecord, NagisDataProcessingStages.UpdateTaxEntityWithCashFlowCustomerDetails);
                //get the records you want to generate invoices for
                //chunk these records
                //lets get chunk size
                int chunkSize = 20;
                string schunkSize = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.ChunkSizeForIPPISInvoiceGeneration);
                if (!string.IsNullOrEmpty(schunkSize))
                {
                    if (Int32.TryParse(schunkSize, out chunkSize)) { chunkSize = 500; }
                }

                SetOldInvoiceSummaryDAO();
                Int64 recordCount = NagisOldInvoiceSummaryDAO.Count(r => r.NagisDataBatch == new NagisDataBatch { Id = batchId });
                if (recordCount < 1) return;

                //get the pages
                int pages = Util.Pages(chunkSize, recordCount);
                int stopper = 0;
                int skip = 0;

                List<ConcurrentStack<NAGISGenerateInvoiceResult>> listOfProcessedInvoices = new List<ConcurrentStack<NAGISGenerateInvoiceResult>> { };

                while (stopper < pages)
                {
                    log.Info(string.Format("Starting invoice generation by chunk for batch Id {0}. Page {1} of {2}", batchId, stopper, pages));

                    List<NAGISDataGenerateInvoiceModel> chunkedRecords = NagisOldInvoiceSummaryDAO.GetChunkedBatch(batchId, chunkSize, skip);
                    var result = ProcessInvoices(chunkedRecords, batchRecord.StateModel.Id).Result;

                    log.Info(string.Format("Done invoice generation by chunk for batch Id {0}. Page {1} of {2}", batchId, stopper, pages));

                    listOfProcessedInvoices.Add(result);
                    skip += chunkSize;
                    stopper++;
                }
                log.Info(string.Format("Saving bundle for invoice generation for batch Id {0}..", batchId));

                SetBatchInvoiceResponseDAO();
                UoW.BeginTransaction();
                //when all the invoice chunk by chunk are process, lets send all the processed chunks to database
                BatchInvoiceResponseDAO.SaveBundle(listOfProcessedInvoices, batchRecord.Id);
                batchRecord.NumberOfRecordSentToCashFlow = recordCount;
                batchRecord.ProccessStage = (int)NagisDataProcessingStages.SavedCashflowBatchInvoiceResponse;
                UoW.Commit();
                log.Info(string.Format("Done invoice generation by for batch Id {0}.", batchId));

                StartHangfireServer();
                BackgroundJob.Enqueue(() => UpdateInvoiceStagingWithCashFlowResponse(tenantName, batchId));
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {0}. Batch ID {1}", NagisDataProcessingStages.Completed, batchId));
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                NagisOldInvoiceSummaryDAO = null;
                BatchDAO = null;
            }
        }

        /// <summary>
        /// do processing for invoice generation
        /// <para>https://github.com/Dasync/AsyncEnumerable</para>
        /// <para>https://markheath.net/post/constraining-concurrent-threads-csharp</para>
        /// <para>https://blog.briandrupieski.com/throttling-asynchronous-methods-in-csharp</para>
        /// </summary>
        /// <param name="chunkedRecords"></param>
        /// <param name="revenueHeadDetails"></param>
        private async Task<ConcurrentStack<NAGISGenerateInvoiceResult>> ProcessInvoices(List<NAGISDataGenerateInvoiceModel> chunkedRecords, int stateId)
        {
            //get invoice details
            SetRevenueHeadDAO();
            ConcurrentStack<NAGISGenerateInvoiceResult> listOfProcessResults = new ConcurrentStack<NAGISGenerateInvoiceResult>();
            InstantiateInvoicingService();

            await chunkedRecords.ParallelForEachAsync(
                async chunk =>
                {
                    try
                    {
                        int revenueHeadId = 0;

                        if (chunk.GroupId > 0)
                        {
                            revenueHeadId = chunk.GroupId;
                        }
                        else
                        {
                            revenueHeadId = chunk.InvoiceItems.FirstOrDefault().RevenueHead.Id;
                        }

                        RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails = RevenueHeadDAO.GetRevenueHeadDetailsForInvoiceGeneration(revenueHeadId);

                        revenueHeadDetails.InvoiceDate = DateTime.Now.ToLocalTime();
                        //get the details you would need to generate the invoice, details such as due date
                        CreateInvoiceHelper helper = InvoiceGenerationType.GetInvoiceHelperModel(revenueHeadDetails);
                        string invoiceModel = JsonConvert.SerializeObject(helper);

                        CashFlowRequestContext context = InvoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", revenueHeadDetails.SMEKey } });


                        CashFlowCreateCustomer createCustomer = new CashFlowCreateCustomer
                        {
                            Address = chunk.Address,
                            CountryID = 1,
                            CustomerId = chunk.CashflowCustomerId,
                            Identifier = chunk.TaxProfileId.ToString(),
                            Name = chunk.Recipient,
                            StateID = stateId,
                            Type = chunk.Type,
                            PryContact = new CashFlowCreateCustomer.Contact
                            {
                                Name = chunk.Recipient,
                            }
                        };

                        List<CashFlowCreateInvoice.CashFlowProductModel> itemsList = new List<CashFlowCreateInvoice.CashFlowProductModel>();
                        chunk.InvoiceItems.ForEach(x => itemsList.Add(new CashFlowCreateInvoice.CashFlowProductModel
                        {
                            ProductName = x.InvoiceDescription,
                            Pos = 1,
                            //Price = x.Amount,
                            Price = x.AmountDue,
                            ProductId = x.RevenueHead.CashFlowProductId,
                            Qty = x.Quantity
                        }));

                        CashFlowCreateInvoice createInvoice = new CashFlowCreateInvoice
                        {
                            Discount = 0m,
                            DiscountType = "Flat",
                            DueDate = helper.DueDate,
                            FootNote = helper.FootNotes,
                            InvoiceDate = helper.InvoiceDate,
                            Title = helper.Title,
                            Items = itemsList,
                            Type = helper.Type
                        };

                        var webCallResult = await InvoicingService.InvoiceService(context).CreateCustomerAndInvoiceAsync(
                        new CashFlowCreateCustomerAndInvoice
                        {
                            CreateCustomer = createCustomer,
                            CreateInvoice = createInvoice,
                            InvoiceUniqueKey = chunk.NAGISOldInvoiceSummaryId.ToString(),
                            PropertyTitle = "CentralBillingSystem"
                        });

                        listOfProcessResults.Push(new NAGISGenerateInvoiceResult { TotalAmount = chunk.Amount, AmountDue = chunk.AmountDue, RevenueHeadId = revenueHeadId, MDAId = revenueHeadDetails.MDAId, ExpertSystemId = revenueHeadDetails.ExpertSystemId, NAGISOldInvoiceNumber = chunk.NagisInvoiceNumber, IntegrationResponseModel = webCallResult, TaxProfileCategoryId = chunk.TaxProfileCategoryId, TaxProfileId = chunk.TaxProfileId, DueDate = helper.DueDate, InvoiceDescription = chunk.InvoiceItems.FirstOrDefault().InvoiceDescription });
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Error:::SummaryId {chunk.NAGISOldInvoiceSummaryId}. Number of Items {chunk.InvoiceItems.Count}", ex);
                    }
                },
                maxDegreeOfParalellism: 0
                );

            return listOfProcessResults;
        }

        private async Task<ConcurrentStack<NAGISGenerateCustomerResult>> ProcessCustomers(List<NAGISDataGenerateInvoiceModel> chunkedRecords, int stateId)
        {
            //get invoice details
            SetRevenueHeadDAO();
            ConcurrentStack<NAGISGenerateCustomerResult> listOfProcessResults = new ConcurrentStack<NAGISGenerateCustomerResult>();

            InstantiateInvoicingService();
            await chunkedRecords.ParallelForEachAsync(
                async chunk =>
                {
                    try
                    {
                        int revenueHeadId = 0;

                        if (chunk.GroupId > 0)
                        {
                            revenueHeadId = chunk.GroupId;
                        }
                        else
                        {
                            revenueHeadId = chunk.InvoiceItems.FirstOrDefault().RevenueHead.Id;
                        }

                        RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails = RevenueHeadDAO.GetRevenueHeadDetailsForInvoiceGeneration(revenueHeadId);

                        CashFlowRequestContext context = InvoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", revenueHeadDetails.SMEKey } });

                        CashFlowCreateCustomer createCustomer = new CashFlowCreateCustomer
                        {
                            Address = chunk.Address,
                            CountryID = 1,
                            CustomerId = chunk.CashflowCustomerId,
                            Identifier = chunk.TaxProfileId.ToString(),
                            Name = chunk.Recipient,
                            StateID = stateId,
                            Type = chunk.Type,
                            PryContact = new CashFlowCreateCustomer.Contact
                            {
                                Name = chunk.Recipient,
                            }
                        };

                        var webCallResult = InvoicingService.CustomerServices(context).CreateCustomer(createCustomer);
                        listOfProcessResults.Push(new NAGISGenerateCustomerResult { CashFlowCustomer = webCallResult, TaxEntityId = chunk.TaxProfileId });
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Error:::SummaryId {chunk.NAGISOldInvoiceSummaryId}. Number of Items {chunk.InvoiceItems.Count}", ex);
                    }
                });

            return listOfProcessResults;
        }

        private ConcurrentStack<NAGISGenerateCustomerResult> ProcessBatchCustomers(List<NAGISDataGenerateInvoiceModel> chunkedRecords, int stateId)
        {
            //get invoice details
            Int64 revenueHeadId = 0;
            string groupRevenueId = ConfigurationManager.AppSettings["NAGISGroupRevenueHeadId"];
            bool parsed = Int64.TryParse(groupRevenueId, out revenueHeadId);
            if (!parsed) { throw new Exception("Unable to get configured NAGIS group revenueHeadId"); }

            SetRevenueHeadDAO();
            InstantiateInvoicingService();

            ConcurrentStack<NAGISGenerateCustomerResult> listOfProcessResults = new ConcurrentStack<NAGISGenerateCustomerResult>();
            RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails = RevenueHeadDAO.GetRevenueHeadDetailsForInvoiceGeneration(revenueHeadId);
            CashFlowRequestContext context = InvoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", revenueHeadDetails.SMEKey } });
            List<CashFlowCreateCustomer> customerList = new List<CashFlowCreateCustomer>();
            ConcurrentStack<CashFlowCreateCustomer> customerConcurrent = new ConcurrentStack<CashFlowCreateCustomer>();
            List<ConcurrentStack<CashFlowCreateCustomer>> customerListConcurrent = new List<ConcurrentStack<CashFlowCreateCustomer>>();


            Parallel.ForEach(chunkedRecords, (chunk) =>
            {
                try
                {
                    CashFlowCreateCustomer createCustomer = new CashFlowCreateCustomer
                    {
                        Address = chunk.Address,
                        CountryID = 1,
                        CustomerId = chunk.CashflowCustomerId,
                        Identifier = chunk.TaxProfileId.ToString(),
                        Name = chunk.Recipient,
                        StateID = stateId,
                        Type = chunk.Type,
                        PryContact = new CashFlowCreateCustomer.Contact
                        {
                            Name = chunk.Recipient,
                        }
                    };
                    customerConcurrent.Push(createCustomer);
                }
                catch (Exception ex)
                {
                    log.Error($"Error:::SummaryId {chunk.NAGISOldInvoiceSummaryId}. Number of Items {chunk.InvoiceItems.Count}", ex);
                }
            });

            customerListConcurrent.Add(customerConcurrent);
            customerListConcurrent.ForEach(x => customerList.AddRange(x));
            var webCallResult = InvoicingService.CustomerServices(context).CreateMultipleCustomer(customerList);

            if (webCallResult.Count > 0)
            {
                webCallResult.ForEach(x => listOfProcessResults.Push(new NAGISGenerateCustomerResult { CashFlowCustomer = x }));
            }

            return listOfProcessResults;
        }


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
                NagisDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);

                CheckProcessStage(batchRecord, NagisDataProcessingStages.SavedCashflowBatchInvoiceResponse);

                UoW.BeginTransaction();
                SetOldInvoiceSummaryDAO();

                NagisOldInvoiceSummaryDAO.UpdateInvoiceStagingWithCashFlowResponse(batchId);
                batchRecord.ProccessStage = (int)NagisDataProcessingStages.UpdateInvoiceStagingWithCashFlowResponse;
                UoW.Commit();
                log.Info($"After updating batch invoice for batch {batchId}");

                StartHangfireServer();
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
                NagisOldInvoiceSummaryDAO = null;
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
                SetUnitofWork(tenantName);
                SetBatchDAO();
                NagisDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);
                CheckProcessStage(batchRecord, NagisDataProcessingStages.UpdateInvoiceStagingWithCashFlowResponse);
                SetInvoiceDAO();
                UoW.BeginTransaction();
                InvoiceDAO.CreateNAGISInvoices(batchId);
                batchRecord.ProccessStage = (int)NagisDataProcessingStages.MoveInvoiceStagingRecordsToInvoiceTable;
                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => CreateInvoiceItems(tenantName, batchRecord.Id));
            }
            catch (Exception exception)
            {
                log.Error($"Error Processing for stage {NagisDataProcessingStages.MoveInvoiceStagingRecordsToInvoiceTable}. Batch ID {batchId}");
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
        public void CreateInvoiceItems(string tenantName, long batchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                NagisDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);
                CheckProcessStage(batchRecord, NagisDataProcessingStages.MoveInvoiceStagingRecordsToInvoiceTable);

                SetInvoiceItemsDAO();
                UoW.BeginTransaction();
                InvoiceItemsDAO.CreateNAGISInvoiceItems(batchId);
                batchRecord.ProccessStage = (int)NagisDataProcessingStages.MoveInvoiceItemsRecordsToInvoiceItemsTable;
                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => CreateTransactionLog(tenantName, batchRecord.Id));
            }
            catch (Exception exception)
            {
                log.Error($"Error Processing for stage {NagisDataProcessingStages.MoveInvoiceItemsRecordsToInvoiceItemsTable}. Batch ID {batchId}");
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchDAO = null;
                InvoiceItemsDAO = null;
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
                SetUnitofWork(tenantName);
                SetBatchDAO();
                NagisDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);
                CheckProcessStage(batchRecord, NagisDataProcessingStages.MoveInvoiceItemsRecordsToInvoiceItemsTable);
                SetTransactionLogDAO();
                UoW.BeginTransaction();
                TransactionLogDAO.CreateNAGISInvoiceTransactionLog(batchId);
                batchRecord.ProccessStage = (int)NagisDataProcessingStages.Completed;
                UoW.Commit();
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
                TransactionLogDAO = null;
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
                SetUnitofWork(tenantName);
                SetBatchDAO();
                NagisDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);
                CheckProcessStage(batchRecord, NagisDataProcessingStages.CreateCustomerOnCashflow);
                SetTaxEntityDAO();
                UoW.BeginTransaction();
                TaxEntityDAO.UpdateTaxEntityWithCashflowCustomerResponse(batchId);

                batchRecord.ProccessStage = (int)NagisDataProcessingStages.UpdateTaxEntityWithCashFlowCustomerDetails;
                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => GenerateInvoices(tenantName, batchId));
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
                InvoiceDAO = null;
            }
        }

        /// <summary>
        /// Check if the process stage is valid
        /// </summary>
        /// <param name="batchRecord"></param>
        /// <param name="expectedStage"></param>
        private void CheckProcessStage(NagisDataBatch batchRecord, NagisDataProcessingStages expectedStage)
        {
            if (batchRecord != null)
            {
                //lets check what stage it is in
                //for this method the expected stage is FileValidationProcessed
                if (batchRecord.ProccessStage != (int)expectedStage)
                { throw new InvalidOperationException($"ReferenceDataBatch processing has already passed this stage. Current stage {(NagisDataProcessingStages)batchRecord.ProccessStage}, Expected stage {expectedStage}, Batch Id {batchRecord.Id}"); }
            }
        }

        private void StartHangfireServer()
        {
            var conStringName = ConfigurationManager.AppSettings["HangfireConnectionStringName"];

            if (string.IsNullOrEmpty(conStringName))
            {
                throw new Exception("Unable to get the hangfire connection string name");
            }

            //Get the connection string
            string dbConnectionString = HangFireScheduler.GetConnectionString(conStringName);
            //Call the Hangfire storage
            GlobalConfiguration.Configuration.UseSqlServerStorage(dbConnectionString);
        }
    }
}
