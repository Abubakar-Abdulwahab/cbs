using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementRequestTransactionConfigCommandZonalCommandRatioDAOManager : IRepository<PSSSettlementRequestTransactionConfigZonalCommandRatio>
    {

        /// <summary>
        /// Do split ratio for zonal command records
        /// </summary>
        /// <param name="batchId"></param>
        void DoSplitRatioForZonalCommand(long batchId);


        /// <summary>
        /// Update fall flag for zonal command
        /// </summary>
        /// <param name="batchId"></param>
        void SetZonalCommandWithFallFlag(long batchId);

    }
}
