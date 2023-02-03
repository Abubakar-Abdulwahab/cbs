using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.DataFilters.PSSUserReport.Contracts;
using Parkway.CBS.Police.Core.DataFilters.PSSUserReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.PSSUserReport
{
    public class PSSUserReportFilter : IPSSUserReportFilter
    {
        private readonly ITransactionManager _transactionManager;

        private readonly IEnumerable<IPSSUserReportFilters> _userReportFilters;

        public PSSUserReportFilter(IOrchardServices orchardService, IEnumerable<IPSSUserReportFilters> userReportFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _userReportFilters = userReportFilters;
        }

        /// <summary>
        /// Gets the view model based of the filter parameter
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public dynamic GetUserReportViewModel(PSSUserReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalNumberOfUserRecords = GetTotalNumberOfUsers(searchParams);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ICriteria GetCriteria(PSSUserReportSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<CBSUser>(nameof(CBSUser)).CreateAlias($"{nameof(CBSUser)}.{nameof(CBSUser.UserPartRecord)}", nameof(CBSUser.UserPartRecord)).CreateAlias($"{nameof(CBSUser)}.{nameof(CBSUser.TaxEntity)}", nameof(CBSUser.TaxEntity))
                .Add(Restrictions.Between(nameof(CBSUser.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));

            foreach (var filter in _userReportFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }

        private IEnumerable<PSSUserReportCBSUserVM> GetReport(PSSUserReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query
                .AddOrder(Order.Desc(nameof(CBSUser.UpdatedAtUtc)))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property(nameof(CBSUser.Name)), nameof(PSSUserReportCBSUserVM.Name))
                .Add(Projections.Property(nameof(CBSUser.Email)), nameof(PSSUserReportCBSUserVM.Email))
                .Add(Projections.Property(nameof(CBSUser.Verified)), nameof(PSSUserReportCBSUserVM.IsVerified))
                .Add(Projections.Property(nameof(CBSUser.CreatedAtUtc)), nameof(PSSUserReportCBSUserVM.CreatedAt))
                .Add(Projections.Property($"{nameof(CBSUser.UserPartRecord)}.{nameof(CBSUser.UserPartRecord.UserName)}"), nameof(PSSUserReportCBSUserVM.UserName))
                .Add(Projections.Property($"{nameof(CBSUser.TaxEntity)}.{nameof(CBSUser.TaxEntity.PayerId)}"), nameof(PSSUserReportCBSUserVM.PayerId))
                .Add(Projections.Property($"{nameof(CBSUser.TaxEntity)}.{nameof(CBSUser.TaxEntity.IdentificationNumber)}"), nameof(PSSUserReportCBSUserVM.IdentificationNumber))
                .Add(Projections.Property(nameof(CBSUser.PhoneNumber)), nameof(PSSUserReportCBSUserVM.PhoneNumber))
                .Add(Projections.Property(nameof(CBSUser.IsAdministrator)), nameof(PSSUserReportCBSUserVM.IsAdministrator)))
                .SetResultTransformer(Transformers.AliasToBean<PSSUserReportCBSUserVM>())
                .Future<PSSUserReportCBSUserVM>();
        }

        private IEnumerable<ReportStatsVM> GetTotalNumberOfUsers(PSSUserReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<CBSUser>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }
    }
}