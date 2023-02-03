using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentReport.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class AccountWalletPaymentReportHandler : IAccountWalletPaymentReportHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly IOrchardServices _orchardServices;
        private readonly Lazy<IAccountWalletPaymentReportFilter> _accountWalletPaymentReportFilters;
        private readonly IPSSExpenditureHeadManager<PSSExpenditureHead> _expenditureHeadManager;
        private readonly IAccountWalletUserConfigurationPSServiceRequestFlowApproverManager<AccountWalletConfigurationPSServiceRequestFlowApprover> _serviceRequestFlowApproverManager;

        public AccountWalletPaymentReportHandler(IHandlerComposition handlerComposition, Lazy<IAccountWalletPaymentReportFilter> accountWalletPaymentReportFilters, IPSSExpenditureHeadManager<PSSExpenditureHead> expenditureHeadManager, IOrchardServices orchardServices, IAccountWalletUserConfigurationPSServiceRequestFlowApproverManager<AccountWalletConfigurationPSServiceRequestFlowApprover> serviceRequestFlowApproverManager)
        {
            _handlerComposition = handlerComposition;
            _accountWalletPaymentReportFilters = accountWalletPaymentReportFilters;
            _expenditureHeadManager = expenditureHeadManager;
            _orchardServices = orchardServices;
            _serviceRequestFlowApproverManager = serviceRequestFlowApproverManager;
        }

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewWalletPaymentReport"></param>
        public void CheckForPermission(Permission canViewWalletPaymentReport)
        {
            _handlerComposition.IsAuthorized(canViewWalletPaymentReport);
        }

        /// <summary>
        /// Gets the Account Wallet Payment Report VM
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public AccountWalletPaymentReportVM GetAccountWalletPaymentReportVM(AccountWalletPaymentSearchParams searchParams)
        {
            searchParams.ApplyRestriction = _serviceRequestFlowApproverManager.CheckIfUserIsWalletRolePlayer(_orchardServices.WorkContext.CurrentUser.Id);

            dynamic recordsAndAggregate = _accountWalletPaymentReportFilters.Value.GetAccountWalletPaymentReportViewModel(searchParams);
            IEnumerable<Core.HelperModels.AccountWalletPaymentReportVM> reports = (IEnumerable<Core.HelperModels.AccountWalletPaymentReportVM>)recordsAndAggregate.ReportRecords;

            return new AccountWalletPaymentReportVM
            {
                ExpenditureHeads = _expenditureHeadManager.GetActiveExpenditureHead(),
                AccountWalletPaymentReports = reports.ToList(),
                SourceAccount = searchParams.SourceAccountName,
                PaymentId = searchParams.PaymentId,
                ExpenditureHeadId = searchParams.ExpenditureHeadId,
                Status = searchParams.Status,
                BeneficiaryAccountNumber = searchParams.BeneficiaryAccoutNumber,
                From = searchParams.StartDate.ToString("dd'/'MM'/'yyyy"),
                End = searchParams.EndDate.ToString("dd'/'MM'/'yyyy"),
                TotalAccountWalletPaymentReportRecord = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalAccountWalletPaymentReportRecord).First().TotalRecordCount,
                TotalAccountWalletPaymentReportAmount = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalAccountWalletPaymentReportRecord).First().TotalAmount
            };
        }

    }
}