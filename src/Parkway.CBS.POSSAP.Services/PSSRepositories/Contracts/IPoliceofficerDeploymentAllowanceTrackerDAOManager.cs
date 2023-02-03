using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPoliceofficerDeploymentAllowanceTrackerDAOManager : IRepository<PoliceofficerDeploymentAllowanceTracker>
    {
        /// <summary>
        /// Get paginated records of all the deployment allowance that is due for the day
        /// </summary>
        /// <param name="chunkSize"></param>
        /// <param name="skip"></param>
        /// <param name="today"></param>
        /// <returns>List<PSSDeploymentAllowanceTrackerVM></returns>
        List<PSSDeploymentAllowanceTrackerVM> GetBatchDueDeploymentAllowance(int chunkSize, int skip, DateTime today);
    }
}
