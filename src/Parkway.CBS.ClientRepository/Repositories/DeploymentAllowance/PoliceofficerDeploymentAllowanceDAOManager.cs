using Parkway.CBS.ClientRepository.Repositories.DeploymentAllowance.Contracts;
using Parkway.CBS.ClientRepository.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.DeploymentAllowance
{
    public class PoliceofficerDeploymentAllowanceDAOManager : IPoliceofficerDeploymentAllowanceDAOManager
    {
        protected readonly IUoW _uow;
        public PoliceofficerDeploymentAllowanceDAOManager(IUoW uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deploymentAllowanceVM"></param>
        public void SaveAllowanceRequest(DeploymentAllowanceVM deploymentAllowanceVM)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Police_Core_PoliceofficerDeploymentAllowance (Status, PoliceOfficer_Id, Amount, Narration, ContributedAmount, PaymentStage, Request_Id, Invoice_Id, CreatedAtUtc, UpdatedAtUtc, Command_Id)" +
                    $" VALUES (:status, :policeOfficerId, :amount, :narration, :contributedAmount, :paymentStage, :requestId, :InvoiceId, :date, :date, commandId)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("status", deploymentAllowanceVM.Status);
                query.SetParameter("policeOfficerId", deploymentAllowanceVM.PoliceOfficerId);
                query.SetParameter("amount", deploymentAllowanceVM.Amount);
                query.SetParameter("narration", deploymentAllowanceVM.Narration);
                query.SetParameter("contributedAmount", deploymentAllowanceVM.ContributedAmount);
                query.SetParameter("paymentStage", deploymentAllowanceVM.PaymentStage);
                query.SetParameter("requestId", deploymentAllowanceVM.RequestId);
                query.SetParameter("InvoiceId", deploymentAllowanceVM.InvoiceId);
                query.SetParameter("commandId", deploymentAllowanceVM.CommandId);
                query.SetParameter("date", DateTime.Now.ToLocalTime());
                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}
