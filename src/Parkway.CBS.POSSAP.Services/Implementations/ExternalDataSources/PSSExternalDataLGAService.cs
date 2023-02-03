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
    public class PSSExternalDataLGAService : IPSSExternalDataSourceProcessor
    {
        private static readonly ILogger log = new Log4netLogger();

        public ICallLogForExternalSystemDAOManager callLogForExternalSystemDAOManager { get; set; }
        public IPSSExternalDataLGAStagingDAOManager pssExternalDataLGAStagingDAOManager { get; set; }
        public IPSSStateModelExternalDataStateDAOManager pssStateModelExternalDataStateDAOManager { get; set; }
        public IUoW UoW { get; set; }

        /// <summary>
        /// Moves LGA records that do not exist in <see cref="Core.Models.LGA"/> but exists in <see cref="PSSExternalDataLGAStaging.Name"/> 
        /// using <see cref="PSSExternalDataLGAStaging.Name"/> is not equals <see cref="Models.LGAModel.Name"/>
        /// and <paramref name="callLogExternalSystemEntityId"/>
        /// </summary>
        /// <param name="callLogExternalSystemEntityId"></param>
        /// <param name="tenantName"></param>
        public void MoveLGARecordsThatDonotExistToLGAModel(long callLogExternalSystemEntityId, string tenantName)
        {

            try
            {
                SetUnitofWork(tenantName);
                SetPSSExternalDataLGAStagingDAOManager();
                SetPSSStateModelExternalDataStateDAOManager();

                long? lastStateCallLogForExternalSystemId = pssStateModelExternalDataStateDAOManager.GetLastCallLogForExternalSystemId();

                if (lastStateCallLogForExternalSystemId.HasValue)
                {
                    UoW.BeginTransaction();

                    pssExternalDataLGAStagingDAOManager.MoveLGARecordsThatDonotExistToLGAModel(callLogExternalSystemEntityId, lastStateCallLogForExternalSystemId.Value);

                    UoW.Commit();

                    BackgroundJob.Enqueue(() => SetLGARecordsToInactiveInLGAIfNotExistInStaging(tenantName, callLogExternalSystemEntityId));

                }
                else
                {
                    throw new Exception($"{nameof(lastStateCallLogForExternalSystemId)} was not found");
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
                pssExternalDataLGAStagingDAOManager = null;
                pssStateModelExternalDataStateDAOManager = null;

            }

        }

        /// <summary>
        /// Moves LGA records to <see cref="PSSLGAModelExternalDataLGA"/> when <see cref="PSSExternalDataLGAStaging.Name"/> equals <see cref="Core.Models.LGAModel.Name"/>
        /// using <paramref name="callLogExternalSystemEntityId"/>
        /// </summary>
        /// <param name="callLogExternalSystemEntityId"></param>
        /// <param name="tenantName"></param>
        public void MoveMatchingLGANameToMain(long callLogExternalSystemEntityId, string tenantName)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetPSSExternalDataLGAStagingDAOManager();

                UoW.BeginTransaction();


                pssExternalDataLGAStagingDAOManager.MoveMatchingLGANameToMain(callLogExternalSystemEntityId);

                UoW.Commit();

                BackgroundJob.Enqueue(() => MoveLGARecordsThatDonotExistToLGAModel(callLogExternalSystemEntityId, tenantName));

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
                pssExternalDataLGAStagingDAOManager = null;
                pssStateModelExternalDataStateDAOManager = null;
            }

        }

        /// <summary>
        /// This gets external lga data for processing
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="dataSourceConfigurationSettingVM"></param>
        /// <returns>string</returns>
        [ProlongExpirationTime]
        public string ProcessExternalDataSource(string tenantName, PSSExternalDataSourceConfigurationSettingVM dataSourceConfigurationSettingVM)
        {
            try
            {
                log.Info($"About to start lga external data source job processing for {dataSourceConfigurationSettingVM.ActionName} queued successfully. External data source config Id: {dataSourceConfigurationSettingVM.ExternalDataSourceConfigId}");

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
                string url = $"{hrSystemBaseURL}/lgas/{hrSystemUsername}/{encodedSignature}";

                IRemoteClient _remoteClient = new RemoteClient.RemoteClient();

                string response = _remoteClient.SendRequest(new RequestModel
                {
                    Headers = null,
                    Model = null,
                    URL = url
                }, HttpMethod.Get, new Dictionary<string, string>());

                // Process response
                var responseObject = JsonConvert.DeserializeObject<GetLGAResponseVM>(response);

                // Save then validate
                if (!responseObject.Error)
                {

                    SetUnitofWork(tenantName);
                    SetCallLogForExternalSystemDAOManager();
                    SetPSSExternalDataLGAStagingDAOManager();

                    CallLogForExternalSystem callLogExternalSystemEntity = new CallLogForExternalSystem
                    {
                        URL = url,
                        CallIsSuccessful = true,
                        CallDescription = "Called the HR system to get the list of LGAs.",
                        ExternalDataSourceConfigurationSetting = new ExternalDataSourceConfigurationSetting { Id = dataSourceConfigurationSettingVM.ExternalDataSourceConfigId },
                    };
                    try
                    {

                        UoW.BeginTransaction();

                        callLogForExternalSystemDAOManager.Add(callLogExternalSystemEntity);

                        pssExternalDataLGAStagingDAOManager.Save(JsonConvert.DeserializeObject<LGAResponseObject>(JsonConvert.SerializeObject(responseObject.ResponseObject)).ReportRecords, callLogExternalSystemEntity.Id);

                        UoW.Commit();

                        BackgroundJob.Enqueue(() => ValidateLGANameAndCode(callLogExternalSystemEntity.Id, tenantName));

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
                    List<GetLGAErrorResponse> errorMessagesObject = JsonConvert.DeserializeObject<List<GetLGAErrorResponse>>(JsonConvert.SerializeObject(responseObject.ResponseObject));
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
                pssExternalDataLGAStagingDAOManager = null;
                callLogForExternalSystemDAOManager = null;
            }

            return "OK";
        }

        /// <summary>
        /// Update lga records in <see cref="Core.Models.LGA"/>  to inactive that do not exists in <see cref="PSSExternalDataLGAStaging"/> 
        /// using <see cref="PSSExternalDataLGAStaging.Name"/> is not equals <see cref="Core.Models.LGA.Name"/>
        /// </summary>
        public void SetLGARecordsToInactiveInLGAIfNotExistInStaging(string tenantName, long callLogExternalSystemEntityId)
        {

            try
            {
                SetUnitofWork(tenantName);
                SetPSSExternalDataLGAStagingDAOManager();

                UoW.BeginTransaction();

                pssExternalDataLGAStagingDAOManager.SetLGARecordsToInactiveInLGAIfNotExistInStaging(callLogExternalSystemEntityId);

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
                pssExternalDataLGAStagingDAOManager = null;

            }

        }

        /// <summary>
        /// Iterates and validates LGA name and code, sets <see cref="PSSExternalDataLGAStagingVM.HasError"/> 
        /// to true and <see cref="PSSExternalDataLGAStagingVM.ErrorMessage"/> with error message if invalid.
        /// Updates PSSExternalDataLGAStaging
        /// </summary>
        /// <param name="callLogExternalSystemEntityId"></param>
        /// <param name="tenantName"></param>
        public void ValidateLGANameAndCode(long callLogExternalSystemEntityId, string tenantName)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetPSSExternalDataLGAStagingDAOManager();

                IEnumerable<PSSExternalDataLGAStagingVM> pssExternalDataLGAStagings = pssExternalDataLGAStagingDAOManager.GetListOfLGA(callLogExternalSystemEntityId);

                if (pssExternalDataLGAStagings.Count() > 0)
                {
                    foreach (var lga in pssExternalDataLGAStagings)
                    {
                        if (string.IsNullOrEmpty(lga.Name))
                        {
                            lga.HasError = true;
                            lga.ErrorMessage += "Name is not valid |";
                        }
                        if (string.IsNullOrEmpty(lga.Code))
                        {
                            lga.HasError = true;
                            lga.ErrorMessage += "Code is not valid |";
                        }
                        if (string.IsNullOrEmpty(lga.StateCode))
                        {
                            lga.HasError = true;
                            lga.ErrorMessage += "State Code is not valid |";
                        }
                    }

                    if (pssExternalDataLGAStagings.Any(x => x.HasError))
                    {

                        pssExternalDataLGAStagingDAOManager.BuildPSSExternalDataLGAStagingBulkUpdate(pssExternalDataLGAStagings.Where(x => x.HasError), out string tempTableName, out string createTempTableQuery, out string updateTableQuery, out DataTable dataTable);

                        UoW.BeginTransaction();

                        pssExternalDataLGAStagingDAOManager.UpdateErrorMessageAfterValidation(dataTable, tempTableName, createTempTableQuery, updateTableQuery);

                        UoW.Commit();
                    }
                    BackgroundJob.Enqueue(() => MoveMatchingLGANameToMain(callLogExternalSystemEntityId, tenantName));

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
                pssExternalDataLGAStagingDAOManager = null;

            }

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
        /// Sets an instance of <see cref="pssExternalDataLGAStagingDAOManager"/> 
        /// </summary>
        private void SetPSSExternalDataLGAStagingDAOManager()
        {
            if (pssExternalDataLGAStagingDAOManager == null) { pssExternalDataLGAStagingDAOManager = new PSSExternalDataLGAStagingDAOManager(UoW); }
        }

        /// <summary>
        /// Sets an instance of <see cref="pssStateModelExternalDataStateDAOManager"/> 
        /// </summary>
        private void SetPSSStateModelExternalDataStateDAOManager()
        {
            if (pssStateModelExternalDataStateDAOManager == null) { pssStateModelExternalDataStateDAOManager = new PSSStateModelExternalDataStateDAOManager(UoW); }
        }
    }
}
