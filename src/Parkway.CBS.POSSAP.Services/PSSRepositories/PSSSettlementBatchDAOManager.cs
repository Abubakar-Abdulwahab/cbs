using System.Linq;
using NHibernate.Linq;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using Parkway.CBS.POSSAP.Services.HelperModel;
using System;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSSettlementBatchDAOManager : Repository<PSSSettlementBatch>, IPSSSettlementBatchDAOManager
    {
        public PSSSettlementBatchDAOManager(IUoW uow) : base(uow)
        { }


        public IEnumerable<PSSSettlementBatchVM> GetBatch(int chunkSize, int skip)
        {
            return _uow.Session.Query<PSSSettlementBatch>()
                .Where(x => x.Status == (int)PSSSettlementBatchStatus.PreQueue).Skip(skip).Take(chunkSize)
                .Select(x => new PSSSettlementBatchVM
                {
                    PSSSettlementId = x.PSSSettlement.Id,
                    Id = x.Id,
                    SettlementRangeStartDate = x.SettlementRangeStartDate,
                    SettlementRangeEndDate = x.SettlementRangeEndDate,
                    ScheduleDate = x.ScheduleDate
                });
        }


        /// <summary>
        /// Save batch settlement
        /// </summary>
        /// <param name="settlementbatch"></param>
        /// <returns>bool</returns>
        public bool SaveBatch(PSSSettlementBatch settlementbatch)
        {
            try
            {
                _uow.Session.Save(settlementbatch);
                return true;
            }
            catch (System.Exception)
            { return false; }
        }


        /// <summary>
        /// Set the settlement that have the status as ready for queueing 
        /// to Prequeue status
        /// </summary>
        public void SetSettlementBatchStatusToReadyForScheduling()
        {
            var queryText = $"UPDATE Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} SET {nameof(PSSSettlementBatch.Status)} = :newStatusVal, {nameof(PSSSettlementBatch.StatusMessage)} = :newStatusNameVal  WHERE Status = :statusVal";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("statusVal", (int)PSSSettlementBatchStatus.ReadyForQueueing);
            query.SetParameter("newStatusVal", (int)PSSSettlementBatchStatus.PreQueue);
            query.SetParameter("newStatusNameVal", "Pre Queueing");
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Get batch details
        /// <para>Only getting the settlement Id and the service Id</para>
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>PSSSettlementBatchVM</returns>
        public PSSSettlementBatchVM GetBatchDetails(long batchId)
        {
            return _uow.Session.Query<PSSSettlementBatch>()
                .Where(x => x.Id == batchId)
                .Select(x => new PSSSettlementBatchVM
                {
                    PSSSettlementId = x.PSSSettlement.Id,
                    ServiceId = x.PSSSettlement.Service.Id,
                    Status = x.Status,
                    SettlementDate = x.SettlementDate
                }).FirstOrDefault();
        }
        
        
        /// <summary>
        /// Update the batch with to the next process status
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="nextStatus"></param>
        /// <param name="nextStatusName"></param>
        public void UpdateBatchStatus(long batchId, PSSSettlementBatchStatus nextStatus, string nextStatusName)
        {
            var queryText = $"UPDATE Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} SET {nameof(PSSSettlementBatch.Status)} = :newStatusVal, {nameof(PSSSettlementBatch.StatusMessage)} = :newStatusNameVal, {nameof(PSSSettlementBatch.UpdatedAtUtc)} = GETDATE()  WHERE {nameof(PSSSettlementBatch.Id)} = :batchId";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("newStatusVal", nextStatus);
            query.SetParameter("newStatusNameVal", nextStatusName);
            query.ExecuteUpdate();
        }


        // <summary>
        /// Update the batch with to the next process status and settlement date
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="nextStatus"></param>
        /// <param name="nextStatusName"></param>
        /// <param name="settlementDate"></param>
        public void UpdateBatchStatus(long batchId, PSSSettlementBatchStatus nextStatus, string nextStatusName, DateTime settlementDate)
        {
            var queryText = $"UPDATE Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} SET {nameof(PSSSettlementBatch.Status)} = :newStatusVal, {nameof(PSSSettlementBatch.StatusMessage)} = :newStatusNameVal, {nameof(PSSSettlementBatch.SettlementDate)} = :settlementDate, {nameof(PSSSettlementBatch.UpdatedAtUtc)} = GETDATE()  WHERE {nameof(PSSSettlementBatch.Id)} = :batchId";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("newStatusVal", nextStatus);
            query.SetParameter("newStatusNameVal", nextStatusName);
            query.SetParameter("settlementDate", settlementDate);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Gets the settlement batch info with the specified batch id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public PSSSettlementFeePartyBatchAggregateSettlementRequestModel GetSettlementBatchInfoForFeePartyBatchAggregateRequestModel(long batchId)
        {
            return _uow.Session.Query<PSSSettlementBatch>().Where(x => x.Id == batchId).Select(x => new PSSSettlementFeePartyBatchAggregateSettlementRequestModel
            {
                RuleCode = x.PSSSettlement.SettlementRule.SettlementEngineRuleIdentifier,
                Narration = $"{x.PSSSettlement.Name} {x.SettlementRangeStartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")} - {x.SettlementRangeEndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}",
                SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                PaymentType = "OTHER",
                StartDate = x.SettlementRangeStartDate,
                EndDate = x.SettlementRangeEndDate
            }).SingleOrDefault();
        }


        /// <summary>
        /// Updates has error flag and error message column for settlement batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="hasError"></param>
        /// <param name="errorMessage"></param>
        public void UpdateSettlementBatchHasErrorAndErrorMessage(long batchId, bool hasError, string errorMessage)
        {
            var queryText = $"UPDATE Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} SET {nameof(PSSSettlementBatch.HasError)} = :hasError, {nameof(PSSSettlementBatch.ErrorMessage)} = :errorMessage WHERE {nameof(PSSSettlementBatch.Id)} = :batchId";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("hasError", hasError);
            query.SetParameter("errorMessage", errorMessage);
            query.SetParameter("batchId", batchId);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Updates settlement date with process date returned by settlement engine
        /// </summary>
        /// <param name="settlementDate"></param>
        public void UpdateSettlementDate(DateTime settlementDate, long batchId)
        {
            var queryText = $"UPDATE Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} SET {nameof(PSSSettlementBatch.SettlementDate)} = :settlementDate WHERE {nameof(PSSSettlementBatch.Id)} = :batchId";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("settlementDate", settlementDate);
            query.SetParameter("batchId", batchId);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Updates settlement amount for batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="settlementAmount"></param>
        public void UpdateSettlementAmountForBatch(long batchId, decimal settlementAmount)
        {
            var queryText = $"UPDATE Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} SET {nameof(PSSSettlementBatch.SettlementAmount)} = :settlementAmount WHERE {nameof(PSSSettlementBatch.Id)} = :batchId";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("settlementAmount", settlementAmount);
            query.SetParameter("batchId", batchId);
            query.ExecuteUpdate();
        }

    }
}

