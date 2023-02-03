using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.DataFilters.Requests.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.Requests.SearchFilters;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Orchard;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PoliceRequestHandler : IRequestListHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly Lazy<IPSServiceManager<PSService>> _serviceManager;
        private readonly IPoliceRequestFilter _requestFilter;
        private readonly IAdminRequestFilter _adminRequestApprovalFilter;
        private readonly Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> _approvalAccesRoleManager;
        private readonly Lazy<IInvoiceFilter> _invoiceFilter;
        private readonly Lazy<IPSSRequestManager<PSSRequest>> _requestManager;
        private readonly Lazy<ICoreStateAndLGA> _coreStateLGAService;
        private readonly Lazy<ICoreCommand> _coreCommand;
        private readonly IOrchardServices _orchardServices;
        private readonly Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> _serviceRequestFlowApprover;

        public PoliceRequestHandler(Lazy<IPSServiceManager<PSService>> serviceManager, IHandlerComposition handlerComposition, IPoliceRequestFilter requestFilter, Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> approvalAccesRoleManager, Lazy<IInvoiceFilter> invoiceFilter, Lazy<IPSSRequestManager<PSSRequest>> requestManager, Lazy<ICoreStateAndLGA> coreStateLGAService, Lazy<ICoreCommand> coreCommand, IAdminRequestFilter adminRequestApprovalFilter, IOrchardServices orchardServices, Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> serviceRequestFlowApprover)
        {
            _handlerComposition = handlerComposition;
            _serviceManager = serviceManager;
            _requestFilter = requestFilter;
            _approvalAccesRoleManager = approvalAccesRoleManager;
            _invoiceFilter = invoiceFilter;
            _requestManager = requestManager;
            _coreStateLGAService = coreStateLGAService;
            _coreCommand = coreCommand;
            _adminRequestApprovalFilter = adminRequestApprovalFilter;
            _orchardServices = orchardServices;
            _serviceRequestFlowApprover = serviceRequestFlowApprover;
        }


        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        public void CheckForPermission(Orchard.Security.Permissions.Permission permission)
        {
            _handlerComposition.IsAuthorized(permission);
        }


        /// <summary>
        /// Get vm for approval requests view
        /// <para>VM that contains the list of requests for approval</para>
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyApprovalAccessRestrictions"></param>
        /// <returns>RequestReportVM</returns>
        public RequestReportVM GetVMForApprovalRequestReport(RequestsReportSearchParams searchParams, bool applyApprovalAccessRestrictions = true)
        {
            RequestReportVM vm = new RequestReportVM { };

            int parsedId = 0;
            if (Int32.TryParse(searchParams.SelectedServiceId, out parsedId)) { searchParams.IntValueSelectedServiceId = parsedId; }
            if (Int32.TryParse(searchParams.SelectedCommand, out parsedId)) { searchParams.CommandId = parsedId; }

            int accessRoleUserId = _approvalAccesRoleManager.Value.GetAccessRoleUserId(searchParams.AdminUserId, Core.Models.Enums.AdminUserType.Approver);
            searchParams.ApprovalAccessRoleUserId = accessRoleUserId;
            searchParams.CheckWorkFlowLogActiveStatus = true;
            bool applyAccessRestrictions = accessRoleUserId > 0;
            applyApprovalAccessRestrictions = accessRoleUserId > 0;

            dynamic recordsAndAggregate = _adminRequestApprovalFilter.GetRecordsBasedOnActiveWorkFlow(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);

            IEnumerable<PSSRequestVM> reports = ((IEnumerable<PSSRequestVM>)recordsAndAggregate.ReportRecords);
            IEnumerable<PSSRequestTypeVM> services = _serviceManager.Value.GetAllServices();
            List<CBS.Core.Models.StateModel> states = _coreStateLGAService.Value.GetStates();

            return new RequestReportVM
            {
                From = searchParams.StartDate.ToString("dd'/'MM'/'yyyy"),
                End = searchParams.EndDate.ToString("dd'/'MM'/'yyyy"),
                Status = searchParams.RequestOptions.RequestStatus,
                InvoiceNumber = searchParams.RequestOptions.InvoiceNumber,
                FileNumber = searchParams.RequestOptions.FileNumber,
                Requests = (reports == null || !reports.Any()) ? new List<PSSRequestVM> { } : reports.ToList(),
                ServiceRequestTypes = services.ToList(),
                ServiceType = searchParams.SelectedServiceId,
                StateLGAs = states,
                ListLGAs = (searchParams.State != 0) ? states.Where(x => x.Id == searchParams.State).First().LGAs.ToList() : null,
                State = searchParams.State,
                LGA = searchParams.LGA,
                SelectedCommand = searchParams.SelectedCommand,
                Commands = (searchParams.LGA != 0) ? _coreCommand.Value.GetCommandsByLGAId(searchParams.LGA) : null,
                TotalRequestRecord = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalRecordCount).First().TotalRecordCount),
                SelectedRequestPhase = searchParams.SelectedRequestPhase
            };
        }

        /// <summary>
        /// Get vm for requests view
        /// <para>VM that contains the list of requests</para>
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>RequestReportVM</returns>
        public RequestReportVM GetVMForRequestReport(RequestsReportSearchParams searchParams)
        {
            RequestReportVM vm = new RequestReportVM { };

            int parsedId = 0;
            if (Int32.TryParse(searchParams.SelectedServiceId, out parsedId)) { searchParams.IntValueSelectedServiceId = parsedId; }
            if (Int32.TryParse(searchParams.SelectedCommand, out parsedId)) { searchParams.CommandId = parsedId; }

            int accessRoleUserId = _approvalAccesRoleManager.Value.GetAccessRoleUserId(searchParams.AdminUserId);
            searchParams.ApprovalAccessRoleUserId = accessRoleUserId;
            bool applyAccessRestrictions = accessRoleUserId > 0;
            bool applyApprovalAccessRestrictions = _approvalAccesRoleManager.Value.GetAccessRoleUserId(searchParams.AdminUserId, AdminUserType.Approver) > 0;
            dynamic recordsAndAggregate = _adminRequestApprovalFilter.GetRecordsBasedOnWorkFlow(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);

            IEnumerable<PSSRequestVM> reports = ((IEnumerable<PSSRequestVM>)recordsAndAggregate.ReportRecords);
            IEnumerable<PSSRequestTypeVM> services = _serviceManager.Value.GetAllServices();
            List<CBS.Core.Models.StateModel> states = _coreStateLGAService.Value.GetStates();

            return new RequestReportVM
            {
                From = searchParams.StartDate.ToString("dd'/'MM'/'yyyy"),
                End = searchParams.EndDate.ToString("dd'/'MM'/'yyyy"),
                Status = searchParams.RequestOptions.RequestStatus,
                InvoiceNumber = searchParams.RequestOptions.InvoiceNumber,
                FileNumber = searchParams.RequestOptions.FileNumber,
                Requests = (reports == null || !reports.Any()) ? new List<PSSRequestVM> { } : reports.ToList(),
                ServiceRequestTypes = services.ToList(),
                ServiceType = searchParams.SelectedServiceId,
                StateLGAs = states,
                ListLGAs = (searchParams.State != 0) ? states.Where(x => x.Id == searchParams.State).First().LGAs.ToList() : null,
                State = searchParams.State,
                LGA = searchParams.LGA,
                SelectedCommand = searchParams.SelectedCommand,
                Commands = (searchParams.LGA != 0) ? _coreCommand.Value.GetCommandsByLGAId(searchParams.LGA) : null,
                TotalRequestRecord = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalRecordCount).First().TotalRecordCount),
                TotalNumberOfInvoices = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalInvoiceCount).First().TotalRecordCount),
                TotalRequestAmount = ((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalInvoiceAmount).First().TotalAmount
            };
        }


        /// <summary>
        /// Get vm for requests view
        /// <para>VM that contains the list of requests</para>
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyApprovalAccessRestrictions"></param>
        /// <returns>RequestReportVM</returns>
        public RequestReportVM GetVMForRequestReport(RequestsReportSearchParams searchParams, bool applyApprovalAccessRestrictions)
        {
            RequestReportVM vm = new RequestReportVM { };

            int parsedId = 0;
            if (Int32.TryParse(searchParams.SelectedServiceId, out parsedId)) { searchParams.IntValueSelectedServiceId = parsedId; }
            if (Int32.TryParse(searchParams.SelectedCommand, out parsedId)) { searchParams.CommandId = parsedId; }

            bool applyAccessRestrictions = _approvalAccesRoleManager.Value.UserHasAcessTypeRole(searchParams.AdminUserId);

            dynamic recordsAndAggregate = GetRequestReport(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);

            IEnumerable<PSSRequestVM> reports = ((IEnumerable<PSSRequestVM>)recordsAndAggregate.ReportRecords);
            IEnumerable<PSSRequestTypeVM> services = _serviceManager.Value.GetAllServices();
            List<CBS.Core.Models.StateModel> states = _coreStateLGAService.Value.GetStates();

            return new RequestReportVM
            {
                From = searchParams.StartDate.ToString("dd'/'MM'/'yyyy"),
                End = searchParams.EndDate.ToString("dd'/'MM'/'yyyy"),
                Status = searchParams.RequestOptions.RequestStatus,
                InvoiceNumber = searchParams.RequestOptions.InvoiceNumber,
                FileNumber = searchParams.RequestOptions.FileNumber,
                Requests = (reports == null || !reports.Any()) ? new List<PSSRequestVM> { } : reports.ToList(),
                ServiceRequestTypes = services.ToList(),
                ServiceType = searchParams.SelectedServiceId,
                StateLGAs = states,
                ListLGAs = (searchParams.State != 0) ? states.Where(x => x.Id == searchParams.State).First().LGAs.ToList() : null,
                State = searchParams.State,
                LGA = searchParams.LGA,
                SelectedCommand = searchParams.SelectedCommand,
                Commands = (searchParams.LGA != 0) ? _coreCommand.Value.GetCommandsByLGAId(searchParams.LGA) : null,
                TotalNumberOfInvoices = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalInvoiceCount).First().TotalRecordCount),
                TotalRequestRecord = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalRecordCount).First().TotalRecordCount),
                TotalRequestAmount = ((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalInvoiceAmount).First().TotalAmount
            };
        }


        /// <summary>
        /// Get report view model for the given search parameters
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>dynamic</returns>
        public dynamic GetRequestReport(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {
            return _requestFilter.GetRequestReportViewModel(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);
        }


        /// <summary>
        /// Get invoices for request with specified id
        /// </summary>
        /// <param name="requestId">Request Id</param>
        /// <returns>PSSRequestInvoiceVM</returns>
        public PSSRequestInvoiceVM GetInvoicesForRequest(long requestId)
        {
            try
            {
                PSSRequestInvoiceVM vm = new PSSRequestInvoiceVM { Invoices = new List<RequestInvoiceVM>() };
                var invoices = _requestManager.Value.GetRequestInvoices(requestId);
                vm.Invoices = invoices;
                return vm;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}