using Parkway.CBS.POSSAP.EGSRegularization.Implementations.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Configuration;
using System.Data;
using System.IO;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;
using ExcelDataReader;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations.Logger;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations.Logger.Contracts;

namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations
{
    public class PSSBranchSubUsersFileUpload : IPSSBranchSubUsersFileUpload
    {

        /// <summary>
        ///  BRANCH STATE CODE - 0, 
        ///  BRANCH LGA CODE - 1,
        ///  BRANCH NAME - 2, 
        ///  BRANCH ADDRESS - 3, 
        ///  SUBUSER NAME - 4,
        ///  SUBUSER EMAIL - 5,
        ///  SUBUSER PHONE NO - 6,
        /// </summary>
        protected readonly string[] headerNames = { "BRANCH STATE CODE".ToLower(), "BRANCH LGA CODE".ToLower(), "BRANCH NAME".ToLower(),
            "BRANCH ADDRESS".ToLower(), "SUBUSER NAME".ToLower(), "SUBUSER EMAIL".ToLower(), "SUBUSER PHONE NO".ToLower() };

        private static readonly ILogger log = new Log4netLogger();

        public IPSSBranchSubUsersFileUploadValidation _pssBranchSubUsersFileUploadValidator { get; set; }

        public IPSSBranchSubUsersBatchItemsDAOManager pssBranchSubUsersItemsDAO { get; set; }

        public IPSSBranchSubUsersBatchDAOManager pssBranchSubUsersDAO { get; set; }

        public IUoW UoW { get; set; }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSBranchSubUsersExcelFileUploadProcessingJob");
            }
        }

        private void SetPSSBranchSubUsersBatchDAOManager()
        {
            if (pssBranchSubUsersDAO == null) { pssBranchSubUsersDAO = new PSSBranchSubUsersBatchDAOManager(UoW); }
        }

        private void SetPSSBranchSubUsersBatchItemsDAOManager()
        {
            if (pssBranchSubUsersItemsDAO == null) { pssBranchSubUsersItemsDAO = new PSSBranchSubUsersBatchItemsDAOManager(UoW); }
        }

        private void SetPSSBranchSubUsersValidationInstance()
        {
            if (_pssBranchSubUsersFileUploadValidator == null) { _pssBranchSubUsersFileUploadValidator = new PSSBranchSubUsersFileUploadValidation(); }
        }

        /// <summary>
        /// Process PSSBranchSubUsers line items for file upload as a background job.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="tenantName"></param>
        public void ProcessPSSBranchSubUsersFileUpload(long batchId, string tenantName)
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
                log.Error(string.Format("Error while queuing PSSBranchSubUsers batch excel file upload to Hangfire."));
                log.Error(exception.Message, exception);
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
                SetPSSBranchSubUsersBatchDAOManager();
                SetPSSBranchSubUsersBatchItemsDAOManager();
                SetPSSBranchSubUsersValidationInstance();

                //Check if the PSSBranchSubUsers batch has already been processed.
                var batchDetails = pssBranchSubUsersDAO.GetPSSBranchSubUsersUploadBatchStatusAndFilePath(batchId);
                if (batchDetails == null)
                {
                    throw new Exception($"Unable to get the processing status and filepath for batch with id {batchId}");
                }
                if (batchDetails.Status != PSSBranchSubUserUploadStatus.BatchInitialized)
                { throw new Exception($"Status mismatch for PSSBranchSubUsers batch with id {batchId}. Batch status - {batchDetails.Status} Expected status - {PSSBranchSubUserUploadStatus.BatchInitialized}"); }
                //Get validated records from PSSBranchSubUsers schedule excel file and save
                PSSBranchSubUsersDataResponse pssBranchSubUsersDataResponse = GetRecordsInScheduleFile(batchId, batchDetails.FilePath);

                UoW.BeginTransaction();
                if (pssBranchSubUsersDataResponse.HeaderValidateObject.Error)
                {
                    pssBranchSubUsersDAO.UpdatePSSBranchSubUsersUploadBatchStatus(PSSBranchSubUserUploadStatus.Fail, batchId, pssBranchSubUsersDataResponse.HeaderValidateObject.ErrorMessage);
                    log.Error($"Error while processing GetRecordsInScheduleFile: for batch {batchId}, {pssBranchSubUsersDataResponse.HeaderValidateObject.ErrorMessage}");
                    UoW.Commit(); return;
                }

                // if no record is returned the batch gets updated with a failed status and the background job terminates.
                if (pssBranchSubUsersDataResponse.PSSBranchSubUsersLineRecords.Count == 0)
                { 
                    pssBranchSubUsersDAO.UpdatePSSBranchSubUsersUploadBatchStatus(PSSBranchSubUserUploadStatus.Fail, batchId, "Empty sub user records");
                    log.Error($"Error while processing GetRecordsInScheduleFile: for batch {batchId}, Empty sub user records");
                    UoW.Commit(); return; 
                }
                pssBranchSubUsersItemsDAO.SavePSSBranchSubUsersLineItemsRecords(pssBranchSubUsersDataResponse.PSSBranchSubUsersLineRecords, batchId);

                //validates if sub user email already exists and updates the item error message if so.
                pssBranchSubUsersItemsDAO.ValidateSubUserEmailIsNotExisitingAndUpdatePSSBranchSubUsersItemErrorMessage(batchId, "Another user with this email already exists.");
                //validate against uploaded duplicate sub user email addresses
                pssBranchSubUsersItemsDAO.ValidateSubUserEmailIsNotDuplicateAndUpdatePSSBranchSubUsersItemErrorMessage(batchId, "Another sub user with this email exists on the document.");
                //validate against uploaded phone numbers already used by existing CBSUser profiles
                pssBranchSubUsersItemsDAO.ValidatePhoneNumberIsNotExisitingAndUpdatePSSBranchSubUsersItemErrorMessage(batchId, "Another user with this phone number already exists.");
                //validate against uploaded duplicate phone numbers
                pssBranchSubUsersItemsDAO.ValidatePhoneNumberIsNotDuplicateAndUpdatePSSBranchSubUsersItemErrorMessage(batchId, "Another sub user with this phone number exists on the document.");
                //validate against uploaded branch name already used by existing TaxEntityProfileLocation profiles
                pssBranchSubUsersItemsDAO.ValidateBranchNameIsNotExisitingAndUpdatePSSBranchSubUsersItemErrorMessage(batchId, "A branch with this branch name already exists.");
                //validate against uploaded duplicate branch name
                pssBranchSubUsersItemsDAO.ValidateBranchNameIsNotDuplicateAndUpdatePSSBranchSubUsersItemErrorMessage(batchId, "Another branch with this branch name exists on the document.");
                //validate against uploaded branch address already used by existing TaxEntityProfileLocation profiles
                pssBranchSubUsersItemsDAO.ValidateAddressIsNotExisitingAndUpdatePSSBranchSubUsersItemErrorMessage(batchId, "Another user with this branch address already exists.");
                //validate against uploaded duplicate branch address
                pssBranchSubUsersItemsDAO.ValidateAddressIsNotDuplicateAndUpdatePSSBranchSubUsersItemErrorMessage(batchId, "Another sub user with this branch address exists on the document.");
                //does validation by resolving the lga and state codes that are valid on the system.
                pssBranchSubUsersItemsDAO.ResolveBranchLGAAndStateIdsAndValuesForCreatedBranches(batchId);
                //updates the error message for items with lga or state codes not on the system.
                pssBranchSubUsersItemsDAO.UpdateBranchLGAAndStateResolutionErrorMessage(batchId, "Could not find LGA or State for branch.");
                //Update PSSBranchSubUsers batch to reflect completion of records saved to table
                pssBranchSubUsersDAO.UpdatePSSBranchSubUsersUploadBatchStatus(PSSBranchSubUserUploadStatus.BatchValidated, batchId);
                UoW.Commit();
            }
            catch (Exception exception)
            {
                log.Error("Exception occured while trying to process PSSBranchSubUsers schedule excel file records." + exception.Message, exception);
                UoW.Rollback();
                throw;
            }
        }


        /// <summary>
        /// Validate the headers and read the file.
        /// <para>
        /// this method validates the headers of the file, if headers are ok, 
        /// it proceeds to read each row on the excel file
        /// </para>
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private PSSBranchSubUsersDataResponse GetRecordsInScheduleFile(long batchId, string filePath)
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
                { headerNames[4], new HeaderValidationModel { } }, { headerNames[5], new HeaderValidationModel { } },
                { headerNames[6], new HeaderValidationModel { } }
            };

            ValidateHeaders(rows[0], ref headers);
            var invalidsHeaders = headers.Where(k => k.Value.HeaderPresent == false);
            if (invalidsHeaders.Count() > 0)
            {
                var msg = invalidsHeaders.Select(x => x.Key + " header not found").ToArray();
                return new PSSBranchSubUsersDataResponse { HeaderValidateObject = new HeaderValidateObject { Error = true, ErrorMessage = string.Join("\n", msg) } };
            }

            rows.RemoveAt(0);

            List<PSSBranchSubUsersItemVM> pssBranchSubUserLineItems = new List<PSSBranchSubUsersItemVM> { };
           
            foreach (DataRow item in rows)
            {
                List<string> lineValues = new List<string>
                {
                    { item.ItemArray[headers[headerNames[0]].IndexOnFile].ToString()},//BRANCH STATE CODE
                    { item.ItemArray[headers[headerNames[1]].IndexOnFile].ToString() }, //BRANCH LGA CODE
                    { item.ItemArray[headers[headerNames[2]].IndexOnFile].ToString() }, //BRANCH NAME
                    { item.ItemArray[headers[headerNames[3]].IndexOnFile].ToString() }, //BRANCH ADDRESS
                    { item.ItemArray[headers[headerNames[4]].IndexOnFile].ToString()}, //SUBUSER NAME
                    { item.ItemArray[headers[headerNames[5]].IndexOnFile].ToString()}, //SUBUSER EMAIL
                    { item.ItemArray[headers[headerNames[6]].IndexOnFile].ToString()}, //SUBUSER PHONE NO
                };

                pssBranchSubUserLineItems.Add(_pssBranchSubUsersFileUploadValidator.ValidateExtractedPSSBranchSubUsersLineItems(lineValues));
            }
            return new PSSBranchSubUsersDataResponse { PSSBranchSubUsersLineRecords = pssBranchSubUserLineItems, HeaderValidateObject = new HeaderValidateObject { } };
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
                if (headerNames[5].Contains(sItem)) { headers[headerNames[5]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[6].Contains(sItem)) { headers[headerNames[6]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
            }
        }
    }
}
