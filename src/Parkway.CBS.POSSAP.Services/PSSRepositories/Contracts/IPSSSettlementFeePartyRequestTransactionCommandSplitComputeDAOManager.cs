using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;


namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementFeePartyRequestTransactionCommandSplitComputeDAOManager : IRepository<PSSSettlementFeePartyRequestTransactionCommandSplitCompute>
    {
        void ForNonAdditionalSplitsCombineFeePartyRequestTransactionWithCommands(long batchId);

        /// <summary>
        /// Set fall flag to be used as base record for 
        /// compute final deduction
        /// </summary>
        /// <param name="batchId"></param>
        void SetFallFlag(long batchId);

        /// <summary>
        /// Here we get the count for all the items 
        /// in the group. We group by the fee party tranx and the batch Id
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateItemCount(long batchId);

        /// <summary>
        /// Here we do the percentage split for non fall flag records
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateSplitItemPercentageForNonFallFlagValues(long batchId);


        /// <summary>
        /// Do percentage split amount and percentage value for fall flag
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateSplitAmountForFallFlag(long batchId);


        /// <summary>
        /// For command splits we need to combine the fee party with the 
        /// transaction and commands
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="adapterValue"></param>
        void ForCommandSplitsCombineFeePartyRequestTransactionWithCommands(long batchId, string adapterValue);

        /// <summary>
        /// For command splits we need to combine the fee party with the 
        /// transaction and commands
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="adapterValue"></param>
        void ForCommandStateSplitsCombineFeePartyRequestTransactionWithCommands(long batchId, string adapterValue);


        /// <summary>
        /// For command splits we need to combine the fee party with the 
        /// transaction and commands
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="adapterValue"></param>
        void ForCommandZonalSplitsCombineFeePartyRequestTransactionWithCommands(long batchId, string adapterValue);


        /// <summary>
        /// For adapter command splits we need to combine the fee party with the 
        /// transaction and commands
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="adapterValue"></param>
        void ForAdapterCommandSplitsCombineFeePartyRequestTransactionWithCommands(long batchId, string adapterValue);

    }
}
