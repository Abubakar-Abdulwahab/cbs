using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class TaxCategoryTaxCategoryPermissionsManager : BaseManager<TaxCategoryTaxCategoryPermissions>, ITaxCategoryTaxCategoryPermissionsManager<TaxCategoryTaxCategoryPermissions>
    {
        private readonly IRepository<TaxCategoryTaxCategoryPermissions> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public TaxCategoryTaxCategoryPermissionsManager(IRepository<TaxCategoryTaxCategoryPermissions> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Gets all active tax category permissions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TaxCategoryTaxCategoryPermissionsVM> GetTaxCategoryPermissions()
        {
            try
            {
                return _transactionManager.GetSession().Query<TaxCategoryTaxCategoryPermissions>()
                    .Where(x => x.TaxCategoryPermissions.IsActive)
                    .GroupBy(x => x.TaxEntityCategory.Id)
                    .ToList()
                    .Select(x => new TaxCategoryTaxCategoryPermissionsVM
                    {
                        TaxCategoryId = x.Key,
                        TaxCategoryPermissions = x.Select(catPermissions => new TaxCategoryPermissionsVM
                        {
                            Name = catPermissions.TaxCategoryPermissions.Name
                        })
                    });
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}