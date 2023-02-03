using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSEscortDayTypeManager : BaseManager<PSSEscortDayType>, IPSSEscortDayTypeManager<PSSEscortDayType>
    {
        private readonly IRepository<PSSEscortDayType> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }


        public PSSEscortDayTypeManager(IRepository<PSSEscortDayType> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Gets escort day types
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PSSEscortDayTypeDTO> GetPSSEscortDayTypes()
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSEscortDayType>().Where(x => x.IsActive).Select(x => new PSSEscortDayTypeDTO { Id = x.Id, Name = x.Name });
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}