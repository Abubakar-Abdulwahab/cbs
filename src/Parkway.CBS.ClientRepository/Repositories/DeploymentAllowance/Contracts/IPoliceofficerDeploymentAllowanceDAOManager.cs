using Parkway.CBS.ClientRepository.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.DeploymentAllowance.Contracts
{
    public interface IPoliceofficerDeploymentAllowanceDAOManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deploymentAllowanceVM"></param>
        void SaveAllowanceRequest(DeploymentAllowanceVM deploymentAllowanceVM);
    }
}
