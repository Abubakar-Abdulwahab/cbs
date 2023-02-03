using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface ICommandWalletDetailsManager<CommandWalletDetails> : IDependency, IBaseManager<CommandWalletDetails>
    {
        /// <summary>
        /// Get command wallet details using the commandId <paramref name="commandId"/>
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns><see cref="CommandWalletDetailsVM"/></returns>
        CommandWalletDetailsVM GetCommandWalletDetailsByCommandId(int commandId);

        /// <summary>
        /// Get command wallet details using the commandCode <paramref name="commandCode"/>
        /// </summary>
        /// <param name="commandCode"></param>
        /// <returns><see cref="CommandWalletDetailsVM"/></returns>
        CommandWalletDetailsVM GetCommandWalletDetailsByCommandCode(string commandCode);

        /// <summary>
        /// Checks if <paramref name="accountNumber"/> already exists in <see cref="CommandWalletDetails"/>
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns>boolean</returns>
        bool CheckIfCommandWalletExist(string accountNumber);

        /// <summary>
        /// Checks if specified command already has the specified account type
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="accountType"></param>
        /// <returns>boolean</returns>
        bool CheckIfCommandWalletAccountTypeExist(int commandId, int accountType);

    }
}
