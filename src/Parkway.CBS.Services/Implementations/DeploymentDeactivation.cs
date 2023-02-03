using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories.PoliceOfficerDeployment;
using Parkway.CBS.ClientRepository.Repositories.PoliceOfficerDeployment.Contracts;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.Services.Implementations.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Services.Implementations
{
    public class DeploymentDeactivation : IDeploymentDeactivation
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
        /// This deactivates officer deployment by setting the active to false and status to completed at the end date of the deployment
        /// </summary>
        /// <returns></returns>
        public string ProcessDeploymentDeactivation(string tenantName)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetPoliceOfficerDeploymentLogDAOManager();
                UoW.BeginTransaction();
                DateTime now = DateTime.Now.ToLocalTime();
                string yesterday = new DateTime(now.Year, now.Month, now.Day).AddDays(-1).ToString("yyyy-MM-dd");
                log.Info($"About to deactivate police officer deployment for end date {yesterday}");
                string response = PoliceOfficerDeploymentLogDAOManager.DeactivateDeployment(yesterday);
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
