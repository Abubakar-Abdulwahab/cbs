using NHibernate;
using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.DataFilters.CBSUserTaxEntityProfileLocationReport.SearchFilters.Contracts
{
    public interface ICBSUserTaxEntityProfileLocationSearchFilter : IDependency
    {
        /// <summary>
        /// Add criteria restrictions
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchParams"></param>
        void AddCriteriaRestriction(ICriteria criteria, CBSUserTaxEntityProfileLocationReportSearchParams searchParams);
    }
}
