using ExcelDataReader;
using Hangfire;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations.Contracts;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations.Logger;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations.Logger.Contracts;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations
{
    public class PSSBranchOfficersFileUpload : IPSSBranchOfficersFileUpload
    {
        /// <summary>
        ///  BRANCH CODE - 0, 
        /// AP NUMBER - 1,
        /// </summary>
        protected readonly string[] headerNames = { "BRANCH CODE".ToLower(), "AP NUMBER".ToLower() };

        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }
        public IPSSBranchOfficersBatchDAOManager PSSBranchOfficersBatchDAOManager { get; set; }
        public IPSSBranchOfficersFileUploadValidation PSSBranchOfficersFileUploadValidator { get; set; }
        public IPSSBranchOfficersBatchItemsDAOManager PSSBranchOfficersBatchItemsDAOManager { get; set; }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSBranchOfficersExcelFileUploadProcessingJob");
            }
        }

        private void SetPSSBranchOfficerValidationInstance()
        {
            if (PSSBranchOfficersFileUploadValidator == null) { PSSBranchOfficersFileUploadValidator = new PSSBranchOfficersFileUploadValidation(); }
        }

        private void SetPSSBranchOfficersBatchDAOManager()
        {
            if (PSSBranchOfficersBatchDAOManager == null) { PSSBranchOfficersBatchDAOManager = new PSSBranchOfficersBatchDAOManager(UoW); }
        }

        private void SetPSSBranchOfficersBatchItemsDAOManager()
        {
            if (PSSBranchOfficersBatchItemsDAOManager == null) { PSSBranchOfficersBatchItemsDAOManager = new PSSBranchOfficersBatchItemsDAOManager(UoW); }
        }

        /// <summary>
        /// Process PSSBranchOfficers line items for file upload as a background job.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="tenantName"></param>
        public void ProcessPSSBranchOfficersFileUpload(long batchId, string tenantName)
        {
            //Commence hangfire processing
            try
            {
                var conStringNameKey = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.HangfireConnectionStringName);
                if (string.IsNullOrEmpty(conStringNameKey))
                {
                    throw new Exception("Unable to get the hangfire connection string name");
                }
                StartHangfireServer(conStringNameKey);
                BackgroundJob.Enqueue(() => BeginProcessing(batchId, tenantName));

            }
            catch (Exception exception)
            {
                log.Error("Error while queuing PSSBranchOfficers batch excel file upload to Hangfire.");
                log.Error(exception.Message, exception);
                throw;
            }

        }

        private void StartHangfireServer(string conStringNameKey)
        {
            string dbConnectionString = HangFireScheduler.GetConnectionString(conStringNameKey);

            GlobalConfiguration.Configuration.UseSqlServerStorage(dbConnectionString);
        }


        /// <summary>
        /// Commence PSSBranchOfficers excel file processing
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="tenantName"></param>
        public void BeginProcessing(long batchId, string tenantName)
        {
            try
            {
                SetUnitofWork(tenantName);

                SetPSSBranchOfficerValidationInstance();
                SetPSSBranchOfficersBatchDAOManager();
                SetPSSBranchOfficersBatchItemsDAOManager();

                //Check if the PSSBranchOfficers batch has already been processed.
                var batchDetails = PSSBranchOfficersBatchDAOManager.GetPSSBranchOfficersUploadBatchStatusAndFilePath(batchId);

                if (batchDetails == null)
                {
                    throw new InvalidOperationException($"Unable to get the processing status and filepath for batch with id {batchId}");
                }

                if (batchDetails.Status != PSSBranchOfficersUploadStatus.BatchInitialized)
                {
                    throw new InvalidOperationException($"Status mismatch for PSSBranchOfficers batch with id {batchId}. Batch status - {batchDetails.Status} Expected status - {PSSBranchOfficersUploadStatus.BatchInitialized}");
                }

                //Get validated records from PSSBranchOfficers schedule excel file and save
                List<PSSBranchOfficersItemVM> lineItems = GetRecordsInScheduleFile(batchId, batchDetails.FilePath);

                if (lineItems.Count == 0) 
                {
                    PSSBranchOfficersBatchDAOManager.UpdatePSSBranchOfficersUploadBatchStatus(PSSBranchOfficersUploadStatus.Fail, batchId, "Empty branch officer records");
                    log.Error($"Error while processing GetRecordsInScheduleFile: for batch {batchId}, Empty branch officer records");
                    return;
                }

                UoW.BeginTransaction();
                PSSBranchOfficersBatchItemsDAOManager.SavePSSBranchOfficersLineItemsRecords(lineItems, batchId);
                //Update enumeration batch to reflect completion of records saved to table
                PSSBranchOfficersBatchDAOManager.UpdatePSSBranchOfficersUploadBatchStatus(PSSBranchOfficersUploadStatus.BatchValidation, batchId);
                UoW.Commit();

                new ServiceNumberValidator().ProcessPSSBranchOfficersFileUpload(batchId, tenantName);
            }
            catch (Exception exception)
            {
                log.Error($"Exception occured while trying to process PSSBranchOfficers schedule excel file records.");
                log.Error(exception.Message, exception);
                UoW.Rollback();
            }
        }


        /// <summary>
        /// Get validated records from PSSBranchOfficers schedule excel file.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private List<PSSBranchOfficersItemVM> GetRecordsInScheduleFile(long batchId, string filePath)
        {

            DataSet result = new DataSet();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    result = reader.AsDataSet();
                }
            }
            var sheet = result.Tables[0];
            var rows = sheet.Rows;

            Dictionary<string, HeaderValidationModel> headers = new Dictionary<string, HeaderValidationModel>
            {
                { headerNames[0], new HeaderValidationModel { } }, { headerNames[1], new HeaderValidationModel { } } };

            ValidateHeaders(rows[0], ref headers);

            if (headers.Any(k => !k.Value.HeaderPresent))
            {
                PSSBranchOfficersBatchDAOManager.UpdatePSSBranchOfficersUploadBatchStatus(batchId, errorMessage: "Invalid headers");
                return new List<PSSBranchOfficersItemVM>();
            }

            rows.RemoveAt(0);

            List<PSSBranchOfficersItemVM> validatedLineItems = new List<PSSBranchOfficersItemVM> { };

            foreach (DataRow item in rows)
            {
                List<string> lineValues = new List<string>
                {
                    { item.ItemArray[headers[headerNames[0]].IndexOnFile].ToString()},//BRANCH CODE
                    { item.ItemArray[headers[headerNames[1]].IndexOnFile].ToString() }, //AP NUMBER
                };

                validatedLineItems.Add(PSSBranchOfficersFileUploadValidator.ValidateExtractedPSSBranchOfficersLineItems(lineValues));
            }
            return validatedLineItems;
        }


        /// <summary>
        /// Validate excel headers
        /// </summary>
        /// <param name="header"></param>
        /// <returns>HeaderValidateObject</returns>
        private void ValidateHeaders(DataRow header, ref Dictionary<string, HeaderValidationModel> headers)
        {
            int counter = -1;
            foreach (object item in header.ItemArray)
            {
                if (item is DBNull) { break; }
                counter++;
                string sItem = ((string)item).Trim().ToLower();

                if (headerNames[0].Contains(sItem)) { headers[headerNames[0]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[1].Contains(sItem)) { headers[headerNames[1]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
            }
        }
    }
}
