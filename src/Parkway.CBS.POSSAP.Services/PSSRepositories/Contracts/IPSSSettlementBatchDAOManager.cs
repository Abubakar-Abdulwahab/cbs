using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.POSSAP.Services.HelperModel;
using System;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementBatchDAOManager : IRepository<PSSSettlementBatch>
    {

        /// <summary>
        /// Save batch settlement
        /// </summary>
        /// <param name="settlementbatch"></param>
        /// <returns>bool</returns>
        bool SaveBatch(PSSSettlementBatch settlementBatch);


        /// <summary>
        /// Set the settlement that have the status as ready for queueing 
        /// to Prequeue status
        /// </summary>
        void SetSettlementBatchStatusToReadyForScheduling();


        IEnumerable<PSSSettlementBatchVM> GetBatch(int chunkSize, int skip);


        /// <summary>
        /// Get batch details
        /// <para>Only getting the settlement Id and the service Id</para>
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>PSSSettlementBatchVM</returns>
        PSSSettlementBatchVM GetBatchDetails(long batchId);

        /// <summary>
        /// Update the batch with to the next process status
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="nextStatus"></param>
        /// <param name="nextStatusName"></param>
        void UpdateBatchStatus(long batchId, PSSSettlementBatchStatus nextStatus, string nextStatusName);

        // <summary>
        /// Update the batch with to the next process status and settlement date
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="nextStatus"></param>
        /// <param name="nextStatusName"></param>
        /// <param name="settlementDate"></param>
        void UpdateBatchStatus(long batchId, PSSSettlementBatchStatus nextStatus, string nextStatusName, DateTime settlementDate);

        /// <summary>
        /// Gets the settlement batch info with the specified batch id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        PSSSettlementFeePartyBatchAggregateSettlementRequestModel GetSettlementBatchInfoForFeePartyBatchAggregateRequestModel(long batchId);

        /// <summary>
        /// Updates has error flag and error message column for settlement batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="hasError"></param>
        /// <param name="errorMessage"></param>
        void UpdateSettlementBatchHasErrorAndErrorMessage(long batchId, bool hasError, string errorMessage);

        /// <summary>
        /// Updates settlement date with process date returned by settlement engine
        /// </summary>
        /// <param name="settlementDate"></param>
        void UpdateSettlementDate(DateTime settlementDate, long batchId);


        /// <summary>
        /// Updates settlement amount for batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="settlementAmount"></param>
        void UpdateSettlementAmountForBatch(long batchId, decimal settlementAmount);
    }
}
