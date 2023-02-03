using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.ClientRepository.Repositories.PoliceOfficerDeployment;
using Parkway.CBS.ClientRepository.Repositories.PoliceOfficerDeployment.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.Services.Implementations.Contracts;
using System;

namespace Parkway.CBS.Services.Implementations
{
    public class DeploymentActivation : IDeploymentActivation
    {

        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IPoliceOfficerDeploymentLogDAOManager PoliceOfficerDeploymentLogDAOManager { get; set; }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSAllowanceJob");
            }
        }

        private void SetPoliceOfficerDeploymentLogDAOManager()
        {
            if (PoliceOfficerDeploymentLogDAOManager == null) { PoliceOfficerDeploymentLogDAOManager = new PoliceOfficerDeploymentLogDAOManager(UoW); }
        }


        /// <summary>
        /// This activates officer deployment by setting the active to true and status to running at the start date of the deployment
        /// </summary>
        /// <returns></returns>
        [ProlongExpirationTime]
        public string ProcessDeploymentActivation(string tenantName)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetPoliceOfficerDeploymentLogDAOManager();
                UoW.BeginTransaction();
                string today = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd");
                log.Info($"About to activate police officer deployment for date {today}");
                string response = PoliceOfficerDeploymentLogDAOManager.ActivateDeployment(today);
                UoW.Commit();
                return response;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }
    }
}
