using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class WalletStatementScheduleLogDAOManager : Repository<WalletStatementScheduleLog>, IWalletStatementScheduleLogDAOManager
    {
        public WalletStatementScheduleLogDAOManager(IUoW uow) : base(uow)
        { }
    }
}
