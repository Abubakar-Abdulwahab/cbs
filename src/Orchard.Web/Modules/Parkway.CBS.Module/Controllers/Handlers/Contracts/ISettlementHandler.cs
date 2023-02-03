using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Module.Controllers.Handlers.Contracts
{
    public interface ISettlementHandler : IDependency
    {
        SettlementRuleVM GetCreateSettlementRule(List<string> mdaIds);


        /// <summary>
        /// Get billable revenue heads that belong to this MDA
        /// </summary>
        /// <param name="sId">string value of the MDAId</param>
        /// <returns>APIResponse</returns>
        APIResponse GetRevenueHeads(string sId);


        /// <summary>
        /// Gets all the revenue heads for the mdas selected
        /// </summary>
        /// <param name="mdaIds"></param>
        /// <returns></returns>
        APIResponse GetRevenueHeadsPerMda(string mdaIds);


        /// <summary>
        /// try create settlement rule
        /// </summary>
        /// <param name="model"></param>
        void TryCreateSettlementRule(SettlementController callback, SettlementRuleVM model);

        /// <summary>
        /// try create settlement rule staging
        /// </summary>
        /// <param name="model"></param>
        void TryCreateSettlementRuleForStaging(SettlementController callback, SettlementRuleVM model);

        /// <summary>
        /// Get settlement rules report
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        SettlementsViewModel GetSettlementRulesReport(SettlementController callBack, SettlementRulesSearchParams searchParams);
    }
}
