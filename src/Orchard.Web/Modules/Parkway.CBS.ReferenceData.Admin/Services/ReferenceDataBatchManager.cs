using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.ReferenceData.Admin.Services.Contracts;
using Parkway.CBS.ReferenceData.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.ReferenceData.Admin.Services
{
    public class ReferenceDataBatchManager : BaseManager<ReferenceDataBatch>, IReferenceDataBatchManager<ReferenceDataBatch> 
    {
        private readonly IRepository<ReferenceDataBatch> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<UserPartRecord> _user;
        private readonly ITransactionManager _transactionManager;

        public ReferenceDataBatchManager(IRepository<ReferenceDataBatch> repository, IRepository<UserPartRecord> user, IOrchardServices orchardService) : base(repository, user, orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }

        public IEnumerable<ReferenceDataBatch> GetBatchRecords(int skip, int take, ReferenceDataBatchSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);
            return query.SetFirstResult(skip).SetMaxResults(take).Future<ReferenceDataBatch>();
        }

        private ICriteria GetCriteria(ReferenceDataBatchSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<ReferenceDataBatch>();

            if (!string.IsNullOrEmpty(searchParams.BatchRef))
            {
                criteria.Add(Restrictions.Eq("BatchRef", searchParams.BatchRef));
            }

            criteria.Add(Restrictions.Between("CreatedAtUtc", searchParams.FromRange.Value, searchParams.EndRange.Value));

            return criteria;
        }

        public IEnumerable<ReferenceDataBatchReportStats> GetAggregateForBatchRecords(ReferenceDataBatchSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var query = GetCriteria(searchParams);
            return query.SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count("Id"), "RecordCounts")
            ).SetResultTransformer(Transformers.AliasToBean<ReferenceDataBatchReportStats>()).Future<ReferenceDataBatchReportStats>();
        }

        /// <summary>
        /// Get batch ref from reference data batch
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ReferenceDataBatch</returns>
        public string GetBatchRef(int id)
        {
           return _transactionManager.GetSession().Query<ReferenceDataBatch>().Where(rf => rf.Id == id).Select(s => s.BatchRef).FirstOrDefault();
        }

        /// <summary>
        /// Get ReferenceDataBatch using BatchRef
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns>ReferenceDataBatch</returns>
        public ReferenceDataBatch GetReferenceDataBatch(string batchRef)
        {
            return _transactionManager.GetSession().Query<ReferenceDataBatch>().Where(rf => rf.BatchRef == batchRef).FirstOrDefault();
        }
    }
}