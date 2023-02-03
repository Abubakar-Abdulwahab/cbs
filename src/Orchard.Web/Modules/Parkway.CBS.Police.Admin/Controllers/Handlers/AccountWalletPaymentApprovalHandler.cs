using Orchard;
using Orchard.Logging;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentApprovalReport.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PaymentRequestSettlement.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class AccountWalletPaymentApprovalHandler : IAccountWalletPaymentApprovalHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly IOrchardServices _orchardServices;
        private readonly IPaymentRequestService _paymentRequestService;
        private readonly Lazy<IAccountWalletPaymentApprovalFilter> _accountWalletPaymentApprovalFilter;
        private readonly IAccountPaymentRequestManager<AccountPaymentRequest> _accountPaymentRequestManager;
        private readonly IAccountPaymentRequestItemManager<AccountPaymentRequestItem> _accountPaymentRequestItemManager;
        private readonly IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover> _serviceRequestFlowApproverManager;
        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _serviceRequestFlowDefinitionLevelManager;
        ILogger Logger { get; set; }


        public AccountWalletPaymentApprovalHandler(IHandlerComposition handlerComposition, Lazy<IAccountWalletPaymentApprovalFilter> accountWalletPaymentApprovalFilter, IAccountPaymentRequestManager<AccountPaymentRequest> accountPaymentRequestManager, IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover> serviceRequestFlowApproverManager, IOrchardServices orchardServices, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> serviceRequestFlowDefinitionLevelManager, IAccountPaymentRequestItemManager<AccountPaymentRequestItem> accountPaymentRequestItemManager, IPaymentRequestService paymentRequestService)
        {
            _handlerComposition = handlerComposition;
            _accountWalletPaymentApprovalFilter = accountWalletPaymentApprovalFilter;
            _accountPaymentRequestManager = accountPaymentRequestManager;
            _serviceRequestFlowApproverManager = serviceRequestFlowApproverManager;
            _orchardServices = orchardServices;
            _serviceRequestFlowDefinitionLevelManager = serviceRequestFlowDefinitionLevelManager;
            Logger = NullLogger.Instance;
            _accountPaymentRequestItemManager = accountPaymentRequestItemManager;
            _paymentRequestService = paymentRequestService;
        }

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewWalletPaymentApprovalReport"></param>
        public void CheckForPermission(Permission canViewWalletPaymentApprovalReport)
        {
            _handlerComposition.IsAuthorized(canViewWalletPaymentApprovalReport);
        }

        /// <summary>
        /// Gets view model for the view 
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public AccountWalletPaymentApprovalRequestVM GetPaymentApprovalRequestVM(AccountWalletPaymentApprovalSearchParams searchParams)
        {
            dynamic recordsAndAggregate = _accountWalletPaymentApprovalFilter.Value.GetAccountWalletPaymentApprovalReportViewModel(searchParams);
            IEnumerable<AccountWalletPaymentApprovalReportVM> reports = (IEnumerable<AccountWalletPaymentApprovalReportVM>)recordsAndAggregate.ReportRecords;

            return new AccountWalletPaymentApprovalRequestVM
            {
                AccountWalletPaymentApprovalReports = reports.ToList(),
                SourceAccount = searchParams.SourceAccountName,
                PaymentId = searchParams.PaymentId,
                From = searchParams.StartDate.ToString("dd'/'MM'/'yyyy"),
                End = searchParams.EndDate.ToString("dd'/'MM'/'yyyy"),
                TotalAccountWalletPaymentApprovalRecord = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalAccountWalletPaymentApprovalRecord).First().TotalRecordCount
            };
        }

        /// <summary>
        /// Gets the view model for the details view
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        public WalletPaymentRequestApprovalDetailVM GetViewDetailVM(string paymentId)
        {
            return _accountPaymentRequestManager.GetWalletPaymentRequestApprovalDetailVM(paymentId);
        }

        /// <summary>
        /// Approves or Declines a request based on the value of <paramref name="approveRequest"/>
        /// And sends to payment provider if it's an authorization
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="approveRequest"></param>
        /// <exception cref="NoRecordFoundException"></exception>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public string ProcessPaymentRequest(string paymentId, bool approveRequest)
        {
            try
            {
                AccountPaymentRequestVM accountPaymentRequest = _accountPaymentRequestManager.GetWalletPaymentDetailByPaymentId(paymentId) ?? throw new NoRecordFoundException($"No record found with payment Id : {paymentId}");

                string message = string.Empty;

                if (!_serviceRequestFlowApproverManager.UserIsValidApproverForDefinitionLevel(_orchardServices.WorkContext.CurrentUser.Id, accountPaymentRequest.FlowDefinitionLevelId))
                {
                    Logger.Error($"Unauthorized access for payment approval with reference {paymentId}. User Id: {_orchardServices.WorkContext.CurrentUser.Id}. Definition Level: {accountPaymentRequest.FlowDefinitionLevelId}");
                    throw new UserNotAuthorizedForThisActionException();
                }

                if (approveRequest)
                {
                    if (_serviceRequestFlowDefinitionLevelManager.CheckIfThisIsLastPaymentApprover(accountPaymentRequest.FlowDefinitionId, accountPaymentRequest.FlowDefinitionLevelPosition))
                    {
                        message = _paymentRequestService.BeginRequestPaymentProcess(paymentId);
                    }
                    else
                    {
                        _paymentRequestService.UpdatePaymentRequestFlow(accountPaymentRequest.Id, accountPaymentRequest.FlowDefinitionLevelPosition, accountPaymentRequest.FlowDefinitionId, PaymentRequestStatus.UNDERAPPROVAL);

                        message = $"₦{accountPaymentRequest.TotalAmount:n2} has been successfully approved with payment reference : {paymentId} on source account: {accountPaymentRequest.SourceAccount}.";
                        Logger.Information($"₦{accountPaymentRequest.TotalAmount:n2} has been successfully approved with payment reference : {paymentId} on source account: {accountPaymentRequest.SourceAccount}. User Id: {_orchardServices.WorkContext.CurrentUser.Id}");
                    }
                }
                else
                {
                    _paymentRequestService.UpdatePaymentRequestFlow(accountPaymentRequest.Id, accountPaymentRequest.FlowDefinitionLevelPosition, accountPaymentRequest.FlowDefinitionId, PaymentRequestStatus.DECLINED);

                    message = $"₦{accountPaymentRequest.TotalAmount:n2} has been successfully declined with payment reference: {paymentId} on source account: {accountPaymentRequest.SourceAccount}.";
                    Logger.Information($"₦{accountPaymentRequest.TotalAmount:n2} has been successfully declined with payment reference: {paymentId} on source account: {accountPaymentRequest.SourceAccount}. User Id: {_orchardServices.WorkContext.CurrentUser.Id}");
                }

                return message;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _accountPaymentRequestManager.RollBackAllTransactions();
                throw;
            }
        }
    }
}