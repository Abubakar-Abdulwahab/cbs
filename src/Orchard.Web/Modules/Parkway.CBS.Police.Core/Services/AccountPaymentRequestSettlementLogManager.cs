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
    public class AccountPaymentRequestSettlementLogManager : BaseManager<AccountPaymentRequestSettlementLog>, IAccountPaymentRequestSettlementLogManager<AccountPaymentRequestSettlementLog>
    {
        private readonly ITransactionManager _transactionManager;

        public AccountPaymentRequestSettlementLogManager(IRepository<AccountPaymentRequestSettlementLog> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// Updates the transaction status and UpdatedAtUtc for <see cref="AccountPaymentRequestItem"/>
        /// </summary>
        /// <param name="reference"></param>
        public void UpdatePaymentRequestItemTransactionStatusFromLog(string reference)
        {
            try
            {
                var targetTable = "Parkway_CBS_Police_Core_" + typeof(AccountPaymentRequestItem).Name;
                var sourceTable = "Parkway_CBS_Police_Core_" + typeof(AccountPaymentRequestSettlementLog).Name;


                var queryText = $"UPDATE {targetTable} SET {nameof(AccountPaymentRequestItem.TransactionStatus)} = RSL.{nameof(AccountPaymentRequestSettlementLog.TransactionStatus)}, {nameof(AccountPaymentRequestItem.UpdatedAtUtc)} = :updateDate FROM {targetTable} APRI INNER JOIN {sourceTable} RSL ON APRI.{nameof(AccountPaymentRequestItem.PaymentReference)} = RSL.{nameof(AccountPaymentRequestSettlementLog.PaymentReference)} WHERE RSL.{nameof(AccountPaymentRequestSettlementLog.Reference)} = '{reference}'";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                RollBackAllTransactions();
                Logger.Error(exception, string.Format("Exception updating details for AccountPaymentRequestItem with reference {0}, Exception message {1}", reference, exception.Message));
                throw;
            }
        }

    }
}