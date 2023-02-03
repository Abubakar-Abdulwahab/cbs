using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class CommandDAOManager : Repository<Command>, ICommandDAOManager
    {
        public CommandDAOManager(IUoW uow) : base(uow)
        {

        }

        public CommandVM GetCommandWithCode(string code)
        {
            return _uow.Session.Query<Command>().Where(cm => cm.Code == code)
               .Select(cm => new CommandVM { Id = cm.Id, Code = cm.Code, Name = cm.Name }).SingleOrDefault(); ;
        }
    }
}
