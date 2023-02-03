using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface ICommandStatementReportHandler : IDependency
    {
        /// <summary>
        /// Get Reports VM for Command Statement Report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        CommandStatementReportVM GetVMForReports(CommandStatementReportSearchParams searchParams);

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewCommandWalletReports"></param>
        void CheckForPermission(Permission canViewCommandWalletReports);

        /// <summary>
        /// Get command wallet details using the commandCode <paramref name="commandCode"/>
        /// </summary>
        /// <param name="commandCode"></param>
        /// <returns><see cref="CommandWalletDetailsVM"/></returns>
        CommandWalletDetailsVM GetCommandWalletDetailsByCommandCode(string commandCode);

        /// <summary>
        /// Get customer account balance
        /// </summary>
        /// <param name="walletIdentifier"></param>
        /// <returns>decimal</returns>
        decimal GetCustomerAccountBalance(string walletIdentifier);
    }
}
