using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSServiceStateManager : BaseManager<PSServiceState>, IPSServiceStateManager<PSServiceState>
    {
        private readonly IRepository<PSServiceState> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSServiceStateManager(IRepository<PSServiceState> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }
    }
}