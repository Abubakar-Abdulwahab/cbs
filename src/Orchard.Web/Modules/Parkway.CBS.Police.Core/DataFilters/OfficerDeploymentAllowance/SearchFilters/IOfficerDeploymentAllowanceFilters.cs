using NHibernate;
using Orchard;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.DataFilters.OfficerDeploymentAllowance.SearchFilters
{
    public interface IOfficerDeploymentAllowanceFilters : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchParams"></param>
        /// <param name="addAlias"></param>
        void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentAllowanceSearchParams searchParams);
    }
}
