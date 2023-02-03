using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;
using Parkway.CBS.POSSAP.Services.HelperModel;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    class PSSSettlementFeePartyBatchAggregateDAOManager : Repository<PSSSettlementFeePartyBatchAggregate>, IPSSSettlementFeePartyBatchAggregateDAOManager
    {
        public PSSSettlementFeePartyBatchAggregateDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// When split have been made we move the 
        /// aggregates for the non additional fee parties
        /// to the aggregate table
        /// </summary>
        /// <param name="batchId"></param>
        public void MoveToAggregateTableNonSplits(long batchId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyBatchAggregate)} " +
                $"({nameof(PSSSettlementFeePartyBatchAggregate.Batch)}_Id, {nameof(PSSSettlementFeePartyBatchAggregate.SettlementFeeParty)}_Id, {nameof(PSSSettlementFeePartyBatchAggregate.FeeParty)}_Id, {nameof(PSSSettlementFeePartyBatchAggregate.TotalSettlementAmount)}, {nameof(PSSSettlementFeePartyBatchAggregate.Percentage)}, {nameof(PSSSettlementFeePartyBatchAggregate.FeePartyName)}, {nameof(PSSSettlementFeePartyBatchAggregate.BankCode)}, {nameof(PSSSettlementFeePartyBatchAggregate.BankAccountNumber)}, {nameof(PSSSettlementFeePartyBatchAggregate.BankName)}, {nameof(PSSSettlementRequestTransactionConfigCommandRatio.CreatedAtUtc)}, {nameof(PSSSettlementRequestTransactionConfigCommandRatio.UpdatedAtUtc)}) " +
                $"SELECT :batchId, sfp.{nameof(PSSSettlementFeeParty.Id)}, fp.{nameof(PSSFeeParty.Id)}, groupSelect.{nameof(PSSSettlementFeePartyRequestTransaction.AmountToSettle)}, sfp.{nameof(PSSSettlementFeeParty.DeductionValue)}, fp.{nameof(PSSFeeParty.Name)}, bnk.{nameof(Bank.Code)}, fp.{nameof(PSSFeeParty.AccountNumber)}, bnk.{nameof(Bank.Name)}, GETDATE(), GETDATE() " +
                $"FROM " +
                $"(SELECT innerSelect.{nameof(PSSSettlementFeePartyRequestTransaction.SettlementFeeParty)}_Id, SUM(innerSelect.{nameof(PSSSettlementFeePartyRequestTransaction.AmountToSettle)}) " +
                $"AS {nameof(PSSSettlementFeePartyRequestTransaction.AmountToSettle)} " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} innerSelect " +
                $"WHERE innerSelect.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id = :batchId " +
                $"AND innerSelect.{nameof(PSSSettlementFeePartyRequestTransaction.HasAdditionalSplit)} = :falseBoolValue " +
                $"GROUP BY innerSelect.{nameof(PSSSettlementFeePartyRequestTransaction.SettlementFeeParty)}_Id, innerSelect.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id) groupSelect " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeeParty)} sfp " +
                $"ON sfp.{nameof(PSSSettlementFeeParty.Id)} = groupSelect.{nameof(PSSSettlementFeePartyRequestTransaction.SettlementFeeParty)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSFeeParty)} fp " +
                $"ON fp.{nameof(PSSFeeParty.Id)} = sfp.{nameof(PSSSettlementFeeParty.FeeParty)}_Id " +
                $"INNER JOIN Parkway_CBS_Core_{nameof(Bank)} bnk " +
                $"ON bnk.{nameof(Bank.Id)} = fp.{nameof(PSSFeeParty.Bank)}_Id ";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("falseBoolValue", false);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Moves records with additional splits to fee party batch aggregate for batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        public void MoveToAggregateTableSplits(long batchId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyBatchAggregate)} " +
                $"({nameof(PSSSettlementFeePartyBatchAggregate.Batch)}_Id, {nameof(PSSSettlementFeePartyBatchAggregate.SettlementFeeParty)}_Id, {nameof(PSSSettlementFeePartyBatchAggregate.FeeParty)}_Id, {nameof(PSSSettlementFeePartyBatchAggregate.TotalSettlementAmount)}, {nameof(PSSSettlementFeePartyBatchAggregate.Percentage)}, {nameof(PSSSettlementFeePartyBatchAggregate.FeePartyName)}, {nameof(PSSSettlementFeePartyBatchAggregate.BankCode)}, {nameof(PSSSettlementFeePartyBatchAggregate.BankAccountNumber)}, {nameof(PSSSettlementFeePartyBatchAggregate.BankName)}, {nameof(PSSSettlementFeePartyBatchAggregate.AdditionalSplitValue)}, {nameof(PSSSettlementFeePartyBatchAggregate.Command)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandRatio.CreatedAtUtc)}, {nameof(PSSSettlementRequestTransactionConfigCommandRatio.UpdatedAtUtc)}) " +
                $"SELECT {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Batch)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.SettlementFeeParty)}_Id, " +
                $"{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.FeeParty)}_Id, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.TotalSettlementAmount)}, " +
                $"{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.CommandPercentage)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.FeePartyName)}, " +
                $"{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankCode)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankAccountNumber)}, " +
                $"{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.BankName)}, {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.AdditionalSplitValue)}, " +
                $"{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Command)}_Id, GETDATE(), GETDATE() " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate)} WHERE {nameof(PSSSettlementPercentageRecalculationFeePartyBatchAggregate.Batch)}_Id = :batchId;";

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


        /// <summary>
        /// Gets fee parties of settlement batch with the specified id
        /// </summary>
        /// <param name="settlementBatchId"></param>
        /// <returns></returns>
        public IEnumerable<PSSSettlementFeePartyBatchAggregateSettlementItemRequestModel> GetFeePartyBatchAggregateItemsForRequest(long settlementBatchId)
        {
            return _uow.Session.Query<PSSSettlementFeePartyBatchAggregate>().Where(x => x.Batch.Id == settlementBatchId).Select(x => new PSSSettlementFeePartyBatchAggregateSettlementItemRequestModel
            {
                AccountName = x.FeePartyName,
                Amount = x.TotalSettlementAmount,
                BankCode = x.BankCode,
                AccountNumber = x.BankAccountNumber,
                Narration = $"{x.FeePartyName} SETTLEMENT {x.Batch.SettlementRangeStartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}-{x.Batch.SettlementRangeEndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}",
            });
        }
    }
}