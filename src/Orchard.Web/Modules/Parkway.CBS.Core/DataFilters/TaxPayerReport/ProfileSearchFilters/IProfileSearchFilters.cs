using NHibernate;
using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.DataFilters.TaxPayerReport.ProfileSearchFilters
{
    public interface IProfileSearchFilters : IDependency
    {
        /// <summary>
        /// Filter name
        /// </summary>
        string FilterName { get; }

        /// <summary>
        /// Add criteria restrictions
        /// </summary>
        /// <param name="criteria"></param>
        void AddCriteriaRestriction(ICriteria criteria, TaxProfilesSearchParams searchParams);
    }
}
