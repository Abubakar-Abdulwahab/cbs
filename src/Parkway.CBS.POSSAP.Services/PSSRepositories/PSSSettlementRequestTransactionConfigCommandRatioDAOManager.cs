using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;


namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSSettlementRequestTransactionConfigCommandRatioDAOManager : Repository<PSSSettlementRequestTransactionConfigCommandRatio>, IPSSSettlementRequestTransactionConfigCommandRatioDAOManager
    {
        public PSSSettlementRequestTransactionConfigCommandRatioDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// insert into the PSSSettlementRequestTransactionConfigCommand table the 
        /// pairing of the transaction, request and the commands
        /// </summary>
        /// <param name="batchId"></param>
        public void SortCommandTransactionRequestByCommandRatio(long batchId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommandRatio)} " +
                $"({nameof(PSSSettlementRequestTransactionConfigCommandRatio.TransactionLog)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatio.ConfigTransaction)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatio.Command)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatio.Batch)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatio.Request)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatio.CommandRatio)}, {nameof(PSSSettlementRequestTransactionConfigCommandRatio.State)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatio.LGA)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatio.CreatedAtUtc)}, {nameof(PSSSettlementRequestTransactionConfigCommandRatio.UpdatedAtUtc)}) " +
                $"SELECT rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.TransactionLog)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.ConfigTransaction)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id, :batchId, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Request)}_Id, COUNT(rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id), rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.LGA)}_Id, GETDATE(), GETDATE() " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommand)} rtcc " +
                $"WHERE rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = :batchId " +
                $"GROUP BY rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Request)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.TransactionLog)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.ConfigTransaction)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.LGA)}_Id;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// When we are done command transaction ratio, we need to set one of the value as the fall value
        /// for computation
        /// </summary>
        /// <param name="batchId"></param>
        public void SetFallRatioFlag(long batchId)
        {
            var queryText = $"UPDATE cr " +
                $"SET {nameof(PSSSettlementRequestTransactionConfigCommandRatio.FallRatioFlag)} = :trueFlagValue " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommandRatio)} cr " +
                $"JOIN " +
                $"(SELECT innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.Batch)}_Id, " +
                $"MAX(innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.Id)}) AS {nameof(PSSSettlementRequestTransactionConfigCommandRatio.Id)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommandRatio)} innerSelect " +
                $"WHERE innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.Batch)}_Id = :batchId " +
                $"GROUP BY innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.Batch)}_Id) " +
                $" groupBySelect " +
                $"ON cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.ConfigTransaction)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.ConfigTransaction)}_Id " +
                $"AND cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.Batch)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.Batch)}_Id " +
                $"WHERE cr.Id = groupBySelect.Id;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueFlagValue", true);
            query.ExecuteUpdate();
        }
    }
}