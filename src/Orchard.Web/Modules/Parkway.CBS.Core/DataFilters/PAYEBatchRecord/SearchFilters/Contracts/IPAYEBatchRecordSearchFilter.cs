using NHibernate;
using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.DataFilters.PAYEBatchRecord.SearchFilters.Contracts
{
    public interface IPAYEBatchRecordSearchFilter : IDependency
    {
        /// <summary>
        /// Add criteria restrictions
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchParams"></param>
        void AddCriteriaRestriction(ICriteria criteria, PAYEBatchRecordSearchParams searchParams);
    }
}
