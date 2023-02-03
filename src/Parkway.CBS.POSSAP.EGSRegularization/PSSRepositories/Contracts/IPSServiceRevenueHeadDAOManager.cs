using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IPSServiceRevenueHeadDAOManager : IRepository<PSServiceRevenueHead>
    {
        /// <summary>
        /// Gets revenue heads with specified service id and flow definition level id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="isApplicationInvoice"></param>
        /// <returns></returns>
        IEnumerable<PSServiceRevenueHeadVM> GetRevenueHead(int serviceId, int levelId);
    }
}
