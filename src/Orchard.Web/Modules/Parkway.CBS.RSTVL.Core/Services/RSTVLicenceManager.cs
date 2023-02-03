using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.RSTVL.Core.Models;
using Parkway.CBS.RSTVL.Core.Services.Contracts;

namespace Parkway.CBS.RSTVL.Core.Services
{
    public class RSTVLicenceManager : BaseManager<RSTVLicence>, IRSTVLicenceManager<RSTVLicence>
    {
        private readonly IRepository<RSTVLicence> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public RSTVLicenceManager(IRepository<RSTVLicence> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }

    }
}