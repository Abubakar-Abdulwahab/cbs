using NHibernate;
using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.DataFilters.SettlementRules.SearchFilters
{
    public interface ISettlementRulesSearchFilter : IDependency
    {
        /// <summary>
        /// Add criteria restrictions
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchParams"></param>
        void AddCriteriaRestriction(ICriteria criteria, SettlementRulesSearchParams searchParams);
}
}
