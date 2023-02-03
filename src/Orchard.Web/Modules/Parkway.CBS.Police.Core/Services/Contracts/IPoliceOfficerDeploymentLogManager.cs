using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog> : IDependency, IBaseManager<PoliceOfficerDeploymentLog>
    {
        /// <summary>
        /// Get police officer deployment log vm with specified deployment log id
        /// </summary>
        /// <param name="deploymentLogId"></param>
        /// <returns></returns>
        PoliceOfficerDeploymentLogVM GetPoliceOfficerDeploymentLogVM(Int64 deploymentLogId);

        /// <summary>
        /// Get count for number of active deployed officers
        /// </summary>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        IEnumerable<ReportStatsVM> GetActiveDeployedPoliceOfficer();

        /// <summary>
        /// Gets the number of officers assigned to the request with the specified file number
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>int</returns>
        int GetEscortOfficerAssignedNumber(string fileNumber);

    }
}
