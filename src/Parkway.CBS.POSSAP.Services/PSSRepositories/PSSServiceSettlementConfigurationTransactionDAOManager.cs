using Parkway.CBS.Core.Models;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSServiceSettlementConfigurationTransactionDAOManager : Repository<PSSServiceSettlementConfigurationTransaction>, IPSSServiceSettlementConfigurationTransactionDAOManager
    {
        public PSSServiceSettlementConfigurationTransactionDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// insert into the PSSServiceSettlementConfigurationTransaction table the 
        /// pairing of the transaction and the configurations
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="psssettlementId"></param>
        /// <param name="serviceId"></param>
        public void PairTransactionWithConfigurations(long batchId, int psssettlementId, int serviceId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSServiceSettlementConfigurationTransaction)} " +
                $"({nameof(PSSServiceSettlementConfigurationTransaction.Batch)}_Id, {nameof(PSSServiceSettlementConfigurationTransaction.Service)}_Id, {nameof(PSSServiceSettlementConfigurationTransaction.MDA)}_Id, {nameof(PSSServiceSettlementConfigurationTransaction.RevenueHead)}_Id, {nameof(PSSServiceSettlementConfigurationTransaction.PaymentProvider)}_Id, {nameof(PSSServiceSettlementConfigurationTransaction.Channel)}, {nameof(PSSServiceSettlementConfigurationTransaction.Request)}_Id, {nameof(PSSServiceSettlementConfigurationTransaction.Invoice)}_Id, {nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id, {nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog.SettlementAmount)}, {nameof(PSSServiceSettlementConfigurationTransaction.CreatedAtUtc)}, {nameof(PSSServiceSettlementConfigurationTransaction.UpdatedAtUtc)}) " +
                $"SELECT :batchId, :serviceId, sct.{nameof(PSSServiceSettlementConfiguration.MDA)}_Id, sct.{nameof(PSSServiceSettlementConfiguration.RevenueHead)}_Id, sct.{nameof(PSSServiceSettlementConfiguration.PaymentProvider)}_Id, sct.{nameof(PSSServiceSettlementConfiguration.Channel)}, pcl.{nameof(PoliceCollectionLog.Request)}_Id, trl.{nameof(TransactionLog.Invoice)}_Id, trl.{nameof(TransactionLog.Id)}, trl.{nameof(TransactionLog.SettlementAmount)}, GETDATE(), GETDATE() " +
                $"FROM Parkway_CBS_Core_{nameof(TransactionLog)} trl " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PoliceCollectionLog)} pcl  ON trl.{nameof(TransactionLog.Id)} = pcl.{nameof(PoliceCollectionLog.TransactionLog)}_Id AND trl.{nameof(TransactionLog.Settled)} = :falseBoolVal " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} batch ON batch.{nameof(PSSSettlementBatch.Id)} = :batchId " +

                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSServiceSettlementConfiguration)} sct ON sct.{nameof(PSSServiceSettlementConfiguration.MDA)}_Id = trl.{nameof(TransactionLog.MDA)}_Id AND sct.{nameof(PSSServiceSettlementConfiguration.RevenueHead)}_Id = trl.{nameof(TransactionLog.RevenueHead)}_Id AND sct.{nameof(PSSServiceSettlementConfiguration.PaymentProvider)}_Id = trl.{nameof(TransactionLog.PaymentProvider)} AND sct.{nameof(PSSServiceSettlementConfiguration.Channel)} = trl.{nameof(TransactionLog.Channel)} " +
                $"WHERE sct.{nameof(PSSServiceSettlementConfiguration.Settlement)}_Id = :psssettlementId AND sct.{nameof(PSSServiceSettlementConfiguration.Service)}_Id = :serviceId AND sct.{nameof(PSSServiceSettlementConfiguration.IsActive)} = :boolVal " +
                $"AND pcl.{nameof(PoliceCollectionLog.CreatedAtUtc)} BETWEEN batch.{nameof(PSSSettlementBatch.SettlementRangeStartDate)} AND batch.{nameof(PSSSettlementBatch.SettlementRangeEndDate)};";


        var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("serviceId", serviceId);
            query.SetParameter("psssettlementId", psssettlementId);
            query.SetParameter("boolVal", true);
            query.SetParameter("falseBoolVal", false);
            query.ExecuteUpdate();
        }

    }
}
