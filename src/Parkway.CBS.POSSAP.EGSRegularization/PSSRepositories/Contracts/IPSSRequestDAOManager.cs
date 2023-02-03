using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModels;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IPSSRequestDAOManager : IRepository<PSSRequest>
    {
        /// <summary>
        /// Gets pss request with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PSSRequestTaxEntityViewVM GetPSSRequest(long id);
    }
}
