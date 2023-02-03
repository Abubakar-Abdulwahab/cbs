using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IWalletStatementScheduleUpdateDAOManager : IRepository<WalletStatementScheduleUpdate>
    {
        /// <summary>
        /// Updates wallet statement schedule date
        /// </summary>
        /// <param name="walletStatementScheduleId"></param>
        /// <param name="scheduleUpdateId"></param>
        void UpdateScheduleDate(int walletStatementScheduleId, int scheduleUpdateId);
    }
}
