using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPoliceofficerDeploymentAllowanceManager<PoliceofficerDeploymentAllowance> : IDependency, IBaseManager<PoliceofficerDeploymentAllowance>
    {
        /// <summary>
        /// Get the deployment allowance view details using deployment allowance request id
        /// </summary>
        /// <param name="deploymentAllowanceRequestId"></param>
        /// <returns>EscortDeploymentRequestDetailsVM</returns>
        EscortDeploymentRequestDetailsVM GetRequestViewDetails(long deploymentAllowanceRequestId);

        /// <summary>
        /// Update the transaction status for a particular deployment allowance payment reference
        /// </summary>
        /// <param name="reference"></param>
        void UpdateDeploymentAllowanceTransactionStatus(string reference, int status);
    }
}
