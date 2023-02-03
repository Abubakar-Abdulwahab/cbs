using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.DataFilters.SettlementRules.SearchFilters
{
    public class SettlementRuleNameFilter : ISettlementRulesSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, SettlementRulesSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.Name))
            {
                criteria.Add(Restrictions.Like("Name", searchParams.Name.Trim(), MatchMode.Anywhere));
            }
        }
    }


    public class SettlementRuleIdentifierFilter : ISettlementRulesSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, SettlementRulesSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.RuleIdentifier))
            {
                criteria.Add(Restrictions.Like("SettlementEngineRuleIdentifier", searchParams.RuleIdentifier.Trim(), MatchMode.Anywhere));
            }
        }
    }
}