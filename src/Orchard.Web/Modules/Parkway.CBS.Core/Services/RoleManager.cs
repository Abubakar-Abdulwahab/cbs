using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Services
{
    public class RoleManager : BaseManager<AccessRole>, IRoleManager<AccessRole>
    {
        private readonly IRepository<AccessRole> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public RoleManager(IRepository<AccessRole> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _transactionManager = orchardServices.TransactionManager;
        }

    }
}