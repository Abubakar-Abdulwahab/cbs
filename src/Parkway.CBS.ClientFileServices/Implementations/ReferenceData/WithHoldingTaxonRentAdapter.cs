using Hangfire;
using Newtonsoft.Json;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.ClientFileServices.Implementations.Contracts;
using Parkway.CBS.ClientFileServices.Implementations.IPPIS;
using Parkway.CBS.ClientFileServices.Implementations.Models;
using Parkway.CBS.ClientFileServices.Logger.Contracts;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl;
using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts;
using Parkway.CBS.ClientServices.Invoicing;
using Parkway.CBS.ClientServices.Invoicing.Contracts;
using Parkway.CBS.ClientServices.Services;
using Parkway.CBS.ClientServices.Services.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Entities.DTO;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using Parkway.CBS.Payee.PayeeAdapters.ReferenceData;
using Parkway.CBS.Payee.ReferenceDataImplementation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;


namespace Parkway.CBS.ClientFileServices.Implementations.ReferenceData
{
    public class WithHoldingTaxonRentAdapter : IReferenceDataFileProcessor
    {
        private static readonly ILogger log = new Log4netLogger();
        public IUoW UoW { get; set; }
        public IReferenceDataBatchDAOManager BatchDAO { get; set; }

        public IReferenceDataAdapter ReferenceAdapter { get; set; }

        public IReferenceDataBatchRecordsDAOManager BatchRecordsDAO { get; set; }

        public IRevenueHeadDAOManager RevenueHeadDAO { get; set; }

        public IReferenceDataTaxEntityStagingDAOManager BatchStagingRecordsDAO { get; set; }

        public IReferenceDataWithHoldingTaxOnRentDAOManager WithholdingTaxonRentDAO { get; set; }

        public IReferenceDataRecordsInvoiceDAOManager RecordsInvoiceDAO { get; set; }

        public IDevelopmentLevyDAOManager DevelopmentLevyDAO { get; set; }

        public IInvoiceDAOManager InvoiceDAO { get; set; }

        public IInvoicingService InvoicingService;

        public IInvoiceGenerationType InvoiceGenerationType = new DirectAssessmentInvoiceGeneration { };

        public WithHoldingTaxonRentAdapter() { }

        private void SetReferenceDataAdapter()
        { if (ReferenceAdapter == null) { ReferenceAdapter = new ReferenceDataAdapter(); } }

        protected void SetBatchDAO()
        { if (BatchDAO == null) { BatchDAO = new ReferenceDataBatchDAOManager(UoW); } }

        protected void SetBatchRecordsDAO()
        { if (BatchRecordsDAO == null) { BatchRecordsDAO = new ReferenceDataRecordsDAOManager(UoW); } }

        protected void SetBatchStagingRecordsDAO()
        { if (BatchStagingRecordsDAO == null) { BatchStagingRecordsDAO = new ReferenceDataTaxEntityStagingDAOManager(UoW); } }

        protected void SetDevelopmentLevyDAO()
        { if (DevelopmentLevyDAO == null) { DevelopmentLevyDAO = new DevelopmentLevyDAOManager(UoW); } }

        protected void SetWithholdingTaxonRentDAO()
        { if (WithholdingTaxonRentDAO == null) { WithholdingTaxonRentDAO = new ReferenceDataWithHoldingTaxOnRentDAOManager(UoW); } }

        private void SetRecordsInvoiceDAO()
        { if (RecordsInvoiceDAO == null) { RecordsInvoiceDAO = new ReferenceDataRecordsInvoiceDAOManager(UoW); } }

        public void InstantiateInvoicingService()
        { if (InvoicingService == null) { InvoicingService = new InvoicingService(); } }

        private void SetInvoiceDAO()
        { if (InvoiceDAO == null) { InvoiceDAO = new InvoiceDAOManager(UoW); } }

