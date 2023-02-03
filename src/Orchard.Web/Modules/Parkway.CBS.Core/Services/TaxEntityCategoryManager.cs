using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Core.Services
{
    public class TaxEntityCategoryManager : BaseManager<TaxEntityCategory>, ITaxEntityCategoryManager<TaxEntityCategory>
    {
        private readonly IRepository<TaxEntityCategory> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly ITransactionManager _transactionManager;

        public TaxEntityCategoryManager(IRepository<TaxEntityCategory> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Get the list of categories
        /// </summary>
        /// <returns>IEnumerable{TaxEntityCategoryVM}</returns>
        public IEnumerable<TaxEntityCategoryVM> GetCategories()
        {
            return _transactionManager.GetSession()
                .CreateCriteria<TaxEntityCategory>()
                .Add(Restrictions.Where<TaxEntityCategory>(c => c.Status))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property<TaxEntityCategoryVM>(m => m.Name), "Name")
                .Add(Projections.Property<TaxEntityCategoryVM>(m => m.Id), "Id")
                .Add(Projections.Property<TaxEntityCategoryVM>(m => m.JSONSettings), "JSONSettings"))
                .SetResultTransformer(Transformers.AliasToBean<TaxEntityCategoryVM>())
                .Future<TaxEntityCategoryVM>();
        }


        /// <summary>
        /// Get the list of categories
        /// </summary>
        /// <returns>List{TaxEntityCategoryVM}</returns>
        public List<TaxEntityCategoryVM> GetTaxEntityCategoryVM()
        {
            var session = _transactionManager.GetSession();
            return session.Query<TaxEntityCategory>().Where(catty => catty.Status)
                .Select( catty => new TaxEntityCategoryVM
                {
                    Id = catty.Id,
                    Status = catty.Status,
                    Identifier = catty.Identifier,
                    Name = catty.Name,
                    RequiresLogin = catty.RequiresLogin,
                    StringIdentifier = catty.StringIdentifier,
                    JSONSettings = catty.JSONSettings,
                }
                ).ToList();
        }


        /// <summary>
        /// Get tax category
        /// <para>Returns a partially populated object</para>
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>TaxEntityCategoryVM</returns>
        public TaxEntityCategoryVM GetTaxEntityCategoryVM(int categoryId)
        {
            return _transactionManager.GetSession().Query<TaxEntityCategory>().Where(catty => catty == new TaxEntityCategory { Id = categoryId })
                .Select(catty => new TaxEntityCategoryVM
                {
                    Id = catty.Id,
                    RequiresLogin = catty.RequiresLogin,
                    JSONSettings = catty.JSONSettings,
                }
                ).ToList().FirstOrDefault();
        }



        /// <summary>
        /// Get first category Id
        /// </summary>
        /// <returns>int</returns>
        public int GetFirstCategoryId()
        {
            return _transactionManager.GetSession().Query<TaxEntityCategory>().First().Id;
        }

    }
}