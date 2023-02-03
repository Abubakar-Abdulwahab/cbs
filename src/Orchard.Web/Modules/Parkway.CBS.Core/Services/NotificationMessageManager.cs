using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;

namespace Parkway.CBS.Core.Services
{
    public class NotificationMessageManager : BaseManager<NotificationMessage>, INotificationMessageManager<NotificationMessage>
    {
        private readonly IRepository<NotificationMessage> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public NotificationMessageManager(IRepository<NotificationMessage> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
        }

    }
}