using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementFeePartyDAOManager : IRepository<PSSSettlementFeeParty>
    {
        /// <summary>
        /// Get active settlement adapters
        /// </summary>
        /// <param name="settlementId"></param>
        /// <returns>List<string></returns>
        List<string> GetActiveSettlementAdapters(int settlementId);
    }
}
