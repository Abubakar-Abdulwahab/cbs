using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Services.Implementations.Contracts
{
    public interface IDeploymentDeactivation
    {
        /// <summary>
        /// This deactivates officer deployment by setting the active to false and status to completed at the end date of the deployment
        /// </summary>
        /// <returns></returns>
        string ProcessDeploymentDeactivation(string tenantName);
    }
}
