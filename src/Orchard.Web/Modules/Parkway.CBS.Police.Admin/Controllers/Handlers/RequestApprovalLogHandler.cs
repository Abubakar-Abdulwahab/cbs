using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class RequestApprovalLogHandler : IRequestApprovalLogHandler
    {
        private readonly Lazy<IPSSRequestApprovalLogManager<PSSRequestApprovalLog>> _approvalLogManager;
        public RequestApprovalLogHandler(Lazy<IPSSRequestApprovalLogManager<PSSRequestApprovalLog>> approvalLogManager)
        {
            _approvalLogManager = approvalLogManager;
        }


        /// <summary>
        /// Get request approval log vm for request with specified Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public RequestApprovalLogVM GetRequestApprovalLogVMByRequestId(long requestId)
        {
            try
            {
                RequestApprovalLogVM approvalLogVM = new RequestApprovalLogVM { ApprovalLog = new List<ApprovalLogVM>() };
                approvalLogVM.ApprovalLog = _approvalLogManager.Value.GetApprovalLogForRequestById(requestId);
                return approvalLogVM;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}