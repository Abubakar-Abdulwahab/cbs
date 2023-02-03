using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSHangfireSettlementReferenceDAOManager : IRepository<PSSHangfireSettlementReference>
    {
        /// <summary>
        /// save a list of hang fire references
        /// </summary>
        /// <param name="hangfireRefs"></param>
        void SaveBundle(IList<PSSHangfireSettlementReference> hangfireRefs);

    }
}
