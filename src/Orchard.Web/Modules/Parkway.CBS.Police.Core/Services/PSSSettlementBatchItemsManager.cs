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
    public class PSSSettlementBatchItemsManager : BaseManager<PSSSettlementBatchItems>, IPSSSettlementBatchItemsManager<PSSSettlementBatchItems>
    {
        private readonly IRepository<PSSSettlementBatchItems> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public PSSSettlementBatchItemsManager(IRepository<PSSSettlementBatchItems> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Gets PSS Settlement Batch Items Records
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public IEnumerable<PSSSettlementBatchItemsVM> GetReportRecords(PSSSettlementReportSearchParams searchParams)
        {
            try
            {
                return null;
                //var query = _transactionManager.GetSession()
                //    .CreateCriteria<PSSSettlementBatchItems>()
                //    .CreateAlias(nameof(PSSSettlementBatchItems.SettlementBatch), nameof(PSSSettlementBatchItems.SettlementBatch))
                //    .CreateAlias(nameof(PSSSettlementBatchItems.Service), nameof(PSSSettlementBatchItems.Service))
                //    .CreateAlias(nameof(PSSSettlementBatchItems.RevenueHead), nameof(PSSSettlementBatchItems.RevenueHead))
                //    .CreateAlias(nameof(PSSSettlementBatchItems.Invoice), nameof(PSSSettlementBatchItems.Invoice))
                //    .CreateAlias($"{nameof(PSSSettlementBatchItems.Invoice)}.{nameof(PSSSettlementBatchItems.Invoice.TaxPayer)}", nameof(PSSSettlementBatchItems.Invoice.TaxPayer))
                //    .Add(Restrictions.Eq($"{nameof(PSSSettlementBatchItems.SettlementBatch)}.{nameof(PSSSettlementBatchItems.SettlementBatch.Id)}", searchParams.BatchId))
                //    .Add(Restrictions.Eq(nameof(PSSSettlementBatchItems.IsDeduction), false));
                //if (searchParams.PageData)
                //{
                //    query.SetFirstResult(searchParams.Skip)
                //         .SetMaxResults(searchParams.Take);
                //}
                //return query
                //        .SetProjection(Projections.ProjectionList()
                //.Add(Projections.Property(nameof(PSSSettlementBatchItems.CreatedAtUtc)), nameof(PSSSettlementBatchItemsVM.TransactionDate))
                //.Add(Projections.Property(nameof(PSSSettlementBatchItems.Amount)), nameof(PSSSettlementBatchItemsVM.SettlementAmount))
                //.Add(Projections.Property($"{nameof(PSSSettlementBatchItems.Service)}.{nameof(PSSSettlementBatchItems.Service.Name)}"), nameof(PSSSettlementBatchItemsVM.ServiceName))
                //.Add(Projections.Property($"{nameof(PSSSettlementBatchItems.RevenueHead)}.{nameof(PSSSettlementBatchItems.RevenueHead.Name)}"), nameof(PSSSettlementBatchItemsVM.RevenueHead))
                //.Add(Projections.Property($"{nameof(PSSSettlementBatchItems.Invoice)}.{nameof(PSSSettlementBatchItems.Invoice.InvoiceNumber)}"), nameof(PSSSettlementBatchItemsVM.InvoiceNumber))
                //.Add(Projections.Property($"{nameof(PSSSettlementBatchItems.SettlementBatch)}.{nameof(PSSSettlementBatchItems.SettlementBatch.BatchRef)}"), nameof(PSSSettlementBatchItemsVM.SettlementBatchRef))
                //.Add(Projections.Property($"{nameof(PSSSettlementBatchItems.Invoice.TaxPayer)}.{nameof(PSSSettlementBatchItems.Invoice.TaxPayer.Recipient)}"), nameof(PSSSettlementBatchItemsVM.CustomerName)))
                //.AddOrder(Order.Desc(nameof(PSSSettlementBatchItems.Id)))
                //.SetResultTransformer(Transformers.AliasToBean<PSSSettlementBatchItemsVM>())
                //.Future<PSSSettlementBatchItemsVM>();
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
        /// <param name="batchId"></param>
        /// <returns>IEnumerable{FileUploadReport}</returns>
        public IEnumerable<long> GetCount(long batchId)
        {
            try
            {
                return null;
                //return _transactionManager.GetSession()
                //                      .CreateCriteria<PSSSettlementBatchItems>(typeof(PSSSettlementBatchItems).Name)
                //                      .CreateAlias(nameof(PSSSettlementBatchItems.SettlementBatch), nameof(PSSSettlementBatchItems.SettlementBatch))
                //                      .Add(Restrictions.Eq($"{nameof(PSSSettlementBatchItems.SettlementBatch)}.{nameof(PSSSettlementBatchItems.SettlementBatch.Id)}", batchId))
                //                      .Add(Restrictions.Eq(nameof(PSSSettlementBatchItems.IsDeduction), false))
                //                      .SetProjection(
                //                      Projections.ProjectionList()
                //                          .Add(Projections.Count<PSSSettlementBatchItems>(x => (x.Id)))
                //                  ).Future<long>();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }

        }
    }
}