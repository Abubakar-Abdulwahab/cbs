using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;


namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class FeePartyRequestTransactionDAOManager : Repository<PSSSettlementFeePartyRequestTransaction>, IFeePartyRequestTransactionDAOManager
    {
        public FeePartyRequestTransactionDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// Here we pair the fee parties for this particular settlement with the transaction
        /// request, confirguration for eventual computation
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="serviceId"></param>
        public void PairFeePartyWithoutAdditionalSplitsWithTransactionRequestConfigurationTransaction(long batchId, int serviceId)
       {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} " +
               $"({nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id, {nameof(PSSSettlementFeePartyRequestTransaction.SettlementFeeParty)}_Id, " +
               $"{nameof(PSSSettlementFeePartyRequestTransaction.FeeParty)}_Id, " +
               $"{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id, {nameof(PSSSettlementFeePartyRequestTransaction.TransactionLog)}_Id, " +
               $"{nameof(PSSSettlementFeePartyRequestTransaction.DeductioPercentage)}, {nameof(PSSSettlementFeePartyRequestTransaction.Request)}_Id, " +
               $"{nameof(PSSSettlementFeePartyRequestTransaction.IsMaxPercentage)}, {nameof(PSSSettlementFeePartyRequestTransaction.TransactionAmount)}, " +
               $"{nameof(PSSSettlementFeePartyRequestTransaction.AmountToSettle)}, {nameof(PSSSettlementFeePartyRequestTransaction.HasAdditionalSplit)}, " +
               $"{nameof(PSSSettlementFeePartyRequestTransaction.AdditionalSplitValue)}, " +
               $"{nameof(PSSSettlementFeePartyRequestTransaction.CreatedAtUtc)}, {nameof(PSSSettlementFeePartyRequestTransaction.UpdatedAtUtc)}) " +
               $"SELECT :batchId, fp.{nameof(PSSSettlementFeeParty.Id)}, fp.{nameof(PSSSettlementFeeParty.FeeParty)}_Id, sct.{nameof(PSSServiceSettlementConfigurationTransaction.Id)}, sct.{nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id, fp.{nameof(PSSSettlementFeeParty.DeductionValue)}, sct.{nameof(PSSServiceSettlementConfigurationTransaction.Request)}_Id, fp.{nameof(PSSSettlementFeeParty.MaxPercentage)}, sct.{nameof(PSSServiceSettlementConfigurationTransaction.SettlementAmount)}, " +
               $"ROUND(((sct.{nameof(PSSServiceSettlementConfigurationTransaction.SettlementAmount)} * fp.{nameof(PSSSettlementFeeParty.DeductionValue)})/100), 2), fp.{nameof(PSSSettlementFeeParty.HasAdditionalSplits)}, fp.{nameof(PSSSettlementFeeParty.AdditionalSplitValue)}, GETDATE(), GETDATE() " +
               $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} bch " +
               $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeeParty)} fp ON fp.{nameof(PSSSettlementFeeParty.Settlement)}_Id = bch.{nameof(PSSSettlementBatch.PSSSettlement)}_Id " +
               $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSServiceSettlementConfigurationTransaction)} sct ON sct.{nameof(PSSServiceSettlementConfigurationTransaction.Batch)}_Id = :batchId " +
               $"WHERE bch.{nameof(PSSSettlementBatch.Id)} = :batchId AND fp.{nameof(PSSSettlementFeeParty.IsActive)} = :isActiceValue;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("isActiceValue", true);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// When then fee party and transaction, requests have been paired we need to
        /// do the calculation for the max percentage
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="serviceId"></param>
        public void DoCalculationForMaxPercentage(long batchId, int serviceId)
        {
            var queryText = $"UPDATE fprtc " +
                $"SET {nameof(PSSSettlementFeePartyRequestTransaction.AmountToSettle)} = fprtc.{nameof(PSSSettlementFeePartyRequestTransaction.TransactionAmount)} - groupBySelect.{nameof(PSSSettlementFeePartyRequestTransaction.AmountToSettle)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} fprtc " +
                $"JOIN " +
                $"(SELECT innerSelect.{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id, SUM(innerSelect.{nameof(PSSSettlementFeePartyRequestTransaction.AmountToSettle)}) AS {nameof(PSSSettlementFeePartyRequestTransaction.AmountToSettle)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} innerSelect WHERE " +
                $"innerSelect.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id = :batchId AND innerSelect.{nameof(PSSSettlementFeePartyRequestTransaction.IsMaxPercentage)} = :falseBoolVal " +
                $"GROUP BY innerSelect.{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id, innerSelect.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id) " +
                $"groupBySelect " +
                $"ON fprtc.{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id = groupBySelect.{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id " +
                $"AND fprtc.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id = groupBySelect.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id " +
                $"WHERE fprtc.{nameof(PSSSettlementFeePartyRequestTransaction.IsMaxPercentage)} = :trueBoolVal;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueBoolVal", true);
            query.SetParameter("falseBoolVal", false);
            query.ExecuteUpdate();
        }

    }
}