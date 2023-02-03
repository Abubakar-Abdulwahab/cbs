using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Police.Core.DataFilters.AdminUserReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.AdminUserReport.SearchFilters
{
    /// <summary>
    /// Role type filter
    /// </summary>
    public class AdminUserReportRoleTypeFilters : IAdminUserReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, AdminUserReportSearchParams searchParams)
        {
            if (searchParams.RoleType > 0)
            {
                criteria.Add(Restrictions.Where<PSSAdminUsers>(x => x.RoleType == new Orchard.Roles.Models.RoleRecord { Id = searchParams.RoleType }));
            }

        }
    }

    /// <summary>
    /// Username filter
    /// </summary>
    public class AdminUserReportUsernameFilters : IAdminUserReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, AdminUserReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.Username))
            {
                criteria.Add(Restrictions.InsensitiveLike($"{nameof(PSSAdminUsers.User)}.{nameof(PSSAdminUsers.User.UserName)}", searchParams.Username, MatchMode.Anywhere));
            }

        }
    }

    /// <summary>
    /// Command type filter
    /// </summary>
    public class AdminUserReportCommandIdFilters : IAdminUserReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, AdminUserReportSearchParams searchParams)
        {
            if (searchParams.CommandId > 0)
            {
                criteria.Add(Restrictions.Where<PSSAdminUsers>(x => x.Command == new Command { Id = searchParams.CommandId }));
            }

        }
    }

    /// <summary>
    /// Formation level filter
    /// </summary>
    public class AdminUserReportFormationLevelFilters : IAdminUserReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, AdminUserReportSearchParams searchParams)
        {
            if (searchParams.CommandCategoryId > 0)
            {
                criteria.Add(Restrictions.Where<PSSAdminUsers>(x => x.CommandCategory == new CommandCategory { Id = searchParams.CommandCategoryId }));
            }

        }
    }

    public class AdminUserReportStatusFilters : IAdminUserReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, AdminUserReportSearchParams searchParams)
        {
            if (searchParams.Status != (int)StatusFilter.All)
            {
                bool status = searchParams.Status == 1;
                criteria.Add(Restrictions.Where<PSSAdminUsers>(x => x.IsActive == status));

            }
        }
    }
}