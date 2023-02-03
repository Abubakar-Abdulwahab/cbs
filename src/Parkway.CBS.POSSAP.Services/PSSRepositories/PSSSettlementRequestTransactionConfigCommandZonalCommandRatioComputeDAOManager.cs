using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;


namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSSettlementRequestTransactionConfigCommandZonalCommandRatioComputeDAOManager : Repository<PSSSettlementRequestTransactionConfigZonalCommandRatioCompute>, IPSSSettlementRequestTransactionConfigCommandZonalCommandRatioComputeDAOManager
    {
        public PSSSettlementRequestTransactionConfigCommandZonalCommandRatioComputeDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// Move PSSSettlementRequestTransactionConfigZonalCommandRatio to zonal ratio compute table
        /// </summary>
        /// <param name="batchId"></param>
        public void MoveZonalRatioToComputeTable(long batchId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute)} " +
                 $"({nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.TransactionLog)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ConfigTransaction)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ZonalCommand)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Batch)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Request)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ZonalCommandRatio)}, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FallRatioFlag)}, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FeePercentage)}, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FeeParty)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FeePartyName)}, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FeePartyAccountNumber)}, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FeePartyBankCodeForAccountNumber)}, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.CommandWalletDetails)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.State)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.LGA)}_Id, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.CreatedAtUtc)}, {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.UpdatedAtUtc)}) " +
                 $"SELECT ztr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.TransactionLog)}_Id, ztr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ConfigTransaction)}_Id, ztr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ZonalCommand)}_Id, :batchId, ztr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Request)}_Id, ztr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ZonalCommandRatio)}, ztr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.FallRatioFlag)}, fp.{nameof(PSSSettlementFeeParty.DeductionValue)}, fp.{nameof(PSSSettlementFeeParty.Id)}, CONCAT('Z: ', cmd.{nameof(Command.Name)}, '-', , '-', ztr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ZonalCommandRatio)}), cwd.{nameof(CommandWalletDetails.AccountNumber)}, cwd.{nameof(CommandWalletDetails.BankCode)}, cwd.{nameof(CommandWalletDetails.Id)}, ztr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.State)}_Id, ztr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.LGA)}_Id, GETDATE(), GETDATE() " +
                 $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio)} ztr " +
                 $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd " +
                 $"ON cmd.{nameof(Command.Id)} = ztr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ZonalCommand)}_Id " +
                 $"INNER JOIN Parkway_CBS_Police_Core_{nameof(CommandWalletDetails)} cwd " +
                 $"ON cmd.{nameof(Command.Id)} = cwd.{nameof(CommandWalletDetails.Command)}_Id " +
                 $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} bch " +
                 $"ON bch.{nameof(PSSSettlementBatch.Id)} = ztr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Batch)}_Id " +
                 $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeeParty)} fp " +
                 $"ON fp.{nameof(PSSSettlementFeeParty.Settlement)}_Id = bch.{nameof(PSSSettlementBatch.PSSSettlement)}_Id " +
                 $"WHERE ztr.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = :batchId AND fp.{nameof(PSSSettlementFeeParty.AdditionalSplitValue)} = :commandSplitValue ";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("commandSplitValue", "Zonal");
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Updtae the compute table with the ratio sum
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateZonalRatioSumOnComputeTable(long batchId)
        {
            var queryText = $"UPDATE zcr " +
                $"SET {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ZonalCommandRatioSum)} = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ZonalCommandRatioSum)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute)} zcr " +
                $"JOIN " +
                $"(SELECT innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Batch)}_Id, SUM(innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ZonalCommandRatio)}) AS {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ZonalCommandRatioSum)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute)} innerSelect " +
                $"WHERE innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Batch)}_Id = :batchId " +
                $"GROUP BY innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Batch)}_Id) " +
                $" groupBySelect " +
                $"ON zcr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ConfigTransaction)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ConfigTransaction)}_Id " +
                $"AND zcr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Batch)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Batch)}_Id;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// update the ratio amount for non flag value 
        /// here we take the percentage from the fee percentage
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateTheRatioAmountForNonFlagRecordsBasedOffFeePercentage(long batchId)
        {
            var queryText = $"UPDATE zr " +
                $"SET {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.RatioAmount)} = " +
                $"ROUND({nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FeePercentage)} * ({nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ZonalCommandRatio)}/{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ZonalCommandRatioSum)}), 2) " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute)} zr " +
                $"WHERE zr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Batch)}_Id = :batchId AND zr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FallRatioFlag)} = :falseBoolVal;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("falseBoolVal", false);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Here once we have the ratio amount of the non flag value
        /// we get the sum of those value and use it in the computation for the flag value
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateTheRatioAmountForFlagRecordsBasedOffRatioAmountSum(long batchId)
        {
            var queryText = $"UPDATE zr " +
                 $"SET {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.RatioAmount)} = zr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FeePercentage)} - groupBySelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.RatioAmount)} " +
                 $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute)} zr " +
                 $"JOIN " +
                 $"(SELECT innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Batch)}_Id, SUM(innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.RatioAmount)}) AS {nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.RatioAmount)} " +
                 $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute)} innerSelect WHERE " +
                 $"innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Batch)}_Id = :batchId " +
                 $"GROUP BY innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Batch)}_Id) " +
                 $"groupBySelect " +
                 $"ON zr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ConfigTransaction)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.ConfigTransaction)}_Id " +
                 $"AND zr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Batch)}_Id = groupBySelect.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.Batch)}_Id " +
                 $"WHERE zr.{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatioCompute.FallRatioFlag)} = :trueBoolVal;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueBoolVal", true);
            query.ExecuteUpdate();
        }


    }
}
