using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using System.Collections.Concurrent;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementBatchItemsDAOManager : IRepository<PSSSettlementBatchItems>
    {
        /// <summary>
        /// Move items to be settled from police collection log to settlement batch items table
        /// </summary>
        /// <param name="pssServiceSettlement"></param>
        /// <param name="settlementRuleVM"></param>
        /// <param name="settlementBatchId"></param>
        /// <returns>bool</returns>
        bool MoveRecordFromPoliceCollectionLogToSettlementBatchItems(PSSServiceSettlementConfigurationVM pssServiceSettlement, PSSSettlementRuleVM settlementRuleVM, int settlementBatchId);

        /// <summary>
        /// Save bundle of records
        /// </summary>
        /// <param name="pssServiceSettlement"></param>
        /// <param name="settlementRuleVM"></param>
        /// <param name="settlementBatchId"></param>
        /// <param name="collectionLogVMs"></param>
        /// <param name="batchLimit"></param>
        void SaveRecords(PSSServiceSettlementConfigurationVM pssServiceSettlement, PSSSettlementRuleVM settlementRuleVM, int settlementBatchId, ConcurrentQueue<PoliceCollectionLogVM> collectionLogVMs, int batchLimit);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pssServiceSettlement"></param>
        /// <param name="settlementBatchId"></param>
        /// <returns></returns>
        dynamic GetBatchAggregateAmount(PSSServiceSettlementConfigurationVM pssServiceSettlement, int settlementBatchId);


        /// <summary>
        /// When compute has been done with fee parties that
        /// do not have additional splits
        /// we move these records to the batch items table
        /// </summary>
        /// <param name="batchId"></param>
        void MoveNonSplitRecordsFromSplitComputeToBatchItems(long batchId);


        /// <summary>
        /// When compute has been done with fee parties that
        /// have additional splits
        /// we move these records to the batch items table
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="adapterValue"></param>
        void MoveAdditionalSplitRecordsFromSplitComputeToBatchItemsForCommandAdapter(long batchId, string adapterValue);


        /// <summary>
        /// When compute has been done with fee parties that
        /// have additional splits and adapter value State
        /// we move these records to the batch items table
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="adapterValue"></param>
        void MoveAdditionalSplitRecordsForStateFromSplitComputeToBatchItems(long batchId, string adapterValue);


        /// <summary>
        /// When compute has been done with fee parties that
        /// have additional splits and adapter value Zonal
        /// we move these records to the batch items table
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="adapterValue"></param>
        void MoveAdditionalSplitRecordsForZonalFromSplitComputeToBatchItems(long batchId, string adapterValue);

        /// <summary>
        /// When compute has been done with fee parties that
        /// have additional splits and adapter value MSS
        /// we move these records to the batch items table
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="adapterValue"></param>
        void MoveAdditionalSplitRecordsFromSplitComputeToBatchItemsForAdapterCommand(long batchId, string adapterValue);


        /// <summary>
        /// Moves records from PSS Settlement Fee Party Request Transaction To Settlement Batch Items
        /// </summary>
        /// <param name="batchId"></param>
        void MoveHasNoCommandSplitRecordsFromFeePartyRequestTransactionToBatchItems(long batchId);

    }
}
