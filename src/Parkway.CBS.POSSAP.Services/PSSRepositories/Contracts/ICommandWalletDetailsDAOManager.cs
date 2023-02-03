using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface ICommandWalletDetailsDAOManager : IRepository<CommandWalletDetails>
    {
        /// <summary>
        /// Get command wallet details
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        IEnumerable<CommandWalletDetailsVM> GetCommandWalletDetails(int skip, int take);
    }
}
