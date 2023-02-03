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
using NHibernate.Linq;
using NHibernate;

namespace Parkway.CBS.Police.Core.Services
{
    public class IdentificationTypeTaxCategoryManager : BaseManager<IdentificationTypeTaxCategory>, IIdentificationTypeTaxCategoryManager<IdentificationTypeTaxCategory>
    {
        private readonly IRepository<IdentificationTypeTaxCategory> _repo;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public IdentificationTypeTaxCategoryManager(IRepository<IdentificationTypeTaxCategory> repo, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repo, user, orchardServices)
        {
            _repo = repo;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Gets all identification types for tax category with specified Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IEnumerable<IdentificationTypeVM> GetIdentificationTypesForCategory(int categoryId)
        {
            try
            {
                return _transactionManager.GetSession().Query<IdentificationTypeTaxCategory>()
                    .Where(x => x.TaxCategory.Id == categoryId && x.TaxCategory.Status == true)
                    .Select(x => new IdentificationTypeVM
                    {
                        Id = x.IdentificationType.Id,
                        Name = x.IdentificationType.Name,
                        HasIntegration = x.IdentificationType.HasIntegration,
                        Hint = x.IdentificationType.Hint
                    }).ToFuture();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Get the list of identity types with biometric support
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>IEnumerable{IdentificationTypeVM}</returns>
        public IEnumerable<IdentificationTypeVM> GetIdentificationTypesWithBiometricSupportForCategoryId(int categoryId)
        {
            try
            {
                return _transactionManager.GetSession().Query<IdentificationTypeTaxCategory>()
                    .Where(x => ((x.TaxCategory.Id == categoryId) && (x.TaxCategory.Status) && (x.IdentificationType.HasBiometricSupport)))
                    .Select(x => new IdentificationTypeVM
                    {
                        Id = x.IdentificationType.Id,
                        Name = x.IdentificationType.Name,
                    });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Check that there is a match between Id
        /// and tax category Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryId"></param>
        /// <returns>IdentificationTypeVM</returns>
        public IdentificationTypeVM GetIdentityTypeWithTaxCategory(int id, int categoryId)
        {
            try
            {
                return _transactionManager.GetSession().Query<IdentificationTypeTaxCategory>()
                    .Where(x => ((x.TaxCategory.Id == categoryId) && (x.TaxCategory.Status) && (x.Id == id)))
                    .Select(x => new IdentificationTypeVM
                    {
                        Id = x.Id,
                        ImplementingClassName = x.IdentificationType.ImplementingClassName,
                        HasBiometricSupport = x.IdentificationType.HasBiometricSupport
                    }).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

    }
}