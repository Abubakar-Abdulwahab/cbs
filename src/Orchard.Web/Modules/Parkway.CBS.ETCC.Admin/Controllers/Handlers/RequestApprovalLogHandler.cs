using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.ETCC.Admin.Controllers.Handlers
{
    public class RequestApprovalLogHandler : IRequestApprovalLogHandler
    {
        private readonly ITaxClearanceCertificateRequestApprovalLogManager<TaxClearanceCertificateRequestApprovalLog> _approvalLogManager;
        private readonly ITaxClearanceCertificateRequestManager<TaxClearanceCertificateRequest> _taxClearanceCertificateManager;
        public RequestApprovalLogHandler(ITaxClearanceCertificateRequestApprovalLogManager<TaxClearanceCertificateRequestApprovalLog> approvalLogManager, ITaxClearanceCertificateRequestManager<TaxClearanceCertificateRequest> taxClearanceCertificateManager)
        {
            _approvalLogManager = approvalLogManager;
            _taxClearanceCertificateManager = taxClearanceCertificateManager;
        }


        /// <summary>
        /// Get request approval log vm for request with specified application number
        /// </summary>
        /// <param name="applicationNumber"></param>
        /// <returns>TCCRequestApprovalLogVM</returns>
        public TCCRequestApprovalLogVM GetRequestApprovalLogVM(string applicationNumber)
        {
            try
            {
                TCCRequestApprovalLogVM approvalLogVM = new TCCRequestApprovalLogVM { ApprovalLog = new List<TCCApprovalLogVM>() };
                long requestId = _taxClearanceCertificateManager.GetRequestRequestId(applicationNumber);
                approvalLogVM.ApprovalLog = _approvalLogManager.GetApprovalLogForRequestById(requestId);
                return approvalLogVM;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}