using NHibernate;
using Orchard;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.DataFilters.OfficerDeployment.SearchFilters
{
    public interface IOfficerDeploymentReportFilters : IDependency
    {
        void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentSearchParams searchParams, bool addAlias = false);
    }
}
