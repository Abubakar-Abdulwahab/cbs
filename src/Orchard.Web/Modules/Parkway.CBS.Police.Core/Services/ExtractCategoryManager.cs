using Orchard;
using System.Linq;
using Orchard.Data;
using NHibernate.Linq;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services
{
    public class ExtractCategoryManager : BaseManager<ExtractCategory>, IExtractCategoryManager<ExtractCategory>
    {
        private readonly IRepository<ExtractCategory> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ExtractCategoryManager(IRepository<ExtractCategory> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Get active extract categories
        /// </summary>
        /// <returns>IEnumerable{ExtractCategoryVM}</returns>
        public IEnumerable<ExtractCategoryVM> GetActiveCategories()
        {
            return _transactionManager.GetSession().Query<ExtractCategory>()
           .Where(x => x.IsActive).OrderBy(sr => sr.Name)
           .Select(sr => new ExtractCategoryVM
           {
               Name = sr.Name,
               Id = sr.Id,
               FreeForm = sr.FreeForm,
           }).ToFuture();
        }


        /// <summary>
        /// Get active sub categories for this category Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>ExtractCategoryVM</returns>
        public ExtractCategoryVM GetActiveSubCategories(int categoryId)
        {
            return _transactionManager.GetSession().Query<ExtractCategory>()
           .Where(sr => ((sr == new ExtractCategory { Id = categoryId }) && (sr.IsActive)))
           .Select(r => new ExtractCategoryVM
           {
               Name = r.Name,
               FreeForm = r.FreeForm,
               SubCategories = r.SubCategories.Select(sub => new ExtractSubCategoryVM { Id = sub.Id, Name = sub.Name, FreeForm = sub.FreeForm })
           }).ToList().FirstOrDefault();
        }


        /// <summary>
        /// Get active sub categories for this category Id. Uses future queries for batching multiple calls
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>ExtractCategoryVM</returns>
        public IEnumerable<ExtractCategoryVM> GetActiveSubCategoriesList(int categoryId)
        {
            return _transactionManager.GetSession().Query<ExtractCategory>()
           .Where(sr => ((sr == new ExtractCategory { Id = categoryId }) && (sr.IsActive)))
           .Select(r => new ExtractCategoryVM
           {
               Id = r.Id,
               Name = r.Name,
               FreeForm = r.FreeForm,
               SubCategories = r.SubCategories.Select(sub => new ExtractSubCategoryVM { Id = sub.Id, Name = sub.Name, FreeForm = sub.FreeForm })
           }).ToFuture();
        }

    }
}