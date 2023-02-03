using NHibernate;
using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.SettlementFeePartiesReport.SearchFilters.Contracts
{
    public interface ISettlementFeePartiesReportFilters : IDependency
    {
        void AddCriteriaRestriction(ICriteria criteria, SettlementFeePartiesSearchParams searchParams);

    }
}
