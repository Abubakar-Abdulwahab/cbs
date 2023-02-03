using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.ETCC.Admin.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts
{
    public interface ITCCReportHandler : IDependency
    {
        /// <summary>
        /// Get all TCC request based on the user selected criteria
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        TCCRequestReportVM GetRequestReport(TCCReportSearchParams searchParams);

        /// <summary>
        /// Get TCC request details for a particular application number
        /// </summary>
        /// <param name="applicationNumber"></param>
        /// <returns>TCCRequestDetailVM</returns>
        TCCRequestDetailVM GetRequestDetail(string applicationNumber);


        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="CanViewTCCRequests"></param>
        void CheckForPermission(Permission CanViewTCCRequests);

        /// <summary>
        /// Get byte doc for TCC certificate
        /// </summary>
        /// <param name="tccNumber"></param>
        /// <returns>CreateReceiptDocumentVM</returns>
        CreateReceiptDocumentVM CreateCertificateByteFile(string tccNumber);

        /// <summary>
        /// Get user approver details using the userAdminId
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <returns>WorkflowApproverDetailVM</returns>
        WorkflowApproverDetailVM GetApproverDetails(int adminUserId);

    }
}
