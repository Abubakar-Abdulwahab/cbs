using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSFeePartyDAOManager : IRepository<PSSFeeParty>
    {
        /// <summary>
        /// Get fee parties
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        IEnumerable<PSSFeePartyVM> GetFeeParties(int skip, int take);
    }
}
