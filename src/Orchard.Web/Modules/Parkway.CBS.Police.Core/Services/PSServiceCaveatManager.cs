using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Linq;
using NHibernate.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSServiceCaveatManager : BaseManager<PSServiceCaveat>, IPSServiceCaveatManager<PSServiceCaveat>
    {
        private readonly IRepository<PSServiceCaveat> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public PSServiceCaveatManager(IRepository<PSServiceCaveat> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Get caveat for service with specified service id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public PSServiceCaveatVM GetServiceCaveat(int serviceId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceCaveat>().Where(x => (x.Service.Id == serviceId) && x.IsActive)
                    .Select(x => new PSServiceCaveatVM { CaveatHeader = x.CaveatHeader, CaveatContent = x.CaveatContent })
                    .SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}