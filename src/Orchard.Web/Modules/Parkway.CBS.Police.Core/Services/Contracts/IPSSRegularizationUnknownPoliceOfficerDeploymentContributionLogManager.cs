using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSRegularizationUnknownPoliceOfficerDeploymentContributionLogManager<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog> : IDependency, IBaseManager<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog>
    {
        /// <summary>
        /// Gets tPSSRegularizationUnknownPoliceOfficerDeploymentContributionLogs
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="invoiceId"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        List<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLogDTO> GetPSSRegularizationUnknownPoliceOfficerDeploymentContributionLogs(long requestId, long invoiceId, int commandId);


        /// <summary>
        /// Gets deployment contribution log using specified parameters
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="invoiceId"></param>
        /// <param name="commandId"></param>
        /// <param name="commandTypeId"></param>
        /// <param name="dayTypeId"></param>
        /// <returns></returns>
        PSSRegularizationUnknownPoliceOfficerDeploymentContributionLogDTO GetDeploymentContributionLog(long requestId, long invoiceId, int commandId, int commandTypeId, int dayTypeId);
    }
}
