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
    public class PaymentReferenceManager : BaseManager<PaymentReference>, IPaymentReferenceManager<PaymentReference>
    {
        private readonly IRepository<PaymentReference> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public PaymentReferenceManager(IRepository<PaymentReference> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }

    }
}