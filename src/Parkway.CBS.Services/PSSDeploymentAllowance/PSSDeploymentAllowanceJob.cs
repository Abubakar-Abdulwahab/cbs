using Hangfire;
using Newtonsoft.Json;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.ClientRepository.Repositories.DeploymentAllowance;
using Parkway.CBS.ClientRepository.Repositories.DeploymentAllowance.Contracts;
using Parkway.CBS.ClientRepository.Repositories.Models;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.HangFireInterface.Notification.RemoteClient;
using Parkway.CBS.HangFireInterface.Notification.RemoteClient.Contracts;
using Parkway.CBS.Services.Models;
using Parkway.CBS.Services.PSSDeploymentAllowance.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Services.PSSDeploymentAllowance
{
    public class PSSDeploymentAllowanceJob : IPSSDeploymentAllowanceJob
    {
        public IUoW UoW { get; set; }

        public IPSSDeploymentAllowanceSettlementEngineDetailsDAOManager SettlementEngineDetailsDAO { get; set; }

        public IPoliceofficerDeploymentAllowanceDAOManager DeploymentAllowanceDAO { get; set; }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSAllowanceJob");
            }
        }

        [ProlongExpirationTime]
        /// <summary>
        /// This queue deployment allowance on Hangfire
        /// </summary>
        /// <param name="deploymentAllowanceRequestId"></param>
        /// <param name="tenantName"></param>
        public void QueueDeploymentAllowanceRequest(long deploymentAllowanceSettlementId, string tenantName = "POSSAP")
        {
            StartHangfireServer();
            BackgroundJob.Enqueue(() => SendDeploymentAllowanceRequest(tenantName, deploymentAllowanceSettlementId));
        }

        [ProlongExpirationTime]
        /// <summary>
        /// This sends deployment allowance request to settlement engine
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="deploymentAllowanceSettlementId"></param>
        public void SendDeploymentAllowanceRequest(string tenantName, long deploymentAllowanceSettlementId)
        {
            int retryCount = 0;
            try
            {
                SetUnitofWork(tenantName);
                SetSettlementEngineDetailsDAO();

                DeploymentAllowanceSettlementVM allowanceSettlementVM = SettlementEngineDetailsDAO.GetDeploymentAllowanceSettlementDetails(deploymentAllowanceSettlementId);
                if(allowanceSettlementVM == null)
                {
                    throw new Exception($"Unable to get deployment allowance settlement engine details for request id {deploymentAllowanceSettlementId}");
                }

                retryCount = allowanceSettlementVM.RetryCount;
                var clientCode = ConfigurationManager.AppSettings["POSSAPSettlementClientCode"];
                var secret = ConfigurationManager.AppSettings["POSSAPSettlementSecret"];
                var settlementBaseURL = ConfigurationManager.AppSettings["SettlementEngineBaseURL"];

                if (string.IsNullOrEmpty(clientCode) || string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(settlementBaseURL))
                {
                    throw new Exception("Deployment allowance details was not found");
                }

                IClient _remoteClient = new Client();
                SettlementEngineAuthVM authRequest = new SettlementEngineAuthVM { ClientCode = clientCode, hmac = Core.Utilities.Util.HMACHash256(clientCode, secret) };
                string authRequestModel = JsonConvert.SerializeObject(authRequest);
                string stoken = _remoteClient.SendRequest(authRequestModel, $"{settlementBaseURL}/auth/gettoken", HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                SettlementEngineAuthResponseVM authtoken = JsonConvert.DeserializeObject<SettlementEngineAuthResponseVM>(stoken);

                var response = _remoteClient.SendRequest(allowanceSettlementVM.SettlementEngineRequestJSON, $"{settlementBaseURL}/rule/directsettlement", HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                SettlementEngineResponse responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(response);

                UoW.BeginTransaction();
                SettlementEngineDetailsDAO.UpdateDeploymentAllowanceSettlementDetails(new DeploymentAllowanceSettlementVM
                {
                    Error = responseModel.Error,
                    TimeFired = DateTime.Now,
                    SettlementEngineResponseJSON = responseModel.Error ? null : response,
                    ErrorMessage = responseModel.Error? response : null,
                    RetryCount = allowanceSettlementVM.RetryCount + 1,
                    SettlementAllowanceRequestId = deploymentAllowanceSettlementId
                });
                UoW.Commit();
            }
            catch (Exception ex)
            {
                UoW.BeginTransaction();
                SettlementEngineDetailsDAO.UpdateDeploymentAllowanceSettlementDetails(new DeploymentAllowanceSettlementVM
                {
                    Error = true,
                    TimeFired = DateTime.Now,
                    ErrorMessage = ex.Message + ex.InnerException,
                    RetryCount = retryCount + 1,
                    SettlementAllowanceRequestId = deploymentAllowanceSettlementId
                });
                UoW.Commit();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
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

        private void SetSettlementEngineDetailsDAO()
        {
            if (SettlementEngineDetailsDAO == null) { SettlementEngineDetailsDAO = new PSSDeploymentAllowanceSettlementEngineDetailsDAOManager(UoW); }
        }
    }
}
