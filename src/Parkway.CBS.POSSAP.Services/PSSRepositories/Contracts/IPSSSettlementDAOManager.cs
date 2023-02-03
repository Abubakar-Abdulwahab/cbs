using System;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.ClientRepository.Repositories.Contracts;


namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementDAOManager : IRepository<PSSSettlement>
    {

        /// <summary>
        /// Get paginated records of all the active POSSAP settlement configurations on the systems.
        /// </summary>
        /// <param name="chunkSize"></param>
        /// <param name="skip"></param>
        /// <param name="today"></param>
        /// <returns>IEnumerable<PSSSettlementRuleVM></returns>
        IEnumerable<PSSSettlementRuleVM> GetBatchActivePOSSAPSettlements(int chunkSize, int skip, DateTime startDate, DateTime endDate);


        /// <summary>
        /// Save the settlement batch along with the hangfire reference
        /// </summary>
        /// <param name="settlementBatchKeyPairs"></param>
        bool SaveSettlementBatchAndHangFireRef(List<KeyValuePair<PSSSettlementBatch, PSSHangfireSettlementReference>> settlementBatchKeyPairs);


        /// <summary>
        /// Save the collection
        /// </summary>
        /// <param name="batch"></param>
        /// <returns>bool | return true if the collection save successfully, else false</returns>
        bool SavePSSSettlementBatch(List<PSSSettlementBatch> batch);
    }
}
