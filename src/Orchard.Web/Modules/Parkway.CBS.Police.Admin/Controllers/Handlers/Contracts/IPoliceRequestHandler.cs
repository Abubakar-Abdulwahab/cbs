using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IRequestListHandler : IDependency
    {

        /// <summary>
        /// Get vm for requests view
        /// <para>VM that contains the list of requests</para>
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyApprovalAccessRestrictions"></param>
        /// <returns>RequestReportVM</returns>
        RequestReportVM GetVMForRequestReport(RequestsReportSearchParams searchParams, bool applyApprovalAccessRestrictions);

        /// <summary>
        /// Get vm for requests view
        /// <para>VM that contains the list of requests</para>
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>RequestReportVM</returns>
        RequestReportVM GetVMForRequestReport(RequestsReportSearchParams searchParams);

        /// <summary>
        /// Get vm for approval requests view
        /// <para>VM that contains the list of requests for approval</para>
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyApprovalAccessRestrictions"></param>
        /// <returns>RequestReportVM</returns>
        RequestReportVM GetVMForApprovalRequestReport(RequestsReportSearchParams searchParams, bool applyApprovalAccessRestrictions = true);

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        void CheckForPermission(Permission canViewRequests);


        /// <summary>
        /// Get invoices for request with specified id
        /// </summary>
        /// <param name="requestId">requestId</param>
        /// <returns>PSSRequestInvoiceVM</returns>
        PSSRequestInvoiceVM GetInvoicesForRequest(long requestId);

    }
}
