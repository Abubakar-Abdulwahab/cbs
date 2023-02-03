using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using NHibernate.Linq;
using System.Linq;
using Parkway.CBS.POSSAP.Services.HelperModel;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSSettlementBatchAggregateDAOManager : Repository<PSSSettlementBatchAggregate>, IPSSSettlementBatchAggregateDAOManager
    {
        public PSSSettlementBatchAggregateDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// Update the batch aggregate with settlement engine response
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="responsePayload"></param>
        public void UpdateBatchAggregateWithSettlementEngineResponse(long batchId, string responsePayload)
        {
            var queryText = $"UPDATE Parkway_CBS_Police_Core_{nameof(PSSSettlementBatchAggregate)} SET {nameof(PSSSettlementBatchAggregate.SettlementEngineResponseJSON)} = :response WHERE {nameof(PSSSettlementBatchAggregate.SettlementBatch)}_Id = :batchId";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("response", responsePayload);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Updates batch aggregate retry count
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="retryCount"></param>
        public void UpdateBatchAggregateRetryCount(long batchId, int retryCount)
        {
            var queryText = $"UPDATE Parkway_CBS_Police_Core_{nameof(PSSSettlementBatchAggregate)} SET {nameof(PSSSettlementBatchAggregate.RetryCount)} = :retryCount WHERE {nameof(PSSSettlementBatchAggregate.SettlementBatch)}_Id = :batchId";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("retryCount", retryCount);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Get settlement batch aggregate with specified batch id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public PSSSettlementBatchAggregateVM GetBatchAggregate(long batchId)
        {
            return _uow.Session.Query<PSSSettlementBatchAggregate>().Where(x => x.SettlementBatch.Id == batchId).Select(x => new PSSSettlementBatchAggregateVM { SettlementEngineRequestJSON = x.SettlementEngineRequestJSON, RetryCount = x.RetryCount }).SingleOrDefault();
        }


        /// <summary>
        /// Updates error, error type and error message for settlement batch aggregate with specified batch id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorType"></param>
        /// <param name="errorMessage"></param>
        public void UpdateErrorTypeAndErrorMessage(long batchId, int errorType, string errorMessage)
        {
            var queryText = $"UPDATE Parkway_CBS_Police_Core_{nameof(PSSSettlementBatchAggregate)} SET {nameof(PSSSettlementBatchAggregate.Error)} = :hasError, {nameof(PSSSettlementBatchAggregate.ErrorType)} = :errorType, {nameof(PSSSettlementBatchAggregate.ErrorMessage)} = :errorMessage WHERE {nameof(PSSSettlementBatchAggregate.SettlementBatch)}_Id = :batchId";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("hasError", true);
            query.SetParameter("errorType", errorType);
            query.SetParameter("errorMessage", errorMessage);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Updates error, error type, SettlementEngineResponseJSON and error message for settlement batch aggregate with specified batch id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorType"></param>
        /// <param name="errorMessage"></param>
        /// <param name="responsePayload"></param>
        public void UpdateErrorTypeAndErrorMessage(long batchId, int errorType, string errorMessage, string responsePayload)
        {
            var queryText = $"UPDATE Parkway_CBS_Police_Core_{nameof(PSSSettlementBatchAggregate)} SET {nameof(PSSSettlementBatchAggregate.Error)} = :hasError, {nameof(PSSSettlementBatchAggregate.ErrorType)} = :errorType, {nameof(PSSSettlementBatchAggregate.ErrorMessage)} = :errorMessage, {nameof(PSSSettlementBatchAggregate.SettlementEngineResponseJSON)} = :responsePayload WHERE {nameof(PSSSettlementBatchAggregate.SettlementBatch)}_Id = :batchId";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("hasError", true);
            query.SetParameter("errorType", errorType);
            query.SetParameter("errorMessage", errorMessage);
            query.SetParameter("responsePayload", responsePayload);
            query.ExecuteUpdate();
        }
    }
}
