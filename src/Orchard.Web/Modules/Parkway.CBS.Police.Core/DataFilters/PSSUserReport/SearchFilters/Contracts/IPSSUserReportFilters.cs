using NHibernate;
using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.PSSUserReport.SearchFilters.Contracts
{
    public interface IPSSUserReportFilters : IDependency
    {
        /// <summary>
        /// Adds a filter restriction on <paramref name="criteria"/>
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchParams"></param>
        void AddCriteriaRestriction(ICriteria criteria, PSSUserReportSearchParams searchParams);
    }
}
