using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services
{
    public class PAYEBatchItemsRefManager : BaseManager<PAYEAPIBatchItemsRef>, IPAYEBatchItemsRefManager<PAYEAPIBatchItemsRef>
    {
        private readonly IRepository<PAYEAPIBatchItemsRef> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PAYEBatchItemsRefManager(IRepository<PAYEAPIBatchItemsRef> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }
    }
}