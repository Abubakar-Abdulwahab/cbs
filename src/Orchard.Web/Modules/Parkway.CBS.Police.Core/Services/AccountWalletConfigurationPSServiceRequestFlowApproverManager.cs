using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class AccountWalletConfigurationPSServiceRequestFlowApproverManager : BaseManager<AccountWalletConfigurationPSServiceRequestFlowApprover>, IAccountWalletUserConfigurationPSServiceRequestFlowApproverManager<AccountWalletConfigurationPSServiceRequestFlowApprover>
    {
        private readonly ITransactionManager _transactionManager;
        public AccountWalletConfigurationPSServiceRequestFlowApproverManager(IRepository<AccountWalletConfigurationPSServiceRequestFlowApprover> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// Gets all wallets assigned to the current user as payment initiator
        /// </summary>
        /// <returns></returns>
        public List<AccountWalletConfigurationVM> GetWalletsAssignedToUser(int userPartRecordId)
        {
            try
            {
                return _transactionManager.GetSession().Query<AccountWalletConfigurationPSServiceRequestFlowApprover>().Where(x => x.PSServiceRequestFlowApprover.AssignedApprover == new UserPartRecord { Id = userPartRecordId } && x.PSServiceRequestFlowApprover.FlowDefinitionLevel.Definition.DefinitionType == (int)DefinitionType.Payment && x.PSServiceRequestFlowApprover.FlowDefinitionLevel.WorkFlowActionValue == (int)RequestDirection.InitiatePaymentRequest && !x.IsDeleted && (x.AccountWalletConfiguration.CommandWalletDetail.SettlementAccountType != (int)SettlementAccountType.DeploymentAllowanceSettlement || x.AccountWalletConfiguration.PSSFeeParty.SettlementAccountType != (int)SettlementAccountType.DeploymentAllowanceSettlement)).Select(x => new AccountWalletConfigurationVM
                {
                    Id = x.AccountWalletConfiguration.Id,
                    WalletName = x.AccountWalletConfiguration.CommandWalletDetail.Command.Name ?? x.AccountWalletConfiguration.PSSFeeParty.Name
                }).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets all command wallets assigned to the current user as payment initiator
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <param name="settlementAccountType"></param>
        /// <returns></returns>
        public List<AccountWalletConfigurationVM> GetCommandWalletsAssignedToUser(int userPartRecordId, SettlementAccountType settlementAccountType)
        {
            try
            {
                return _transactionManager.GetSession().Query<AccountWalletConfigurationPSServiceRequestFlowApprover>().Where(x => (x.PSServiceRequestFlowApprover.AssignedApprover == new UserPartRecord { Id = userPartRecordId }) && (x.PSServiceRequestFlowApprover.FlowDefinitionLevel.Definition.DefinitionType == (int)DefinitionType.Payment) && (x.PSServiceRequestFlowApprover.FlowDefinitionLevel.WorkFlowActionValue == (int)RequestDirection.InitiatePaymentRequest) && !x.IsDeleted && (x.AccountWalletConfiguration.CommandWalletDetail.SettlementAccountType == (int)settlementAccountType)).Select(x => new AccountWalletConfigurationVM
                {
                    Id = x.AccountWalletConfiguration.Id,
                    WalletName = x.AccountWalletConfiguration.CommandWalletDetail.Command.Name ?? x.AccountWalletConfiguration.PSSFeeParty.Name
                }).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets the users assigned to the wallet
        /// </summary>
        /// <param name="walletId"></param>
        /// <returns></returns>
        public List<WalletUsersVM> GetWalletUsers(int walletId)
        {
            try
            {
                return _transactionManager.GetSession().Query<AccountWalletConfigurationPSServiceRequestFlowApprover>().Where(x => x.AccountWalletConfiguration == new AccountWalletConfiguration { Id = walletId} && !x.IsDeleted).Select(x => new WalletUsersVM
                {
                    CommandName = x.AccountWalletConfiguration.CommandWalletDetail.Command.Name ?? x.AccountWalletConfiguration.PSSFeeParty.Name,
                    FlowDefintionLevelName = x.PSServiceRequestFlowApprover.FlowDefinitionLevel.PositionName,
                    SelectedFlowDefinitionLevelId = x.PSServiceRequestFlowApprover.FlowDefinitionLevel.Id,
                    Username = x.PSServiceRequestFlowApprover.AssignedApprover.UserName
                }).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Checks if user is an approver for wallet(s) 
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <returns></returns>
        public bool CheckIfUserIsWalletRolePlayer(int userPartRecordId)
        {
            return _transactionManager.GetSession().Query<AccountWalletConfigurationPSServiceRequestFlowApprover>().Count(x => x.PSServiceRequestFlowApprover.AssignedApprover == new UserPartRecord { Id = userPartRecordId } && !x.IsDeleted) > 0;
        }


        /// <summary>
        /// Set IsDeleted for deleted approver(s)
        /// </summary>
        public void UpdateDeletedApprover()
        {
            try
            {
                var sourceTable = $"Parkway_CBS_Police_Core_{nameof(PSServiceRequestFlowApprover)}";
                var targetTable = $"Parkway_CBS_Police_Core_{nameof(AccountWalletConfigurationPSServiceRequestFlowApprover)}";

                var queryText = $"UPDATE {targetTable} SET {nameof(AccountWalletConfigurationPSServiceRequestFlowApprover.IsDeleted)} = PSRFA.{nameof(PSServiceRequestFlowApprover.IsDeleted)} FROM {targetTable} AWCPRFA INNER JOIN {sourceTable} PSRFA ON AWCPRFA.{nameof(AccountWalletConfigurationPSServiceRequestFlowApprover.PSServiceRequestFlowApprover)}_Id = PSRFA.Id";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception updating details for AccountWalletConfigurationPSServiceRequestFlowApprover, Exception message {0}",  exception.Message));
                RollBackAllTransactions();
                throw;
            }
        }
    }
}