using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services
{
    public class IdentificationTypeFileAttachmentManager : BaseManager<IdentificationTypeFileAttachment>, IIdentificationTypeFileAttachmentManager<IdentificationTypeFileAttachment>
    {
        private readonly IRepository<IdentificationTypeFileAttachment> _repo;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public IdentificationTypeFileAttachmentManager(IRepository<IdentificationTypeFileAttachment> repo, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repo, user, orchardServices)
        {
            _repo = repo;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }
    }
}