using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPoliceCollectionLogDAOManager : IRepository<PoliceCollectionLog>
    {
        /// <summary>
        /// Get collection log count
        /// </summary>
        /// <param name="pssServiceSettlement"></param>
        /// <param name="settlementRuleVM"></param>
        /// <returns>IEnumerable<PoliceCollectionLogReportStatsVM></returns>
        IEnumerable<PoliceCollectionLogReportStatsVM> GetCollectionLogCount(PSSServiceSettlementConfigurationVM pssServiceSettlement, PSSSettlementRuleVM settlementRuleVM);


        /// <summary>
        /// Get paginated collection logs
        /// </summary>
        /// <param name="pssServiceSettlement"></param>
        /// <param name="settlementRuleVM"></param>
        /// <returns>IEnumerable<PoliceCollectionLogVM></returns>
        List<PoliceCollectionLogVM> GetPagedCollectionLogs(PSSServiceSettlementConfigurationVM pssServiceSettlement, PSSSettlementRuleVM settlementRuleVM, int take, int skip);

        /// <summary>
        /// Split the cost of service from the original amount
        /// </summary>
        /// <param name="pssPresettlementDeduction"></param>
        /// <param name="collectionLogVMs"></param>
        /// <returns></returns>
        ConcurrentQueue<PoliceCollectionLogVM> ComputeCostofService(PSSPresettlementDeductionConfigurationVM pssPresettlementDeduction, List<PoliceCollectionLogVM> collectionLogVMs);

    }
}
