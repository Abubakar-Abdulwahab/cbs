using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.OSGOF.Admin.Services.Contracts
{
    public interface ICellSiteScheduleStagingManager<CellSitesScheduleStaging> : IDependency, IBaseManager<CellSitesScheduleStaging>
    {
        /// <summary>
        /// Get details for this schedule
        /// </summary>
        /// <param name="scheduleBatchRef"></param>
        /// <returns>CellSitesScheduleVM</returns>
        CellSitesScheduleVM GetScheduleDetails(string scheduleBatchRef);

        /// <summary>
        /// Get details for this schedule
        /// </summary>
        /// <param name="scheduleBatchRef"></param>
        /// <returns>CellSitesScheduleVM</returns>
        CellSitesScheduleStagingVM GetScheduleStatgingDetails(string scheduleRef);
    }
}
