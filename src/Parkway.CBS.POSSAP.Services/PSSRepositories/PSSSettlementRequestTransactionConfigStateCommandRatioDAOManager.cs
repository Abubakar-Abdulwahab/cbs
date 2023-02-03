using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;


namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public class PSSSettlementRequestTransactionConfigStateCommandRatioDAOManager : Repository<PSSSettlementRequestTransactionConfigStateCommandRatio>, IPSSSettlementRequestTransactionConfigStateCommandRatioDAOManager
    {
        public PSSSettlementRequestTransactionConfigStateCommandRatioDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// insert into the PSSSettlementRequestTransactionConfigStateCommandRatio table the 
        /// pairing of the transaction, request, commands ans state command
        /// </summary>
        /// <param name="batchId"></param>
        public void SortCommandTransactionRequestByStateCommandRatio(long batchId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio)} " +
                $"({nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.TransactionLog)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.ConfigTransaction)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.StateCommand)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Batch)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Request)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.StateCommandRatio)}, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.State)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.LGA)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.CreatedAtUtc)}, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.UpdatedAtUtc)}) " +
                $"SELECT stcc.{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.TransactionLog)}_Id, stcc.{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.ConfigTransaction)}_Id, stcc.{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.StateCommand)}_Id, :batchId, stcc.{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.Request)}_Id, COUNT(stcc.{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.StateCommand)}_Id), stcc.{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.State)}_Id, stcc.{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.LGA)}_Id, GETDATE(), GETDATE() " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand)} stcc " +
                $"WHERE stcc.{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.Batch)}_Id = :batchId " +
                $"GROUP BY stcc.{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.Request)}_Id, stcc.{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.TransactionLog)}_Id, stcc.{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.StateCommand)}_Id, stcc.{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.Batch)}_Id, stcc.{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.ConfigTransaction)}_Id, stcc.{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.State)}_Id, stcc.{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.LGA)}_Id;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.ExecuteUpdate();
        }



        /// <summary>
        /// When we are done with the count for each state command
        /// we need to set the fall flag value to each group
        /// </summary>
        /// <param name="batchId"></param>
        public void SetFallFlagForStateCommand(long batchId)
        {
            var queryText = $"UPDATE str " +
               $"SET {nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.FallRatioFlag)} = :trueFlagValue " +
               $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio)} str " +
               $"JOIN " +
               $"(SELECT innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Batch)}_Id, " +
               $"MAX(innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Id)}) AS {nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Id)} " +
               $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio)} innerSelect " +
               $"WHERE innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Batch)}_Id = :batchId " +
               $"GROUP BY innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Batch)}_Id) " +
               $" groupBySelect " +
               $"ON str.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.ConfigTransaction)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.ConfigTransaction)}_Id " +
               $"AND str.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Batch)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Batch)}_Id " +
               $"WHERE str.Id = groupBySelect.Id;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueFlagValue", true);
            query.ExecuteUpdate();
        }

    }
}