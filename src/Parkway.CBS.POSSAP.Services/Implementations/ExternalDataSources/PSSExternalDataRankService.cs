using Hangfire;
using Newtonsoft.Json;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Scheduler.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.Implementations.ExternalDataSources.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using Parkway.CBS.RemoteClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.Services.Implementations.ExternalDataSources
{
    public class PSSExternalDataRankService : IPSSExternalDataSourceProcessor
    {
        private static readonly ILogger log = new Log4netLogger();

        public ICallLogForExternalSystemDAOManager callLogForExternalSystemDAOManager { get; set; }
        public IPSSExternalDataRankStagingDAOManager pssExternalDataRankStagingDAOManager { get; set; }
        public IUoW UoW { get; set; }

        /// <summary>
        /// This gets external ranks data for processing
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="dataSourceConfigurationSettingVM"></param>
        /// <returns>string</returns>
        [ProlongExpirationTime]
        public string ProcessExternalDataSource(string tenantName, PSSExternalDataSourceConfigurationSettingVM dataSourceConfigurationSettingVM)
        {
            try
            {
                log.Info($"About to start state external data source job processing for {dataSourceConfigurationSettingVM.ActionName} queued successfully. External data source config Id: {dataSourceConfigurationSettingVM.ExternalDataSourceConfigId}");

                var hrSystemBaseURL = ConfigurationManager.AppSettings["HRSystemBaseURL"];
                var hrSystemUsername = ConfigurationManager.AppSettings["HRSystemUsername"];
                var hrSystemKey = ConfigurationManager.AppSettings["HRSystemKey"];

                string[] requiredParameters = { hrSystemBaseURL, hrSystemUsername, hrSystemKey };

                if (requiredParameters.Any(x => string.IsNullOrEmpty(x)))
                {
                    //throw exception
                    throw new Exception("Required parameter(s) for HR system not found");
                }

                //Do work of calling HR system here

                string signature = $"{hrSystemUsername}{hrSystemKey}::";
                string encodedSignature = Util.SHA256ManagedHash(signature);
                string url = $"{hrSystemBaseURL}/ranks/{hrSystemUsername}/{encodedSignature}";

                IRemoteClient _remoteClient = new RemoteClient.RemoteClient();

                string response = _remoteClient.SendRequest(new RequestModel
                {
                    Headers = null,
                    Model = null,
                    URL = url
                }, HttpMethod.Get, new Dictionary<string, string>());

                // Process response
                var responseObject = JsonConvert.DeserializeObject<BaseHRResponseVM>(response);

                // Save then validate
                if (!responseObject.Error)
                {

                    SetUnitofWork(tenantName);
                    SetCallLogForExternalSystemDAOManager();
                    SetPSSExternalDataRankStagingDAOManager();

                    CallLogForExternalSystem callLogExternalSystemEntity = new CallLogForExternalSystem
                    {
                        URL = url,
                        CallIsSuccessful = true,
                        CallDescription = "Called the HR system to get the list of ranks.",
                        ExternalDataSourceConfigurationSetting = new ExternalDataSourceConfigurationSetting { Id = dataSourceConfigurationSettingVM.ExternalDataSourceConfigId },
                    };
                    try
                    {

                        UoW.BeginTransaction();

                        callLogForExternalSystemDAOManager.Add(callLogExternalSystemEntity);

                        pssExternalDataRankStagingDAOManager.Save(JsonConvert.DeserializeObject<RankResponseObject>(JsonConvert.SerializeObject(responseObject.ResponseObject)).ReportRecords, callLogExternalSystemEntity.Id);

                        UoW.Commit();

                        BackgroundJob.Enqueue(() => ValidateRankRecords(callLogExternalSystemEntity.Id, tenantName));

                    }
                    catch (Exception exception)
                    {
                        log.Error(exception.Message, exception);
                        UoW.Rollback();
                        throw;
                    }

                }
                else
                {
                    List<BaseHRErrorReponseVM> errorMessagesObject = JsonConvert.DeserializeObject<List<BaseHRErrorReponseVM>>(JsonConvert.SerializeObject(responseObject.ResponseObject));
                    string errorMessage = string.Join(" : ", errorMessagesObject.Select(p => $"{p.FieldName} {p.ErrorMessage}"));

                    Exception exception = new Exception($"Call to hr external system failed and returned error code : {responseObject.ErrorCode} {errorMessage}");
                    log.Error(responseObject.ErrorCode, exception);
                    throw exception;
                }

            }
            catch (Exception exception)
            {
                log.Error(exception.Message, exception);
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                pssExternalDataRankStagingDAOManager = null;
                callLogForExternalSystemDAOManager = null;
            }

            return "OK";
        }

        /// <summary>
        /// Sets an instance of Unit of work <see cref="UoW"/> using the <paramref name="tenantName"/>
        /// </summary>
        /// <param name="tenantName"></param>
        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSExternalDataJob");
            }
        }

        /// <summary>
        /// Sets an instance of <see cref="callLogForExternalSystemDAOManager"/> 
        /// </summary>
        private void SetCallLogForExternalSystemDAOManager()
        {
            if (callLogForExternalSystemDAOManager == null) { callLogForExternalSystemDAOManager = new CallLogForExternalSystemDAOManager(UoW); }
        }

        /// <summary>
        /// Sets an instance of <see cref="pssExternalDataRankStagingDAOManager"/> 
        /// </summary>
        private void SetPSSExternalDataRankStagingDAOManager()
        {
            if (pssExternalDataRankStagingDAOManager == null) { pssExternalDataRankStagingDAOManager = new PSSExternalDataRankStagingDAOManager(UoW); }
        }


        /// <summary>
        /// Iterates and validates rank name, code and external data rank id setting<see cref="PSSExternalDataRankStagingVM.HasError"/>
        /// to true and <see cref="PSSExternalDataRankStagingVM.ErrorMessage"/> with error message if invalid.
        /// Updates PSSExternalDataRankStaging 
        /// </summary>
        /// <param name="callLogExternalSystemEntityId"></param>
        /// <param name="tenantName"></param>
        public void ValidateRankRecords(long callLogExternalSystemEntityId, string tenantName)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetPSSExternalDataRankStagingDAOManager();

                IEnumerable<PSSExternalDataRankStagingVM> pssExternalDataRankRecords = pssExternalDataRankStagingDAOManager.GetRanks(callLogExternalSystemEntityId);

                if (pssExternalDataRankRecords.Count() > 0)
                {
                    foreach (var rank in pssExternalDataRankRecords)
                    {
                        if (string.IsNullOrEmpty(rank.Name) || rank.Name.Trim().Length == 0)
                        {
                            rank.HasError = true;
                            rank.ErrorMessage += "Name is not valid |";
                        }
                        if (string.IsNullOrEmpty(rank.Code) || rank.Code.Trim().Length == 0)
                        {
                            rank.HasError = true;
                            rank.ErrorMessage += "Code is not valid |";
                        }
                        if (string.IsNullOrEmpty(rank.ExternalDataRankId) || rank.ExternalDataRankId.Trim().Length == 0)
                        {
                            rank.HasError = true;
                            rank.ErrorMessage += "ExternalDataRankId is not valid |";
                        }
                        if (rank.HasError) { rank.ErrorMessage = rank.ErrorMessage.TrimEnd(new char[] { '|' }); }
                    }

                    if (pssExternalDataRankRecords.Any(x => x.HasError))
                    {

                        pssExternalDataRankStagingDAOManager.BuildPSSExternalDataRankStagingBulkUpdate(pssExternalDataRankRecords.Where(x => x.HasError), out string tempTableName, out string createTempTableQuery, out string updateTableQuery, out DataTable dataTable);

                        UoW.BeginTransaction();

                        pssExternalDataRankStagingDAOManager.UpdateErrorMessageAfterValidation(dataTable, tempTableName, createTempTableQuery, updateTableQuery);

                        UoW.Commit();
                    }

                    BackgroundJob.Enqueue(() => UpdatePoliceRankingTable(callLogExternalSystemEntityId, tenantName));
                }
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
                pssExternalDataRankStagingDAOManager = null;

            }
        }


        /// <summary>
        /// Synchronizes the police ranking table with records in the PSSExternalDataRankStaging table using the specified callLogExternalSystemEntityId
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        /// <param name="tenantName"></param>
        public void UpdatePoliceRankingTable(long callLogExternalSystemEntityId, string tenantName)
        {

            try
            {
                SetUnitofWork(tenantName);
                SetPSSExternalDataRankStagingDAOManager();

                UoW.BeginTransaction();

                pssExternalDataRankStagingDAOManager.UpdatePoliceRankingTable(callLogExternalSystemEntityId);

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
                pssExternalDataRankStagingDAOManager = null;
            }
        }
    }
}
