using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IWalletStatementDAOManager : IRepository<WalletStatement>
    {
        /// <summary>
        /// Moves wallet statements with specified reference from staging table to main table
        /// </summary>
        /// <param name="reference"></param>
        void MoveStatementsFromStagingToMain(string reference);
    }
}
