using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Data;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class AccountWalletConfigurationHandler : IAccountWalletConfigurationHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly IAccountWalletConfigurationManager<AccountWalletConfiguration> _accountWalletConfigurationManager;
        private readonly Lazy<IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel>> _serviceRequestFlowDefinitionLevelManager;
        private readonly Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> _serviceRequestFlowApprover;
        private readonly Lazy<IAccountWalletUserConfigurationPSServiceRequestFlowApproverManager<AccountWalletConfigurationPSServiceRequestFlowApprover>> _accountWalletUserConfigurationPSServiceManager;
        private readonly IPSSAdminUsersManager<PSSAdminUsers> _adminUsersManager;
        private readonly Lazy<IPSServiceRequestFlowApproverStagingManager<PSServiceRequestFlowApproverStaging>> _serviceRequestFlowApproverStaging;

        ILogger Logger { get; set; }

        public AccountWalletConfigurationHandler(IHandlerComposition handlerComposition, IAccountWalletConfigurationManager<AccountWalletConfiguration> accountWalletConfigurationManager, Lazy<IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel>> serviceRequestFlowDefinitionLevelManager, Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> serviceRequestFlowApprover, IPSSAdminUsersManager<PSSAdminUsers> adminUsersManager, Lazy<IAccountWalletUserConfigurationPSServiceRequestFlowApproverManager<AccountWalletConfigurationPSServiceRequestFlowApprover>> accountWalletUserConfigurationPSService, Lazy<IPSServiceRequestFlowApproverStagingManager<PSServiceRequestFlowApproverStaging>> serviceRequestFlowApproverStaging)
        {
            _handlerComposition = handlerComposition;
            Logger = NullLogger.Instance;
            _accountWalletConfigurationManager = accountWalletConfigurationManager;
            _serviceRequestFlowDefinitionLevelManager = serviceRequestFlowDefinitionLevelManager;
            _serviceRequestFlowApprover = serviceRequestFlowApprover;
            _adminUsersManager = adminUsersManager;
            _accountWalletUserConfigurationPSServiceManager = accountWalletUserConfigurationPSService;
            _serviceRequestFlowApproverStaging = serviceRequestFlowApproverStaging;
        }

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canAddAccountWalletConfiguration"></param>
        public void CheckForPermission(Orchard.Security.Permissions.Permission canAddAccountWalletConfiguration)
        {
            _handlerComposition.IsAuthorized(canAddAccountWalletConfiguration);
        }

        /// <summary>
        /// Get the view model for adding account wallet configuration
        /// </summary>
        /// <returns><see cref="AddOrRemoveAccountWalletConfigurationVM"/></returns>
        public AddOrRemoveAccountWalletConfigurationVM GetAddOrRemoveWalletConfigurationVM(int walletAcctId)
        {

            return new AddOrRemoveAccountWalletConfigurationVM
            {
                AccountWalletId = walletAcctId,
                WalletUsers = _accountWalletUserConfigurationPSServiceManager.Value.GetWalletUsers(walletAcctId),
                WalletName = _accountWalletConfigurationManager.GetWalletName(walletAcctId),
                FlowDefinitionLevels = _accountWalletConfigurationManager.GetFlowDefinitionLevelByWalletId(walletAcctId)
            };
        }

        /// <summary>
        /// Saves the configuration to <see cref="PSServiceRequestFlowApprover"/>
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="model"></param>
        public void AddOrRemoveWalletConfiguration(ref List<ErrorModel> errors, AddOrRemoveAccountWalletConfigurationVM model)
        {
            try
            {
                if (model.AccountWalletId <= 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = PoliceLang.nowalletidfound.Text, FieldName = nameof(model.AccountWalletId) });
                    throw new DirtyFormDataException();
                }

                if (model.AddedWalletUsers != null && model.AddedWalletUsers.Count > 0)
                {
                    foreach (WalletUsersVM walletUserConf in model.AddedWalletUsers)
                    {
                        AdminUserVM adminUser = ValidateUser(errors, walletUserConf);

                        if (_serviceRequestFlowApprover.Value.IsAlreadyAssignedToFlowDefinitionLevel(adminUser.UserPartRecordId, walletUserConf.SelectedFlowDefinitionLevelId))
                        {
                            errors.Add(new ErrorModel { ErrorMessage = PoliceLang.ToLocalizeString("User already selected for a different role").Text, FieldName = nameof(model.WalletUsers) });
                            throw new DirtyFormDataException();
                        }

                        PSServiceRequestFlowApprover pssServiceRequestFlowApprover = new PSServiceRequestFlowApprover
                        {
                            FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = walletUserConf.SelectedFlowDefinitionLevelId },
                            PSSAdminUser = new PSSAdminUsers { Id = adminUser.Id },
                            AssignedApprover = new Orchard.Users.Models.UserPartRecord { Id = adminUser.UserPartRecordId },
                        };

                        if (!_serviceRequestFlowApprover.Value.Save(pssServiceRequestFlowApprover))
                        {
                            throw new CouldNotSaveRecord();
                        }

                        if (!_accountWalletUserConfigurationPSServiceManager.Value.Save(new AccountWalletConfigurationPSServiceRequestFlowApprover
                        {
                            PSServiceRequestFlowApprover = pssServiceRequestFlowApprover,
                            AccountWalletConfiguration = new AccountWalletConfiguration { Id = model.AccountWalletId }
                        }))
                        {
                            throw new CouldNotSaveRecord();
                        }

                    }
                }

                if (model.RemovedWalletUsers != null && model.RemovedWalletUsers.Count > 0)
                {
                    foreach (WalletUsersVM removedUser in model.RemovedWalletUsers)
                    {
                        AdminUserVM adminUser = ValidateUser(errors, removedUser);

                        string reference = string.Format("PSRFAS-{0}-ACCID-{1}", DateTime.Now.Ticks, Util.StrongRandom());

                        DataTable dataTable = new DataTable("Parkway_CBS_Police_Core" + typeof(PSServiceRequestFlowApproverStaging).Name);

                        dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApproverStaging.FlowDefinitionLevel) + "_Id", typeof(int)));
                        dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApproverStaging.PSSAdminUser) + "_Id", typeof(int)));
                        dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApproverStaging.AssignedApprover) + "_Id", typeof(int)));
                        dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApproverStaging.IsDeleted), typeof(bool)));
                        dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApproverStaging.Reference), typeof(string)));
                        dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApproverStaging.CreatedAtUtc), typeof(DateTime)));
                        dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApproverStaging.UpdatedAtUtc), typeof(DateTime)));


                        DataRow row = dataTable.NewRow();
                        row[nameof(PSServiceRequestFlowApproverStaging.FlowDefinitionLevel) + "_Id"] = removedUser.SelectedFlowDefinitionLevelId;
                        row[nameof(PSServiceRequestFlowApproverStaging.PSSAdminUser) + "_Id"] = adminUser.Id;
                        row[nameof(PSServiceRequestFlowApproverStaging.AssignedApprover) + "_Id"] = adminUser.UserPartRecordId;
                        row[nameof(PSServiceRequestFlowApproverStaging.IsDeleted)] = true;
                        row[nameof(PSServiceRequestFlowApproverStaging.Reference)] = reference;
                        row[nameof(PSServiceRequestFlowApproverStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                        row[nameof(PSServiceRequestFlowApproverStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);

                        if (!_serviceRequestFlowApproverStaging.Value.SaveBundle(dataTable, "Parkway_CBS_Police_Core_" + typeof(PSServiceRequestFlowApproverStaging).Name))
                        {
                            throw new CouldNotSaveRecord();

                        }

                        _serviceRequestFlowApproverStaging.Value.UpdateServiceRequestFlowApproverFromStaging(reference);
                        _accountWalletUserConfigurationPSServiceManager.Value.UpdateDeletedApprover();
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _serviceRequestFlowApprover.Value.RollBackAllTransactions();
                throw;
            }
        }

        private AdminUserVM ValidateUser(List<ErrorModel> errors, WalletUsersVM walletUser)
        {
            if (string.IsNullOrEmpty(walletUser.Username?.Trim()))
            {
                errors.Add(new ErrorModel { ErrorMessage = PoliceLang.ToLocalizeString($"Username is required").Text, FieldName = nameof(walletUser.Username) });
                throw new DirtyFormDataException();
            }

            AdminUserVM adminUser = _adminUsersManager.GetAdminUserPartRecordId(walletUser.Username.Trim());

            if (adminUser == null)
            {
                errors.Add(new ErrorModel { ErrorMessage = PoliceLang.ToLocalizeString($"No admin with username {walletUser.Username} found").Text, FieldName = nameof(walletUser.Username) });
                throw new DirtyFormDataException();
            }

            if (walletUser.SelectedFlowDefinitionLevelId <= 0 || !_serviceRequestFlowDefinitionLevelManager.Value.CheckIfDefinitionLevelExist(walletUser.SelectedFlowDefinitionLevelId))
            {
                errors.Add(new ErrorModel { ErrorMessage = PoliceLang.ToLocalizeString($"No definition level with Id {walletUser.SelectedFlowDefinitionLevelId} found").Text, FieldName = nameof(walletUser.SelectedFlowDefinitionLevelId) });
                throw new DirtyFormDataException();
            }

            return adminUser;
        }

    }
}