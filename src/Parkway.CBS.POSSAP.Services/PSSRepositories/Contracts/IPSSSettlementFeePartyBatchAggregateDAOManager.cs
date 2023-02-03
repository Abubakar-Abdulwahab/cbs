using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.POSSAP.Services.HelperModel;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementFeePartyBatchAggregateDAOManager : IRepository<PSSSettlementFeePartyBatchAggregate>
    {

        /// <summary>
        /// When split have been made we move the 
        /// aggregates for the non additional fee parties
        /// to the aggregate table
        /// </summary>
        /// <param name="batchId"></param>
        void MoveToAggregateTableNonSplits(long batchId);


        /// <summary>
        /// Moves records with additional splits to fee party batch aggregate for batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        void MoveToAggregateTableSplits(long batchId);


        /// <summary>
        /// Gets fee parties of settlement batch with the specified id
        /// </summary>
        /// <param name="settlementBatchId"></param>
        /// <returns></returns>
        IEnumerable<PSSSettlementFeePartyBatchAggregateSettlementItemRequestModel> GetFeePartyBatchAggregateItemsForRequest(long settlementBatchId);

    }
}
