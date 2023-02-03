using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using System.Collections.Concurrent;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IDeploymentAllowanceDAOManager : IRepository<PoliceofficerDeploymentAllowance>
    {
        /// <summary>
        /// Save bundle of records
        /// </summary>
        /// <param name="deploymentAllowances"></param>
        /// <param name="batchLimit"></param>
        int SaveRecords(ConcurrentQueue<PoliceOfficerDeploymentAllowanceVM> deploymentAllowances, int batchLimit);
    }
}
