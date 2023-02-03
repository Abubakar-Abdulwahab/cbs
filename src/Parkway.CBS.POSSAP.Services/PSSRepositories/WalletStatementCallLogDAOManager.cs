using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class WalletStatementCallLogDAOManager : Repository<WalletStatementCallLog>, IWalletStatementCallLogDAOManager
    {
        public WalletStatementCallLogDAOManager(IUoW uow) : base(uow)
        { }
    }
}
