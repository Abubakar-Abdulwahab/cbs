using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class CommandWalletDetailsDAOManager : Repository<CommandWalletDetails>, ICommandWalletDetailsDAOManager
    {
        public CommandWalletDetailsDAOManager(IUoW uow) : base(uow)
        { }

        /// <summary>
        /// Get command wallet details
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public IEnumerable<CommandWalletDetailsVM> GetCommandWalletDetails(int skip, int take)
        {
            return _uow.Session.Query<CommandWalletDetails>()
                .Where(x => x.IsActive)
                .Skip(skip)
                .Take(take)
                .Select(x => new CommandWalletDetailsVM { Id = x.Id, AccountNumber = x.AccountNumber });
        }
    }
}
