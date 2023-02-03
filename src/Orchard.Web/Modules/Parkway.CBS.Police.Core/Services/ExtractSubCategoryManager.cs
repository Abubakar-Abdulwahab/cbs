using Orchard;
using System.Linq;
using Orchard.Data;
using NHibernate.Linq;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Services.Contracts;


namespace Parkway.CBS.Police.Core.Services
{
    public class ExtractSubCategoryManager : BaseManager<ExtractSubCategory>, IExtractSubCategoryManager<ExtractSubCategory>
    {
        private readonly IRepository<ExtractSubCategory> _repo;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ExtractSubCategoryManager(IRepository<ExtractSubCategory> repo, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repo, user, orchardServices)
        {
            _repo = repo;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }



        /// <summary>
        /// Get the sub category with the given Id
        /// </summary>
        /// <param name="catId"></param>
        /// <param name="subCatId"></param>
        /// <returns>ExtractSubCategoryVM</returns>
        public ExtractSubCategoryVM GetSubCategory(int subCatId, int subCatId1)
        {
            return _transactionManager.GetSession().Query<ExtractSubCategory>()
          .Where(sr => sr == new ExtractSubCategory { Id = subCatId })
          .Select(r => new ExtractSubCategoryVM
          {
              Category = new ExtractCategoryVM { FreeForm = r.ExtractCategory.FreeForm, Id = r.ExtractCategory.Id, Name = r.ExtractCategory.Name },
              Name = r.Name,
              Id = r.Id,
              FreeForm = r.FreeForm
          }).ToList().SingleOrDefault();
        }
    }
}