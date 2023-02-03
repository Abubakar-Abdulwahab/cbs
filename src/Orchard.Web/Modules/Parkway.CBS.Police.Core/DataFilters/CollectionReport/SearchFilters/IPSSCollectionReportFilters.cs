using NHibernate;
using Orchard;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.DataFilters.CollectionReport.SearchFilters
{
    public interface IPSSCollectionReportFilters : IDependency
    {
        /// <summary>
        /// Add criteria restrictions
        /// <para>throws exception if Id for associate tables in PSServiceRequest are not found</para>
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchParams"></param>
        /// <exception cref="NoRecordFoundException"></exception>
        void AddCriteriaRestriction(ICriteria criteria, PSSCollectionSearchParams searchParams);
    }
}
