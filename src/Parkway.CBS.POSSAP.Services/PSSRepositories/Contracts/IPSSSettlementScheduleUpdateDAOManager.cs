using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementScheduleUpdateDAOManager : IRepository<PSSSettlementScheduleUpdate>
    {
        /// <summary>
        /// Save settlement schedule update items
        /// </summary>
        /// <param name="settlementScheduleUpdateItems"></param>
        /// <returns>bool | return true if saved successfully</returns>
        bool SaveItems(List<PSSSettlementScheduleUpdate> settlementScheduleUpdateItems);


        /// <summary>
        /// Updates settlement schedule date
        /// </summary>
        /// <param name="preflightBatchId"></param>
        void UpdateSettlementScheduleDate(long preflightBatchId);
    }
}
