using Parkway.CBS.Core.Models;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class TransactionLogDAOManager : Repository<TransactionLog>, ITransactionLogDAOManager
    {
        public TransactionLogDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// When the funds have been settled we mark the settled transaction as settled on the tranlog table
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="settlementDate">Date the settlement was made</param>
        public void MarkSettledTransaction(long batchId, DateTime settlementDate)
        {
            var queryText = $"UPDATE trl " +
                $"SET {nameof(TransactionLog.Settled)} = :trueFlagValue, " +
                $"{nameof(TransactionLog.SettlementDate)} = :settlementDate, " +
                $"{nameof(TransactionLog.SettlmentBatchIdentifier)} = :batchId " +
                $"FROM Parkway_CBS_Core_{nameof(TransactionLog)} trl " +
                $"INNER JOIN " +
                $"Parkway_CBS_Police_Core_{nameof(PSSServiceSettlementConfigurationTransaction)} psctx " +
                $"ON psctx.{nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id = trl.{nameof(TransactionLog.Id)} " +
                $"WHERE psctx.{nameof(PSSServiceSettlementConfigurationTransaction.Batch)}_Id = :batchId;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueFlagValue", true);
            query.SetParameter("settlementDate", settlementDate);
            query.ExecuteUpdate();
        }

    }
}