using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IAccountWalletConfigurationManager<AccountWalletConfiguration> : IDependency, IBaseManager<AccountWalletConfiguration>
    {
        /// <summary>
        /// Checks if wallet configuration exist using <paramref name="acctWalletId"/>
        /// </summary>
        /// <param name="acctWalletId"></param>
        /// <returns></returns>
        bool CheckIfAccountWalletConfigurationExist(int acctWalletId);

        /// <summary>
        /// Gets command attached to source account with specified id
        /// </summary>
        /// <param name="accountWalletId"></param>
        /// <returns></returns>
        CommandVM GetCommandAttachedToSourceAccount(int accountWalletId);

        /// <summary>
        /// Gets the flow defintion Id using <paramref name="acctWalletId"/>
        /// </summary>
        /// <param name="acctWalletId"></param>
        /// <returns></returns>
        int GetFlowDefinitionByWalletId(int acctWalletId);

        /// <summary>
        /// Gets the account number using the <paramref name="accountWalletConfigurationId"/>
        /// </summary>
        /// <param name="accountWalletConfigurationId"></param>
        /// <returns></returns>
        string GetWalletAccountNumber(int accountWalletConfigurationId);


        /// <summary>
        /// Get account wallet configuration using <paramref name="accountWalletId"/>
        /// </summary>
        /// <param name="accountWalletId"></param>
        /// <returns></returns>
        AccountWalletConfigurationDTO GetAccountWalletConfigurationDetail(int accountWalletId);

        /// <summary>
        /// Get account wallet configuration and command details using <paramref name="accountWalletId"/>
        /// </summary>
        /// <param name="accountWalletId"></param>
        /// <returns></returns>
        AccountWalletConfigurationDTO GetAccountWalletConfigurationDetailWithCommandDetails(int accountWalletId);

        /// <summary>
        /// Gets the flow definition levels using the <paramref name="acctWalletId"/>
        /// </summary>
        /// <param name="acctWalletId"></param>
        /// <returns></returns>
        List<PSServiceRequestFlowDefinitionLevelDTO> GetFlowDefinitionLevelByWalletId(int acctWalletId);

        /// <summary>
        /// Gets the account name using the <paramref name="acctWalletId"/>
        /// </summary>
        /// <param name="acctWalletId"></param>
        /// <returns></returns>
        string GetWalletName(int acctWalletId);
    }
}
