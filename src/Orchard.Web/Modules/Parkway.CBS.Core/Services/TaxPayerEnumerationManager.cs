using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Core.Services
{
    public class TaxPayerEnumerationManager : BaseManager<TaxPayerEnumeration>, ITaxPayerEnumerationManager<TaxPayerEnumeration>
    {
        private readonly IRepository<TaxPayerEnumeration> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public TaxPayerEnumerationManager(IRepository<TaxPayerEnumeration> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;

        }
    }
}