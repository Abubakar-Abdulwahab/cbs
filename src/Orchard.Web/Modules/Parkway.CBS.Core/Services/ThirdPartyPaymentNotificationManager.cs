using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Services
{
    public class ThirdPartyPaymentNotificationManager : BaseManager<ThirdPartyPaymentNotification>, IThirdPartyPaymentNotificationManager<ThirdPartyPaymentNotification>
    {
        private readonly IRepository<ThirdPartyPaymentNotification> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public ThirdPartyPaymentNotificationManager(IRepository<ThirdPartyPaymentNotification> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }

    }
}