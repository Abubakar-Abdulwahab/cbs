using ExcelDataReader;
using Hangfire;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModels;
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
    public class GenerateRequestWithoutOfficersFileUpload : IGenerateRequestWithoutOfficersFileUpload
    {
        /// <summary>
        ///  BRANCH CODE - 0, 
        ///  NUMBER - 1,
        ///  COMMAND CODE - 2, 
        ///  COMMAND TYPE - 3, 
        ///  DAY TYPE - 4
        /// </summary>
        protected readonly string[] headerNames = { "BRANCH CODE".ToLower(), "NUMBER".ToLower(), "COMMAND CODE".ToLower(),
            "COMMAND TYPE".ToLower(), "DAY TYPE".ToLower() };

        private static readonly ILogger _log = new Log4netLogger();

        public IGenerateRequestWithoutOfficersUploadValidation _pssGenerateRequestWithoutOfficersFileUploadValidator { get; set; }

        public IGenerateRequestWithoutOfficersUploadBatchItemsDAOManager _generateRequestWithoutOfficersUploadBatchItemsDAOManager { get; set; }

        public IGenerateRequestWithoutOfficersUploadBatchDAOManager _generateRequestWithoutOfficersUploadBatchDAOManager { get; set; }

        public IUoW UoW { get; set; }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "GenerateRequestWithoutOfficersExcelFileUploadProcessingJob");
            }
        }

        private void SetGenerateRequestWithoutOfficersUploadBatchDAOManager()
        {
            if (_generateRequestWithoutOfficersUploadBatchDAOManager == null) { _generateRequestWithoutOfficersUploadBatchDAOManager = new GenerateRequestWithoutOfficersUploadBatchDAOManager(UoW); }
        }

        private void SetGenerateRequestWithoutOfficersUploadBatchItemsDAOManager()
        {
            if (_generateRequestWithoutOfficersUploadBatchItemsDAOManager == null) { _generateRequestWithoutOfficersUploadBatchItemsDAOManager = new GenerateRequestWithoutOfficersUploadBatchItemsDAOManager(UoW); }
        }

        private void SetGenerateRequestWithoutOfficersValidationInstance()
        {
            if (_pssGenerateRequestWithoutOfficersFileUploadValidator == null) { _pssGenerateRequestWithoutOfficersFileUploadValidator = new GenerateRequestWithoutOfficersUploadValidation(); }
        }

        /// <summary>
        /// Process GenerateRequestWithoutOfficers line items for file upload as a background job.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="tenantName"></param>
        public void ProcessGenerateRequestWithoutOfficersFileUpload(long batchId, string tenantName)
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
                _log.Error(string.Format("Error while queuing PSSBranchSubUsers batch excel file upload to Hangfire."));
                _log.Error(exception.Message, exception);
                throw;
            }

        }

        private void StartHangfireServer(string conStringNameKey)
        {
            //Get the connection string
            string dbConnectionString = HangFireScheduler.GetConnectionString(conStringNameKey);
            //Call the Hangfire storage
            GlobalConfiguration.Configuration.UseSqlServerStorage(dbConnectionString);
        }


        /// <summary>
        /// Commence PSSBranchSubUsers excel file processing
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="tenantName"></param>
        public void BeginProcessing(long batchId, string tenantName)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetGenerateRequestWithoutOfficersUploadBatchDAOManager();
                SetGenerateRequestWithoutOfficersUploadBatchItemsDAOManager();
                SetGenerateRequestWithoutOfficersValidationInstance();

                //Check if the batch has already been processed.
                _log.Info($"Check if the batch has already been processed for batch id {batchId}");
                var batchDetails = _generateRequestWithoutOfficersUploadBatchDAOManager.GetGenerateRequestWithoutOfficersUploadBatchStatusAndFilePath(batchId);
                if (batchDetails == null)
                {
                    throw new Exception($"Unable to get the processing status and filepath for batch with id {batchId}");
                }

                if (batchDetails.Status != GenerateRequestWithoutOfficersUploadStatus.BatchInitialized)
                { 
                    throw new Exception($"Status mismatch for GenerateRequestWithoutOfficers batch with id {batchId}. Batch status - {batchDetails.Status} Expected status - {GenerateRequestWithoutOfficersUploadStatus.BatchInitialized}"); 
                }

                //Get validated records from GenerateRequestWithoutOfficers schedule excel file and save
                _log.Info($"Get validated records from GenerateRequestWithoutOfficers schedule excel file and save for batch id {batchId}");
                GenerateRequestWithoutOfficersDataResponse generateRequestWithoutOfficersDataResponse = GetRecordsInScheduleFile(batchDetails.FilePath);

                UoW.BeginTransaction();
                if (generateRequestWithoutOfficersDataResponse.HeaderValidateObject.Error)
                {
                    _generateRequestWithoutOfficersUploadBatchDAOManager.UpdateGenerateRequestWithoutOfficersUploadBatchStatus(GenerateRequestWithoutOfficersUploadStatus.Fail, batchId, generateRequestWithoutOfficersDataResponse.HeaderValidateObject.ErrorMessage);
                    _log.Error($"Error while processing GetRecordsInScheduleFile: for batch {batchId}, {generateRequestWithoutOfficersDataResponse.HeaderValidateObject.ErrorMessage}");
                    UoW.Commit(); return;
                }

                // if no record is returned the batch gets updated with a failed status and the background job terminates.
                if (generateRequestWithoutOfficersDataResponse.GenerateRequestWithoutOfficersLineRecords.Count == 0)
                {
                    _generateRequestWithoutOfficersUploadBatchDAOManager.UpdateGenerateRequestWithoutOfficersUploadBatchStatus(GenerateRequestWithoutOfficersUploadStatus.Fail, batchId, "Empty sub user records");
                    _log.Error($"Error while processing GetRecordsInScheduleFile: for batch {batchId}, Empty sub user records");
                    UoW.Commit(); return;
                }

                _log.Info($"Saving GenerateRequestWithoutOfficers Line Items for batch id {batchId}");
                _generateRequestWithoutOfficersUploadBatchItemsDAOManager.SaveGenerateRequestWithoutOfficersLineItemsRecords(generateRequestWithoutOfficersDataResponse.GenerateRequestWithoutOfficersLineRecords, batchId);

                //validate against branch code
                _log.Info($"validate against branch code for batch id {batchId}");
                _generateRequestWithoutOfficersUploadBatchItemsDAOManager.ValidateBranchCodeIsSameAsCurrentBranchAndUpdateGenerateRequestWithoutOfficersItemErrorMessage(batchId, "Branch code is not same as the current branch.");

                //does validation by resolving the command codes that are valid on the system.
                _log.Info($"validation by resolving the command codes for command Ids that are valid on the system for batch id {batchId}");
                _generateRequestWithoutOfficersUploadBatchItemsDAOManager.ResolveCommandIdsForGenerateRequestsWithoutOfficersAlreadyCreated(batchId);

                //validate that duplicates of Command Code, Command Type and Day Type combined are not existing
                _log.Info($"validate that duplicates of Command Code, Command Type and Day Type combined are not existing for batch id {batchId}");
                _generateRequestWithoutOfficersUploadBatchItemsDAOManager.ValidateCommandCodeCommandTypeDayTypeCombinationIsNotDuplicateAndUpdateGenerateRequestWithoutOfficersItemErrorMessage(batchId, "Another request with same Command Code, Command Type and Day Type combination already exists on the document.");

                //updates the error message for items with command codes not on the system.
                _log.Info($"update the error message for items with command codes not on the system. for batch id {batchId}");
                _generateRequestWithoutOfficersUploadBatchItemsDAOManager.UpdateCommandIdResolutionErrorMessage(batchId, "Could not find Command.");

                //Update GenerateRequestWithoutOfficers batch status to reflect completion of records saved to table
                _log.Info($"Update GenerateRequestWithoutOfficers batch status to reflect completion of records saved to table for batch id {batchId}");
                _generateRequestWithoutOfficersUploadBatchDAOManager.UpdateGenerateRequestWithoutOfficersUploadBatchStatus(GenerateRequestWithoutOfficersUploadStatus.BatchValidated, batchId);
                UoW.Commit();
            }
            catch (Exception exception)
            {
                _log.Error("Exception occured while trying to process PSSBranchSubUsers schedule excel file records." + exception.Message, exception);
                UoW.Rollback();
            }
        }


        /// <summary>
        /// Validate the headers and read the file.
        /// <para>
        /// this method validates the headers of the file, if headers are ok, 
        /// it proceeds to read each row on the excel file
        /// </para>
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private GenerateRequestWithoutOfficersDataResponse GetRecordsInScheduleFile(string filePath)
        {

            DataSet result = new DataSet();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                { result = reader.AsDataSet(); }
            }
            var sheet1 = result.Tables[0];
            var rows = sheet1.Rows;

            Dictionary<string, HeaderValidationModel> headers = new Dictionary<string, HeaderValidationModel>
            {
                { headerNames[0], new HeaderValidationModel { } }, { headerNames[1], new HeaderValidationModel { } },
                { headerNames[2], new HeaderValidationModel { } }, { headerNames[3], new HeaderValidationModel { } },
                { headerNames[4], new HeaderValidationModel { } }
            };

            ValidateHeaders(rows[0], ref headers);
            var invalidsHeaders = headers.Where(k => k.Value.HeaderPresent == false);
            if (invalidsHeaders.Count() > 0)
            {
                var msg = invalidsHeaders.Select(x => x.Key + " header not found").ToArray();
                return new GenerateRequestWithoutOfficersDataResponse { HeaderValidateObject = new HeaderValidateObject { Error = true, ErrorMessage = string.Join("\n", msg) } };
            }

            rows.RemoveAt(0);

            List<GenerateRequestWithoutOfficersUploadItemVM> generateRequestWithoutOfficersUploadLineRecords = new List<GenerateRequestWithoutOfficersUploadItemVM> { };

            foreach (DataRow item in rows)
            {
                List<string> lineValues = new List<string>
                {
                    { item.ItemArray[headers[headerNames[0]].IndexOnFile].ToString()},//BRANCH CODE
                    { item.ItemArray[headers[headerNames[1]].IndexOnFile].ToString()}, //NUMBER
                    { item.ItemArray[headers[headerNames[2]].IndexOnFile].ToString()}, //COMMAND CODE
                    { item.ItemArray[headers[headerNames[3]].IndexOnFile].ToString()}, //COMMAND TYPE
                    { item.ItemArray[headers[headerNames[4]].IndexOnFile].ToString()}, //DAY TYPE
                };

                generateRequestWithoutOfficersUploadLineRecords.Add(_pssGenerateRequestWithoutOfficersFileUploadValidator.ValidateExtractedGenerateRequestWithoutOfficersLineItems(lineValues));
            }
            return new GenerateRequestWithoutOfficersDataResponse { GenerateRequestWithoutOfficersLineRecords = generateRequestWithoutOfficersUploadLineRecords, HeaderValidateObject = new HeaderValidateObject { } };
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
                if (headerNames[2].Contains(sItem)) { headers[headerNames[2]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[3].Contains(sItem)) { headers[headerNames[3]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[4].Contains(sItem)) { headers[headerNames[4]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
            }
        }

    }
}
