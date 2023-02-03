using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.ClientFileServices.Implementations.Contracts;
using Parkway.CBS.ClientFileServices.Implementations.Models;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
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
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using Parkway.CBS.Payee.PayeeAdapters.IPPIS;
using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Parkway.CBS.ClientFileServices.Logger.Contracts;
using Newtonsoft.Json;
using System.Configuration;
using Hangfire;
using Parkway.CBS.Payee;
using System.Threading;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.Payee.Models;

namespace Parkway.CBS.ClientFileServices.Implementations.IPPIS
{
    public class IPPISFileProcessor : IFileWatcherProcessor
    {
        private static readonly ILogger log = new Log4netLogger();

        public IPPISFileProcessor() { }


        public IIPPISBatchDAOManager BatchDAO { get; set; }

        public IUoW UoW { get; set; }

        public IIPPISPayeeAdapter PayeeAdapter { get; set; }

        public IIPPISBatchRecordsDAOManager BatchRecordsDAO { get; set; }

        public IIPPISTaxPayerSummaryDAOManager SummaryDAO { get; set; }

        public IIPPISBatchRecordsInvoiceDAOManager BatchRecordsInvoiceDAO { get; set; }

        public IRevenueHeadDAOManager RevenueHeadDAO { get; set; }

        public IIPPISBatchRecordsInvoiceDAOManager RecordsInvoiceDAO { get; set; }

        public IDirectAssessmentBatchRecordDAOManager DirectAssessmentBatchRecordDAO { get; set; }

        public IDirectAssessmentPayeeRecordDAOManager DirectAssessmentPayeeRecordDAO { get; set; }

        public IInvoiceDAOManager InvoiceDAO { get; set; }


        public IInvoiceGenerationType InvoiceGenerationType = new DirectAssessmentInvoiceGeneration { };

        public IInvoicingService InvoicingService;


        private Dictionary<string, int> listOfMonths = new Dictionary<string, int> { { "Jan", 1 }, { "Feb", 2 }, { "Mar", 3 }, { "Apr", 4 }, { "May", 5 }, { "Jun", 6 }, { "Jul", 7 }, { "Aug", 8 }, { "Sep", 9 }, { "Oct", 10 }, { "Nov", 11 }, { "Dec", 12 } };

        public ValidateFileResponse GetValidationObject(int v)
        {
            return new ValidateFileResponse { BatchId = v };
        }


