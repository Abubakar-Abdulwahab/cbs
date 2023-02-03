using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSSettlementRequestTransactionConfigStateCommandRatioComputeDAOManager : Repository<PSSSettlementRequestTransactionConfigStateCommandRatioCompute>, IPSSSettlementRequestTransactionConfigStateCommandRatioComputeDAOManager
    {
        public PSSSettlementRequestTransactionConfigStateCommandRatioComputeDAOManager(IUoW uow) : base(uow)
        { }



        /// <summary>
        /// Move the state command ratio details to the compute table
        /// for further computation
        /// </summary>
        /// <param name="batchId"></param>
        public void MoveStateCommandRatioToComputeTable(long batchId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute)} " +
                 $"({nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.TransactionLog)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.ConfigTransaction)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.StateCommand)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Batch)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Request)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.StateCommandRatio)}, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FallRatioFlag)}, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FeePercentage)}, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FeeParty)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FeePartyName)}, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FeePartyAccountNumber)}, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FeePartyBankCodeForAccountNumber)}, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.CommandWalletDetails)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.State)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.LGA)}_Id, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.CreatedAtUtc)}, {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.UpdatedAtUtc)}) " +
                 $"SELECT str.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.TransactionLog)}_Id, str.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.ConfigTransaction)}_Id, str.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.StateCommand)}_Id, :batchId, str.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Request)}_Id, str.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.StateCommandRatio)}, str.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.FallRatioFlag)}, fp.{nameof(PSSSettlementFeeParty.DeductionValue)}, fp.{nameof(PSSSettlementFeeParty.Id)}, CONCAT(cmd.{nameof(Command.Name)}, ' ', , ' ', str.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.StateCommandRatio)}), cwd.{nameof(CommandWalletDetails.AccountNumber)}, cwd.{nameof(CommandWalletDetails.BankCode)}, cwd.{nameof(CommandWalletDetails.Id)}, str.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.State)}_Id, str.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.LGA)}_Id, GETDATE(), GETDATE() " +
                 $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio)} str " +
                 $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd " +
                 $"ON cmd.{nameof(Command.Id)} = str.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.StateCommand)}_Id " +
                 $"INNER JOIN Parkway_CBS_Police_Core_{nameof(CommandWalletDetails)} cwd " +
                 $"ON cmd.{nameof(Command.Id)} = cwd.{nameof(CommandWalletDetails.Command)}_Id " +
                 $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} bch " +
                 $"ON bch.{nameof(PSSSettlementBatch.Id)} = str.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Batch)}_Id " +
                 $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeeParty)} fp " +
                 $"ON fp.{nameof(PSSSettlementFeeParty.Settlement)}_Id = bch.{nameof(PSSSettlementBatch.PSSSettlement)}_Id " +
                 $"WHERE str.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = :batchId AND fp.{nameof(PSSSettlementFeeParty.AdditionalSplitValue)} = :commandSplitValue ";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("commandSplitValue", "State");
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Add ratio amount to state command on the compute
        /// table
        /// </summary>
        /// <param name="batchId"></param>
        public void AddRatioAmountSumToStateCommand(long batchId)
        {
            var queryText = $"UPDATE cr " +
                $"SET {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.StateCommandRatioSum)} = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.StateCommandRatioSum)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute)} cr " +
                $"JOIN " +
                $"(SELECT innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Batch)}_Id, SUM(innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.StateCommandRatio)}) AS {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.StateCommandRatioSum)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute)} innerSelect " +
                $"WHERE innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Batch)}_Id = :batchId " +
                $"GROUP BY innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Batch)}_Id) " +
                $" groupBySelect " +
                $"ON cr.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.ConfigTransaction)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.ConfigTransaction)}_Id " +
                $"AND cr.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Batch)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Batch)}_Id;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Update the ratio amount for non flaged values from the fee percentage
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateNoFlagWithRatioAmountFromFeePercentage(long batchId)
        {
            var queryText = $"UPDATE cr " +
                $"SET {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.RatioAmount)} = " +
                $"ROUND({nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FeePercentage)} * ({nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.StateCommandRatio)}/{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.StateCommandRatioSum)}), 2) " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute)} cr " +
                $"WHERE cr.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Batch)}_Id = :batchId AND cr.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FallRatioFlag)} = :falseBoolVal;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("falseBoolVal", false);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Update the flag value ratio amount with the sum of the non flag values 
        /// ratio amount 
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateStateCommandFlagValueWithRatioAmountFromRatioAmountOfNonFlagRecords(long batchId)
        {
            var queryText = $"UPDATE cr " +
                 $"SET {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.RatioAmount)} = cr.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FeePercentage)} - groupBySelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.RatioAmount)} " +
                 $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute)} cr " +
                 $"JOIN " +
                 $"(SELECT innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Batch)}_Id, SUM(innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.RatioAmount)}) AS {nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.RatioAmount)} " +
                 $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute)} innerSelect WHERE " +
                 $"innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Batch)}_Id = :batchId " +
                 $"GROUP BY innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Batch)}_Id) " +
                 $"groupBySelect " +
                 $"ON cr.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.ConfigTransaction)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.ConfigTransaction)}_Id " +
                 $"AND cr.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Batch)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.Batch)}_Id " +
                 $"WHERE cr.{nameof(PSSSettlementRequestTransactionConfigStateCommandRatioCompute.FallRatioFlag)} = :trueBoolVal;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueBoolVal", true);
            query.ExecuteUpdate();
        }

    }
}