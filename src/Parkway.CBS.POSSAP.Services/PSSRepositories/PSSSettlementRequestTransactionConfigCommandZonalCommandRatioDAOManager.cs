using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSSettlementRequestTransactionConfigCommandZonalCommandRatioDAOManager : Repository<PSSSettlementRequestTransactionConfigZonalCommandRatio>, IPSSSettlementRequestTransactionConfigCommandZonalCommandRatioDAOManager
    {
        public PSSSettlementRequestTransactionConfigCommandZonalCommandRatioDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// Do split ratio for zonal command records
        /// </summary>
        /// <param name="batchId"></param>
        public void DoSplitRatioForZonalCommand(long batchId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio)} " +
                $"({nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.TransactionLog)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ConfigTransaction)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ZonalCommand)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Batch)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Request)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ZonalCommandRatio)}, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.State)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.LGA)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.CreatedAtUtc)}, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.UpdatedAtUtc)}) " +
                $"SELECT ztcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.TransactionLog)}_Id, ztcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.ConfigTransaction)}_Id, ztcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.ZonalCommand)}_Id, :batchId, ztcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.Request)}_Id, COUNT(ztcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.ZonalCommand)}_Id), ztcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.State)}_Id, ztcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.LGA)}_Id, GETDATE(), GETDATE() " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand)} ztcc " +
                $"WHERE ztcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.Batch)}_Id = :batchId " +
                $"GROUP BY ztcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.Request)}_Id, ztcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.TransactionLog)}_Id, ztcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.ZonalCommand)}_Id, ztcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.Batch)}_Id, ztcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.ConfigTransaction)}_Id, ztcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.State)}_Id, ztcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.LGA)}_Id;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Update fall flag for zonal command
        /// </summary>
        /// <param name="batchId"></param>
        public void SetZonalCommandWithFallFlag(long batchId)
        {
            var queryText = $"UPDATE ztr " +
              $"SET {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.FallRatioFlag)} = :trueFlagValue " +
              $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio)} ztr " +
              $"JOIN " +
              $"(SELECT innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Batch)}_Id, " +
              $"MAX(innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Id)}) AS {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Id)} " +
              $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio)} innerSelect " +
              $"WHERE innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Batch)}_Id = :batchId " +
              $"GROUP BY innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Batch)}_Id) " +
              $" groupBySelect " +
              $"ON ztr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ConfigTransaction)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ConfigTransaction)}_Id " +
              $"AND ztr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Batch)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Batch)}_Id " +
              $"WHERE ztr.Id = groupBySelect.Id;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueFlagValue", true);
            query.ExecuteUpdate();
        }

    }
}
