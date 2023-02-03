using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;


namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    class PSSSettlementFeePartyRequestTransactionCommandSplitComputeDAOManager : Repository<PSSSettlementFeePartyRequestTransactionCommandSplitCompute>, IPSSSettlementFeePartyRequestTransactionCommandSplitComputeDAOManager
    {
        public PSSSettlementFeePartyRequestTransactionCommandSplitComputeDAOManager(IUoW uow) : base(uow)
        { }



        /// <summary>
        /// Move the state command ratio details to the compute table
        /// for further computation
        /// </summary>
        /// <param name="batchId"></param>
        public void ForNonAdditionalSplitsCombineFeePartyRequestTransactionWithCommands(long batchId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} " +
                $"({nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ConfigTransaction)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AmountToSplit)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.RequestCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.State)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.LGA)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.CreatedAtUtc)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.UpdatedAtUtc)}) " +
                $"SELECT fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.Id)}, :batchId, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.SettlementFeeParty)}_Id, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.FeeParty)}_Id, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.AmountToSettle)}, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.RequestCommand)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.LGA)}_Id, anotherCMD.{nameof(Command.Id)}, cmd.{nameof(Command.ZonalCommand)}_Id, GETDATE(), GETDATE() " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommand)} rtcc " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} fprqtx " +
                $"ON rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.ConfigTransaction)}_Id = fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id " +
                $"AND  rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd " +
                $"ON cmd.{nameof(Command.Id)} = rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} anotherCMD " +
                $"ON anotherCMD.{nameof(Command.State)}_Id = rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id WHERE anotherCMD.{nameof(Command.CommandCategory)}_Id = :commandCategory " +
                $"AND rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = :batchId AND fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.HasAdditionalSplit)} = :falseBoolVal;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("falseBoolVal", false);
            query.SetParameter("commandCategory",  Police.Core.Models.Enums.PSSCommandCategoryLevel.State);
            query.ExecuteUpdate();
        }

        public void ForCommandSplitsCombineFeePartyRequestTransactionWithCommands(long batchId, string adapterValue)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} " +
                $"({nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ConfigTransaction)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AmountToSplit)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.RequestCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.State)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.LGA)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.CreatedAtUtc)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.UpdatedAtUtc)},{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand)}_Id) " +
                $"SELECT fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.Id)}, :batchId, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.HasAdditionalSplit)}, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)}, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.SettlementFeeParty)}_Id, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.FeeParty)}_Id, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.AmountToSettle)}, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.RequestCommand)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.LGA)}_Id, anotherCMD.{nameof(Command.Id)}, cmd.{nameof(Command.ZonalCommand)}_Id, GETDATE(), GETDATE(), rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommand)} rtcc " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} fprqtx " +
                $"ON rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.ConfigTransaction)}_Id = fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id " +
                $"AND  rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd " +
                $"ON cmd.{nameof(Command.Id)} = rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} anotherCMD " +
                $"ON anotherCMD.{nameof(Command.State)}_Id = rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id WHERE anotherCMD.{nameof(Command.CommandCategory)}_Id = :commandCategory " +
                $"AND rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = :batchId AND fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.HasAdditionalSplit)} = :trueBoolVal AND fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)} = :adapterVal;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueBoolVal", true);
            query.SetParameter("commandCategory", Police.Core.Models.Enums.PSSCommandCategoryLevel.State);
            query.SetParameter("adapterVal", adapterValue);
            query.ExecuteUpdate();
        }

        public void ForCommandStateSplitsCombineFeePartyRequestTransactionWithCommands(long batchId, string adapterValue)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} " +
                $"({nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ConfigTransaction)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AmountToSplit)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.RequestCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.State)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.LGA)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.CreatedAtUtc)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.UpdatedAtUtc)},{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand)}_Id) " +
                $"SELECT fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.Id)}, :batchId, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.HasAdditionalSplit)}, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)}, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.SettlementFeeParty)}_Id, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.FeeParty)}_Id, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.AmountToSettle)}, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.RequestCommand)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.LGA)}_Id, anotherCMD.{nameof(Command.Id)}, cmd.{nameof(Command.ZonalCommand)}_Id, GETDATE(), GETDATE(), anotherCMD.{nameof(Command.Id)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommand)} rtcc " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} fprqtx " +
                $"ON rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.ConfigTransaction)}_Id = fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id " +
                $"AND  rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd " +
                $"ON cmd.{nameof(Command.Id)} = rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} anotherCMD " +
                $"ON anotherCMD.{nameof(Command.State)}_Id = rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id WHERE anotherCMD.{nameof(Command.CommandCategory)}_Id = :commandCategory " +
                $"AND rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = :batchId AND fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.HasAdditionalSplit)} = :trueBoolVal AND fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)} = :adapterVal;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueBoolVal", true);
            query.SetParameter("commandCategory", Police.Core.Models.Enums.PSSCommandCategoryLevel.State);
            query.SetParameter("adapterVal", adapterValue);
            query.ExecuteUpdate();
        }

        public void ForCommandZonalSplitsCombineFeePartyRequestTransactionWithCommands(long batchId, string adapterValue)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} " +
                $"({nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ConfigTransaction)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AmountToSplit)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.RequestCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.State)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.LGA)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.CreatedAtUtc)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.UpdatedAtUtc)},{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand)}_Id) " +
                $"SELECT fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.Id)}, :batchId, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.HasAdditionalSplit)}, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)}, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.SettlementFeeParty)}_Id, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.FeeParty)}_Id, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.AmountToSettle)}, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.RequestCommand)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.LGA)}_Id, anotherCMD.{nameof(Command.Id)}, cmd.{nameof(Command.ZonalCommand)}_Id, GETDATE(), GETDATE(), cmd.{nameof(Command.ZonalCommand)}_Id " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommand)} rtcc " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} fprqtx " +
                $"ON rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.ConfigTransaction)}_Id = fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id " +
                $"AND  rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd " +
                $"ON cmd.{nameof(Command.Id)} = rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} anotherCMD " +
                $"ON anotherCMD.{nameof(Command.State)}_Id = rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id WHERE anotherCMD.{nameof(Command.CommandCategory)}_Id = :commandCategory " +
                $"AND rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = :batchId AND fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.HasAdditionalSplit)} = :trueBoolVal AND fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)} = :adapterVal;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueBoolVal", true);
            query.SetParameter("commandCategory", Police.Core.Models.Enums.PSSCommandCategoryLevel.State);
            query.SetParameter("adapterVal", adapterValue);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Set fall flag to be used as base record for 
        /// compute final deduction
        /// </summary>
        /// <param name="batchId"></param>
        public void SetFallFlag(long batchId)
        {
            var queryText = $"UPDATE cr " +
                $"SET {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FallFlag)} = :trueFlagValue " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} cr " +
                $"JOIN " +
                $"(SELECT innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id, innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id, " +
                $"MAX(innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Id)}) AS {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Id)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} innerSelect " +
                $"WHERE innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = :batchId " +
                $"GROUP BY innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id, innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id) " +
                $" groupBySelect " +
                $"ON cr.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id = groupBySelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id " +
                $"AND cr.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = groupBySelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id " +
                $"WHERE cr.Id = groupBySelect.Id;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueFlagValue", true);
            query.ExecuteUpdate();
        }



        /// <summary>
        /// Here we get the count for all the items 
        /// in the group. We group by the fee party tranx and the batch Id
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateItemCount(long batchId)
        {
            var queryText = $"UPDATE cr " +
                $"SET {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitItemCount)} = groupBySelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitItemCount)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} cr " +
                $"JOIN " +
                $"(SELECT innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id, innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id, " +
                $"COUNT(innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Id)}) AS {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitItemCount)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} innerSelect " +
                $"WHERE innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = :batchId " +
                $"GROUP BY innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id, innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id) " +
                $"groupBySelect " +
                $"ON cr.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id = groupBySelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id " +
                $"AND cr.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = groupBySelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Here we do the percentage split for non fall flag records
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateSplitItemPercentageForNonFallFlagValues(long batchId)
        {
            var queryText = $"UPDATE cr " +
                $"SET {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitPercentage)} = ROUND(100/{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitItemCount)}, 2), " +
                $"{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitAmount)} = ROUND(((ROUND(100/{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitItemCount)}, 2)) * {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AmountToSplit)})/100, 2)" +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} cr " +
                $"WHERE cr.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = :batchId AND cr.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FallFlag)} = :falseBoolVal;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("falseBoolVal", false);
            query.ExecuteUpdate();
        }

        /// <summary>
        /// Do percentage split amount and percentage value for fall flag
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateSplitAmountForFallFlag(long batchId)
        {
            var queryText = $"UPDATE cr " +
                 $"SET {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitPercentage)} = 100 - groupBySelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitPercentage)}, " +
                 $"{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitAmount)} = cr.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AmountToSplit)} - groupBySelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitAmount)} " +
                 $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} cr " +
                 $"JOIN " +
                 $"(SELECT innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id, innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id, SUM(innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitPercentage)}) AS {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitPercentage)}, SUM(innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitAmount)}) AS {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitAmount)} " +
                 $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} innerSelect WHERE " +
                 $"innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = :batchId " +
                 $"GROUP BY innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id, innerSelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id) " +
                 $"groupBySelect " +
                 $"ON cr.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id = groupBySelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id " +
                 $"AND cr.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = groupBySelect.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id " +
                 $"WHERE cr.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FallFlag)} = :trueBoolVal;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueBoolVal", true);
            query.ExecuteUpdate();
        }

        /// <summary>
        /// For adapter command splits we need to combine the fee party with the 
        /// transaction and commands
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="adapterValue"></param
        public void ForAdapterCommandSplitsCombineFeePartyRequestTransactionWithCommands(long batchId, string adapterValue)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} " +
                $"({nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeePartyRequestTransaction)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ConfigTransaction)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AmountToSplit)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.RequestCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.State)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.LGA)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.CreatedAtUtc)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.UpdatedAtUtc)}, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand)}_Id, {nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementAccountType)}) " +
                $"SELECT fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.Id)}, :batchId, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.HasAdditionalSplit)}, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)}, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.SettlementFeeParty)}_Id, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.FeeParty)}_Id, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id, fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.AmountToSettle)}, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.RequestCommand)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id, rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.LGA)}_Id, anotherCMD.{nameof(Command.Id)}, cmd.{nameof(Command.ZonalCommand)}_Id, GETDATE(), GETDATE(), sac.{nameof(PSSSettlementAdapterCommand.SettlementCommand)}_Id, sac.{nameof(PSSSettlementAdapterCommand.SettlementAccountType)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommand)} rtcc " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} fprqtx " +
                $"ON rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.ConfigTransaction)}_Id = fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id " +
                $"AND  rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd " +
                $"ON cmd.{nameof(Command.Id)} = rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} anotherCMD " +
                $"ON anotherCMD.{nameof(Command.State)}_Id = rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSFeePartyAdapterConfiguration)} fpac ON fpac.{nameof(PSSFeePartyAdapterConfiguration.Name)} = :adapterVal " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementAdapterCommand)} sac ON sac.{nameof(PSSSettlementAdapterCommand.ServiceCommand)}_Id = rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id AND sac.{nameof(PSSSettlementAdapterCommand.FeePartyAdapter)}_Id = fpac.{nameof(PSSFeePartyAdapterConfiguration.Id)} " +
                $"WHERE anotherCMD.{nameof(Command.CommandCategory)}_Id = :commandCategory " +
                $"AND rtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = :batchId AND fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.HasAdditionalSplit)} = :trueBoolVal AND fprqtx.{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)} = :adapterVal;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueBoolVal", true);
            query.SetParameter("commandCategory", Police.Core.Models.Enums.PSSCommandCategoryLevel.State);
            query.SetParameter("adapterVal", adapterValue);
            query.ExecuteUpdate();
        }

    }
}