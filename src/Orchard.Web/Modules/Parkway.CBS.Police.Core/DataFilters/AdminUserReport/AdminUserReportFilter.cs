using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.AdminUserReport.Contracts;
using Parkway.CBS.Police.Core.DataFilters.AdminUserReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.AdminUserReport
{
    public class AdminUserReportFilter : IAdminUserReportFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<Lazy<IAdminUserReportFilters>> _searchAdminUserReportFilters;

        public AdminUserReportFilter(IOrchardServices orchardService, IEnumerable<Lazy<IAdminUserReportFilters>> searchAdminUserReportFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchAdminUserReportFilters = searchAdminUserReportFilters;
        }

        /// <summary>
        /// Gets view model for admin user report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public dynamic GetAdminUserReportViewModel(AdminUserReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalNumberOfAdminUsersRecords = GetTotalAdminUserRecords(searchParams);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the total number of admin user records
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalAdminUserRecords(AdminUserReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PSSAdminUsers>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        private IEnumerable<AdminUserVM> GetReport(AdminUserReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);
            query = query.AddOrder(Order.Desc("Id"));

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property(nameof(AdminUserVM.PhoneNumber)), nameof(AdminUserVM.PhoneNumber))
                .Add(Projections.Property($"{nameof(PSSAdminUsers.Id)}"), nameof(AdminUserVM.Id))
                .Add(Projections.Property($"{nameof(PSSAdminUsers.User)}.{nameof(PSSAdminUsers.User.Id)}"), nameof(AdminUserVM.UserPartRecordId))
                 .Add(Projections.Property($"{nameof(PSSAdminUsers)}.{nameof(PSSAdminUsers.IsActive)}"), nameof(AdminUserVM.IsActive))
                .Add(Projections.Property($"{nameof(PSSAdminUsers.User)}.{nameof(PSSAdminUsers.User.UserName)}"), nameof(AdminUserVM.Username))
                .Add(Projections.Property($"{nameof(PSSAdminUsers.RoleType)}.{nameof(PSSAdminUsers.RoleType.Name)}"), nameof(AdminUserVM.RoleName))
                .Add(Projections.Property($"{nameof(PSSAdminUsers.CommandCategory)}.{nameof(PSSAdminUsers.CommandCategory.Name)}"), nameof(AdminUserVM.CommandCategoryName))
                .Add(Projections.Property($"{nameof(PSSAdminUsers.Command)}.{nameof(PSSAdminUsers.Command.Name)}"), nameof(AdminUserVM.CommandName))
                .Add(Projections.Property(nameof(AdminUserVM.Email)), nameof(AdminUserVM.Email))
                .Add(Projections.Property(nameof(AdminUserVM.Fullname)), nameof(AdminUserVM.Fullname)))
                .SetResultTransformer(Transformers.AliasToBean<AdminUserVM>())
                .Future<AdminUserVM>();
        }

        public ICriteria GetCriteria(AdminUserReportSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<PSSAdminUsers>(nameof(PSSAdminUsers));
            criteria.CreateAlias($"{nameof(PSSAdminUsers)}.{nameof(PSSAdminUsers.User)}", nameof(PSSAdminUsers.User));
            criteria.CreateAlias($"{nameof(PSSAdminUsers)}.{nameof(PSSAdminUsers.RoleType)}", nameof(PSSAdminUsers.RoleType));
            criteria.CreateAlias($"{nameof(PSSAdminUsers)}.{nameof(PSSAdminUsers.CommandCategory)}", nameof(PSSAdminUsers.CommandCategory));
            criteria.CreateAlias($"{nameof(PSSAdminUsers)}.{nameof(PSSAdminUsers.Command)}", nameof(PSSAdminUsers.Command));

            foreach (var filter in _searchAdminUserReportFilters)
            {
                filter.Value.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }

    }
}