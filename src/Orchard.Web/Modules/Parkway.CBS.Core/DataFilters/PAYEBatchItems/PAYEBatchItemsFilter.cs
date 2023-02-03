using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.DataFilters.PAYEBatchItems.Contracts;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Core.DataFilters.PAYEBatchItems
{
    public class PAYEBatchItemsFilter : IPAYEBatchItemsFilter
    {
        private readonly ITransactionManager _transactionManager;
        public PAYEBatchItemsFilter(IOrchardServices orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
        }


        public ICriteria GetCriteria(PAYEBatchItemSearchParams searchParams)
        {
            var criteria = _transactionManager.GetSession().CreateCriteria<Models.PAYEBatchItems>("PBI")
                                                           .CreateAlias("PBI.PAYEBatchRecord", "PAYEBatchRecord")
                                                           .CreateAlias("PBI.TaxEntity","TaxEntity");
            criteria.Add(Restrictions.Where<Models.PAYEBatchItems>(x => x.PAYEBatchRecord.BatchRef == searchParams.BatchRef));
            return criteria;
        }

        /// <summary>
        /// Get PAYE Batch items for batch using search params
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public IEnumerable<PAYEBatchItemsVM> GetPAYEBatchItemsForBatch(PAYEBatchItemSearchParams searchParams)
        {
            var criteria = GetCriteria(searchParams);
            if (searchParams.PageData)
            {
                criteria
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return criteria.SetProjection(Projections.ProjectionList()
                .Add(Projections.Property<Models.PAYEBatchItems>(x => x.Exemptions), nameof(PAYEBatchItemsVM.Exemptions))
                .Add(Projections.Property<Models.PAYEBatchItems>(x => x.GrossAnnual), nameof(PAYEBatchItemsVM.GrossAnnual))
                .Add(Projections.Property<Models.PAYEBatchItems>(x => x.IncomeTaxPerMonth), nameof(PAYEBatchItemsVM.IncomeTaxPerMonth))
                .Add(Projections.Property<Models.PAYEBatchItems>(x => x.Month), nameof(PAYEBatchItemsVM.MonthId))
                .Add(Projections.Property<Models.PAYEBatchItems>(x => x.Year), nameof(PAYEBatchItemsVM.Year))
                .Add(Projections.Property<Models.PAYEBatchItems>(x => x.TaxEntity.Recipient), nameof(PAYEBatchItemsVM.PayerName))
                .Add(Projections.Property<Models.PAYEBatchItems>(x => x.TaxEntity.PayerId), nameof(PAYEBatchItemsVM.PayerId)))
                .SetResultTransformer(Transformers.AliasToBean<PAYEBatchItemsVM>()).Future<PAYEBatchItemsVM>();

        }


        public IEnumerable<ReportStatsVM> GetAggregate(PAYEBatchItemSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);
            return query.SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count<Models.PAYEBatchItems>(x => x.Id), "TotalRecordCount")
            ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        public dynamic GetBatchItems(PAYEBatchItemSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();
            returnOBJ.BatchItemRecords = GetPAYEBatchItemsForBatch(searchParams);
            returnOBJ.Aggregate = GetAggregate(searchParams);
            return returnOBJ;
        }
    }
}