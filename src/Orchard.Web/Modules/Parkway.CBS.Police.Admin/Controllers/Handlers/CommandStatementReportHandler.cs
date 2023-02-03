using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DataFilters.CommandStatementReport.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class CommandStatementReportHandler : ICommandStatementReportHandler
    {
        private readonly Lazy<ICoreCommand> _coreCommand;
        private readonly Lazy<ICommandStatementReportFilter> _commandReportFilter;
        private readonly IHandlerComposition _handlerComposition;
        private readonly ICommandWalletDetailsManager<CommandWalletDetails> _commandWalletDetailsManager;
        private readonly ICoreReadyCashService _coreReadyCashService;

        public CommandStatementReportHandler(Lazy<ICoreCommand> coreCommand, Lazy<ICommandStatementReportFilter> commandstatementReportFilter, IHandlerComposition handlerComposition, ICommandWalletDetailsManager<CommandWalletDetails> commandWalletDetailsManager, ICoreReadyCashService coreReadyCashService)
        {
            _coreCommand = coreCommand;
            _commandReportFilter = commandstatementReportFilter;
            _handlerComposition = handlerComposition;
            _commandWalletDetailsManager = commandWalletDetailsManager;
            _coreReadyCashService = coreReadyCashService;
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
        /// Get command wallet details using the commandCode <paramref name="commandCode"/>
        /// </summary>
        /// <param name="commandCode"></param>
        /// <returns><see cref="CommandWalletDetailsVM"/></returns>
        public CommandWalletDetailsVM GetCommandWalletDetailsByCommandCode(string commandCode)
        {
            return _commandWalletDetailsManager.GetCommandWalletDetailsByCommandCode(commandCode);
        }

        /// <summary>
        /// Get customer account balance
        /// </summary>
        /// <param name="walletIdentifier"></param>
        /// <returns>decimal</returns>
        public decimal GetCustomerAccountBalance(string walletIdentifier)
        {
            return _coreReadyCashService.GetCustomerAccountBalance(walletIdentifier);
        }

        /// <summary>
        /// Get Reports VM for Command Statement Report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public CommandStatementReportVM GetVMForReports(CommandStatementReportSearchParams searchParams)
        {
            dynamic recordsAndAggregate = _commandReportFilter.Value.GetCommandStatementReportViewModel(searchParams);
            IEnumerable<CommandWalletStatementVM> reports = (IEnumerable<CommandWalletStatementVM>)recordsAndAggregate.ReportRecords;

            return new CommandStatementReportVM
            {
                TransactionReference = searchParams.TransactionReference,
                ValueDate = searchParams.ValueDate.HasValue ? searchParams.ValueDate.Value.ToString("dd/MM/yyyy") : null,
                TransactionType = (TransactionType)searchParams.TransactionTypeId,
                From = searchParams.StartDate.ToString("dd/MM/yyyy"),
                End = searchParams.EndDate.ToString("dd/MM/yyyy"),
                CommandWalletStatements = reports,
                TotalCommandWalletStatementRecord = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfStatementRecords).First().TotalRecordCount,
            };
        }


    }
}