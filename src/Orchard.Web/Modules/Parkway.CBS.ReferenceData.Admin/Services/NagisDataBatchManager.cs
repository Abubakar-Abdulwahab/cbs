using NHibernate;
using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.ReferenceData.Admin.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.ReferenceData.Admin.Services
{
    public class NagisDataBatchManager : BaseManager<NagisDataBatch>, INagisDataBatchManager<NagisDataBatch>
    {
        private readonly IRepository<NagisDataBatch> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<UserPartRecord> _user;
        private readonly ITransactionManager _transactionManager;


        public NagisDataBatchManager(IRepository<NagisDataBatch> repository, IRepository<UserPartRecord> user, IOrchardServices orchardService) : base(repository, user, orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;

        }

        public IEnumerable<NagisDataBatch> GetBatchRecords()
        {
            var query = GetCriteria();
            return query.Future<NagisDataBatch>();
        }

        public string GetBatchRef(int id)
        {
            return _transactionManager.GetSession().Query<NagisDataBatch>().Where(rf => rf.Id == id).Select(s => s.BatchRef).FirstOrDefault();
        }

        public NagisDataBatch GetNagisDataBatch(string batchRef)
        {
            return _transactionManager.GetSession().Query<NagisDataBatch>().Where(rf => rf.BatchRef == batchRef).FirstOrDefault();
        }

        private ICriteria GetCriteria()
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<NagisDataBatch>();

            return criteria;
        }

    }
}