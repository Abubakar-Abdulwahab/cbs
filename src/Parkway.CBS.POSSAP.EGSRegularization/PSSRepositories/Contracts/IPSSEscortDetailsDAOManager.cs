using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IPSSEscortDetailsDAOManager : IRepository<PSSEscortDetails>
    {
        /// <summary>
        /// Gets escort details with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EscortDetailsDTO GetEscortDetails(long id);
    }
}
