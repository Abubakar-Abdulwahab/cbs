using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.Requests.Contracts
{
    public interface IAdminRequestFilter : IDependency
    {
        /// <summary>
        /// Get approval request report 
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <param name="applyApprovalAccessRestrictions"></param>
        /// <returns>dynamic</returns>
        dynamic GetRecordsBasedOnActiveWorkFlow(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions);

        /// <summary>
        /// Get admin dashboard request statistics
        /// </summary>
        /// <param name="status"></param>
        /// <param name="adminUserId"></param>
        /// <param name="accessRoleUserId"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <param name="applyApprovalAccessRestrictions"></param>
        /// <param name="v"></param>
        /// <param name="checkWorkFlowLogActiveStatus"></param>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        IEnumerable<ReportStatsVM> GetAdminRequestStatistics(PSSRequestStatus status, int adminUserId, int accessRoleUserId, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions, bool v, bool checkWorkFlowLogActiveStatus=true);

        /// <summary>
        /// Get request report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <param name="applyApprovalAccessRestrictions"></param>
        /// <returns>dynamic</returns>
        dynamic GetRecordsBasedOnWorkFlow(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions);

    }
}
