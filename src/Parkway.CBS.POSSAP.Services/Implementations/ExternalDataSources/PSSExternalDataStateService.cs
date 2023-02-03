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

namespace Parkway.CBS.POSSAP.Services.Implementations.ExternalDataSources
{
    public class PSSExternalDataStateService : IPSSExternalDataSourceProcessor
    {
        private static readonly ILogger log = new Log4netLogger();

        public ICallLogForExternalSystemDAOManager callLogForExternalSystemDAOManager { get; set; }
        public IPSSExternalDataStateStagingDAOManager pssExternalDataStateStagingDAOManager { get; set; }
        public IUoW UoW { get; set; }

        /// <summary>
        /// This gets external state data for processing
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
                string url = $"{hrSystemBaseURL}/states/{hrSystemUsername}/{encodedSignature}";

                IRemoteClient _remoteClient = new RemoteClient.RemoteClient();

                string response = _remoteClient.SendRequest(new RequestModel
                {
                    Headers = null,
                    Model = null,
                    URL = url
                }, HttpMethod.Get, new Dictionary<string, string>());

                // Process response
                var responseObject = JsonConvert.DeserializeObject<GetStateResponseVM>(response);

                // Save then validate
                if (!responseObject.Error)
                {

                    SetUnitofWork(tenantName);
                    SetCallLogForExternalSystemDAOManager();
                    SetPSSExternalDataStateStagingDAOManager();

                    CallLogForExternalSystem callLogExternalSystemEntity = new CallLogForExternalSystem
                    {
                        URL = url,
                        CallIsSuccessful = true,
                        CallDescription = "Called the HR system to get the list of states.",
                        ExternalDataSourceConfigurationSetting = new ExternalDataSourceConfigurationSetting { Id = dataSourceConfigurationSettingVM.ExternalDataSourceConfigId },
                    };
                    try
                    {

                        UoW.BeginTransaction();

                        callLogForExternalSystemDAOManager.Add(callLogExternalSystemEntity);

                        pssExternalDataStateStagingDAOManager.Save(JsonConvert.DeserializeObject<StateResponseObject>(JsonConvert.SerializeObject(responseObject.ResponseObject)).ReportRecords, callLogExternalSystemEntity.Id);

                        UoW.Commit();

                        BackgroundJob.Enqueue(() => ValidateStateNameAndCode(callLogExternalSystemEntity.Id, tenantName));


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
                    List<GetStateErrorResponse> errorMessagesObject = JsonConvert.DeserializeObject<List<GetStateErrorResponse>>(JsonConvert.SerializeObject(responseObject.ResponseObject));
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
                pssExternalDataStateStagingDAOManager = null;
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
        /// Sets an instance of <see cref="pssExternalDataStateStagingDAOManager"/> 
        /// </summary>
        private void SetPSSExternalDataStateStagingDAOManager()
        {
            if (pssExternalDataStateStagingDAOManager == null) { pssExternalDataStateStagingDAOManager = new PSSExternalDataStateStagingDAOManager(UoW); }
        }

        /// <summary>
        /// Iterates and validates state name and code, sets <see cref="PSSExternalDataStateStagingVM.HasError"/> 
        /// to true and <see cref="PSSExternalDataStateStagingVM.ErrorMessage"/> with error message if invalid.
        /// Updates PSSExternalDataStateStaging
        /// </summary>
        /// <param name="callLogExternalSystemEntityId"></param>
        public void ValidateStateNameAndCode(long callLogExternalSystemEntityId, string tenantName)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetPSSExternalDataStateStagingDAOManager();

                IEnumerable<PSSExternalDataStateStagingVM> pssExternalDataStateStagings = pssExternalDataStateStagingDAOManager.GetListOfState(callLogExternalSystemEntityId);

                if (pssExternalDataStateStagings.Count() > 0)
                {
                    foreach (var state in pssExternalDataStateStagings)
                    {
                        if (string.IsNullOrEmpty(state.Name))
                        {
                            state.HasError = true;
                            state.ErrorMessage += "Name is not valid |";
                        }
                        if (string.IsNullOrEmpty(state.Code))
                        {
                            state.HasError = true;
                            state.ErrorMessage += "Code is not valid |";
                        }
                    }

                    if (pssExternalDataStateStagings.Any(x => x.HasError))
                    {

                        pssExternalDataStateStagingDAOManager.BuildPSSExternalDataStateStagingBulkUpdate(pssExternalDataStateStagings.Where(x => x.HasError), out string tempTableName, out string createTempTableQuery, out string updateTableQuery, out DataTable dataTable);

                        UoW.BeginTransaction();

                        pssExternalDataStateStagingDAOManager.UpdateErrorMessageAfterValidation(dataTable, tempTableName, createTempTableQuery, updateTableQuery);

                        UoW.Commit();
                    }

                    BackgroundJob.Enqueue(() => MoveMatchingStateNameToMain(callLogExternalSystemEntityId, tenantName));


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
                pssExternalDataStateStagingDAOManager = null;

            }

        }

        /// <summary>
        /// Moves state records to <see cref="PSSStateModelExternalDataState"/> when <see cref="PSSExternalDataStateStaging.Name"/> equals <see cref="Core.Models.StateModel.Name"/>
        /// using <paramref name="callLogExternalSystemEntityId"/>
        /// </summary>
        /// <param name="callLogExternalSystemEntityId"></param>
        /// <param name="tenantName"></param>
        public void MoveMatchingStateNameToMain(long callLogExternalSystemEntityId, string tenantName)
        {

            try
            {
                SetUnitofWork(tenantName);
                SetPSSExternalDataStateStagingDAOManager();

                UoW.BeginTransaction();

                pssExternalDataStateStagingDAOManager.MoveMatchingStateNameToMain(callLogExternalSystemEntityId);

                UoW.Commit();

                BackgroundJob.Enqueue(() => MoveStateRecordsThatDonotExistToStateModel(callLogExternalSystemEntityId, tenantName));
            }
            catch (Exception exception)
            {
                log.Error(exception.Message, exception);
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                pssExternalDataStateStagingDAOManager = null;
            }

        }

        /// <summary>
        /// Moves state records that do not exist in <see cref="Core.Models.StateModel"/> but exists in <see cref="PSSExternalDataStateStaging.Name"/> 
        /// using <see cref="PSSExternalDataStateStaging.Name"/> is not equals <see cref="Core.Models.StateModel.Name"/>
        /// and <paramref name="callLogForExternalSystemId"/>
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>

        public void MoveStateRecordsThatDonotExistToStateModel(long callLogExternalSystemEntityId, string tenantName)
        {

            try
            {
                SetUnitofWork(tenantName);
                SetPSSExternalDataStateStagingDAOManager();

                UoW.BeginTransaction();

                pssExternalDataStateStagingDAOManager.MoveStateRecordsThatDonotExistToStateModel(callLogExternalSystemEntityId);

                UoW.Commit();

                BackgroundJob.Enqueue(() => SetStateRecordsToInactiveInStateModelIfNotExistInStaging(tenantName, callLogExternalSystemEntityId));
            }
            catch (Exception exception)
            {
                log.Error(exception.Message, exception);
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                pssExternalDataStateStagingDAOManager = null;

            }

        }

        /// <summary>
        /// Update state records in <see cref="Core.Models.StateModel"/>  to inactive that do not exists in <see cref="PSSExternalDataStateStaging"/> 
        /// using <see cref="PSSExternalDataStateStaging.Name"/> is not equals <see cref="Core.Models.StateModel.Name"/>
        /// </summary>
        public void SetStateRecordsToInactiveInStateModelIfNotExistInStaging(string tenantName, long callLogExternalSystemEntityId)
        {

            try
            {
                SetUnitofWork(tenantName);
                SetPSSExternalDataStateStagingDAOManager();

                UoW.BeginTransaction();

                pssExternalDataStateStagingDAOManager.SetStateRecordsToInactiveInStateModelIfNotExistInStaging(callLogExternalSystemEntityId);

                UoW.Commit();

            }
            catch (Exception exception)
            {
                log.Error(exception.Message, exception);
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                pssExternalDataStateStagingDAOManager = null;

            }

        }
    }
}
