using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.ExpenditureHeadReport.Contracts;
using Parkway.CBS.Police.Core.DataFilters.ExpenditureHeadReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.ExpenditureHeadReport
{
    public class ExpenditureHeadReportFilter : IExpenditureHeadReportFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<Lazy<IExpenditureHeadReportFilters>> _expenditureHeadReportFilters;

        public ExpenditureHeadReportFilter(IOrchardServices orchardService, IEnumerable<Lazy<IExpenditureHeadReportFilters>> expenditureHeadReportFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _expenditureHeadReportFilters = expenditureHeadReportFilters;
        }

        /// <summary>
        /// Get view model for expenditure head reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalExpenditureHeadRecord }</returns>
        public dynamic GetExpenditureHeadReportViewModel(ExpenditureHeadReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalExpenditureHeadRecord = GetTotalNumberOfExpenditureHead(searchParams);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the total number of expenditure heads
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalNumberOfExpenditureHead(ExpenditureHeadReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PSSExpenditureHead>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get the list of expenditure heads
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns><see cref="IEnumerable{HelperModels.ExpenditureHeadReportVM}"/></returns>
        private IEnumerable<HelperModels.ExpenditureHeadReportVM> GetReport(ExpenditureHeadReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query
                .AddOrder(Order.Desc(nameof(PSSExpenditureHead.UpdatedAtUtc)))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property(nameof(PSSExpenditureHead.Code)), nameof(HelperModels.ExpenditureHeadReportVM.Code))
                .Add(Projections.Property(nameof(PSSExpenditureHead.Id)), nameof(HelperModels.ExpenditureHeadReportVM.Id))
                .Add(Projections.Property(nameof(PSSExpenditureHead.IsActive)), nameof(HelperModels.ExpenditureHeadReportVM.IsActive))
                .Add(Projections.Property(nameof(PSSExpenditureHead.Name)), nameof(HelperModels.ExpenditureHeadReportVM.Name)))
                .SetResultTransformer(Transformers.AliasToBean<HelperModels.ExpenditureHeadReportVM>())
                .Future<HelperModels.ExpenditureHeadReportVM>();
        }


        public ICriteria GetCriteria(ExpenditureHeadReportSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<PSSExpenditureHead>(nameof(PSSExpenditureHead));

            foreach (var filter in _expenditureHeadReportFilters)
            {
                filter.Value.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }
    }
}