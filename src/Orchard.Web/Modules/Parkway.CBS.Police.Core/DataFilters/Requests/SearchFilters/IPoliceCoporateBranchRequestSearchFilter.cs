using NHibernate;
using NHibernate.Criterion;
using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.Requests.SearchFilters
{
    public interface IPoliceCoporateBranchRequestSearchFilter : IDependency
    {
        /// <summary>
        /// Add criteria restrictions
        /// <para>throws exception if Id for associate tables in PSServiceRequest are not found</para>
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchParams"></param>
        /// <exception cref="NoRecordFoundException"></exception>
        void AddCriteriaRestriction(ICriteria criteria, RequestsReportSearchParams searchParams);


        /// <summary>
        /// Add criteria restrictions
        /// <para>throws exception if Id for associate tables in PSServiceRequest are not found</para>
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchParams"></param>
        /// <exception cref="NoRecordFoundException"></exception>
        void AddDetachedCriteriaRestriction(DetachedCriteria criteria, RequestsReportSearchParams searchParams);
    }
}
