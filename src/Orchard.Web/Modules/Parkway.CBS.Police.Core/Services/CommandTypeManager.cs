using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class CommandTypeManager : BaseManager<CommandType>, ICommandTypeManager<CommandType>
    {
        private readonly IRepository<CommandType> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public CommandTypeManager(IRepository<CommandType> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Gets all active command types
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CommandTypeVM> GetCommandTypes()
        {
            return _transactionManager.GetSession().Query<CommandType>().Where(x => x.IsActive && x.IsVisible).Select(x => new CommandTypeVM
            {
                Id = x.Id,
                Name = x.Name
            });
        }


        /// <summary>
        /// Gets command type with specified id
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        public CommandTypeVM GetCommandType(int commandTypeId)
        {
            return _transactionManager.GetSession().Query<CommandType>().Where(x => x.IsActive && x.IsVisible && x.Id == commandTypeId).Select(x => new CommandTypeVM
            {
                Id = x.Id,
                Name = x.Name
            }).SingleOrDefault();
        }
    }
}