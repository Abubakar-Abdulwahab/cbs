using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.DataFilters.TCCReport.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.SMS.Provider.Contracts;
using Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.ETCC.Admin.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using OrchardPermission = Orchard.Security.Permissions.Permission;


namespace Parkway.CBS.ETCC.Admin.Controllers.Handlers
{
    public class TCCReportHandler : ITCCReportHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        private readonly Lazy<ITCCRequestReportFilter> _tccRequestReportFilter;
        private readonly Lazy<ITaxClearanceCertificateRequestManager<TaxClearanceCertificateRequest>> _tccRequestManager;
        private readonly Lazy<ICoreTaxClearanceCertificateRequestService> _coreTCCRequestService;
        private readonly Lazy<IPAYEBatchItemsManager<PAYEBatchItems>> _payeBatchItemsManager;
        private readonly Lazy<ITaxClearanceCertificateRequestApproverManager<TaxClearanceCertificateRequestApprover>> _tccRequestApproverManager;


        public TCCReportHandler(IOrchardServices orchardServices, Lazy<ITCCRequestReportFilter> tccRequestReportFilter, Lazy<ITaxClearanceCertificateRequestManager<TaxClearanceCertificateRequest>> tccRequestManager, Lazy<ICoreTaxClearanceCertificateRequestService> coreTCCRequestService, Lazy<IPAYEBatchItemsManager<PAYEBatchItems>> payeBatchItemsManager, Lazy<ITaxClearanceCertificateRequestApproverManager<TaxClearanceCertificateRequestApprover>> tccRequestApproverManager)
        {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            _tccRequestReportFilter = tccRequestReportFilter;
            _tccRequestManager = tccRequestManager;
            _coreTCCRequestService = coreTCCRequestService;
            _payeBatchItemsManager = payeBatchItemsManager;
            _tccRequestApproverManager = tccRequestApproverManager;
        }

        public void CheckForPermission(Permission CanViewTCCRequests)
        {
            IsAuthorized(CanViewTCCRequests);
        }

        /// <summary>
        /// Get TCC request details for a particular application number
        /// </summary>
        /// <param name="applicationNumber"></param>
        /// <returns></returns>
        public TCCRequestDetailVM GetRequestDetail(string applicationNumber)
        {
            TCCRequestDetailVM tccRequestDetails = _tccRequestManager.Value.GetRequestDetails(applicationNumber);
            tccRequestDetails.Payments = _payeBatchItemsManager.Value.GetPAYEPayments(tccRequestDetails.ApplicationYear, tccRequestDetails.TaxEntityId);
            return tccRequestDetails;
        }

        /// <summary>
        /// Get all TCC request based on the user selected criteria
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public TCCRequestReportVM GetRequestReport(TCCReportSearchParams searchParams)
        {
            dynamic recordsAndAggregate = _tccRequestReportFilter.Value.GetReportViewModel(searchParams);
            IEnumerable<TCCRequestVM> records = ((IEnumerable<TCCRequestVM>)recordsAndAggregate.ReportRecords);          

            return new TCCRequestReportVM
            {
                From = searchParams.StartDate.ToString("dd'/'MM'/'yyyy"),
                End = searchParams.EndDate.ToString("dd'/'MM'/'yyyy"),
                ApplicantName = searchParams.ApplicantName,
                ApplicationNumber = searchParams.ApplicationNumber,
                PayerId = searchParams.PayerId,
                Status = searchParams.SelectedStatus,
                Requests = (records == null || !records.Any()) ? new List<TCCRequestVM> { } : records.ToList(),
                TotalRequestRecord = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount),
            };
        }

        /// <summary>
        /// Check if the user is authorized to perform an action
        /// </summary>
        /// <param name="permission"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public void IsAuthorized(OrchardPermission permission)
        {
            if (!_authorizer.Authorize(permission, ErrorLang.usernotauthorized()))
                throw new UserNotAuthorizedForThisActionException();
        }

        /// <summary>
        /// Get byte doc for TCC certificate
        /// </summary>
        /// <param name="tccNumber"></param>
        /// <returns>CreateReceiptDocumentVM</returns>
        public CreateReceiptDocumentVM CreateCertificateByteFile(string tccNumber)
        {
            if (_tccRequestManager.Value.Count(x => x.TCCNumber == tccNumber) < 1)
            {
                throw new NoRecordFoundException("404 for TCC application request. TCC Number " + tccNumber);
            }

            return _coreTCCRequestService.Value.CreateCertificateDocument(tccNumber, true);
        }

        /// <summary>
        /// Get user approver details using the userAdminId
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <returns>WorkflowApproverDetailVM</returns>
        public WorkflowApproverDetailVM GetApproverDetails(int adminUserId)
        {
            return _tccRequestApproverManager.Value.GetApproverDetails(adminUserId);
        }

    }
}