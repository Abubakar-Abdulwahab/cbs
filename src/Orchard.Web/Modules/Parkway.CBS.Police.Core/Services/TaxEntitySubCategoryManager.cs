using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class TaxEntitySubCategoryManager : BaseManager<TaxEntitySubCategory>, ITaxEntitySubCategoryManager<TaxEntitySubCategory>
    {
        private readonly IRepository<TaxEntitySubCategory> _repo;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public TaxEntitySubCategoryManager(IRepository<TaxEntitySubCategory> repo, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repo, user, orchardServices)
        {
            _repo = repo;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }


        public IEnumerable<TaxEntitySubCategoryVM> GetActiveTaxEntitySubCategoryByCategoryId(int categoryId)
        {
            return GetCollection().Where(x => x.TaxEntityCategory.Id == categoryId && x.IsActive == true).Select(x => new TaxEntitySubCategoryVM { Id = x.Id, Name = x.Name, IsActive = x.IsActive }).ToList();
        }


        /// <summary>
        /// Check if subcategory exists
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryId"></param>
        /// <returns>bool</returns>
        public bool SubCategoryExists(int id, int categoryId)
        {
            return _transactionManager.GetSession().Query<TaxEntitySubCategory>().Count(s => ((s.Id == id) && (s.TaxEntityCategory == new CBS.Core.Models.TaxEntityCategory { Id = categoryId }))) == 1;
        }

    }
}