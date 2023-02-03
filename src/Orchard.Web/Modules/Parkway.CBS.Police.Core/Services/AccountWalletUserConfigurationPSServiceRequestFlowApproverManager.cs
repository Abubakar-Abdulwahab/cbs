using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class AccountWalletUserConfigurationPSServiceRequestFlowApproverManager : BaseManager<AccountWalletUserConfigurationPSServiceRequestFlowApprover>, IAccountWalletUserConfigurationPSServiceRequestFlowApproverManager<AccountWalletUserConfigurationPSServiceRequestFlowApprover>
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public AccountWalletUserConfigurationPSServiceRequestFlowApproverManager(IRepository<AccountWalletUserConfigurationPSServiceRequestFlowApprover> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;

        }

        /// <summary>
        /// Gets all wallets assigned to the current user as payment initiator
        /// </summary>
        /// <returns></returns>
        public List<AccountWalletConfigurationVM> GetWalletsAssignedToCurrentUser()
        {
            try
            {
                List<AccountWalletConfigurationVM> accountWalletConfigurations = new List<AccountWalletConfigurationVM>();

                accountWalletConfigurations.AddRange(GetFeePartyWalletsAssignedToCurrentUser());
                accountWalletConfigurations.AddRange(GetCommandWalletsAssignedToCurrentUser());

                return accountWalletConfigurations;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        private List<AccountWalletConfigurationVM> GetFeePartyWalletsAssignedToCurrentUser()
        {
            return _transactionManager.GetSession().Query<AccountWalletUserConfigurationPSServiceRequestFlowApprover>().Where(x => x.PSServiceRequestFlowApprover.AssignedApprover == new UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id } && x.PSServiceRequestFlowApprover.FlowDefinitionLevel.DefinitionType == (int)Models.Enums.DefinitionType.Payment && x.PSServiceRequestFlowApprover.FlowDefinitionLevel.WorkFlowActionValue == (int)Models.Enums.RequestDirection.InitiatePaymentRequest && x.WalletUserConfiguration.PSSFeeParty != null).Select(x => new AccountWalletConfigurationVM
            {
                Id = x.WalletUserConfiguration.Id,
                WalletName = x.WalletUserConfiguration.PSSFeeParty.Name
            }).ToList();
        }

        private List<AccountWalletConfigurationVM> GetCommandWalletsAssignedToCurrentUser()
        {
            return _transactionManager.GetSession().Query<AccountWalletUserConfigurationPSServiceRequestFlowApprover>().Where(x => x.PSServiceRequestFlowApprover.AssignedApprover == new UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id } && x.PSServiceRequestFlowApprover.FlowDefinitionLevel.DefinitionType == (int)Models.Enums.DefinitionType.Payment && x.PSServiceRequestFlowApprover.FlowDefinitionLevel.WorkFlowActionValue == (int)Models.Enums.RequestDirection.InitiatePaymentRequest && x.WalletUserConfiguration.CommandWalletDetail != null).Select(x => new AccountWalletConfigurationVM
            {
                Id = x.WalletUserConfiguration.Id,
                WalletName = x.WalletUserConfiguration.CommandWalletDetail.Command.Name

            }).ToList();
        }
    }
}