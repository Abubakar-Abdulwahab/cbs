using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.DataFilters.AccountWalletReport.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class AccountsWalletReportHandler : IAccountsWalletReportHandler
    {
        private readonly Lazy<IAccountWalletReportFilter> _accountWalletReportFilter;
        private readonly IHandlerComposition _handlerComposition;
        private readonly IBankManager<Bank> _bankManager;

        public AccountsWalletReportHandler(IHandlerComposition handlerComposition, Lazy<IAccountWalletReportFilter> accountWalletReportFilter, IBankManager<Bank> bankManager)
        {
            _handlerComposition = handlerComposition;
            _accountWalletReportFilter = accountWalletReportFilter;
            _bankManager = bankManager;
        }

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewAccountWalletReport"></param>
        public void CheckForPermission(Permission canViewAccountWalletReport)
        {
            _handlerComposition.IsAuthorized(canViewAccountWalletReport);
        }

        /// <summary>
        /// Get Reports VM for account wallet Report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public AccountsWalletReportVM GetVMForReports(AccountWalletReportSearchParams searchParams)
        {
            dynamic recordsAndAggregate = _accountWalletReportFilter.Value.GetAccountWalletReportViewModel(searchParams);
            IEnumerable<Core.HelperModels.AccountsWalletReportVM> reports = (IEnumerable<Core.HelperModels.AccountsWalletReportVM>)recordsAndAggregate.ReportRecords;

            return new AccountsWalletReportVM
            {
                Banks = _bankManager.GetAllActiveBanks(),
                AccountsWalletReports = reports,
                AccountNumber = searchParams.AccountNumber,
                AccountName = searchParams.AccountName,
                BankId = searchParams.BankId,
                TotalAccountWalletRecord = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalAccountWalletRecord).First().TotalRecordCount
            };
        }
    }
}