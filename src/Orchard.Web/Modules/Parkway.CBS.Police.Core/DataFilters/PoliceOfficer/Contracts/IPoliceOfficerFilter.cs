using Orchard;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.DataFilters.PoliceOfficerReport.Contracts
{
    public interface IPoliceOfficerFilter : IDependency
    {

        /// <summary>
        /// Get veiw model for request reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalNumberOfActiveOfficers }</returns>
        dynamic GetRequestReportViewModel(PoliceOfficerSearchParams searchParams, bool applyAccessRestrictions);


    }
}
