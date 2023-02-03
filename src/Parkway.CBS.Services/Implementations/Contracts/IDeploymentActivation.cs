using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Services.Implementations.Contracts
{
    public interface IDeploymentActivation
    {
        /// <summary>
        /// This activates officer deployment by setting the active to true and status to running at the start date of the deployment
        /// </summary>
        /// <returns></returns>
        string ProcessDeploymentActivation(string tenantName);
    }
}
