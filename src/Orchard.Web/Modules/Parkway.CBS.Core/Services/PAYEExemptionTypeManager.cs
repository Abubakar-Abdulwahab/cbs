using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Payee.PayeeAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Services
{
    public class PAYEExemptionTypeManager : BaseManager<PAYEExemptionType>, IPAYEExemptionTypeManager<PAYEExemptionType>
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<PAYEExemptionType> _repository;
        private readonly ITransactionManager _transactionManager;
        public PAYEExemptionTypeManager(IRepository<PAYEExemptionType> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Returns a list of PAYEExemptionTypeVM of all active exemptionTypes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PAYEExemptionTypeVM> GetAllActivePAYEExemptionTypes()
        {
            try
            {

                return _transactionManager.GetSession()
                                          .Query<PAYEExemptionType>()
                                          .Where(et => et.IsActive)
                                          .Select(et => new PAYEExemptionTypeVM { Name = et.Name, Id = et.Id })
                                          .ToFuture();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}