using NHibernate;
using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.DataFilters.PAYEReceiptUtilizationReport.SearchFilters.Contracts
{
    public interface IPAYEReceiptUtilizationSearchFilter : IDependency
    {
        /// <summary>
        /// Add criteria restrictions
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchParams"></param>
        void AddCriteriaRestriction(ICriteria criteria, PAYEReceiptSearchParams searchParams);
    }
}
