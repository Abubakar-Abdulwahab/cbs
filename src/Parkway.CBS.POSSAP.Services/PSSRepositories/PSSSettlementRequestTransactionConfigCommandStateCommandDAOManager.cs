using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;


namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSSettlementRequestTransactionConfigCommandStateCommandDAOManager : Repository<PSSSettlementRequestTransactionConfigCommandStateCommand>, IPSSSettlementRequestTransactionConfigCommandStateCommandDAOManager
    {
        public PSSSettlementRequestTransactionConfigCommandStateCommandDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// merge the command transaction request with the state and zonal command 
        /// pairing of the transaction, request, the commands, state and zonal commands
        /// </summary>
        /// <param name="batchId"></param>
        public void MergeCommandTransactionRequestWithStateCommand(long batchId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand)} " +
                $"({nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.TransactionLog)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.ConfigTransaction)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.StateCommand)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.RequestCommand)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.RequestAndCommand)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.Batch)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.Request)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.State)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.LGA)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.CreatedAtUtc)}, {nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.UpdatedAtUtc)}) " +
                $"SELECT rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.TransactionLog)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.ConfigTransaction)}_Id, cmd.{nameof(Command.Id)}, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.RequestCommand)}_Id, :batchId, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Request)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.LGA)}_Id, GETDATE(), GETDATE() " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommand)} rtcc " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd " +
                $"ON rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id = cmd.{nameof(Command.State)}_Id AND cmd.{nameof(Command.CommandCategory)}_Id = :stateCommandCategory " +
                $"WHERE rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = :batchId ";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("stateCommandCategory", Police.Core.Models.Enums.PSSCommandCategoryLevel.State);
            query.ExecuteUpdate();
        }



        /// <summary>
        /// Update the compute table with the sum
        /// of the command ratio when grouped by config and batch
        /// </summary>
        /// <param name="batchId"></param>
        public void SumCommandRatio(long batchId)
        {
            var queryText = $"UPDATE cr " +
                $"SET {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.CommandRatioSum)} = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.CommandRatioSum)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute)} cr " +
                $"JOIN " +
                $"(SELECT innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Batch)}_Id, SUM(innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.CommandRatio)}) AS {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.CommandRatioSum)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute)} innerSelect " +
                $"WHERE innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Batch)}_Id = :batchId " +
                $"GROUP BY innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Batch)}_Id) " +
                $" groupBySelect " +
                $"ON cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.ConfigTransaction)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.ConfigTransaction)}_Id " +
                $"AND cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Batch)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Batch)}_Id;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Calculate ratio amount for non fall flag records
        /// </summary>
        /// <param name="batchId"></param>
        public void CalculateRatioAmount(long batchId)
        {
            var queryText = $"UPDATE cr " +
                $"SET {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.RatioAmount)} = " +
                $"ROUND({nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FeePercentage)} * ({nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.CommandRatio)}/{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.CommandRatioSum)}), 2) " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute)} cr " +
                $"WHERE cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Batch)}_Id = :batchId AND cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FallRatioFlag)} = :falseBoolVal;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("falseBoolVal", false);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// When every other ratio amount has been calculated we
        /// need to get the value for the fall ratio which would be a sum of all not fall ratio
        /// substracted from the fee percentage
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateFallRatio(long batchId)
        {
            var queryText = $"UPDATE cr " +
                $"SET {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.RatioAmount)} = cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FeePercentage)} - groupBySelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.RatioAmount)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute)} cr " +
                $"JOIN " +
                $"(SELECT innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Batch)}_Id, SUM(innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.RatioAmount)}) AS {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.RatioAmount)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute)} innerSelect WHERE " +
                $"innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Batch)}_Id = :batchId " +
                $"GROUP BY innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Batch)}_Id) " +
                $"groupBySelect " +
                $"ON cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.ConfigTransaction)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.ConfigTransaction)}_Id " +
                $"AND cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Batch)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Batch)}_Id " +
                $"WHERE cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FallRatioFlag)} = :trueBoolVal;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueBoolVal", true);
            query.ExecuteUpdate();
        }

    }
}