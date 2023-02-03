using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSSettlementPercentageRecalculationFeePartyBatchAggregateDAOManager : Repository<PSSSettlementPercentageRecalculationFeePartyBatchAggregate>, IPSSSettlementPercentageRecalculationFeePartyBatchAggregateDAOManager
    {
        public PSSSettlementPercentageRecalculationFeePartyBatchAggregateDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// When split have been made we move the 
        /// aggregates for the fee parties with additional splits and additional split vallue Command
        /// to the percentage recalculation aggregate table
        /// </summary>
        /// <param name="batchId"></param>
        public void MoveAdditionalSplitsToPercentageRecalculationAggregateTable(long batchId, string adapterValue)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate)} " +
                $"({nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Batch)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.AdditionalSplitValue)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Command)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.SettlementFeeParty)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.FeeParty)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.TotalSettlementAmount)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Percentage)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.AggregateTotalSettlementAmount)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.FeePartyName)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankCode)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankAccountNumber)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankName)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.CreatedAtUtc)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.UpdatedAtUtc)}) " +
                $"SELECT :batchId, sfp.{nameof(PSSSettlementFeeParty.AdditionalSplitValue)}, gs.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command)}_Id, gs.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id, gs.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, gs.SplitAggregate AS SplitAggregate, sfp.{nameof(PSSSettlementFeeParty.DeductionValue)}, " +

                $"(SELECT SUM(innerSelectGS.SplitAggregate) FROM (SELECT SUM(innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitAmount)}) AS SplitAggregate, " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command)}_Id, innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id," +
                $" innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id " +
                $"FROM [Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)}] innerselecttwo " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} AS t2 ON " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id = t2.{nameof(PSSSettlementFeePartyRequestTransaction.Id)} " +
                $"AND t2.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)} = :adapterVal " +
                $"WHERE innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = :batchId GROUP BY " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command)}_Id, " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id) innerSelectGS) " +

                $",CONCAT(cmd.{nameof(Command.Name)},' ',sfp.{nameof(PSSSettlementFeeParty.AdditionalSplitValue)}), bnk.{nameof(Core.Models.Bank.Code)}, cmdwd.{nameof(CommandWalletDetails.AccountNumber)}, " +
                $"bnk.{nameof(Core.Models.Bank.Name)}, GETDATE(), GETDATE() " +
                $"FROM (SELECT SUM(innerSelect.SplitAmount) AS SplitAggregate, innerSelect.Command_Id, innerselect.FeeParty_Id, innerselect.SettlementFeeParty_Id " +
                $"FROM [Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)}] " +
                $"innerselect INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} AS t2 " +
                $"ON innerselect.FeePartyRequestTransaction_Id = t2.{nameof(PSSSettlementFeePartyRequestTransaction.Id)} AND " +
                $"t2.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)} = :adapterVal " +
                $"WHERE innerselect.Batch_Id = :batchId GROUP BY innerSelect.Command_Id, innerselect.FeeParty_Id, innerselect.SettlementFeeParty_Id) gs " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(CommandWalletDetails)} AS cmdwd " +
                $"ON cmdwd.{nameof(CommandWalletDetails.Command)}_Id = gs.Command_Id INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd ON cmd.Id = gs.Command_Id " +
                $"INNER JOIN Parkway_CBS_Core_{nameof(Core.Models.Bank)} AS bnk ON cmdwd.{nameof(CommandWalletDetails.BankCode)} = bnk.{nameof(Core.Models.Bank.Code)} " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeeParty)} AS sfp ON sfp.{nameof(PSSSettlementFeeParty.Id)} = gs.SettlementFeeParty_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSFeeParty)} AS fp ON fp.{nameof(PSSFeeParty.Id)} = gs.{nameof(PSSSettlementFeeParty.FeeParty)}_Id; ";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("adapterVal", adapterValue);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// When split have been made we move the 
        /// aggregates for the fee parties with additional splits and additional split value State
        /// to the percentage recalculation aggregate table
        /// </summary>
        /// <param name="batchId"></param>
        public void MoveAdditionalSplitsForStateToPercentageRecalculationAggregateTable(long batchId, string adapterValue)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate)} " +
                $"({nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Batch)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.AdditionalSplitValue)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Command)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.SettlementFeeParty)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.FeeParty)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.TotalSettlementAmount)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Percentage)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.AggregateTotalSettlementAmount)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.FeePartyName)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankCode)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankAccountNumber)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankName)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.CreatedAtUtc)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.UpdatedAtUtc)}) " +
                $"SELECT :batchId, sfp.{nameof(PSSSettlementFeeParty.AdditionalSplitValue)}, gs.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand)}_Id, gs.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id, gs.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, gs.SplitAggregate AS SplitAggregate, sfp.{nameof(PSSSettlementFeeParty.DeductionValue)}, " +

                $"(SELECT SUM(innerSelectGS.SplitAggregate) FROM (SELECT SUM(innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitAmount)}) AS SplitAggregate, " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand)}_Id, innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id," +
                $" innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id " +
                $"FROM [Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)}] innerselecttwo " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} AS t2 ON " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id = t2.{nameof(PSSSettlementFeePartyRequestTransaction.Id)} " +
                $"AND t2.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)} = :adapterVal " +
                $"WHERE innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = :batchId GROUP BY " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand)}_Id, " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id) innerSelectGS) " +

                $",CONCAT(cmd.{nameof(Command.Name)},' ',sfp.{nameof(PSSSettlementFeeParty.AdditionalSplitValue)}), bnk.{nameof(Core.Models.Bank.Code)}, cmdwd.{nameof(CommandWalletDetails.AccountNumber)}, " +
                $"bnk.{nameof(Core.Models.Bank.Name)}, GETDATE(), GETDATE() " +
                $"FROM (SELECT SUM(innerSelect.SplitAmount) AS SplitAggregate, innerSelect.StateCommand_Id, innerselect.FeeParty_Id, innerselect.SettlementFeeParty_Id " +
                $"FROM [Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)}] " +
                $"innerselect INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} AS t2 " +
                $"ON innerselect.FeePartyRequestTransaction_Id = t2.{nameof(PSSSettlementFeePartyRequestTransaction.Id)} AND " +
                $"t2.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)} = :adapterVal " +
                $"WHERE innerselect.Batch_Id = :batchId GROUP BY innerSelect.StateCommand_Id, innerselect.FeeParty_Id, innerselect.SettlementFeeParty_Id) gs " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(CommandWalletDetails)} AS cmdwd " +
                $"ON cmdwd.{nameof(CommandWalletDetails.Command)}_Id = gs.StateCommand_Id INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd ON cmd.Id = gs.StateCommand_Id " +
                $"INNER JOIN Parkway_CBS_Core_{nameof(Core.Models.Bank)} AS bnk ON cmdwd.{nameof(CommandWalletDetails.BankCode)} = bnk.{nameof(Core.Models.Bank.Code)} " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeeParty)} AS sfp ON sfp.{nameof(PSSSettlementFeeParty.Id)} = gs.SettlementFeeParty_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSFeeParty)} AS fp ON fp.{nameof(PSSFeeParty.Id)} = gs.{nameof(PSSSettlementFeeParty.FeeParty)}_Id; ";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("adapterVal", adapterValue);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// When split have been made we move the 
        /// aggregates for the fee parties with additional splits and additional split value Zonal
        /// to the percentage recalculation aggregate table
        /// </summary>
        /// <param name="batchId"></param>
        public void MoveAdditionalSplitsForZonalToPercentageRecalculationAggregateTable(long batchId, string adapterValue)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate)} " +
                $"({nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Batch)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.AdditionalSplitValue)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Command)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.SettlementFeeParty)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.FeeParty)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.TotalSettlementAmount)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Percentage)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.AggregateTotalSettlementAmount)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.FeePartyName)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankCode)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankAccountNumber)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankName)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.CreatedAtUtc)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.UpdatedAtUtc)}) " +
                $"SELECT :batchId, sfp.{nameof(PSSSettlementFeeParty.AdditionalSplitValue)}, gs.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand)}_Id, gs.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id, gs.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, gs.SplitAggregate AS SplitAggregate, sfp.{nameof(PSSSettlementFeeParty.DeductionValue)}, " +

                $"(SELECT SUM(innerSelectGS.SplitAggregate) FROM (SELECT SUM(innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitAmount)}) AS SplitAggregate, " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand)}_Id, innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id," +
                $" innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id " +
                $"FROM [Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)}] innerselecttwo " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} AS t2 ON " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id = t2.{nameof(PSSSettlementFeePartyRequestTransaction.Id)} " +
                $"AND t2.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)} = :adapterVal " +
                $"WHERE innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = :batchId GROUP BY " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand)}_Id, " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id) innerSelectGS) " +

                $",CONCAT(cmd.{nameof(Command.Name)},' ',sfp.{nameof(PSSSettlementFeeParty.AdditionalSplitValue)}), bnk.{nameof(Core.Models.Bank.Code)}, cmdwd.{nameof(CommandWalletDetails.AccountNumber)}, " +
                $"bnk.{nameof(Core.Models.Bank.Name)}, GETDATE(), GETDATE() " +
                $"FROM (SELECT SUM(innerSelect.SplitAmount) AS SplitAggregate, innerSelect.ZonalCommand_Id, innerselect.FeeParty_Id, innerselect.SettlementFeeParty_Id " +
                $"FROM [Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)}] " +
                $"innerselect INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} AS t2 " +
                $"ON innerselect.FeePartyRequestTransaction_Id = t2.{nameof(PSSSettlementFeePartyRequestTransaction.Id)} AND " +
                $"t2.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)} = :adapterVal " +
                $"WHERE innerselect.Batch_Id = :batchId GROUP BY innerSelect.ZonalCommand_Id, innerselect.FeeParty_Id, innerselect.SettlementFeeParty_Id) gs " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(CommandWalletDetails)} AS cmdwd " +
                $"ON cmdwd.{nameof(CommandWalletDetails.Command)}_Id = gs.ZonalCommand_Id INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd ON cmd.Id = gs.ZonalCommand_Id " +
                $"INNER JOIN Parkway_CBS_Core_{nameof(Core.Models.Bank)} AS bnk ON cmdwd.{nameof(CommandWalletDetails.BankCode)} = bnk.{nameof(Core.Models.Bank.Code)} " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeeParty)} AS sfp ON sfp.{nameof(PSSSettlementFeeParty.Id)} = gs.SettlementFeeParty_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSFeeParty)} AS fp ON fp.{nameof(PSSFeeParty.Id)} = gs.{nameof(PSSSettlementFeeParty.FeeParty)}_Id; ";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("adapterVal", adapterValue);
            query.ExecuteUpdate();
        }

        /// <summary>
        /// When split have been made we move the 
        /// aggregates for the fee parties with additional splits and additional split value in Adapter command table
        /// to the percentage recalculation aggregate table
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="adapterValue"></param>
        public void MoveAdditionalSplitsForAdapterCommandToPercentageRecalculationAggregateTable(long batchId, string adapterValue)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate)} " +
                $"({nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Batch)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.AdditionalSplitValue)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Command)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.SettlementFeeParty)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.FeeParty)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.TotalSettlementAmount)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Percentage)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.AggregateTotalSettlementAmount)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.FeePartyName)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankCode)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankAccountNumber)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankName)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.CreatedAtUtc)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.UpdatedAtUtc)}) " +
                $"SELECT :batchId, sfp.{nameof(PSSSettlementFeeParty.AdditionalSplitValue)}, gs.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand)}_Id, gs.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id, gs.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, gs.SplitAggregate AS SplitAggregate, sfp.{nameof(PSSSettlementFeeParty.DeductionValue)}, " +

                $"(SELECT SUM(innerSelectGS.SplitAggregate) FROM (SELECT SUM(innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitAmount)}) AS SplitAggregate, " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand)}_Id, innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id," +
                $" innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id " +
                $"FROM [Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)}] innerselecttwo " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} AS t2 ON " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id = t2.{nameof(PSSSettlementFeePartyRequestTransaction.Id)} " +
                $"AND t2.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)} = :adapterVal " +
                $"WHERE innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = :batchId GROUP BY " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand)}_Id, " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, " +
                $"innerselecttwo.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id) innerSelectGS) " +

                $",CONCAT(cmd.{nameof(Command.Name)},' ',sfp.{nameof(PSSSettlementFeeParty.AdditionalSplitValue)}), bnk.{nameof(Core.Models.Bank.Code)}, cmdwd.{nameof(CommandWalletDetails.AccountNumber)}, " +
                $"bnk.{nameof(Core.Models.Bank.Name)}, GETDATE(), GETDATE() " +
                $"FROM (SELECT SUM(innerSelect.SplitAmount) AS SplitAggregate, innerSelect.SettlementCommand_Id, innerselect.FeeParty_Id, innerselect.SettlementFeeParty_Id, innerselect.SettlementAccountType " +
                $"FROM [Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)}] " +
                $"innerselect INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} AS t2 " +
                $"ON innerselect.FeePartyRequestTransaction_Id = t2.{nameof(PSSSettlementFeePartyRequestTransaction.Id)} AND " +
                $"t2.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)} = :adapterVal " +
                $"WHERE innerselect.Batch_Id = :batchId GROUP BY innerSelect.SettlementCommand_Id, innerselect.FeeParty_Id, innerselect.SettlementFeeParty_Id, innerselect.SettlementAccountType) gs " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(CommandWalletDetails)} AS cmdwd " +
                $"ON cmdwd.{nameof(CommandWalletDetails.Command)}_Id = gs.SettlementCommand_Id AND cmdwd.{nameof(CommandWalletDetails.SettlementAccountType)} = gs.SettlementAccountType INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd ON cmd.Id = gs.SettlementCommand_Id " +
                $"INNER JOIN Parkway_CBS_Core_{nameof(Core.Models.Bank)} AS bnk ON cmdwd.{nameof(CommandWalletDetails.BankCode)} = bnk.{nameof(Core.Models.Bank.Code)} " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeeParty)} AS sfp ON sfp.{nameof(PSSSettlementFeeParty.Id)} = gs.SettlementFeeParty_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSFeeParty)} AS fp ON fp.{nameof(PSSFeeParty.Id)} = gs.{nameof(PSSSettlementFeeParty.FeeParty)}_Id; ";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("adapterVal", adapterValue);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// When we are done command transaction ratio, we need to set one of the value as the fall value
        /// for computation
        /// </summary>
        /// <param name="batchId"></param>
        public void SetFallRatioFlag(long batchId)
        {
            var queryText = $"UPDATE Parkway_CBS_Police_Core_{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate)} SET FallFlag = :trueFlagValue WHERE Command_Id IN (SELECT MAX(Command_Id) " +
                            $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate)} GROUP BY FeeParty_Id) AND Batch_Id = :batchId";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueFlagValue", true);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Computes command percentage for non fall flags
        /// </summary>
        /// <param name="batchId"></param>
        public void ComputeCommandPercentageForNonFallFlags(long batchId)
        {
            var queryText = $"UPDATE t1 SET t1.CommandPercentage = (SELECT (TotalSettlementAmount * Percentage)/AggregateTotalSettlementAmount FROM" +
                            $" Parkway_CBS_Police_Core_{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate)} t2 WHERE t1.Command_Id = t2.Command_Id AND t1.FeeParty_Id = t2.FeeParty_Id " +
                            $"AND t2.Batch_Id = :batchId) FROM" +
                            $" Parkway_CBS_Police_Core_{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate)} t1 WHERE t1.FallFlag = :falseFlagValue AND t1.Batch_Id = :batchId";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("falseFlagValue", false);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Computes command percentage for fall flags
        /// </summary>
        /// <param name="batchId"></param>
        public void ComputeCommandPercentageForFallFlags(long batchId)
        {
            var queryText = $"UPDATE t1 SET t1.CommandPercentage = t1.Percentage - Coalesce((SELECT SUM(CommandPercentage) FROM " +
                            $"Parkway_CBS_Police_Core_{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate)} t2" +
                            $" WHERE t2.FallFlag = 0 AND t1.FeeParty_Id = t2.FeeParty_Id AND t2.Batch_Id = :batchId GROUP BY t2.FeeParty_Id),0) " +
                            $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate)} t1" +
                            $" WHERE Batch_Id = :batchId AND FallFlag = :trueFlagValue ";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueFlagValue", true);
            query.ExecuteUpdate();
        }

    }
}
