using Parkway.CBS.ClientRepository.Repositories.Models.Enums;
using Parkway.CBS.ClientRepository.Repositories.PoliceOfficerDeployment.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.PoliceOfficerDeployment
{
    public class PoliceOfficerDeploymentLogDAOManager : IPoliceOfficerDeploymentLogDAOManager
    {
        protected readonly IUoW _uow;
        public PoliceOfficerDeploymentLogDAOManager(IUoW uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// This activates officer deployment by setting the active to true and status to running at the start date of the deployment
        /// </summary>
        /// <returns></returns>
        public string ActivateDeployment(string today)
        {
            try
            {

                var queryText = $"UPDATE depLog SET depLog.Isactive = :newActiveStatus, depLog.status = :newStatus, depLog.UpdatedAtUtc = :dateUpdated FROM Parkway_CBS_Police_Core_PoliceOfficerDeploymentLog depLog WHERE depLog.StartDate = :startDate AND depLog.Status = :currentStatus AND depLog.IsActive = :currentActiveStatus";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("currentStatus", (int)DeploymentStatus.Pending);
                query.SetParameter("dateUpdated", DateTime.Now.ToLocalTime());
                query.SetParameter("startDate", today);
                query.SetParameter("newStatus", (int)DeploymentStatus.Running);
                query.SetParameter("currentActiveStatus", false);
                query.SetParameter("newActiveStatus", true);

                int affectedRecord = query.ExecuteUpdate();
                return $"Successful. {affectedRecord} processed.";
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This deactivates officer deployment by setting the active to false and status to completed at the end date of the deployment
        /// </summary>
        /// <returns></returns>
        public string DeactivateDeployment(string yesterday)
        {
            try
            {
                var queryText = $"UPDATE depLog SET depLog.Isactive = :newActiveStatus, depLog.status = :newStatus, depLog.UpdatedAtUtc = :dateUpdated FROM Parkway_CBS_Police_Core_PoliceOfficerDeploymentLog depLog WHERE depLog.EndDate = :endDate AND depLog.Status = :currentStatus AND depLog.IsActive = :currentActiveStatus";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("currentStatus", (int)DeploymentStatus.Running);
                query.SetParameter("dateUpdated", DateTime.Now.ToLocalTime());
                query.SetParameter("endDate", yesterday);
                query.SetParameter("newStatus", (int)DeploymentStatus.Completed);
                query.SetParameter("currentActiveStatus", true);
                query.SetParameter("newActiveStatus", false);

                int affectedRecord = query.ExecuteUpdate();
                return $"Successful. {affectedRecord} processed.";
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
