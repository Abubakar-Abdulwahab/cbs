using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IAccountWalletUserConfigurationPSServiceRequestFlowApproverManager<AccountWalletUserConfigurationPSServiceRequestFlowApprover> : IDependency, IBaseManager<AccountWalletUserConfigurationPSServiceRequestFlowApprover>
    {
        /// <summary>
        /// Checks if user is an approver for wallet(s) 
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <returns></returns>
        bool CheckIfUserIsWalletRolePlayer(int userPartRecordId);

        /// <summary>
        /// Gets all wallets assigned to the current user as payment initiator
        /// </summary>
        /// <returns></returns>
        List<AccountWalletConfigurationVM> GetWalletsAssignedToUser(int userPartRecordId);

        /// <summary>
        /// Gets all command wallets assigned to the current user as payment initiator
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <param name="settlementAccountType"></param>
        /// <returns></returns>
        List<AccountWalletConfigurationVM> GetCommandWalletsAssignedToUser(int userPartRecordId, Models.Enums.SettlementAccountType settlementAccountType);

        /// <summary>
        /// Gets the users assigned to the wallet
        /// </summary>
        /// <param name="walletId"></param>
        /// <returns></returns>
        List<WalletUsersVM> GetWalletUsers(int walletId);

        /// <summary>
        /// Set IsDeleted for deleted approver(s)
        /// </summary>
        void UpdateDeletedApprover();
    }
}
