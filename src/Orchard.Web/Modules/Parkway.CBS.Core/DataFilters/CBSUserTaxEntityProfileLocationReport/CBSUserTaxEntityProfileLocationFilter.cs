using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.DataFilters.CBSUserTaxEntityProfileLocationReport.Contracts;
using Parkway.CBS.Core.DataFilters.CBSUserTaxEntityProfileLocationReport.SearchFilters.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Core.DataFilters.CBSUserTaxEntityProfileLocationReport
{
    public class CBSUserTaxEntityProfileLocationFilter : ICBSUserTaxEntityProfileLocationFilter
    {
        private readonly ITransactionManager _transactionManager;
        protected readonly IEnumerable<ICBSUserTaxEntityProfileLocationSearchFilter> _searchFilters;

        public CBSUserTaxEntityProfileLocationFilter(IOrchardServices orchardService, IEnumerable<ICBSUserTaxEntityProfileLocationSearchFilter> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }

        /// <summary>
        /// Get aggreagate for the search param
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        public IEnumerable<ReportStatsVM> GetAggregate(CBSUserTaxEntityProfileLocationReportSearchParams searchParams)
        {
            return GetCriteria(searchParams).SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count<CBSUserTaxEntityProfileLocation>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
            ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get CBS User Tax Entity Profile Location report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<CBSUserTaxEntityProfileLocationVM></returns>
        public IEnumerable<CBSUserTaxEntityProfileLocationVM> GetReport(CBSUserTaxEntityProfileLocationReportSearchParams searchParams)
        {
            ICriteria criteria = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                criteria.SetFirstResult(searchParams.Skip).SetMaxResults(searchParams.Take);
            }

            return criteria.AddOrder(Order.Desc("Id"))
                .CreateAlias(nameof(CBSUserTaxEntityProfileLocation.CBSUser)+"."+nameof(CBSUser.UserPartRecord),nameof(CBSUser.UserPartRecord))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property(nameof(CBSUserTaxEntityProfileLocation.CBSUser)+"."+nameof(CBSUser.Name)), nameof(CBSUserTaxEntityProfileLocationVM.Name))
                .Add(Projections.Property(nameof(CBSUserTaxEntityProfileLocation.CBSUser)+"."+nameof(CBSUser.PhoneNumber)), nameof(CBSUserTaxEntityProfileLocationVM.PhoneNumber))
                .Add(Projections.Property(nameof(CBSUserTaxEntityProfileLocation.CBSUser)+"."+nameof(CBSUser.Email)), nameof(CBSUserTaxEntityProfileLocationVM.Email))
                .Add(Projections.Property(nameof(CBSUserTaxEntityProfileLocation.CBSUser)+"."+nameof(CBSUser.Address)), nameof(CBSUserTaxEntityProfileLocationVM.Address))
                .Add(Projections.Property(nameof(CBSUserTaxEntityProfileLocation.CBSUser)+"."+nameof(CBSUser.Verified)), nameof(CBSUserTaxEntityProfileLocationVM.Verified))
                .Add(Projections.Property(nameof(CBSUserTaxEntityProfileLocation.CBSUser) + "." + nameof(CBSUser.UserPartRecord) + "." + nameof(UserPartRecord.Id)), nameof(CBSUserTaxEntityProfileLocationVM.UserPartRecordId))
                .Add(Projections.Property(nameof(CBSUserTaxEntityProfileLocation.CBSUser) +"."+nameof(CBSUser.IsAdministrator)), nameof(CBSUserTaxEntityProfileLocationVM.IsAdministrator))
                .Add(Projections.Property(nameof(CBSUserTaxEntityProfileLocation.CBSUser) + "." + nameof(CBSUser.IsActive)), nameof(CBSUserTaxEntityProfileLocationVM.IsActive))
                .Add(Projections.Property(nameof(CBSUserTaxEntityProfileLocation.TaxEntityProfileLocation)+"."+nameof(TaxEntityProfileLocation.Name)), nameof(CBSUserTaxEntityProfileLocationVM.BranchName))
                ).SetResultTransformer(Transformers.AliasToBean<CBSUserTaxEntityProfileLocationVM>())
                .Future<CBSUserTaxEntityProfileLocationVM>();
        }


        /// <summary>
        /// Get report model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        public dynamic GetReportViewModel(CBSUserTaxEntityProfileLocationReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.Aggregate = GetAggregate(searchParams);
            }
            catch (NoRecordFoundException)
            {
                returnOBJ.ReportRecords = new List<CBSUserTaxEntityProfileLocation> { };
            }
            return returnOBJ;
        }


        /// <summary>
        /// Creates base criteria query
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public ICriteria GetCriteria(CBSUserTaxEntityProfileLocationReportSearchParams searchParams)
        {
            var criteria = _transactionManager.GetSession().CreateCriteria<CBSUserTaxEntityProfileLocation>(nameof(CBSUserTaxEntityProfileLocation))
                .CreateAlias(nameof(CBSUserTaxEntityProfileLocation.CBSUser), nameof(CBSUser))
                .CreateAlias(nameof(CBSUserTaxEntityProfileLocation.TaxEntityProfileLocation), nameof(TaxEntityProfileLocation))
                .CreateAlias(nameof(TaxEntityProfileLocation) +"."+nameof(TaxEntity), nameof(TaxEntity));

            criteria.Add(Restrictions.Eq(nameof(TaxEntity) + "." + nameof(TaxEntity.Id), searchParams.TaxEntityId));
                    //.Add(Restrictions.Between(nameof(CBSUserTaxEntityProfileLocation.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));

            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }
    }
}