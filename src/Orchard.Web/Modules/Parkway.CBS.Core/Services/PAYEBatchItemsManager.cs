using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services
{
    public class PAYEBatchItemsManager : BaseManager<PAYEBatchItems>, IPAYEBatchItemsManager<PAYEBatchItems>
    {
        private readonly IRepository<PAYEBatchItems> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PAYEBatchItemsManager(IRepository<PAYEBatchItems> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Get PAYE payment summary for specified year and taxEntityId
        /// </summary>
        /// <param name="year"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public IEnumerable<TCCCertificateStatsVM> GetPAYEPaymentSummary(int year, long taxEntityId)
        {
            var criteria = _transactionManager.GetSession().CreateCriteria<PAYEBatchItems>("PBI")
            .CreateAlias(nameof(PAYEBatchItems.PAYEBatchRecord), nameof(PAYEBatchItems.PAYEBatchRecord));

            var payeBatchItemReceiptCriteria = DetachedCriteria.For<PAYEBatchItemReceipt>("PBIR")
            .Add(Restrictions.EqProperty("PAYEBatchItem.Id", "PBI.Id"))
            .SetProjection(Projections.Constant(1));
            criteria.Add(Subqueries.Exists(payeBatchItemReceiptCriteria));

            return criteria.Add(Restrictions.Where<PAYEBatchItems>(x => x.TaxEntity.Id == taxEntityId && x.Year == year && x.PAYEBatchRecord.PaymentCompleted))
            .SetProjection(
                        Projections.ProjectionList()
                            .Add(Projections.Sum<PAYEBatchItems>(x => x.IncomeTaxPerMonth), "TotalTaxPaid")
                            .Add(Projections.Sum<PAYEBatchItems>(x => x.GrossAnnual), "TotalIncome")
                    ).SetResultTransformer(Transformers.AliasToBean<TCCCertificateStatsVM>()).Future<TCCCertificateStatsVM>();
        }

        /// <summary>
        /// Get PAYE payments for specified year and taxEntityId
        /// </summary>
        /// <param name="year"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>IEnumerable<TaxPaymentDetailVM></returns>
        public IEnumerable<TaxPaymentDetailVM> GetPAYEPayments(int year, long taxEntityId)
        {
            return _transactionManager.GetSession().CreateCriteria<PAYEBatchItems>()
            .CreateAlias(nameof(PAYEBatchItems.PAYEBatchRecord), nameof(PAYEBatchItems.PAYEBatchRecord))
            .Add(Restrictions.Where<PAYEBatchItems>(x => x.TaxEntity.Id == taxEntityId && x.Year == year && x.PAYEBatchRecord.PaymentCompleted))
            .SetProjection(
                        Projections.ProjectionList()
                            .Add(Projections.Property(nameof(PAYEBatchItems.IncomeTaxPerMonth)), nameof(TaxPaymentDetailVM.TaxPaid))
                            .Add(Projections.Property(nameof(PAYEBatchItems.Month)), nameof(TaxPaymentDetailVM.MonthId))
                            .Add(Projections.Property(nameof(PAYEBatchItems.Year)), nameof(TaxPaymentDetailVM.Year))
                    ).SetResultTransformer(Transformers.AliasToBean<TaxPaymentDetailVM>()).Future<TaxPaymentDetailVM>();
        }

    }
}