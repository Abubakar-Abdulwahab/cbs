using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementBatchAggregateDAOManager : IRepository<PSSSettlementBatchAggregate>
    {
        /// <summary>
        /// Update the batch aggregate with settlement engine response
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="responsePayload"></param>
        void UpdateBatchAggregateWithSettlementEngineResponse(long batchId, string responsePayload);

        /// <summary>
        /// Updates batch aggregate retry count
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="retryCount"></param>
        void UpdateBatchAggregateRetryCount(long batchId, int retryCount);


        /// <summary>
        /// Get settlement batch aggregate with specified batch id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        PSSSettlementBatchAggregateVM GetBatchAggregate(long batchId);

        /// <summary>
        /// Updates error, error type and error message for settlement batch aggregate with specified batch id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorType"></param>
        /// <param name="errorMessage"></param>
        void UpdateErrorTypeAndErrorMessage(long batchId, int errorType, string errorMessage);

        /// <summary>
        /// Updates error, error type and error message for settlement batch aggregate with specified batch id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorType"></param>
        /// <param name="errorMessage"></param>
        /// <param name="responsePayload"></param>
        void UpdateErrorTypeAndErrorMessage(long batchId, int errorType, string errorMessage, string responsePayload);
    }
}