        private void SetRevenueHeadDAO()
        { if (RevenueHeadDAO == null) { RevenueHeadDAO = new RevenueHeadDAOManager(UoW); } }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName + "_SessionFactory", "ClientFileServices");
            }
        }

        [ProlongExpirationTime]
        /// <summary>
        /// Save the reference data into the db
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ValidateFileResponse SaveFile(string tenantName, string filePath, long withHoldingTaxBatchId)
        {
            try
            {
                log.Info($"Starting Validate the file stage {ReferenceDataProcessingStages.NotProcessed} {filePath} {withHoldingTaxBatchId}");

                //set unit of work
                SetUnitofWork(tenantName);
                //instantiate the ReferenceDataBatch repository
                SetBatchDAO();

                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(withHoldingTaxBatchId);

                SetReferenceDataAdapter();
                //now lets validate and save the file contents
                ReferenceDataResponse referenceDataResponse = ReferenceAdapter.GetReferenceDataResponseModels(filePath);
                if (referenceDataResponse.HeaderValidateObject.Error)
                {
                    batchRecord.ErrorOccurred = true;
                    batchRecord.ErrorMessage = referenceDataResponse.HeaderValidateObject.ErrorMessage;
                    return new ValidateFileResponse { BatchId = batchRecord.Id, ErrorMessage = "Error reading the file. Some header parameters are missing for batch Id: " + batchRecord.Id, ErrorOccurred = true };
                }
                //if no error occurred while reading the file, lets save the records to DB

                UoW.BeginTransaction();

                SetBatchRecordsDAO();

                int count = BatchRecordsDAO.SaveReferenceDataRecords(tenantName, batchRecord.LGA.Id.ToString(), batchRecord.Id, referenceDataResponse.ReferenceDataLineRecords);
                batchRecord.NumberOfRecords = count;
                //update the stage 
                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.FileSaved;
                batchRecord.FilePath = filePath;
                batchRecord.FileName = System.IO.Path.GetFileName(filePath);
                batchRecord.PercentageProgress = 100;
                UoW.Commit();

                log.Info(string.Format("Processing for stage {1} completed. FilePath {0}", filePath, ReferenceDataProcessingStages.FileSaved));

                StartHangfireServer();
                BackgroundJob.Enqueue(() => MoveReferenceDataRecordsToTaxEntityStaging(tenantName, batchRecord.Id));
                return new ValidateFileResponse { BatchId = batchRecord.Id };
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {1}. FilePath {0}", filePath, ReferenceDataProcessingStages.NotProcessed));
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchDAO = null;
                BatchRecordsDAO = null;
            }
        }

        /// <summary>
        /// Copy the records (essential columns that will be needed to create Tax Entity) from Reference Data Records to the Tax Entity Staging
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void MoveReferenceDataRecordsToTaxEntityStaging(string tenantName, long withHoldingTaxBatchId)
        {
            try
            {
                log.Info($"About to move records to staging from stage {ReferenceDataProcessingStages.FileSaved} for batch {withHoldingTaxBatchId}");
                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(withHoldingTaxBatchId);
                //check the process stage to make sure we are at the right stage
                CheckProcessStage(batchRecord, ReferenceDataProcessingStages.FileSaved);

                UoW.BeginTransaction();
                SetBatchRecordsDAO();
                log.Info($"About to update batch stage to {ReferenceDataProcessingStages.RecordsMovedToTaxEntityStaging} for batch {withHoldingTaxBatchId}");
                BatchRecordsDAO.MoveReferenceDataToTaxEntityStaging(withHoldingTaxBatchId);
                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.RecordsMovedToTaxEntityStaging;
                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => MatchReferenceDataRecordsToTypeOfTaxpaid(tenantName, withHoldingTaxBatchId));
                log.Info($"Records moved to staging for batch {withHoldingTaxBatchId}");
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
                BatchRecordsDAO = null;
            }
        }

        /// <summary>
        /// Update the ReferenceDataRecord Id column in ReferenceDataTypeOfTaxPaidMapping Table
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void MatchReferenceDataRecordsToTypeOfTaxpaid(string tenantName, long withHoldingTaxBatchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(withHoldingTaxBatchId);

                CheckProcessStage(batchRecord, ReferenceDataProcessingStages.RecordsMovedToTaxEntityStaging);

                UoW.BeginTransaction();
                SetBatchRecordsDAO();

                BatchRecordsDAO.MatchReferenceDataRecordsToTypeOfTaxpaid(withHoldingTaxBatchId);
                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.MatchedRecordsToTypeOfTaxPaid;

                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => UpdateReferenceDataTaxEntityStagingRecordsOperationType(tenantName, withHoldingTaxBatchId));
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
                BatchRecordsDAO = null;
                BatchDAO = null;
            }
        }

        /// <summary>
        /// Update the operation Type for Staging Records
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void UpdateReferenceDataTaxEntityStagingRecordsOperationType(string tenantName, long withHoldingTaxBatchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(withHoldingTaxBatchId);

                CheckProcessStage(batchRecord, ReferenceDataProcessingStages.MatchedRecordsToTypeOfTaxPaid);

                UoW.BeginTransaction();
                SetBatchStagingRecordsDAO();

                BatchStagingRecordsDAO.UpdateReferenceDataTaxEntityStagingRecordsOperationType(withHoldingTaxBatchId);
                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.UpdateTaxEntityStagingOperationType;
                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => CreateTaxEntityWithReferenceDataRecords(tenantName, withHoldingTaxBatchId));
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
                BatchStagingRecordsDAO = null;
                BatchDAO = null;
            }
        }

        /// <summary>
        /// Create the Tax Entity for the payer that doesn't exist
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void CreateTaxEntityWithReferenceDataRecords(string tenantName, long withHoldingTaxBatchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(withHoldingTaxBatchId);

                CheckProcessStage(batchRecord, ReferenceDataProcessingStages.UpdateTaxEntityStagingOperationType);


                UoW.BeginTransaction();
                SetBatchStagingRecordsDAO();

                BatchStagingRecordsDAO.CreateTaxEntityWithReferenceDataRecords(withHoldingTaxBatchId);
                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.TaxPayerCreated;
                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => UpdateTaxEntityWithReferenceDataRecords(tenantName, withHoldingTaxBatchId));
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
                BatchStagingRecordsDAO = null;
                BatchDAO = null;
            }
        }

        /// <summary>
        /// Update the Tax Entity records for the payers that already exist CBS
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void UpdateTaxEntityWithReferenceDataRecords(string tenantName, long withHoldingTaxBatchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(withHoldingTaxBatchId);

                CheckProcessStage(batchRecord, ReferenceDataProcessingStages.TaxPayerCreated);


                UoW.BeginTransaction();
                SetBatchStagingRecordsDAO();

                BatchStagingRecordsDAO.UpdateTaxEntityWithReferenceDataRecords(withHoldingTaxBatchId);
                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.UpdateExistingTaxPayerRecords;
                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => UpdateReferenceDataTaxEntityStagingRecordsTaxEntityId(tenantName, withHoldingTaxBatchId));
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
                BatchStagingRecordsDAO = null;
                BatchDAO = null;
            }
        }

        /// <summary>
        /// Update the TaxEntity Id on the TaxEntityStagingRecords after creation on the Tax Entity Table
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void UpdateReferenceDataTaxEntityStagingRecordsTaxEntityId(string tenantName, long withHoldingTaxBatchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(withHoldingTaxBatchId);

                CheckProcessStage(batchRecord, ReferenceDataProcessingStages.UpdateExistingTaxPayerRecords);

                UoW.BeginTransaction();
                SetBatchStagingRecordsDAO();

                BatchStagingRecordsDAO.UpdateReferenceDataTaxEntityStagingRecordsTaxEntityId(withHoldingTaxBatchId);
                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.UpdateReferenceDataTaxEntityStaging;

                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => MoveTaxEntityStagingRecordsToWithHoldingTaxOnRent(tenantName, withHoldingTaxBatchId));
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
                BatchStagingRecordsDAO = null;
                BatchDAO = null;
            }
        }

        /// <summary>
        /// Copy the records (necessary colums) into the WithholdingTaxonRent Table
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void MoveTaxEntityStagingRecordsToWithHoldingTaxOnRent(string tenantName, long withHoldingTaxBatchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(withHoldingTaxBatchId);

                CheckProcessStage(batchRecord, ReferenceDataProcessingStages.UpdateReferenceDataTaxEntityStaging);

                UoW.BeginTransaction();
                SetBatchStagingRecordsDAO();

                BatchStagingRecordsDAO.MoveTaxEntityStagingRecordsToWithHoldingTaxOnRent(withHoldingTaxBatchId);
                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.MoveTaxEntityStagingRecordsToWithHoldingTaxOnRent;
                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => MoveWithHoldingTaxOnRentToInvoiceStagingTable(tenantName, withHoldingTaxBatchId));
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
                BatchStagingRecordsDAO = null;
                BatchDAO = null;
            }
        }

        /// <summary>
        /// Copy the records (necessary colums) that are required to create invoice into the Invoice Staging Table
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void MoveWithHoldingTaxOnRentToInvoiceStagingTable(string tenantName, long withHoldingTaxBatchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(withHoldingTaxBatchId);

                CheckProcessStage(batchRecord, ReferenceDataProcessingStages.MoveTaxEntityStagingRecordsToWithHoldingTaxOnRent);

                UoW.BeginTransaction();
                SetBatchStagingRecordsDAO();

                BatchStagingRecordsDAO.MoveWithHoldingTaxOnRentRecordsToInvoiceStaging(withHoldingTaxBatchId);
                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.MoveWithHoldingTaxOnRentToInvoiceStaging;
                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => GenerateInvoiceForWithholdingTaxOnRent(tenantName, withHoldingTaxBatchId, batchRecord.StateModel.Id));
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
                BatchStagingRecordsDAO = null;
                BatchDAO = null;
            }
        }

        /// <summary>
        /// Generate Invoice for the records in the WithholdingTaxonRent table for the specified batch Id
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        /// <param name="stateId"></param>
        [ProlongExpirationTime]
        public void GenerateInvoiceForWithholdingTaxOnRent(string tenantName, long withHoldingTaxBatchId, int stateId)
        {
            try
            {
                log.Info($"About to send batch invoice request for WithholdingTaxOnRent to cashflow for batch {withHoldingTaxBatchId}");

                string callbackURL = ConfigurationManager.AppSettings["BatchInvoiceCallbackURL"];
                if (string.IsNullOrEmpty(callbackURL))
                {
                    throw new Exception("Unable to get config Batch Invoice Callback URL");
                }

                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(withHoldingTaxBatchId);
                CheckProcessStage(batchRecord, ReferenceDataProcessingStages.MoveWithHoldingTaxOnRentToInvoiceStaging);

                //get invoice details
                SetRevenueHeadDAO();
                RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails = RevenueHeadDAO.GetRevenueHeadDetailsForInvoiceGenerationForPayee(batchRecord.RevenueHead.Id);
                revenueHeadDetails.InvoiceDate = DateTime.Now.ToLocalTime();
                //get the details you would need to generate the invoice, details such as due date
                CreateInvoiceHelper helper = InvoiceGenerationType.GetInvoiceHelperModel(revenueHeadDetails);

                SetWithholdingTaxonRentDAO();
                List<ReferenceDataGenerateInvoiceModel> records = WithholdingTaxonRentDAO.GetBatch(withHoldingTaxBatchId);
                if (records.Count < 1) return;

                var resp = ProcessWithHoldingTaxonRentInvoices(records, helper, revenueHeadDetails, stateId, batchRecord.GeneralBatchReference.Id.ToString(), callbackURL);

                var fileNameSplit = batchRecord.FileName.Split('-');
                long developmentLevyBatchId = 0;
                Int64.TryParse(fileNameSplit[1].Trim(), out developmentLevyBatchId);

                UoW.BeginTransaction();
                batchRecord.NumberOfRecordSentToCashFlow = records.Count;
                batchRecord.BatchInvoiceCallBackURL = callbackURL;
                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.SentBulkInvoiceRequestToCashFlow;
                UoW.Commit();
                string jsonResp = JsonConvert.SerializeObject(resp);
                log.Info($"Sent Bulk WithHolding Tax on Rent Invoice request to CashFlow for batch Id {batchRecord.Id}. CashFlow Response {jsonResp}");

                StartHangfireServer();
                BackgroundJob.Enqueue(() => MoveDevelopmentLevyRecordToInvoiceStagingTable(tenantName, developmentLevyBatchId));
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
                RevenueHeadDAO = null;
                WithholdingTaxonRentDAO = null;
            }
        }

        /// <summary>
        /// Copy the Tax Entity (necessary colums) records without Federal Agencies and State Agencies into the Invoice Staging Table
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        [ProlongExpirationTime]
        public void MoveDevelopmentLevyRecordToInvoiceStagingTable(string tenantName, long developmentLevyBatchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(developmentLevyBatchId);


                UoW.BeginTransaction();
                SetBatchStagingRecordsDAO();

                BatchStagingRecordsDAO.MoveDevelopmentLevyRecordToInvoiceStagingTable(developmentLevyBatchId);
                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.MoveWithHoldingTaxOnRentToInvoiceStaging;
                UoW.Commit();

                StartHangfireServer();
                BackgroundJob.Enqueue(() => GenerateInvoiceForDevelopmentLevy(tenantName, developmentLevyBatchId, batchRecord.StateModel.Id));
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
                BatchStagingRecordsDAO = null;
                BatchDAO = null;
            }
        }

        /// <summary>
        /// Generate Development Levy Invoices for the records in the Tax Entity table
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        /// <param name="stateId"></param>
        [ProlongExpirationTime]
        public void GenerateInvoiceForDevelopmentLevy(string tenantName, long developmentLevyBatchId, int stateId)
        {
            try
            {
                log.Info($"About to send batch invoice request for DevelopmentLevy to cashflow for batch {developmentLevyBatchId}");

                string callbackURL = ConfigurationManager.AppSettings["BatchInvoiceCallbackURL"];
                if (string.IsNullOrEmpty(callbackURL))
                {
                    throw new Exception("Unable to get config Batch Invoice Callback URL");
                }


                SetUnitofWork(tenantName);
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(developmentLevyBatchId);

                //get invoice details
                SetRevenueHeadDAO();

                RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails = RevenueHeadDAO.GetRevenueHeadDetailsForInvoiceGenerationForPayee(batchRecord.RevenueHead.Id);
                revenueHeadDetails.InvoiceDate = DateTime.Now.ToLocalTime();
                //get the details you would need to generate the invoice, details such as due date
                CreateInvoiceHelper helper = InvoiceGenerationType.GetInvoiceHelperModel(revenueHeadDetails);

                SetDevelopmentLevyDAO();

                //Get all the Tax Entities for invoice generation
                List<ReferenceDataGenerateInvoiceModel> records = DevelopmentLevyDAO.GetTaxEntitiesForDevelopmentLevy();
                var resp = ProcessDevelopmentLevyInvoices(records, helper, revenueHeadDetails, stateId, batchRecord.GeneralBatchReference.Id.ToString(), callbackURL);

                UoW.BeginTransaction();
                batchRecord.NumberOfRecords = records.Count;
                batchRecord.NumberOfRecordSentToCashFlow = records.Count;
                batchRecord.BatchInvoiceCallBackURL = callbackURL;
                batchRecord.ProccessStage = (int)ReferenceDataProcessingStages.SentBulkInvoiceRequestToCashFlow;
                UoW.Commit();
                string jsonResp = JsonConvert.SerializeObject(resp);
                log.Info($"Sent Bulk Development Levy Invoice request to CashFlow for batch Id {batchRecord.Id}. CashFlow Response {jsonResp}");
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
                RevenueHeadDAO = null;
                DevelopmentLevyDAO = null;
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
        /// Process bulk Invoice for WithHolding Tax on Rent
        /// </summary>
        /// <param name="records"></param>
        /// <param name="helper"></param>
        /// <param name="revenueHeadDetails"></param>
        /// <param name="stateId"></param>
        /// <param name="batchIdentifier"></param>
        /// <returns></returns>
        private IntegrationResponseModel ProcessWithHoldingTaxonRentInvoices(List<ReferenceDataGenerateInvoiceModel> records, CreateInvoiceHelper helper, RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails, int stateId, string batchIdentifier, string callbackURL)
        {
            InstantiateInvoicingService();
            CashFlowRequestContext context = InvoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", revenueHeadDetails.SMEKey } });
            List<CashFlowCreateCustomerAndInvoice> customerAndInvoiceList = new List<CashFlowCreateCustomerAndInvoice>();
            ConcurrentStack<CashFlowCreateCustomerAndInvoice> customerAndInvoiceConcurrent = new ConcurrentStack<CashFlowCreateCustomerAndInvoice>();
            List<ConcurrentStack<CashFlowCreateCustomerAndInvoice>> customerAndInvoiceListConcurrent = new List<ConcurrentStack<CashFlowCreateCustomerAndInvoice>>();

            Parallel.ForEach(records, (model) =>
            {
                decimal computedAmount = 0;
                string configAmount = string.Empty;
                bool parsed = false;

                //Check if the rent amount from reference data is zero or less,
                //then, pick the preconfigure amount in the config file
                if (model.Amount <= 0)
                {
                    switch (model.TaxProfileCategoryId)
                    {
                        //case 1 is for individual
                        case 1:
                            configAmount = ConfigurationManager.AppSettings["IndividualWithoutRentAmount"];
                            parsed = decimal.TryParse(configAmount, out computedAmount);
                            if (!parsed) { throw new Exception("Unable to get default Individual Without Rent Amount value"); }
                            break;
                        //case 2 is for corporate
                        case 2:
                            configAmount = ConfigurationManager.AppSettings["CorporateWithoutRentAmount"];
                            parsed = decimal.TryParse(configAmount, out computedAmount);
                            if (!parsed) { throw new Exception("Unable to get default Corporate Without Rent Amount value"); }
                            break;
                    }
                }
                else
                {
                    int percentage = 0;
                    var configPercentage = ConfigurationManager.AppSettings["WithholdingTaxOnRentPercentage"];
                    parsed = int.TryParse(configPercentage, out percentage);
                    if (!parsed) { throw new Exception("Unable to get Withholding Tax On Rent Percentage value"); }

                    computedAmount = Math.Round(((percentage / 100m) * model.Amount), 2);
                }

                CashFlowCreateCustomerAndInvoice customerAndInvoice = new CashFlowCreateCustomerAndInvoice
                {
                    CreateCustomer = new CashFlowCreateCustomer
                    {
                        Address = model.Address,
                        CountryID = 1,
                        CustomerId = model.CashflowCustomerId,
                        Identifier = model.TaxProfileId.ToString(),
                        Name = model.Recipient,
                        StateID = stateId,
                        Type = Cashflow.Ng.Models.Enums.CashFlowCustomerType.Individual,
                        PryContact = new CashFlowCreateCustomer.Contact
                        {
                            Name = model.Recipient,
                            Email = model.Email
                        }
                    },
                    CreateInvoice = new CashFlowCreateInvoice
                    {
                        Discount = 0m,
                        DiscountType = "Flat",
                        DueDate = helper.DueDate,
                        FootNote = helper.FootNotes,
                        InvoiceDate = helper.InvoiceDate,
                        Items = new List<CashFlowCreateInvoice.CashFlowProductModel> {
                                    { new CashFlowCreateInvoice.CashFlowProductModel { ProductName = "Withholding Tax on Rent", Pos = 1, Price = computedAmount, ProductId = revenueHeadDetails.CashFlowProductId, Qty = 1 } }
                                    },
                        Title = helper.Title,
                        Type = helper.Type,
                    },
                    InvoiceUniqueKey = model.WithholdingTaxonRentId.ToString(),
                    PropertyTitle = "CentralBillingSystem",
                };

                customerAndInvoiceConcurrent.Push(customerAndInvoice);
            });

            customerAndInvoiceListConcurrent.Add(customerAndInvoiceConcurrent);
            customerAndInvoiceListConcurrent.ForEach(x => customerAndInvoiceList.AddRange(x));
            var webCallResult = InvoicingService.InvoiceService(context).CreateCustomerAndInvoice(new CreateCustomerAndInvoiceBatch { Models = customerAndInvoiceList, CallBackURL= callbackURL, BatchReference = batchIdentifier });
            if (webCallResult.HasErrors)
            {
                throw new Exception($"{JsonConvert.SerializeObject(webCallResult)} Batch Identifier => {batchIdentifier}");
            }

            return webCallResult;
        }

        /// <summary>
        /// Process bulk Invoice for WithHolding Tax on Rent
        /// </summary>
        /// <param name="records"></param>
        /// <param name="helper"></param>
        /// <param name="revenueHeadDetails"></param>
        /// <param name="stateId"></param>
        /// <param name="batchIdentifier"></param>
        /// <returns></returns>
        private IntegrationResponseModel ProcessDevelopmentLevyInvoices(List<ReferenceDataGenerateInvoiceModel> records, CreateInvoiceHelper helper, RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails, int stateId, string batchIdentifier, string callbackURL)
        {
            InstantiateInvoicingService();
            CashFlowRequestContext context = InvoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", revenueHeadDetails.SMEKey } });
            List<CashFlowCreateCustomerAndInvoice> customerAndInvoiceList = new List<CashFlowCreateCustomerAndInvoice>();
            ConcurrentStack<CashFlowCreateCustomerAndInvoice> customerAndInvoiceConcurrent = new ConcurrentStack<CashFlowCreateCustomerAndInvoice>();
            List<ConcurrentStack<CashFlowCreateCustomerAndInvoice>> customerAndInvoiceListConcurrent = new List<ConcurrentStack<CashFlowCreateCustomerAndInvoice>>();

            Parallel.ForEach(records, (model) =>
            {               
                CashFlowCreateCustomerAndInvoice customerAndInvoice = new CashFlowCreateCustomerAndInvoice
                {
                    CreateCustomer = new CashFlowCreateCustomer
                    {
                        Address = model.Address,
                        CountryID = 1,
                        CustomerId = model.CashflowCustomerId,
                        Identifier = model.TaxProfileId.ToString(),
                        Name = model.Recipient,
                        StateID = stateId,
                        Type = Cashflow.Ng.Models.Enums.CashFlowCustomerType.Individual,
                        PryContact = new CashFlowCreateCustomer.Contact
                        {
                            Name = model.Recipient,
                            Email = model.Email
                        }
                    },
                    CreateInvoice = new CashFlowCreateInvoice
                    {
                        Discount = 0m,
                        DiscountType = "Flat",
                        DueDate = helper.DueDate,
                        FootNote = helper.FootNotes,
                        InvoiceDate = helper.InvoiceDate,
                        Items = new List<CashFlowCreateInvoice.CashFlowProductModel> {
                                    { new CashFlowCreateInvoice.CashFlowProductModel { ProductName = "Withholding Tax on Rent", Pos = 1, Price = model.Amount, ProductId = revenueHeadDetails.CashFlowProductId, Qty = 1 } }
                                    },
                        Title = helper.Title,
                        Type = helper.Type,
                    },
                    InvoiceUniqueKey = model.TaxProfileId.ToString(),
                    PropertyTitle = "CentralBillingSystem",
                };
                customerAndInvoiceConcurrent.Push(customerAndInvoice);
            });

            customerAndInvoiceListConcurrent.Add(customerAndInvoiceConcurrent);
            customerAndInvoiceListConcurrent.ForEach(x => customerAndInvoiceList.AddRange(x));
            var webCallResult = InvoicingService.InvoiceService(context).CreateCustomerAndInvoice(new CreateCustomerAndInvoiceBatch { Models = customerAndInvoiceList, CallBackURL = callbackURL, BatchReference = batchIdentifier });
            if (webCallResult.HasErrors)
            {
                throw new Exception($"{JsonConvert.SerializeObject(webCallResult)} Batch Identifier => {batchIdentifier}");
            }

            return webCallResult;
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
