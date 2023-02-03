using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Core.Services
{
    public class UnreconciledPayePaymentsManager : BaseManager<UnreconciledPayePayments>, IUnreconciledPayePaymentsManager<UnreconciledPayePayments>
    {
        private readonly IRepository<UnreconciledPayePayments> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public UnreconciledPayePaymentsManager(IRepository<UnreconciledPayePayments> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _transactionManager = orchardServices.TransactionManager;
        }
    }
}