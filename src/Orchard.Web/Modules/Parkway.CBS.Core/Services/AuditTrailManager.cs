using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Core.Services
{
    public class AuditTrailManager : BaseManager<AuditTrail>, IAuditTrailManager<AuditTrail>
    {
        private readonly IRepository<AuditTrail> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;

        public AuditTrailManager(IRepository<AuditTrail> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
        }
    }
}