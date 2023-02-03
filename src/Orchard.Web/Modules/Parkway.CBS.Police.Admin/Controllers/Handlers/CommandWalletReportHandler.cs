using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DataFilters.CommandReport.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class CommandWalletReportHandler : ICommandWalletReportHandler
    {
        private readonly Lazy<ICoreCommand> _coreCommand;
        private readonly Lazy<ICommandWalletReportFilter> _commandWalletReportFilter;
        private readonly IHandlerComposition _handlerComposition;

        public CommandWalletReportHandler(Lazy<ICoreCommand> coreCommand, Lazy<ICommandWalletReportFilter> commandWalletReportFilter, IHandlerComposition handlerComposition)
        {
            _coreCommand = coreCommand;
            _commandWalletReportFilter = commandWalletReportFilter;
            _handlerComposition = handlerComposition;
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
        /// Get Reports VM for Command Wallet Report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public CommandReportVM GetVMForReports(CommandWalletReportSearchParams searchParams)
        {
            dynamic recordsAndAggregate = _commandWalletReportFilter.Value.GetCommandWalletReportViewModel(searchParams);
            IEnumerable<CommandWalletReportVM> reports = (IEnumerable<CommandWalletReportVM>)recordsAndAggregate.ReportRecords;

            return new CommandReportVM
            {
                CommandWallets = reports,
                AccountNumber = searchParams.AccountNumber,
                CommandName = searchParams.CommandName,
                TotalActiveCommandRecord = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfActiveCommands).First().TotalRecordCount,
                SelectedAccountType = (SettlementAccountType)searchParams.SelectedAccountType,
            };
        }


    }
}