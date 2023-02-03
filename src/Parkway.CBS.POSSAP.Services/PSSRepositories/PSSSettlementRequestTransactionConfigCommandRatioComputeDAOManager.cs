using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;


namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSSettlementRequestTransactionConfigCommandRatioComputeDAOManager : Repository<PSSSettlementRequestTransactionConfigCommandRatioCompute>, IPSSSettlementRequestTransactionConfigCommandRatioComputeDAOManager
    {
        public PSSSettlementRequestTransactionConfigCommandRatioComputeDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// When we have gotten the commands that serviced the request, we need to compute their fee percentage
        /// </summary>
        /// <param name="batchId"></param>
        public void AddRecordsFromRatioToCompute(long batchId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute)} " +
                $"({nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.TransactionLog)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.ConfigTransaction)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Command)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Batch)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.Request)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.CommandRatio)}, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FallRatioFlag)}, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FeePercentage)}, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FeeParty)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FeePartyName)}, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FeePartyAccountNumber)}, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.FeePartyBankCodeForAccountNumber)}, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.CommandWalletDetails)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.State)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.LGA)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.CreatedAtUtc)}, {nameof(PSSSettlementRequestTransactionConfigCommandRatioCompute.UpdatedAtUtc)}) " +
                $"SELECT cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.TransactionLog)}_Id, cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.ConfigTransaction)}_Id, cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.Command)}_Id, :batchId, cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.Request)}_Id, cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.CommandRatio)}, cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.FallRatioFlag)}, fp.{nameof(PSSSettlementFeeParty.DeductionValue)}, fp.{nameof(PSSSettlementFeeParty.Id)}, CONCAT(cmd.{nameof(Command.Name)}, ' ', , ' ', cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.CommandRatio)}), cwd.{nameof(CommandWalletDetails.AccountNumber)}, cwd.{nameof(CommandWalletDetails.BankCode)}, cwd.{nameof(CommandWalletDetails.Id)}, cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.State)}_Id, cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.LGA)}_Id, GETDATE(), GETDATE() " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommandRatio)} cr " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd " +
                $"ON cmd.{nameof(Command.Id)} = cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.Command)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(CommandWalletDetails)} cwd " +
                $"ON cmd.{nameof(Command.Id)} = cwd.{nameof(CommandWalletDetails.Command)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} bch " +
                $"ON bch.{nameof(PSSSettlementBatch.Id)} = cr.{nameof(PSSSettlementRequestTransactionConfigCommandRatio.Batch)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeeParty)} fp " +
                $"ON fp.{nameof(PSSSettlementFeeParty.Settlement)}_Id = bch.{nameof(PSSSettlementBatch.PSSSettlement)}_Id " +
                $"WHERE cr.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = :batchId AND fp.{nameof(PSSSettlementFeeParty.AdditionalSplitValue)} = :commandSplitValue ";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("commandSplitValue", "Command");
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