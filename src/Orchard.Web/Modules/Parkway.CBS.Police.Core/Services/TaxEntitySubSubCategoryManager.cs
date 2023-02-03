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
    public class TaxEntitySubSubCategoryManager : BaseManager<TaxEntitySubSubCategory>, ITaxEntitySubSubCategoryManager<TaxEntitySubSubCategory>
    {
        private readonly IRepository<TaxEntitySubSubCategory> _repo;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public TaxEntitySubSubCategoryManager(IRepository<TaxEntitySubSubCategory> repo, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repo, user, orchardServices)
        {
            _repo = repo;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }


        public IEnumerable<TaxEntitySubSubCategoryVM> GetActiveTaxEntitySubSubCategoryBySubCategoryId(int subCategoryId)
        {
            return GetCollection().Where(x => x.TaxEntitySubCategory.Id == subCategoryId && x.IsActive == true && x.IsDefault == false).Select(x => new TaxEntitySubSubCategoryVM { Id = x.Id, Name = x.Name, IsActive = x.IsActive }).ToList();
        }


        public TaxEntitySubCategoryVM GetActiveDefaultTaxEntitySubCategoryById(int subCategoryId)
        {
            return _transactionManager.GetSession().Query<TaxEntitySubSubCategory>().Where(x => x.TaxEntitySubCategory.Id == subCategoryId && x.IsActive == true && x.IsDefault == true).Select(x => new TaxEntitySubCategoryVM { Id = x.TaxEntitySubCategory.Id, CategoryId = x.TaxEntitySubCategory.TaxEntityCategory.Id, SubSubTaxEntityCategoryId = x.Id }).FirstOrDefault();
        }

        public TaxEntitySubSubCategoryVM GetActiveDefaultTaxEntitySubSubCategoryById(int subCategoryId)
        {
            return GetCollection().Where(x => x.TaxEntitySubCategory.Id == subCategoryId && x.IsActive == true && x.IsDefault == true).Select(x => new TaxEntitySubSubCategoryVM { Id = x.Id }).FirstOrDefault();
        }


        /// <summary>
        /// Check that this sub sub category has a relationship to subcategory
        /// </summary>
        /// <param name="id"></param>
        /// <param name="subCategoryId"></param>
        /// <returns>bool</returns>
        public bool CheckIfExists(int id, int subCategoryId)
        {
            return _transactionManager.GetSession().Query<TaxEntitySubSubCategory>().Count(s => ((s.Id == id) && (s.TaxEntitySubCategory == new TaxEntitySubCategory { Id = subCategoryId }))) == 1;
        }

    }
}