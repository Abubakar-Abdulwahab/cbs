using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSProposedRegularizationUnknownPoliceOfficerDeploymentLogManager<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog> : IDependency, IBaseManager<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog>
    {
        /// <summary>
        /// Gets officers log for the request with the specified id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="invoiceId"></param>
        /// <returns>IEnumerable<RegularizationRequestWithoutOfficersUploadBatchItemsStagingDTO></returns>
        IEnumerable<RegularizationRequestWithoutOfficersUploadBatchItemsStagingDTO> GetEscortRegularizationOfficerDeployment(long requestId, long invoiceId);
    }
}
