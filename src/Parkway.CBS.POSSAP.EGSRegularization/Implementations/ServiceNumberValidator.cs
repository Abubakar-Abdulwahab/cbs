using Hangfire;
using Newtonsoft.Json;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.ViewModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModels;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations.Contracts;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using Parkway.CBS.RemoteClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;

namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations
{
    public class ServiceNumberValidator : IServiceNumberValidator
    {
        private static readonly ILogger _log = new Log4netLogger();

        public IPSSBranchOfficersBatchDAOManager PSSBranchOfficersBatchDAOManager { get; set; }
        public IPSSBranchOfficersBatchItemsDAOManager PSSBranchOfficersBatchItemsDAOManager { get; set; }
        public ICommandDAOManager CommandDAOManager { get; set; }
        public IPoliceRankingDAOManager PoliceRankingDAOManager { get; set; }
        public IPolicerOfficerLogDAOManager PolicerOfficerLogDAOManager { get; set; }
        public IUoW UoW { get; set; }


        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSBranchOfficersExcelFileUploadProcessingJob");
            }
        }

        private void SetCommandDAOManager()
        {
            if (CommandDAOManager == null) { CommandDAOManager = new CommandDAOManager(UoW); }
        }

        private void SetPoliceRankingDAOManager()
        {
            if (PoliceRankingDAOManager == null) { PoliceRankingDAOManager = new PoliceRankingDAOManager(UoW); }
        }

        private void SetPolicerOfficerLogDAOManager()
        {
            if (PolicerOfficerLogDAOManager == null) { PolicerOfficerLogDAOManager = new PolicerOfficerLogDAOManager(UoW); }
        }

        private void SetPSSBranchOfficersBatchDAOManager()
        {
            if (PSSBranchOfficersBatchDAOManager == null) { PSSBranchOfficersBatchDAOManager = new PSSBranchOfficersBatchDAOManager(UoW); }
        }

        private void SetPSSBranchOfficersBatchItemsDAOManager()
        {
            if (PSSBranchOfficersBatchItemsDAOManager == null) { PSSBranchOfficersBatchItemsDAOManager = new PSSBranchOfficersBatchItemsDAOManager(UoW); }
        }

        private void StartHangfireServer(string conStringNameKey)
        {
            string dbConnectionString = HangFireScheduler.GetConnectionString(conStringNameKey);

            GlobalConfiguration.Configuration.UseSqlServerStorage(dbConnectionString);
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
                _log.Error("Error while queuing PSSBranchOfficers batch excel file upload to Hangfire.");
                _log.Error(exception.Message, exception);
                throw;
            }

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

                SetPSSBranchOfficersBatchDAOManager();
                SetPSSBranchOfficersBatchItemsDAOManager();
                SetCommandDAOManager();
                SetPolicerOfficerLogDAOManager();
                SetPoliceRankingDAOManager();

                //Check if the PSSBranchOfficers batch has already been processed.
                var batchDetails = PSSBranchOfficersBatchDAOManager.GetPSSBranchOfficersUploadBatchStatusAndFilePath(batchId);

                if (batchDetails == null)
                {
                    throw new InvalidOperationException($"Unable to get the processing status for batch with id {batchId}");
                }

                if (batchDetails.Status != PSSBranchOfficersUploadStatus.BatchValidation)
                {
                    throw new InvalidOperationException($"Status mismatch for PSSBranchOfficers batch with id {batchId}. Batch status - {batchDetails.Status} Expected status - {PSSBranchOfficersUploadStatus.BatchValidation}");
                }


                //validate against BRANCH CODE
                PSSBranchOfficersBatchItemsDAOManager.ValidateBranchCodeIsSameAsCurrentBranchAndUpdatePSSBranchOfficersItemErrorMessage(batchId, "Branch code is not same as the current branch.");

                //validate against APNumber on active deployment
                PSSBranchOfficersBatchItemsDAOManager.ValidateAPNumberIsNotOnActiveDeploymentndUpdatePSSBranchOfficersItemErrorMessage(batchId, "Another officer with this APNumber is on active deployment.");

                //validate against duplicate APNumbers
                PSSBranchOfficersBatchItemsDAOManager.ValidateAPNumberIsNotDuplicateAndUpdatePSSBranchOfficersItemErrorMessage(batchId, "Duplicate APNumber exists on the uploaded document.");

                List<PSSBranchOfficersItemVM> lineItems = PSSBranchOfficersBatchItemsDAOManager.GetValidBranchOfficersByBatchId(batchId).ToList();

                if (lineItems.Count == 0) 
                {
                    //if no records passed the sql scripts APNumber validations so far
                    UoW.BeginTransaction();
                    PSSBranchOfficersBatchDAOManager.UpdatePSSBranchOfficersUploadBatchStatus(PSSBranchOfficersUploadStatus.BatchValidated, batchId);
                    UoW.Commit(); 
                    return;
                }

                List<APNumberValidationVM> validatedAPNumbers = new List<APNumberValidationVM>();

                _log.Info($"Validating AP Numbers for batch id {batchId}");
                ValidateServiceNumbers(lineItems, ref validatedAPNumbers);
                _log.Info($"AP Number validation completed for batch id {batchId}");

                _log.Info($"Building PSSBranchOfficersUploadBatchItemsStaging temp table for bulk update for batch id {batchId}");
                PSSBranchOfficersBatchItemsDAOManager.BuildPSSBranchOfficersUploadBatchItemsStagingBulkUpdate(validatedAPNumbers, batchId, out string tempTableName, out string createTempTableQuery, out string updateTableQuery, out System.Data.DataTable dataTable);
                _log.Info($"Completed PSSBranchOfficersUploadBatchItemsStaging temp table creation for batch id {batchId}");

                
                UoW.BeginTransaction();

                _log.Info($"Saving collection of Police Officer logs for batch id {batchId}");
                PolicerOfficerLogDAOManager.SaveBundle(validatedAPNumbers.Where(x => !x.HasError).Select(x => x.PoliceOfficerLogModel));
                _log.Info($"Completed saving collection of Police Officer logs for batch id {batchId}");

                _log.Info($"Commencing bulk update of PSSBranchOfficersUploadBatchItemsStaging for batch id {batchId}");
                PSSBranchOfficersBatchItemsDAOManager.UpdateRecordsAfterValidation(dataTable, tempTableName, createTempTableQuery, updateTableQuery);
                _log.Info($"Bulk update completed successfully for batch id {batchId}");

                _log.Info($"Updating batch status to validated for batch id {batchId}");
                //Update enumeration batch to reflect completion of records saved to table
                PSSBranchOfficersBatchDAOManager.UpdatePSSBranchOfficersUploadBatchStatus(PSSBranchOfficersUploadStatus.BatchValidated, batchId);
                UoW.Commit();
                _log.Info($"Batch status updated successfully for batch id {batchId}");
            }
            catch (Exception exception)
            {
                _log.Error("Exception occured while trying to process PSSBranchOfficers schedule excel file records. " + exception.Message, exception);
                UoW.Rollback();
                throw;
            }
        }


        /// <summary>
        /// Get police officer
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private APNumberValidationVM GetPoliceOfficer(PersonnelRequestModel requestModel)
        {
            APNumberValidationVM APNumberValidationResponse = new APNumberValidationVM { HasError = false };
            try
            {
                var hrSystemBaseURL = ConfigurationManager.AppSettings["HRSystemBaseURL"];
                var hrSystemUsername = ConfigurationManager.AppSettings["HRSystemUsername"];
                var hrSystemKey = ConfigurationManager.AppSettings["HRSystemKey"];

                string[] requiredParameters = { hrSystemBaseURL, hrSystemUsername, hrSystemKey };

                if (requiredParameters.Any(x => string.IsNullOrEmpty(x)))
                {
                    //throw exception
                    throw new Exception("Required parameter(s) for HR system not found");
                }

                var body = BuildRequestModel(requestModel);
                var sb = new System.Text.StringBuilder($"{hrSystemUsername}{hrSystemKey}:");
                foreach (var item in body)
                {
                    sb.Append($":{item.Value}");
                }

                string signature = sb.ToString();
                string encodedSignature = Util.SHA256ManagedHash(signature);
                string url = $"{hrSystemBaseURL}/personnel/{hrSystemUsername}/{encodedSignature}";


                IRemoteClient _remoteClient = new RemoteClient.RemoteClient();
                string response = _remoteClient.SendRequest(new RequestModel
                {
                    Headers = null,
                    Model = body,
                    URL = url
                }, HttpMethod.Post, new Dictionary<string, string>(), isFormData: true);

                var requestResponse = JsonConvert.DeserializeObject<RootPersonnelResponse>(response);

                if (!requestResponse.Error)
                {
                    PersonnelResponseModel personnelRecords = JsonConvert.DeserializeObject<PersonnelResponseModel>(JsonConvert.SerializeObject(requestResponse.ResponseObject));

                    PersonnelReportRecord personnelReportRecord = personnelRecords.ReportRecords.FirstOrDefault();

                    if(personnelReportRecord == null)
                    {
                        APNumberValidationResponse.HasError = true;
                        APNumberValidationResponse.ErrorMessage = $"No records found for AP number";
                        return APNumberValidationResponse;
                    }

                    string commandCode = string.Format("{0}-{1}-{2}-{3}", personnelReportRecord.CommandLevelCode, personnelReportRecord.CommandCode, personnelReportRecord.SubCommandCode, personnelReportRecord.SubSubCommandCode);

                    commandCode = commandCode.Replace("-0", "");

                    var commandVM = CommandDAOManager.GetCommandWithCode(commandCode);
                    var rank = PoliceRankingDAOManager.GetPoliceRank(personnelReportRecord.RankCode);

                    if (commandVM == null)
                    {
                        _log.Error($"Exception occured while trying to process PSSBranchOfficers for APNUMBER {requestModel.ServiceNumber}, Command for command code {commandCode} was not found");
                        APNumberValidationResponse.HasError = true;
                        APNumberValidationResponse.ErrorMessage = $"Command was not found";
                        return APNumberValidationResponse;
                    }

                    if (rank == null)
                    {
                        _log.Error($"Exception occured while trying to process PSSBranchOfficers for APNUMBER {requestModel.ServiceNumber}, Rank for rank code {personnelReportRecord.RankCode} was not found");
                        APNumberValidationResponse.HasError = true;
                        APNumberValidationResponse.ErrorMessage = $"Rank was not found";
                        return APNumberValidationResponse;
                    }

                    string validationResponse = DoBasicValidation(personnelReportRecord);
                    if (!string.IsNullOrEmpty(validationResponse))
                    {
                        _log.Error($"{requestModel.ServiceNumber} validation response is {validationResponse}");
                        APNumberValidationResponse.HasError = true;
                        APNumberValidationResponse.ErrorMessage = validationResponse;
                        return APNumberValidationResponse;
                    }

                    personnelReportRecord.CommandCode = commandCode;
                    personnelReportRecord.CommandName = commandVM.Name;
                    personnelReportRecord.RankId = rank.Id.ToString();
                    personnelReportRecord.CommandLevelCode = commandVM.Id.ToString();
                    personnelReportRecord.RankName = rank.RankName;
                    personnelReportRecord.PhoneNumber = personnelReportRecord.PhoneNumber.Split(new char[] { ',', '/' })[0];

                    APNumberValidationResponse.PoliceOfficerLogModel = new PolicerOfficerLog
                    {
                        Name = string.Format("{0} {1}", personnelReportRecord.FirstName, personnelReportRecord.Surname).ToUpper(),
                        PhoneNumber = personnelReportRecord.PhoneNumber.Split(new char[] { ',', '/' })[0],
                        Rank = new PoliceRanking { Id = rank.Id },
                        IdentificationNumber = personnelReportRecord.ServiceNumber.ToUpper(),
                        IPPISNumber = personnelReportRecord.IPPSNumber,
                        Command = new Command { Id = commandVM.Id },
                        Gender = personnelReportRecord.Gender,
                        AccountNumber = personnelReportRecord.AccountNumber,
                        BankCode = personnelReportRecord.BankCode
                    };

                    APNumberValidationResponse.PersonnelReportRecord = personnelReportRecord;
                    return APNumberValidationResponse;
                }
                APNumberValidationResponse.HasError = requestResponse.Error;
                APNumberValidationResponse.ErrorMessage = $"No records found for AP number";
                return APNumberValidationResponse;

            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                APNumberValidationResponse.HasError = true;
                APNumberValidationResponse.ErrorMessage = "Unable to process AP number";
            }
            return APNumberValidationResponse;
        }


        /// <summary>
        /// Build key value pair for HR form-data content type
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns>List<KeyValuePair<string, string>></returns>
        private static List<KeyValuePair<string, string>> BuildRequestModel(PersonnelRequestModel requestModel)
        {
            var body = new List<KeyValuePair<string, string>>();
            Type t = requestModel.GetType();
            foreach (System.Reflection.PropertyInfo pi in t.GetProperties())
            {
                string value = pi.GetValue(requestModel, null)?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    body.Add(new KeyValuePair<string, string>(pi.Name, value));
                }
            }

            return body;
        }


        /// <summary>
        /// Validates service numbers
        /// </summary>
        /// <param name="pssBranchOfficersItems"></param>
        private void ValidateServiceNumbers(List<PSSBranchOfficersItemVM> pssBranchOfficersItems, ref List<APNumberValidationVM> validatedAPNumbers)
        {
            foreach (var item in pssBranchOfficersItems)
            {
                APNumberValidationVM result = GetPoliceOfficer(new PersonnelRequestModel { ServiceNumber = item.APNumber, Page = "1", PageSize = "1"});
                result.PSSBranchSubUsersUploadBatchItemsStagingId = item.Id;
                validatedAPNumbers.Add(result);
            }
        }

        /// <summary>
        /// Do basic validation for some of the personnel fields
        /// </summary>
        /// <param name="pssBranchOfficersItems"></param>
        private string DoBasicValidation(PersonnelReportRecord personnelReportRecord)
        {
            string errorMessage = string.Empty;
            string message = string.Empty;
            ValidateStringLength(personnelReportRecord.AccountNumber, "Account Number", false, ref errorMessage, 10, 10);
            if (!string.IsNullOrEmpty(errorMessage)) { message += errorMessage + "\n| "; }

            ValidateStringLength(personnelReportRecord.BankCode, "Bank Code", false, ref errorMessage, 3, 15);
            if (!string.IsNullOrEmpty(errorMessage)) { message +=errorMessage + "\n| "; }

            string phoneNumber = personnelReportRecord.PhoneNumber.Split(new char[] { ',', '/' })[0];
            ValidateStringLength(phoneNumber, "Phone Number", false, ref errorMessage, 11, 14);
            if (!string.IsNullOrEmpty(errorMessage)) { message += errorMessage + "\n| "; }
            if (string.IsNullOrEmpty(errorMessage)) { if (!DoPhoneNumberValidation(phoneNumber)) { message += $"Phone Number {phoneNumber} is not valid.\n| "; } }

            if (!string.IsNullOrEmpty(message))
            {
                message = message.Trim().TrimEnd('|');
            }
            return message;
        }


        /// <summary>
        /// validate the length of a stringValue
        /// </summary>
        /// <param name="stringValue">string value</param>
        /// <param name="headerValue">excel header or label value </param>
        /// <param name="allowEmpty">allow empty value</param>
        /// <param name="errorMsg">error message</param>
        /// <param name="minValidLength">minimum string value length</param>
        /// <param name="maxValidLength">maximum string value length</param>
        /// <returns>validated string</returns>
        private string ValidateStringLength(string stringValue, string headerValue, bool allowEmpty, ref string errorMsg, int minValidLength = 0, int maxValidLength = 0)
        {
            errorMsg = string.Empty;
            if (string.IsNullOrEmpty(stringValue) || string.IsNullOrWhiteSpace(stringValue))
            {
                if (allowEmpty) { return stringValue; }
                errorMsg = string.Format("{0} is empty.", headerValue);
                return stringValue;
            }
            stringValue = stringValue.Trim();

            if (minValidLength > 0)
            {
                if (stringValue.Length < minValidLength)
                {
                    errorMsg = string.Format("{0} value {2} is too small. Enter a valid {0}. Minimum number of characters is {1}. ", headerValue, minValidLength, stringValue);
                    return stringValue;
                }
            }

            if (maxValidLength > 0)
            {
                if (stringValue.Length > maxValidLength)
                {
                    errorMsg = string.Format("{0} value {2} is too long. Enter a valid {0}. Maximum number of characters is {1}.", headerValue, maxValidLength, stringValue);
                    return stringValue;
                }
            }
            return stringValue;
        }

        /// <summary>
        /// Check if this phone number is valid
        /// </summary>
        /// <param name="sPhoneNumber"></param>
        /// <returns>bool</returns>
        private bool DoPhoneNumberValidation(string sPhoneNumber)
        {
            if (string.IsNullOrEmpty(sPhoneNumber))
            { return false; }
            sPhoneNumber = Util.NormalizePhoneNumber(sPhoneNumber);
            //validate phone number
            bool isANumber = long.TryParse(sPhoneNumber, out _);
            if (!isANumber || (sPhoneNumber.Length != 13 && sPhoneNumber.Length != 11))
            { return false; }
            return true;
        }

    }
}