        /// <summary>
        /// Get the service details
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="directoryInfo"></param>
        /// <param name="sstateId"></param>
        /// <param name="sunknownTaxPayerCodeId"></param>
        /// <returns>FileServiceHelper</returns>
        public FileServiceHelper GetFileServiceHelper(string tenantName, DirectoryInfo directoryInfo, string sstateId, string sunknownTaxPayerCodeId, string filePath, string summaryCSVProcessedFilePath, string summaryFilePath)
        {
            try
            {
                var monthName = directoryInfo.Parent.Name;
                var yearValue = directoryInfo.Parent.Parent.Name;
                int year = 0;

                if (!Int32.TryParse(yearValue, out year))
                { throw new FormatException("Folder name for year not a valid year. Value: " + yearValue); }

                var monthVal = listOfMonths.Where(x => monthName.Contains(x.Key)).FirstOrDefault();
                if (monthVal.Key == null) { throw new FormatException("Folder name for month not a month. Value: " + monthName); }

                int stateId = 0;
                int unknownCodeId = 0;
                if (!Int32.TryParse(sstateId, out stateId)) { throw new Exception("No state Id present"); }

                if (!Int32.TryParse(sunknownTaxPayerCodeId, out unknownCodeId)) { throw new Exception("No UnknownTaxPayerCodeId present"); }

                //summary file path details
                var summaryDirInfo = new DirectoryInfo(summaryFilePath);
                summaryFilePath = summaryFilePath + string.Format(@"\{0}\{1}", year, monthVal.Key);
                if (!Directory.Exists(summaryFilePath))
                {
                    Directory.CreateDirectory(summaryFilePath);
                }

                string summaryFilePathWithFileName = summaryFilePath + string.Format(@"\{0} {1} Summary.csv", tenantName, monthVal.Key);

                //process CSV file path details
                summaryCSVProcessedFilePath = summaryCSVProcessedFilePath + string.Format(@"\{0}\{1}\", year, monthVal.Key);
                if (!Directory.Exists(summaryCSVProcessedFilePath))
                {
                    Directory.CreateDirectory(summaryCSVProcessedFilePath);
                }
                summaryCSVProcessedFilePath = summaryCSVProcessedFilePath + string.Format(@"{0} {1} Processed.csv", tenantName, monthVal.Key);


                return new FileServiceHelper
                {
                    TenantName = tenantName,
                    StateId = stateId,
                    UnknownTaxPayerCodeId = unknownCodeId,
                    Month = monthVal.Value,
                    Year = year,
                    FilePath = filePath,
                    SummaryPath = summaryFilePath,
                    ProcessedSummaryCSVFilePath = summaryCSVProcessedFilePath,
                    SummaryFilePathWithFileName = summaryFilePathWithFileName
                };
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error GetFileServiceHelper for tenant {0}, state id {1}, unknownpayercode {2}, dir for month {3}, dir for year {4}.", tenantName, sstateId, sunknownTaxPayerCodeId, directoryInfo.Parent.Name, directoryInfo.Parent != null ? directoryInfo.Parent.Parent.Name : "undefined"));
                log.Error(exception.Message, exception);
                throw;
            }
        }


        [ProlongExpirationTime]
        public ValidateFileResponse ValidateTheFile(FileServiceHelper serviceProperties, string filePath)
        {
            try
            {
                log.Info("Starting ValidateTheFile stage" + IPPISProcessingStages.NotProcessed);
                log.Info(string.Format("Month {0} Year {1} FilePath {2}", serviceProperties.Month, serviceProperties.Year, filePath));

                //lets check if the file for the month and year has already been processed
                //set unit of work
                SetUnitofWork(serviceProperties.TenantName);
                //instantiate the IPPISBatch repository
                SetBatchDAO();
                log.Info(string.Format("Getting batch record for Month {0} Year {1}", serviceProperties.Month, serviceProperties.Year));
                IPPISBatch batchRecord = BatchDAO.GetRecordForMonthAndYear(serviceProperties.Month, serviceProperties.Year);
                UoW.BeginTransaction();
                if (batchRecord != null)
                {
                    //lets check what stage it is in
                    //for file processing the expected stage is NotProcessed
                    CheckProcessStage(batchRecord, IPPISProcessingStages.NotProcessed, serviceProperties.Month, serviceProperties.Year);
                }
                else
                {
                    batchRecord = new IPPISBatch { FilePath = filePath, ProccessStage = (int)IPPISProcessingStages.NotProcessed, Month = serviceProperties.Month, Year = serviceProperties.Year };
                    BatchDAO.Add(batchRecord);
                }
                //now lets validate and save the file contents
                SetPayeeAdapter();
                log.Info(string.Format("Processing file for batch record for Month {0} Year {1}. FilePath {2}", serviceProperties.Month, serviceProperties.Year, filePath));
                IPPISPayeeResponse payeResponse = PayeeAdapter.GetPayeeResponseModels<IPPISPayeeResponse>(filePath, null, null, serviceProperties.Month, serviceProperties.Year);
                if (payeResponse.HeaderValidateObject.Error)
                {
                    batchRecord.ErrorOccurred = true;
                    batchRecord.ErrorMessage = payeResponse.HeaderValidateObject.ErrorMessage;
                    return new ValidateFileResponse { BatchId = batchRecord.Id, ErrorMessage = "Error reading the file. Some header parameters are missing for batch Id: " + batchRecord.Id, ErrorOccurred = true };
                }
                //if no error occurred while reading the file, lets save the records to DB
                SetBatchRecordsDAO();

                log.Info(string.Format("Saving Processed file for batch record for Month {0} Year {1}. FilePath {2}", serviceProperties.Month, serviceProperties.Year, filePath));

                int count = BatchRecordsDAO.SaveIPPISRecords(batchRecord.Id, payeResponse.Payees);
                batchRecord.NumberOfRecords = count;
                //update the stage 
                batchRecord.ProccessStage = (int)IPPISProcessingStages.CategorizationOfTaxPayerByCode;
                UoW.Commit();

                log.Info(string.Format("Processing for stage {3} completed for batch record for Month {0} Year {1}. FilePath {2}", serviceProperties.Month, serviceProperties.Year, filePath, IPPISProcessingStages.FileValidationProcessed));

                //Call the Hangfire storage
                StartHangfireServer();
                BackgroundJob.Enqueue(() => CategorizeByTaxPayerCode(serviceProperties, batchRecord.Id));

                return new ValidateFileResponse { BatchId = batchRecord.Id };
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {3}. Batch record for Month {0} Year {1}. FilePath {2}", serviceProperties.Month, serviceProperties.Year, filePath, IPPISProcessingStages.FileValidationProcessed));
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchRecordsDAO = null;
                BatchDAO = null;
                PayeeAdapter = null;
            }
        }


        [ProlongExpirationTime]
        /// <summary>
        /// This method categorizes the records store into distinct tax payer code
        /// </summary>
        /// <param name="batchId"></param>
        public void CategorizeByTaxPayerCode(FileServiceHelper serviceProperties, Int64 batchId)
        {
            try
            {
                log.Info(string.Format("Starting Categorize By Tax Payer Code processing for batch record for Month {0} Year {1}. Stage {2}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.CategorizationOfTaxPayerByCode));

                SetUnitofWork(serviceProperties.TenantName);
                SetBatchDAO();
                IPPISBatch batchRecord = BatchDAO.GetBatchRecord(batchId);
                CheckProcessStage(batchRecord, IPPISProcessingStages.CategorizationOfTaxPayerByCode, serviceProperties.Month, serviceProperties.Year);
                UoW.BeginTransaction();
                SetBatchRecordsDAO();

                log.Info(string.Format("Doing grouping By Tax Payer Code for batch record for Month {0} Year {1}. Stage {2}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.CategorizationOfTaxPayerByCode));

                BatchRecordsDAO.GroupRecordsFromIPPISBatchRecordsTableByTaxPayerCode(batchId);
                batchRecord.ProccessStage = (int)IPPISProcessingStages.MapTaxPayerCodeToTaxProfile;
                UoW.Commit();

                //Call the Hangfire storage
                log.Info(string.Format("Call the hangfire storage to queue the next job. Stage {0}", IPPISProcessingStages.CategorizationOfTaxPayerByCode));
                StartHangfireServer();
                BackgroundJob.Enqueue(() => MatchTaxEntityToTaxPayerCode(serviceProperties, batchRecord.Id));
                log.Info(string.Format("Queued the job successfully. Stage {0}", IPPISProcessingStages.CategorizationOfTaxPayerByCode));
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {2}. Batch record for Month {0} Year {1}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.CategorizationOfTaxPayerByCode));
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



        [ProlongExpirationTime]
        /// <summary>
        /// Match the tax payer code to the corrresponding tax entity agency code.
        /// Match identifier would be the Id of the TaxEntity
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="tenantName"></param>
        public void MatchTaxEntityToTaxPayerCode(FileServiceHelper serviceProperties, long batchId)
        {
            try
            {
                log.Info(string.Format("Starting Match Tax Entity To Tax Payer Code processing for batch record for Month {0} Year {1}. Stage {2}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.MapTaxPayerCodeToTaxProfile));

                SetUnitofWork(serviceProperties.TenantName);
                SetBatchDAO();
                IPPISBatch batchRecord = BatchDAO.GetBatchRecord(batchId);
                CheckProcessStage(batchRecord, IPPISProcessingStages.MapTaxPayerCodeToTaxProfile, serviceProperties.Month, serviceProperties.Year);
                UoW.BeginTransaction();
                SetBatchRecordsDAO();

                log.Info(string.Format("Doing matching By Tax Payer Code to Tax Profile for batch record for Month {0} Year {1}. Stage {2}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.MapTaxPayerCodeToTaxProfile));

                BatchRecordsDAO.MapTaxPayerCodeToTaxEntityId(batchId);
                batchRecord.ProccessStage = (int)IPPISProcessingStages.MapUnknownTaxPayerCodeToUnknownTaxProfile;
                UoW.Commit();

                log.Info(string.Format("Done. Batch record for Month {0} Year {1}. Stage {2}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.MapTaxPayerCodeToTaxProfile));

                //Call the Hangfire storage
                log.Info(string.Format("Call the hangfire storage to queue the next job. Stage {0}", IPPISProcessingStages.MapTaxPayerCodeToTaxProfile));
                StartHangfireServer();
                BackgroundJob.Enqueue(() => AttachUnknownTaxPayerCodeToUnknownTaxEntity(serviceProperties, batchRecord.Id));
                log.Info(string.Format("Queued the job successfully. Stage {0}", IPPISProcessingStages.MapTaxPayerCodeToTaxProfile));
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {2}. Batch record for Month {0} Year {1}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.MapTaxPayerCodeToTaxProfile));
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


        [ProlongExpirationTime]
        /// <summary>
        /// when the tax payer code have been matched with the corresponding tax entity
        /// there might be some tax payer codes that do not have a corresponding tax entity
        /// this method would tie these tax payer code to a default value
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="tenantName"></param>
        public void AttachUnknownTaxPayerCodeToUnknownTaxEntity(FileServiceHelper serviceProperties, long batchId)
        {
            try
            {
                log.Info(string.Format("Starting Attach Unknown Tax Payer Code To Unknown Tax Entity processing for batch record for Month {0} Year {1}. Stage {2}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.MapUnknownTaxPayerCodeToUnknownTaxProfile));

                SetUnitofWork(serviceProperties.TenantName);
                SetBatchDAO();
                IPPISBatch batchRecord = BatchDAO.GetBatchRecord(batchId);

                CheckProcessStage(batchRecord, IPPISProcessingStages.MapUnknownTaxPayerCodeToUnknownTaxProfile, serviceProperties.Month, serviceProperties.Year);
                UoW.BeginTransaction();
                SetBatchRecordsDAO();

                log.Info(string.Format("Doing mapping for unknown tax payer code for batch record for Month {0} Year {1}. Stage {2}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.MapUnknownTaxPayerCodeToUnknownTaxProfile));

                BatchRecordsDAO.MapNullTaxPayerCodeToUnknownTaxEntityId(batchId, serviceProperties.UnknownTaxPayerCodeId);
                batchRecord.ProccessStage = (int)IPPISProcessingStages.GenerateInvoices;
                UoW.Commit();

                log.Info(string.Format("Done. Batch record for Month {0} Year {1}. Stage {2}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.MapUnknownTaxPayerCodeToUnknownTaxProfile));

                //Call the Hangfire storage
                log.Info(string.Format("Call the hangfire storage to queue the next job. Stage {0}", IPPISProcessingStages.MapUnknownTaxPayerCodeToUnknownTaxProfile));
                StartHangfireServer();
                BackgroundJob.Enqueue(() => GenerateInvoices(serviceProperties, batchRecord.Id));
                log.Info(string.Format("Queued the job successfully. Stage {0}", IPPISProcessingStages.MapUnknownTaxPayerCodeToUnknownTaxProfile));
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {2}. Batch record for Month {0} Year {1}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.MapUnknownTaxPayerCodeToUnknownTaxProfile));
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


        [ProlongExpirationTime]
        /// <summary>
        /// Generate invoices for the batch records
        /// </summary>
        /// <param name="batchId"></param>
        public void GenerateInvoices(FileServiceHelper serviceProperties, long batchId)
        {
            try
            {
                log.Info(string.Format("Starting Generate Invoices processing for batch record for Month {0} Year {1}. Stage {2}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.GenerateInvoices));

                SetUnitofWork(serviceProperties.TenantName);
                SetBatchDAO();
                IPPISBatch batchRecord = BatchDAO.GetBatchRecord(batchId);

                CheckProcessStage(batchRecord, IPPISProcessingStages.GenerateInvoices, serviceProperties.Month, serviceProperties.Year);
                //get the records you want to generate invoices for
                //chunk these records
                //lets get chunk size
                int chunkSize = 20;
                string schunkSize = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.ChunkSizeForIPPISInvoiceGeneration);
                if (!string.IsNullOrEmpty(schunkSize))
                {
                    if (Int32.TryParse(schunkSize, out chunkSize)) { chunkSize = 500; }
                }
                SetSummaryDAO();
                //get the count of records that have been grouped by the batch Id
                Int64 recordCount = SummaryDAO.Count(r => r.IPPISBatch == new IPPISBatch { Id = batchId });
                if (recordCount < 1) return;

                //get the pages
                int pages = Util.Pages(chunkSize, recordCount);
                int stopper = 0;
                int skip = 0;
                //get invoice details
                SetRevenueHeadDAO();
                RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails = RevenueHeadDAO.GetRevenueHeadDetailsForInvoiceGenerationForPayee();
                revenueHeadDetails.InvoiceDate = DateTime.Now.ToLocalTime();
                //get the details you would need to generate the invoice, details such as due date
                CreateInvoiceHelper helper = InvoiceGenerationType.GetInvoiceHelperModel(revenueHeadDetails);
                string invoiceModel = JsonConvert.SerializeObject(helper);

                List<ConcurrentStack<IPPISGenerateInvoiceResult>> listOfProcessedInvoices = new List<ConcurrentStack<IPPISGenerateInvoiceResult>> { };

                log.Info(string.Format("Starting chunking for invoice generation of data to size {3}. Total Datasize {4}. Invoices processing for batch record for Month {0} Year {1}. Stage {2}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.GenerateInvoices, chunkSize, recordCount));

                while (stopper < pages)
                {
                    log.Info(string.Format("Starting invoice generation by chunk for batch Id {0}. Page {1} of {2}", batchId, stopper, pages));

                    List<IPPISGenerateInvoiceModel> chunkedRecords = SummaryDAO.GetChunkedBatch(batchId, chunkSize, skip);
                    var result = ProcessInvoices(chunkedRecords, helper, revenueHeadDetails, serviceProperties.StateId, serviceProperties.Month).Result;

                    log.Info(string.Format("Done invoice generation by chunk for batch Id {0}. Page {1} of {2}", batchId, stopper, pages));

                    listOfProcessedInvoices.Add(result);
                    skip += chunkSize;
                    stopper++;
                }
                log.Info(string.Format("Saving bundle for invoice generation for batch Id {0}..", batchId));

                SetRecordsInvoiceDAO();
                UoW.BeginTransaction();
                //when all the invoice chunk by chunk are process, lets send all the processed chunks to database
                RecordsInvoiceDAO.SaveBundle(listOfProcessedInvoices, batchId, invoiceModel);
                batchRecord.ProccessStage = (int)IPPISProcessingStages.CreateDirectAssessments;
                UoW.Commit();
                log.Info(string.Format("Done invoice generation by for batch Id {0}.", batchId));

                //Call the Hangfire storage
                log.Info(string.Format("Call the hangfire storage to queue the next job. Stage {0}", IPPISProcessingStages.GenerateInvoices));
                StartHangfireServer();
                BackgroundJob.Enqueue(() => CreateDirectAssessments(serviceProperties, batchRecord.Id));
                log.Info(string.Format("Queued the job successfully. Stage {0}", IPPISProcessingStages.GenerateInvoices));

            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {2}. Batch record for Month {0} Year {1}. Batch ID {3}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.GenerateInvoices, batchId));
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchDAO = null;
                RevenueHeadDAO = null;
                SummaryDAO = null;
                RecordsInvoiceDAO = null;
            }
        }


        [ProlongExpirationTime]
        /// <summary>
        /// Once invoices have been generated for the batch, lets create the direct assessments for each record
        /// </summary>
        /// <param name="batchId"></param>
        public void CreateDirectAssessments(FileServiceHelper serviceProperties, long batchId)
        {
            try
            {
                log.Info(string.Format("Starting Create Direct Assessments processing for batch record for Month {0} Year {1}. Stage {2} and batch ID {3}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.CreateDirectAssessments, batchId));

                SetUnitofWork(serviceProperties.TenantName);
                SetBatchDAO();
                IPPISBatch batchRecord = BatchDAO.GetBatchRecord(batchId);
                CheckProcessStage(batchRecord, IPPISProcessingStages.CreateDirectAssessments, serviceProperties.Month, serviceProperties.Year);
                SetDirectAssessmentBatchRecordDAO();
                SetRevenueHeadDAO();
                RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails = RevenueHeadDAO.GetRevenueHeadDetailsForInvoiceGenerationForPayee();
                UoW.BeginTransaction();

                DirectAssessmentBatchRecordDAO.CreateDirectAssessmentsForIPPIS(batchId, revenueHeadDetails, serviceProperties.Month, serviceProperties.Year);
                batchRecord.ProccessStage = (int)IPPISProcessingStages.CreateDirectAssessmentRecords;
                UoW.Commit();

                log.Info(string.Format("Done Creating Direct Assessments for batch Id {0}. Month {1} Year {2}", batchId, serviceProperties.Month, serviceProperties.Year));

                //Call the Hangfire storage
                log.Info(string.Format("Call the hangfire storage to queue the next job. Stage {0}", IPPISProcessingStages.CreateDirectAssessments));
                StartHangfireServer();
                BackgroundJob.Enqueue(() => MoveDirectAssessmentsBatchRecords(serviceProperties, batchRecord.Id));
                log.Info(string.Format("Queued the job successfully. Stage {0}", IPPISProcessingStages.CreateDirectAssessments));
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {2}. Batch record for Month {0} Year {1}. Batch ID {3}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.CreateDirectAssessments, batchId));
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchDAO = null;
                RevenueHeadDAO = null;
                DirectAssessmentBatchRecordDAO = null;
            }
        }


        [ProlongExpirationTime]
        /// <summary>
        /// We have created the direct assessment batch record,
        /// now lets move the records for assesment into the direct assessment batch record table
        /// </summary>
        /// <param name="batchId"></param>
        public void MoveDirectAssessmentsBatchRecords(FileServiceHelper serviceProperties, long batchId)
        {
            try
            {
                log.Info(string.Format("Starting Create Direct Assessment Records processing for batch record for Month {0} Year {1}. Stage {2} and batch ID {3}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.CreateDirectAssessmentRecords, batchId));

                var monthVal = listOfMonths.Where(x => serviceProperties.Month == x.Value).FirstOrDefault();
                if (monthVal.Key == null) { throw new FormatException("Folder name for month not a month. Value: " + serviceProperties.Month); }

                SetUnitofWork(serviceProperties.TenantName);
                SetBatchDAO();
                IPPISBatch batchRecord = BatchDAO.GetBatchRecord(batchId);

                CheckProcessStage(batchRecord, IPPISProcessingStages.CreateDirectAssessmentRecords, serviceProperties.Month, serviceProperties.Year);
                SetDirectAssessmentPayeeRecordDAO();
                UoW.BeginTransaction();
                DirectAssessmentPayeeRecordDAO.MigrateIPPISEmployeeRecordsToDirectAssessmentPayeeRecordTable(batchId, serviceProperties.Month, monthVal.Key, serviceProperties.Year);
                batchRecord.ProccessStage = (int)IPPISProcessingStages.CreateInvoices;
                UoW.Commit();

                log.Info(string.Format("Done Creating Direct Assessment Records for batch Id {0}. Month {1} Year {2}", batchId, serviceProperties.Month, serviceProperties.Year));

                //Call the Hangfire storage
                log.Info(string.Format("Call the hangfire storage to queue the next job. Stage {0}", IPPISProcessingStages.CreateDirectAssessmentRecords));
                StartHangfireServer();
                BackgroundJob.Enqueue(() => CreateInvoices(serviceProperties, batchRecord.Id));
                log.Info(string.Format("Queued the job successfully. Stage {0}", IPPISProcessingStages.CreateDirectAssessmentRecords));
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {2}. Batch record for Month {0} Year {1}. Batch ID {3}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.CreateDirectAssessmentRecords, batchId));
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchDAO = null;
                DirectAssessmentPayeeRecordDAO = null;
            }
        }


        [ProlongExpirationTime]
        /// <summary>
        /// Create the invoices for each tax payer on the IPPIS file
        /// </summary>
        /// <param name="batchId"></param>
        public void CreateInvoices(FileServiceHelper serviceProperties, long batchId)
        {
            try
            {
                log.Info(string.Format("Starting Create Invoices processing for batch record for Month {0} Year {1}. Stage {2} and batch ID {3}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.CreateInvoices, batchId));

                var monthVal = listOfMonths.Where(x => serviceProperties.Month == x.Value).FirstOrDefault();
                if (monthVal.Key == null) { throw new FormatException("Folder name for month not a month. Value: " + serviceProperties.Month); }

                SetUnitofWork(serviceProperties.TenantName);
                SetBatchDAO();
                IPPISBatch batchRecord = BatchDAO.GetBatchRecord(batchId);
                //.GetRecordForMonthAndYear(serviceProperties.Month, serviceProperties.Year);
                CheckProcessStage(batchRecord, IPPISProcessingStages.CreateInvoices, serviceProperties.Month, serviceProperties.Year);
                SetInvoiceDAO();
                SetRevenueHeadDAO();
                RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails = RevenueHeadDAO.GetRevenueHeadDetailsForInvoiceGenerationForPayee();
                UoW.BeginTransaction();
                InvoiceDAO.CreateIPPISInvoices(batchId, revenueHeadDetails);
                batchRecord.ProccessStage = (int)IPPISProcessingStages.CreateTransactionLogs;
                UoW.Commit();

                log.Info(string.Format("Done Create Invoices for batch Id {0}. Month {1} Year {2}", batchId, serviceProperties.Month, serviceProperties.Year));

                //Call the Hangfire storage
                log.Info(string.Format("Call the hangfire storage to queue the next job. Stage {0}", IPPISProcessingStages.CreateInvoices));
                StartHangfireServer();
                BackgroundJob.Enqueue(() => CreateTransactionLogs(serviceProperties, batchRecord.Id));
                log.Info(string.Format("Queued the job successfully. Stage {0}", IPPISProcessingStages.CreateTransactionLogs));
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {2}. Batch record for Month {0} Year {1}. Batch ID {3}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.CreateInvoices, batchId));
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchDAO = null;
                RevenueHeadDAO = null;
                InvoiceDAO = null;
            }
        }

        [ProlongExpirationTime]
        /// <summary>
        /// Create the invoices for each tax payer on the IPPIS file
        /// </summary>
        /// <param name="batchId"></param>
        public void CreateTransactionLogs(FileServiceHelper serviceProperties, long batchId)
        {
            try
            {
                log.Info(string.Format("Starting Create Transaction Logs processing for batch record for Month {0} Year {1}. Stage {2} and batch ID {3}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.CreateTransactionLogs, batchId));

                var monthVal = listOfMonths.Where(x => serviceProperties.Month == x.Value).FirstOrDefault();
                if (monthVal.Key == null) { throw new FormatException("Folder name for month not a month. Value: " + serviceProperties.Month); }

                SetUnitofWork(serviceProperties.TenantName);
                SetBatchDAO();
                IPPISBatch batchRecord = BatchDAO.GetBatchRecord(batchId);
                //.GetRecordForMonthAndYear(serviceProperties.Month, serviceProperties.Year);
                CheckProcessStage(batchRecord, IPPISProcessingStages.CreateTransactionLogs, serviceProperties.Month, serviceProperties.Year);
                SetInvoiceDAO();
                UoW.BeginTransaction();
                InvoiceDAO.CreateIPPISInvoiceTransactionLog(batchId);
                batchRecord.ProccessStage = (int)IPPISProcessingStages.ConfirmDirectAssessmentInvoices;
                UoW.Commit();

                log.Info(string.Format("Done Create Invoices for batch Id {0}. Month {1} Year {2}", batchId, serviceProperties.Month, serviceProperties.Year));

                //Call the Hangfire storage
                log.Info(string.Format("Call the hangfire storage to queue the next job. Stage {0}", IPPISProcessingStages.CreateTransactionLogs));
                StartHangfireServer();
                BackgroundJob.Enqueue(() => ConfirmDirectAssessments(serviceProperties, batchRecord.Id));
                log.Info(string.Format("Queued the job successfully. Stage {0}", IPPISProcessingStages.CreateDirectAssessmentRecords));
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {2}. Batch record for Month {0} Year {1}. Batch ID {3}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.CreateTransactionLogs, batchId));
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


        [ProlongExpirationTime]
        /// <summary>
        /// We have created the invoices for direct assessment generation
        /// now we confirm these invoices
        /// </summary>
        /// <param name="batchId"></param>
        public void ConfirmDirectAssessments(FileServiceHelper serviceProperties, long batchId)
        {
            try
            {
                log.Info(string.Format("Starting Confirm Direct Assessments processing for batch record for Month {0} Year {1}. Stage {2} and batch ID {3}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.CreateInvoices, batchId));

                var monthVal = listOfMonths.Where(x => serviceProperties.Month == x.Value).FirstOrDefault();
                if (monthVal.Key == null) { throw new FormatException("Folder name for month not a month. Value: " + serviceProperties.Month); }

                SetUnitofWork(serviceProperties.TenantName);
                SetBatchDAO();
                IPPISBatch batchRecord = BatchDAO.GetBatchRecord(batchId);
                //.GetRecordForMonthAndYear(serviceProperties.Month, serviceProperties.Year);
                CheckProcessStage(batchRecord, IPPISProcessingStages.ConfirmDirectAssessmentInvoices, serviceProperties.Month, serviceProperties.Year);
                SetDirectAssessmentBatchRecordDAO();

                UoW.BeginTransaction();
                DirectAssessmentBatchRecordDAO.SetInvoiceConfirmationForIPPISToTrue(serviceProperties.Month, serviceProperties.Year);
                batchRecord.ProccessStage = (int)IPPISProcessingStages.Processed;
                UoW.Commit();

                log.Info(string.Format("Done Confirm Direct Assessments for batch Id {0}. Month {1} Year {2}", batchId, serviceProperties.Month, serviceProperties.Year));

                //Call the Hangfire storage
                log.Info(string.Format("Call the hangfire storage to queue the next job. Stage {0}", IPPISProcessingStages.Processed));
                StartHangfireServer();
                BackgroundJob.Enqueue(() => MoveCSVToSummaryPath(serviceProperties, batchRecord.Id));
                log.Info(string.Format("Queued the job successfully. Stage {0}", IPPISProcessingStages.MovedCSVToSummaryPath));
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {2}. Batch record for Month {0} Year {1}. Batch ID {3}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.ConfirmDirectAssessmentInvoices, batchId));
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchDAO = null;
                DirectAssessmentBatchRecordDAO = null;
            }
        }


        [ProlongExpirationTime]
        /// <summary>
        /// move CSV to summary file path
        /// </summary>
        /// <param name="serviceProperties"></param>
        /// <param name="batchId"></param>
        public void MoveCSVToSummaryPath(FileServiceHelper serviceProperties, long batchId)
        {
            try
            {
                int counter = 0;
                bool keepTrying = true;
                string errorMessage = string.Empty;
                bool hasError = false;
                //
                int stopper = 5;
                string sstopper = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.NumberOfRetriesForMovingSummaryFileProcess);
                if (!string.IsNullOrEmpty(sstopper))
                {
                    if (Int32.TryParse(sstopper, out stopper)) { stopper = 5; }
                }
                //
                int timeLapseForRetry = 60000; //one minute
                string stimeLapseForRetry = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.RetryWaitTimeForSummaryFileMovement);
                if (!string.IsNullOrEmpty(stimeLapseForRetry))
                {
                    if (Int32.TryParse(stimeLapseForRetry, out timeLapseForRetry)) { timeLapseForRetry = 60000; }
                }

                SetUnitofWork(serviceProperties.TenantName);
                SetBatchDAO();
                IPPISBatch batchRecord = BatchDAO.GetBatchRecord(batchId);
                if (batchRecord == null) { throw new KeyNotFoundException("Record for batch id " + batchId); }
                CheckProcessStage(batchRecord, IPPISProcessingStages.Processed, serviceProperties.Month, serviceProperties.Year);

                //now check if the summary has been done
                //if summary file is ready, return
                if (batchRecord.IsSummaryFileReady) { return; }
                //if summary file is ready and has already been moved, return
                if (batchRecord.HasSummaryFileMoved) { keepTrying = false; }

                if (!Directory.Exists(serviceProperties.SummaryPath))
                {
                    Directory.CreateDirectory(serviceProperties.SummaryPath);
                }

                //now if summary file is ready and has not been moved, lets move it
                while (keepTrying)
                {
                    try
                    {
                        //try move
                        if (!File.Exists(serviceProperties.SummaryFilePathWithFileName))
                        {
                            File.Move(serviceProperties.ProcessedSummaryCSVFilePath, serviceProperties.SummaryFilePathWithFileName);
                        }
                        hasError = false;
                        keepTrying = false;
                    }
                    catch (DirectoryNotFoundException exception)
                    {
                        errorMessage = string.Format("Error Processing for stage {2}. Batch record for Month {0} Year {1}. Batch ID {3}. DirectoryNotFoundException for path summary: {4} ProcessedCSVPath: {5}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.MovedCSVToSummaryPath, batchId, serviceProperties.SummaryPath, serviceProperties.ProcessedSummaryCSVFilePath);

                        log.Error(errorMessage);
                        log.Error(exception.Message, exception);
                        hasError = true;
                        keepTrying = false;
                    }
                    catch (Exception exception)
                    {
                        if (counter > stopper)
                        {
                            //we have waited for 5 minutes and no show
                            hasError = true;
                            errorMessage = string.Format("Error Processing for stage {2}. Batch record for Month {0} Year {1}. Batch ID {3}. For path summary: {4} ProcessedCSVPath: {5}. Exception {6}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.MovedCSVToSummaryPath, batchId, serviceProperties.SummaryPath, serviceProperties.ProcessedSummaryCSVFilePath, exception.Message);
                            keepTrying = false;
                            continue;
                        }
                        //wait a minute
                        Thread.Sleep(timeLapseForRetry);
                        counter++;
                    }
                }
                UoW.BeginTransaction();
                if (hasError)
                {
                    batchRecord.ErrorProcessingSummaryFile = true;
                    batchRecord.ErrorMessageProcessingSummaryFile = errorMessage;
                }
                else
                {
                    //batchRecord.ProccessStage = (int)IPPISProcessingStages.MovedCSVToSummaryPath;
                    batchRecord.HasSummaryFileMoved = true;
                }
                UoW.Commit();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {2}. Batch record for Month {0} Year {1}. Batch ID {3}", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.MovedCSVToSummaryPath, batchId));
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchDAO = null;
            }
        }


        [ProlongExpirationTime]
        /// <summary>
        /// This method convert's the IPPIS summary file to CSV
        /// now we confirm these invoices
        /// </summary>
        /// <param name="serviceProperties"></param>
        /// <param name="batchId"></param>
        /// <param name="fileDestination"></param>
        public void ConvertIPPISSummaryFileToCSV(FileServiceHelper serviceProperties)
        {
            try
            {
                log.Info(string.Format("Starting ConvertIPPISFileToCSV processing for batch record for Month {0} Year {1}. ", serviceProperties.Month, serviceProperties.Year));
                SetPayeeAdapter();

                log.Info(string.Format("Processing file for batch record for Month {0} Year {1}. FilePath {2}", serviceProperties.Month, serviceProperties.Year, serviceProperties.FilePath));
                HeaderValidateObject conversionResponse = null;
                string exceptionMsg = string.Empty;

                try
                {
                    var monthVal = listOfMonths.Where(x => serviceProperties.Month == x.Value).FirstOrDefault();
                    if (monthVal.Key == null) { throw new FormatException("Folder name for month not a month. Value: " + serviceProperties.Month); }

                    SettlementDetails settlementDetails = ProcessSettlement(serviceProperties.TenantName);
                    settlementDetails.Month = monthVal.Key;
                    settlementDetails.Year = serviceProperties.Year.ToString();

                    conversionResponse = PayeeAdapter.ConvertExcelToCSV(serviceProperties.FilePath, serviceProperties.ProcessedSummaryCSVFilePath, settlementDetails);
                }
                catch (Exception exception)
                {
                    if (conversionResponse == null) { conversionResponse = new HeaderValidateObject { }; }

                    log.Error(string.Format("Error Processing for stage {2} ConvertIPPISFileToCSV. Batch record for Month {0} Year {1}.", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.ConvertExcelToCSV));
                    log.Error(exception.Message, exception);
                    exceptionMsg = exception.Message + " | " + exception.StackTrace;
                }
                //
                SetUnitofWork(serviceProperties.TenantName);
                SetBatchDAO();
                IPPISBatch batchRecord = BatchDAO.GetRecordForMonthAndYear(serviceProperties.Month, serviceProperties.Year);
                //if the batch record has not been processed, return
                if (batchRecord == null) { return; }

                UoW.BeginTransaction();

                if (!string.IsNullOrEmpty(exceptionMsg) || conversionResponse.Error)
                {
                    batchRecord.ErrorProcessingSummaryFile = true;
                    batchRecord.ErrorMessageProcessingSummaryFile = string.Format("Error ConvertIPPISSummaryFileToCSV for Month {0} Year {1}. Error message {2} {3}", serviceProperties.Month, serviceProperties.Year, conversionResponse.ErrorMessage, exceptionMsg);
                    log.Info(batchRecord.ErrorMessageProcessingSummaryFile);
                }
                else
                {
                    log.Info(string.Format("Done ConvertIPPISSummaryFileToCSV for Month {0} Year {1}", serviceProperties.Month, serviceProperties.Year));

                    batchRecord.IsSummaryFileReady = true;
                    batchRecord.ErrorProcessingSummaryFile = false;
                    //now we check if this stage is at processed
                    //check if we can move the file to summary path
                    if (batchRecord.GetProcessStage() == IPPISProcessingStages.Processed)
                    {
                        //if the batch has been processed
                        //lets move the file to summary path
                        try
                        {
                            if (File.Exists(serviceProperties.SummaryFilePathWithFileName))
                                File.Move(serviceProperties.SummaryFilePathWithFileName, string.Format("{2}\\Backup {0} {1} {3}.csv", serviceProperties.Month, serviceProperties.Year, serviceProperties.SummaryPath, DateTime.Now.Ticks.ToString()));

                            File.Move(serviceProperties.ProcessedSummaryCSVFilePath, serviceProperties.SummaryFilePathWithFileName);
                            batchRecord.HasSummaryFileMoved = true;
                        }
                        catch (Exception exception)
                        {
                            batchRecord.ErrorProcessingSummaryFile = true;
                            batchRecord.ErrorMessageProcessingSummaryFile = exception.Message;
                            log.Error(string.Format("Error Processing for stage {2}. Batch record for Month {0} Year {1}.", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.ConvertExcelToCSV));
                            log.Error(exception.Message, exception);
                        }
                    }
                }
                UoW.Commit();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {2}. Batch record for Month {0} Year {1}.", serviceProperties.Month, serviceProperties.Year, IPPISProcessingStages.ConvertExcelToCSV));
                log.Error(exception.Message, exception);
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchDAO = null;
            }
        }



        private SettlementDetails ProcessSettlement(string tenantName)
        {
            //get settlement details
            Dictionary<int, string> additionalInfo = new Dictionary<int, string> { };
            var settlementConfig = Util.GetIPPISSettlementConfig(tenantName);
            decimal totalPercentage = 0.00M;

            List<SettlementParty> parties = new List<SettlementParty> { };

            settlementConfig.SettlementParties.ForEach(party =>
            {
                totalPercentage += party.Percentage;

                if (totalPercentage > 100.00m)
                { throw new Exception(string.Format("Percentage calculation does not add up to 100%. Actual value {0}", totalPercentage)); }

                party.Node.ForEach(node =>
                    {
                        additionalInfo.Add(node.Index, node.Value);
                    });

                parties.Add(new SettlementParty { Name = party.Name, Percentage = party.Percentage, Cap = party.Cap, DetailRows = new Dictionary<int, string>(additionalInfo) });


                additionalInfo.Clear();
            });

            if (totalPercentage != 100.00m)
            { throw new Exception(string.Format("Percentage calculation does not add up to 100%. Actual value {0}", totalPercentage)); }

            return new SettlementDetails { Parties = parties, Spacing = settlementConfig.Spacing };
        }


        /// <summary>
        /// Check if the process stage is valid
        /// </summary>
        /// <param name="batchRecord"></param>
        /// <param name="expectedStage"></param>
        private void CheckProcessStage(IPPISBatch batchRecord, IPPISProcessingStages expectedStage, int month, int year)
        {
            log.Info(string.Format("Doing check on process stage for batch record for Month {0} Year {1}. Expected stage {2}", month, year, expectedStage));

            if (batchRecord != null)
            {
                //lets check what stage it is in
                //for this method the expected stage is FileValidationProcessed
                if (batchRecord.ProccessStage != (int)expectedStage)
                { throw new InvalidOperationException(string.Format("IPPIS processing for this month {0} and year {1} have already passed this stage. Current stage {2}, Expected stage {3}", month, year, (IPPISProcessingStages)batchRecord.ProccessStage, expectedStage)); }
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
        private async Task<ConcurrentStack<IPPISGenerateInvoiceResult>> ProcessInvoices(List<IPPISGenerateInvoiceModel> chunkedRecords, CreateInvoiceHelper helper, RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails, int stateId, int month)
        {
            ConcurrentStack<IPPISGenerateInvoiceResult> listOfProcessResults = new ConcurrentStack<IPPISGenerateInvoiceResult>();
            InstantiateInvoicingService();

            CashFlowRequestContext context = InvoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", revenueHeadDetails.SMEKey } });

            var monthVal = listOfMonths.Where(x => x.Value == month).FirstOrDefault().Key;

            await chunkedRecords.ParallelForEachAsync(
                async chunk =>
                {
                    var webCallResult = await InvoicingService.InvoiceService(context).CreateCustomerAndInvoiceAsync(
                             new CashFlowCreateCustomerAndInvoice
                             {
                                 CreateCustomer = new CashFlowCreateCustomer
                                 {
                                     Address = chunk.Address,
                                     CountryID = 1,
                                     CustomerId = chunk.CashflowCustomerId,
                                     Identifier = chunk.TaxProfileId.ToString(),
                                     Name = chunk.Recipient,
                                     StateID = stateId,
                                     Type = Cashflow.Ng.Models.Enums.CashFlowCustomerType.Business,
                                     PryContact = new CashFlowCreateCustomer.Contact
                                     {
                                         Name = chunk.Recipient,
                                         Email = chunk.Email
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
                                    { new CashFlowCreateInvoice.CashFlowProductModel { ProductName = "Direct Payee Assessment for " + monthVal + "/"+ chunk.Year.ToString(), Pos = 1, Price = chunk.Amount, ProductId = revenueHeadDetails.CashFlowProductId, Qty = 1 } }
                                     },
                                     Title = helper.Title,
                                     Type = helper.Type,
                                 },
                                 InvoiceUniqueKey = chunk.IPPISTaxPayerSummaryId.ToString(),
                                 PropertyTitle = "CentralBillingSystem",
                             });

                    listOfProcessResults.Push(new IPPISGenerateInvoiceResult { IPPISTaxPayerSummaryId = chunk.IPPISTaxPayerSummaryId, IntegrationResponseModel = webCallResult, TaxProfileCategoryId = chunk.TaxProfileCategoryId, TaxProfileId = chunk.TaxProfileId, DueDate = helper.DueDate, InvoiceDescription = "Direct Payee Assessment for " + monthVal + "/" + chunk.Year.ToString() });
                },
                maxDegreeOfParalellism: 0
                );
            return listOfProcessResults;
        }

        private void SetPayeeAdapter()
        { if (PayeeAdapter == null) { PayeeAdapter = new IPPISPayeeAdapter(); } }


        protected void SetBatchDAO()
        { if (BatchDAO == null) { BatchDAO = new IPPISBatchDAOManager(UoW); } }


        protected void SetBatchRecordsDAO()
        { if (BatchRecordsDAO == null) { BatchRecordsDAO = new IPPISBatchRecordsDAOManager(UoW); } }

        protected void SetSummaryDAO()
        { if (SummaryDAO == null) { SummaryDAO = new IPPISTaxPayerSummaryDAOManager(UoW); } }

        private void SetRevenueHeadDAO()
        { if (RevenueHeadDAO == null) { RevenueHeadDAO = new RevenueHeadDAOManager(UoW); } }

        private void SetRecordsInvoiceDAO()
        { if (RecordsInvoiceDAO == null) { RecordsInvoiceDAO = new IPPISBatchRecordsInvoiceDAOManager(UoW); } }


        private void SetDirectAssessmentBatchRecordDAO()
        { if (DirectAssessmentBatchRecordDAO == null) { DirectAssessmentBatchRecordDAO = new DirectAssessmentBatchRecordDAOManager(UoW); } }


        private void SetDirectAssessmentPayeeRecordDAO()
        { if (DirectAssessmentPayeeRecordDAO == null) { DirectAssessmentPayeeRecordDAO = new DirectAssessmentPayeeRecordDAOManager(UoW); } }


        private void SetInvoiceDAO()
        { if (InvoiceDAO == null) { InvoiceDAO = new InvoiceDAOManager(UoW); } }


        public void InstantiateInvoicingService()
        { if (InvoicingService == null) { InvoicingService = new InvoicingService(); } }


        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName + "_SessionFactory", "ClientFileServices");
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
