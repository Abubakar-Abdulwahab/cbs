using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.POSSAP.Scheduler.Models;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSStateModelExternalDataStateDAOManager : IRepository<PSSStateModelExternalDataState>
    {
        /// <summary>
        /// Get the last CallLogForExternalSystem_Id
        /// </summary>
        /// <returns> <see cref="long"/></returns>
        long? GetLastCallLogForExternalSystemId();
    }
}
