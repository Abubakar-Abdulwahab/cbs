using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Core.Services
{
    public class NotificationMessageLogManager : BaseManager<NotificationMessageLog>, INotificationMessageLogManager<NotificationMessageLog>
    {
        private readonly IRepository<NotificationMessageLog> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public NotificationMessageLogManager(IRepository<NotificationMessageLog> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }
    }
}