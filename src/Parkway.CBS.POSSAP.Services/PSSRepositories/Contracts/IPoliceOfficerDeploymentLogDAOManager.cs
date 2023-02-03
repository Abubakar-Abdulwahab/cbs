using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPoliceOfficerDeploymentLogDAOManager : IRepository<PoliceOfficerDeploymentLog>
    {
        /// <summary>
        /// Get the list of deployed officers for a specified request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>List<PoliceOfficerDeploymentVM></returns>
        List<PoliceOfficerDeploymentVM> GetDeployedOfficerForRequest(long requestId);
    }
}
