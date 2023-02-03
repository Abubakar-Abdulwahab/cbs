using NHibernate.Criterion;
using NHibernate.Transform;
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

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSSettlementBatchAggregateManager : BaseManager<PSSSettlementBatchAggregate>, IPSSSettlementBatchAggregateManager<PSSSettlementBatchAggregate>
    {
        private readonly IRepository<PSSSettlementBatchAggregate> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public PSSSettlementBatchAggregateManager(IRepository<PSSSettlementBatchAggregate> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Get PSS Settlement Batch Aggregate Records
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public IEnumerable<PSSSettlementBatchAggregateVM> GetReportRecords(PSSSettlementReportSearchParams searchParams)
        {
            try
            {
                var query = _transactionManager.GetSession()
                    .CreateCriteria<PSSSettlementBatchAggregate>()
                    .CreateAlias(nameof(PSSSettlementBatchAggregate.SettlementBatch), "SettlementBatch")
                    .CreateAlias("SettlementBatch.SettlementRule", "SettlementRule")
                    .Add(Restrictions.Where<PSSSettlementBatchAggregate>(x => x.CreatedAtUtc >= searchParams.StartDate && x.CreatedAtUtc <= searchParams.EndDate));
                if (searchParams.PageData)
                {
                    query.SetFirstResult(searchParams.Skip)
                         .SetMaxResults(searchParams.Take);
                }
                return query
                        .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property("CreatedAtUtc"), nameof(PSSSettlementBatchAggregateVM.TransactionDate))
                .Add(Projections.Property("Amount"), nameof(PSSSettlementBatchAggregateVM.SettlementAmount))
                .Add(Projections.Property("SettlementBatch.BatchRef"), nameof(PSSSettlementBatchAggregateVM.SettlementBatchRef))
                .Add(Projections.Property("SettlementRule.Name"), nameof(PSSSettlementBatchAggregateVM.SettlementName)))
                .AddOrder(Order.Desc("Id"))
                .SetResultTransformer(Transformers.AliasToBean<PSSSettlementBatchAggregateVM>())
                .Future<PSSSettlementBatchAggregateVM>();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Get report aggregate
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public IEnumerable<int> GetCount(PSSSettlementReportSearchParams searchParams)
        {
            return _transactionManager.GetSession()
                                  .CreateCriteria<PSSSettlementBatchAggregate>(typeof(PSSSettlementBatchAggregate).Name)
                                  .Add(Restrictions.Where<PSSSettlementBatchAggregate>(x => x.CreatedAtUtc >= searchParams.StartDate && x.CreatedAtUtc <= searchParams.EndDate))
                                  .SetProjection(
                                  Projections.ProjectionList()
                                      .Add(Projections.Count<PSSSettlementBatchAggregate>(x => (x.Id)))
                              ).Future<int>();
        }
    }
}