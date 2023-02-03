using NHibernate.Transform;
using Parkway.CBS.ClientRepository.Repositories.DeploymentAllowance.Contracts;
using Parkway.CBS.ClientRepository.Repositories.Models;
using System;
using System.Linq;

namespace Parkway.CBS.ClientRepository.Repositories.DeploymentAllowance
{
    public class PSSDeploymentAllowanceSettlementEngineDetailsDAOManager : IPSSDeploymentAllowanceSettlementEngineDetailsDAOManager
    {
        protected readonly IUoW _uow;
        public PSSDeploymentAllowanceSettlementEngineDetailsDAOManager(IUoW uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// Get the deployment allowance settlement engine request details
        /// </summary>
        /// <param name="settlementAllowanceRequestId"></param>
        /// <returns>DeploymentAllowanceSettlementVM</returns>
        public DeploymentAllowanceSettlementVM GetDeploymentAllowanceSettlementDetails(long settlementAllowanceRequestId)
        {
            try
            {
                var queryString = $"Select RetryCount, SettlementEngineRequestJSON FROM Parkway_CBS_Police_Core_PSSDeploymentAllowanceSettlementEngineDetails WHERE Id = {settlementAllowanceRequestId}";

                return _uow.Session.CreateSQLQuery(queryString).SetResultTransformer(Transformers.AliasToBean<DeploymentAllowanceSettlementVM>()).List<DeploymentAllowanceSettlementVM>().SingleOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Update deployment allowance settlement details after sending the request to the Settlement Engine
        /// </summary>
        /// <param name="deploymentAllowanceVM"></param>
        public void UpdateDeploymentAllowanceSettlementDetails(DeploymentAllowanceSettlementVM deploymentAllowanceVM)
        {
            try
            {
                var queryText = $"UPDATE pdas SET pdas.SettlementEngineResponseJSON = :settlementEngineResponseJSON, pdas.Error = :error, pdas.ErrorMessage = :errorMessage, pdas.TimeFired = :timeFired, pdas.RetryCount = :retryCount, pdas.UpdatedAtUtc = GETDATE() FROM Parkway_CBS_Police_Core_PSSDeploymentAllowanceSettlementEngineDetails pdas WHERE pdas.Id = :deploymentAllowanceSettlementId";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("settlementEngineResponseJSON", deploymentAllowanceVM.SettlementEngineResponseJSON);
                query.SetParameter("error", deploymentAllowanceVM.Error);
                query.SetParameter("errorMessage", deploymentAllowanceVM.ErrorMessage);
                query.SetParameter("timeFired", deploymentAllowanceVM.TimeFired);
                query.SetParameter("retryCount", deploymentAllowanceVM.RetryCount);
                query.SetParameter("deploymentAllowanceSettlementId", deploymentAllowanceVM.SettlementAllowanceRequestId);
                query.ExecuteUpdate(); ;
            }
            catch (Exception)
            { throw; }
        }
    }
}
