using NHibernate.Criterion;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.ReferenceData.Admin.Services.Contracts;
using System.Collections.Generic;

namespace Parkway.CBS.ReferenceData.Admin.Services
{
    public class ReferenceDataLGAManager : BaseManager<LGA>, IReferenceDataLGAManager<LGA>
    {
        private readonly IRepository<LGA> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<UserPartRecord> _user;
        private readonly ITransactionManager _transactionManager;

        public ReferenceDataLGAManager(IRepository<LGA> repository, IRepository<UserPartRecord> user, IOrchardServices orchardService) : base(repository, user, orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }

        public IEnumerable<LGA> GetLGAs(int stateId)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<LGA>("lga");
            criteria.Add(Restrictions.Eq("lga.State.Id", stateId));

            return criteria.Future<LGA>();
        }
    }
}