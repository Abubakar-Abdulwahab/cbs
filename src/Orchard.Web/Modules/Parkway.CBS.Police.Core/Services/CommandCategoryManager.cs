using NHibernate.Criterion;
using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Services
{
    public class CommandCategoryManager : BaseManager<CommandCategory>, ICommandCategoryManager<CommandCategory>
    {
        private readonly IRepository<CommandCategory> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public CommandCategoryManager(IRepository<CommandCategory> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// Get list of command categories
        /// </summary>
        /// <returns>IEnumerable<CommandCategoryVM></returns>
        public IEnumerable<CommandCategoryVM> GetCategories()
        {
            return _transactionManager.GetSession().Query<CommandCategory>()
                  .Select(c => new CommandCategoryVM
                  {
                      Id = c.Id,
                      Name = c.Name
                  }).OrderBy(x => x.Id);
        }


        /// <summary>
        /// Gets list of command categories with no parents i.e the parent command categories
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CommandCategoryVM> GetParentCategories()
        {
            return _transactionManager.GetSession().Query<CommandCategory>()
                    .Where(x => x.ParentCommandCategory == null)
                  .Select(c => new CommandCategoryVM
                  {
                      Id = c.Id,
                      Name = c.Name
                  }).OrderBy(x => x.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IEnumerable<CommandCategory> GetCategory(int categoryId)
        {
            var session = _transactionManager.GetSession();
            return session.CreateCriteria<CommandCategory>().Add(Restrictions.Eq("Id", categoryId)).Future<CommandCategory>();
        }
    }
}