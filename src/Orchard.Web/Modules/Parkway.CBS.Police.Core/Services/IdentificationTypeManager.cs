using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class IdentificationTypeManager : BaseManager<IdentificationType>, IIdentificationTypeManager<IdentificationType>
    {
        private readonly IRepository<IdentificationType> _repo;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public IdentificationTypeManager(IRepository<IdentificationType> repo, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repo, user, orchardServices)
        {
            _repo = repo;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get identification type VM for identification type with specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IdentificationTypeVM GetIdentificationTypeVM(int id)
        {
            try {
                return _transactionManager.GetSession().Query<IdentificationType>().Where(x => x.Id == id).Select(x => new IdentificationTypeVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    HasIntegration = x.HasIntegration,
                    ImplementingClassName = x.ImplementingClassName,
                    Hint = x.Hint,
                    HasBiometricSupport = x.HasBiometricSupport
                }).SingleOrDefault();

            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Check if the identity type has biometric support
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bool</returns>
        public bool HasBiometricSupport(int id)
        {
            try
            {
                return _transactionManager.GetSession()
                    .Query<IdentificationType>()
                    .Count(x => ((x.Id == id) && (x.HasBiometricSupport))) == 1;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

    }
}