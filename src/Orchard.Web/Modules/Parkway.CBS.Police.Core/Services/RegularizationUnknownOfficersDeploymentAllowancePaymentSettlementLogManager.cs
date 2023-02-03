using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
namespace Parkway.CBS.Police.Core.Services
{
    public class RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLogManager : BaseManager<RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog>, IRegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLogManager<RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog>
    {
        private readonly ITransactionManager _transactionManager;
        public RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLogManager(IRepository<RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// Updates the transaction status and UpdatedAtUtc for <see cref="DeploymentAllowancePaymentRequestItem"/>
        /// </summary>
        /// <param name="reference"></param>
        public void UpdateRegularizationUnknownOfficersDeploymentAllowancePaymentRequestItemTransactionStatusFromLog(string reference)
        {
            try
            {
                var targetTable = "Parkway_CBS_Police_Core_" + typeof(DeploymentAllowancePaymentRequestItem).Name;
                var sourceTable = "Parkway_CBS_Police_Core_" + typeof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog).Name;
                var queryText = $"UPDATE DAPRI SET DAPRI.{nameof(DeploymentAllowancePaymentRequestItem.TransactionStatus)} = RSL.{nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.TransactionStatus)}, " +
                                $"DAPRI.{nameof(DeploymentAllowancePaymentRequestItem.UpdatedAtUtc)} = :updateDate FROM {targetTable} DAPRI INNER JOIN {sourceTable} RSL " +
                                $"ON DAPRI.{nameof(DeploymentAllowancePaymentRequestItem.PaymentReference)} = RSL.{nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.PaymentReference)} " +
                                $"WHERE RSL.{nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.Reference)} = '{reference}'";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                RollBackAllTransactions();
                Logger.Error(exception, string.Format("Exception updating details for DeploymentAllowancePaymentRequestItem with reference {0}, Exception message {1}", reference, exception.Message));
                throw;
            }
        }
    }
}