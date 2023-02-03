using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.API.Controllers.Handlers.Contracts
{
    public interface IUSSDRequestApprovalHandler : IDependency
    {
        /// <summary>
        /// Process request approval details
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="sRequestFormDump"></param>
        /// <returns>RequestApprovalResponse</returns>
        RequestApprovalResponse ProcessRequestApproval(string fileNumber, ref List<ErrorModel> errors, dynamic userInput, string phoneNumber);


        /// <summary>
        /// Do process for request rejection
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="errors"></param>
        /// <param name="comment"></param>
        /// <param name="id"></param>
        /// <returns>RequestApprovalResponse</returns>
        RequestApprovalResponse ProcessRequestRejection(string fileNumber, ref List<ErrorModel> errors, string comment, string phoneNumber);


        /// <summary>
        /// Check if admin can assign officers
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <param name="phoneNumber"></param>
        /// <returns>bool</returns>
        bool CanAdminAssignOfficers(string fileNumber, string phoneNumber);

        /// <summary>
        /// Confirms the admin user has viewed the service specific draft document during approval
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        void ConfirmAdminHasViewedDraftDocument(string fileRefNumber, string phoneNumber);
    }
}
