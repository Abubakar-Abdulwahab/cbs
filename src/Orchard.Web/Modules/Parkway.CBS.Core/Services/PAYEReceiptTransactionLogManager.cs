using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Core.Services
{
    public class PAYEReceiptTransactionLogManager : BaseManager<PAYEReceiptTransactionLog>, IPAYEReceiptTransactionLogManager<PAYEReceiptTransactionLog>
    {
        private readonly IRepository<PAYEReceiptTransactionLog> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PAYEReceiptTransactionLogManager(IRepository<PAYEReceiptTransactionLog> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }
    }
}