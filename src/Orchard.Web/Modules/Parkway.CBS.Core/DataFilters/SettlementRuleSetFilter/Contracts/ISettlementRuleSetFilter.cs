using Orchard;

namespace Parkway.CBS.Core.DataFilters.SettlementRuleSetFilter.Contracts
{
    public interface ISettlementRuleSetFilter : IDependency
    {
        void FindParentSet(Services.Contracts.ISettlementRuleManager<Models.SettlementRule> _settlementRepo, Models.SettlementRule settlementRule);
    }
}
