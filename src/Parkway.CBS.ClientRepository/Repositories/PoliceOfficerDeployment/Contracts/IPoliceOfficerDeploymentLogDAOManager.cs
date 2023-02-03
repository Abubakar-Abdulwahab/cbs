using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.PoliceOfficerDeployment.Contracts
{
    public interface IPoliceOfficerDeploymentLogDAOManager
    {
        /// <summary>
        /// This activates officer deployment by setting the active to true and status to running at the start date of the deployment
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        string ActivateDeployment(string today);

        /// <summary>
        /// This deactivates officer deployment by setting the active to false and status to completed at the end date of the deployment
        /// </summary>
        /// <param name="yesterday"></param>
        /// <returns></returns>
        string DeactivateDeployment(string yesterday);
    }
}
