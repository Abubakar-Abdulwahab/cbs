using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.DataFilters.TaxEntityProfileLocationReport.Contracts;
using Parkway.CBS.Core.DataFilters.TaxEntityProfileLocationReport.SearchFilters.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.DataFilters.TaxEntityProfileLocationReport
{
    public class TaxEntityProfileLocationFilter : ITaxEntityProfileLocationFilter
    {
        private readonly ITransactionManager _transactionManager;
        protected readonly IEnumerable<ITaxEntityProfileLocationSearchFilter> _searchFilters;

        public TaxEntityProfileLocationFilter(IOrchardServices orchardService, IEnumerable<ITaxEntityProfileLocationSearchFilter> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }


        /// <summary>
        /// Get aggreagate for the search param
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        public IEnumerable<ReportStatsVM> GetAggregate(TaxEntityProfileLocationReportSearchParams searchParams)
        {
            return GetCriteria(searchParams).SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count<TaxEntityProfileLocation>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
            ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get Tax Entity Profile Location report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<TaxEntityProfileLocationVM></returns>
        public IEnumerable<TaxEntityProfileLocationVM> GetReport(TaxEntityProfileLocationReportSearchParams searchParams)
        {
            ICriteria criteria = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                criteria.SetFirstResult(searchParams.Skip).SetMaxResults(searchParams.Take);
            }

            return criteria.AddOrder(Order.Desc("Id"))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property(nameof(TaxEntityProfileLocation.CreatedAtUtc)), nameof(TaxEntityProfileLocationVM.CreatedAt))
                .Add(Projections.Property(nameof(TaxEntityProfileLocation.Name)), nameof(TaxEntityProfileLocationVM.Name))
                .Add(Projections.Property(nameof(TaxEntityProfileLocation.Address)), nameof(TaxEntityProfileLocationVM.Address))
                .Add(Projections.Property(nameof(TaxEntityProfileLocation.Id)), nameof(TaxEntityProfileLocationVM.Id))
                .Add(Projections.Property(nameof(StateModel) + "." + nameof(StateModel.Name)), nameof(TaxEntityProfileLocationVM.StateName))
                .Add(Projections.Property(nameof(LGA) + "." + nameof(LGA.Name)), nameof(TaxEntityProfileLocationVM.LGAName))
                .Add(Projections.Property(nameof(TaxEntityProfileLocation.Code)), nameof(TaxEntityProfileLocationVM.Code))
                ).SetResultTransformer(Transformers.AliasToBean<TaxEntityProfileLocationVM>())
                .Future<TaxEntityProfileLocationVM>();
        }


        /// <summary>
        /// Get report model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        public dynamic GetReportViewModel(TaxEntityProfileLocationReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.Aggregate = GetAggregate(searchParams);
            }
            catch (NoRecordFoundException)
            {
                returnOBJ.ReportRecords = new List<TaxEntityProfileLocation> { };
            }
            return returnOBJ;
        }


        /// <summary>
        /// Creates base criteria query
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public ICriteria GetCriteria(TaxEntityProfileLocationReportSearchParams searchParams)
        {
            var criteria = _transactionManager.GetSession().CreateCriteria<TaxEntityProfileLocation>(nameof(TaxEntityProfileLocation))
                .CreateAlias(nameof(TaxEntityProfileLocation.TaxEntity), nameof(TaxEntity))
                .CreateAlias(nameof(TaxEntityProfileLocation.State), nameof(StateModel))
                .CreateAlias(nameof(TaxEntityProfileLocation.LGA), nameof(LGA));

            criteria.Add(Restrictions.Eq(nameof(TaxEntity) + "." + nameof(TaxEntity.Id), searchParams.TaxEntityId));
                    //.Add(Restrictions.Between(nameof(TaxEntityProfileLocation.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));

            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }
    }
}