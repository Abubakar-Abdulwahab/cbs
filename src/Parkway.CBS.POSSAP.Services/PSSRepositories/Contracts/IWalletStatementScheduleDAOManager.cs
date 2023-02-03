using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IWalletStatementScheduleDAOManager : IRepository<WalletStatementSchedule>
    {
        /// <summary>
        /// Gets first or default schedule
        /// </summary>
        /// <returns></returns>
        WalletStatementScheduleVM GetSchedule();
    }
}
