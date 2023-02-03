using Orchard;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.DataFilters.OfficerDeploymentAllowance.Contracts
{
    public interface IOfficerDeploymentAllowanceFilter : IDependency
    {
        /// <summary>
        /// Get veiw model for deployment allowance requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        dynamic GetRequestReportViewModel(OfficerDeploymentAllowanceSearchParams searchParams, bool applyAccessRestrictions);
    }
}
