using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Core.Services
{
    public class TaxCategoryPermissionsManager : BaseManager<TaxCategoryPermissions>, ITaxCategoryPermissionsManager<TaxCategoryPermissions>
    {
        private readonly IRepository<TaxCategoryPermissions> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public TaxCategoryPermissionsManager(IRepository<TaxCategoryPermissions> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
        }
    }
}