using System.Collections.Generic;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementPreFlightItemsDAOManager : IRepository<PSSSettlementPreFlightItems>
    {
        /// <summary>
        /// Save pre flight items
        /// </summary>
        /// <param name="preFligtSettlementItems"></param>
        /// <returns>bool | return true if saved successfully</returns>
        bool SaveItems(List<PSSSettlementPreFlightItems> preFligtSettlementItems);

        /// <summary>
        /// We need to set the column to initiate queueing of items that are have not be added to the settlement batch
        /// to true, so the next job can pick them up and add to the settlement batch table
        /// </summary>
        /// <param name="preflightBatchId"></param>
        void SetItemsForSettlementBatchInsertion(long preflightBatchId);


        /// <summary>
        /// Move the items that have AddToSettlementBatch set to true to the batch settlement table
        /// </summary>
        /// <param name="preflightBatchId"></param>
        void InsertItemsForSettlementIntoSettlementBatchTable(long preflightBatchId);
    }
}
