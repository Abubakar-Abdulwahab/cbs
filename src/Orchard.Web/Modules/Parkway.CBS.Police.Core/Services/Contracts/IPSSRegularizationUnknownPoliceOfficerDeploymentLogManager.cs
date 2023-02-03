using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSRegularizationUnknownPoliceOfficerDeploymentLogManager<PSSRegularizationUnknownPoliceOfficerDeploymentLog> : IDependency, IBaseManager<PSSRegularizationUnknownPoliceOfficerDeploymentLog>
    {
        /// <summary>
        /// Gets regularization unknown police officer deployment log with specified command type id, day type id, command id, request id and invoice id
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <param name="dayTypeId"></param>
        /// <param name="commandId"></param>
        /// <param name="requestId"></param>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        PSSRegularizationUnknownPoliceOfficerDeploymentLogDTO GetPSSRegularizationUnknownPoliceOfficerDeploymentLog(int commandTypeId, int dayTypeId, int commandId, long requestId, long invoiceId);
    }
}
