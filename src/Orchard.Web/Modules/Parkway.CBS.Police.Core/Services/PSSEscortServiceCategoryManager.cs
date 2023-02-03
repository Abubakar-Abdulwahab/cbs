using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSEscortServiceCategoryManager : BaseManager<PSSEscortServiceCategory>, IPSSEscortServiceCategoryManager<PSSEscortServiceCategory>
    {
        private readonly IRepository<PSSEscortServiceCategory> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public PSSEscortServiceCategoryManager(IRepository<PSSEscortServiceCategory> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            _user = user;
        }

        /// <summary>
        /// Gets all active escort service categories
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PSSEscortServiceCategoryVM> GetEscortServiceCategories()
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSEscortServiceCategory>().Where(x => x.Parent == null && x.IsActive).Select(x => new PSSEscortServiceCategoryVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    MinimumRequiredOfficers = x.MinimumRequiredOfficers,
                    ShowExtraFields = x.ShowExtraFields,
                }).ToFuture();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets an active escort service category with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PSSEscortServiceCategoryVM GetEscortServiceCategoryWithId(int id)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSEscortServiceCategory>().Where(x => x.Id == id && x.IsActive).Select(x => new PSSEscortServiceCategoryVM
                {
                    Id = x.Id,
                    ParentId = (x.Parent != null) ? x.Parent.Id : 0,
                    Name = x.Name,
                    ParentName = (x.Parent != null) ? x.Parent.Name : string.Empty,
                    MinimumRequiredOfficers = x.MinimumRequiredOfficers,
                    ShowExtraFields = x.ShowExtraFields,
                }).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets category types for service category with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<PSSEscortServiceCategoryVM> GetCategoryTypesForServiceCategoryWithId(int id)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSEscortServiceCategory>().Where(x => x.Parent == new PSSEscortServiceCategory { Id = id }).Select(x => new PSSEscortServiceCategoryVM
                {
                    Id = x.Id,
                    ParentId = (x.Parent != null) ? x.Parent.Id : 0,
                    Name = x.Name,
                    MinimumRequiredOfficers = x.MinimumRequiredOfficers,
                    ShowExtraFields = x.ShowExtraFields,
                }).ToFuture();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}