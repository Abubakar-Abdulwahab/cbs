using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Roles.Services;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.Contracts;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.ViewModels;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Mail.Provider.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.Utilities;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSAdminUserHandler : IPSSAdminUserHandler
    {
        ILogger Logger { get; set; }
        private readonly IRoleService _roleService;
        private readonly ICommandCategoryManager<CommandCategory> _commandCategoryManager;
        private readonly IHandlerComposition _handlerComposition;
        private readonly ICoreCommand _coreCommand;
        private readonly ICorePSSAdminUserService _corePSSAdminUser;
        private readonly IPSServiceManager<PSService> _psServiceManager;
        private readonly IPSSAdminUsersManager<PSSAdminUsers> _pssAdminUsersManager;
        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _pSServiceRequestFlowDefinitionLevelManager;
        private readonly Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> _serviceRequestFlowApprover;
        private readonly Lazy<IPSServiceRequestFlowApproverStagingManager<PSServiceRequestFlowApproverStaging>> _serviceRequestFlowApproverStaging;
        private readonly IApprovalAccessListManager<ApprovalAccessList> _approvalAccessListManager;
        private readonly IApprovalAccessListStagingManager<ApprovalAccessListStaging> _approvalAccessListStagingManager;
        private readonly Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> _approvalAccesRoleManager;
        private readonly Lazy<IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel>> _serviceRequestFlowDefinitionLevelManager;
        private readonly IOrchardServices _orchardServices;
        private readonly Lazy<IExternalDataOfficers> _externalDataOfficers;
        private readonly Lazy<IPoliceRankingManager<PoliceRanking>> _policeRankingManager;
        private readonly Lazy<IServiceWorkflowDifferentialManager<ServiceWorkflowDifferential>> _serviceWorkflowDifferentialManager;
        private readonly ICommandManager<Command> _commandManager;
        private readonly IEnumerable<Lazy<IPSSEmailProvider>> _emailProvider;

        public PSSAdminUserHandler(IOrchardServices orchardServices, IRoleService roleService, ICommandCategoryManager<CommandCategory> commandCategoryManager, IHandlerComposition handlerComposition, ICoreCommand coreCommand, ICorePSSAdminUserService corePSSAdminUser, IPSServiceManager<PSService> psServiceManager, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> pSServiceRequestFlowDefinitionLevelManager, Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> serviceRequestFlowApprover, IApprovalAccessListManager<ApprovalAccessList> approvalAccessListManager, Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> approvalAccesRoleManager, Lazy<IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel>> serviceRequestFlowDefinitionLevelManager, Lazy<IExternalDataOfficers> externalDataOfficers, Lazy<IPoliceRankingManager<PoliceRanking>> policeRankingManager, ICommandManager<Command> commandManager, Lazy<IServiceWorkflowDifferentialManager<ServiceWorkflowDifferential>> serviceWorkflowDifferentialManager, IPSSAdminUsersManager<PSSAdminUsers> pssAdminUsersManager, IApprovalAccessListStagingManager<ApprovalAccessListStaging> approvalAccessListStagingManager, Lazy<IPSServiceRequestFlowApproverStagingManager<PSServiceRequestFlowApproverStaging>> serviceRequestFlowApproverStaging, IEnumerable<Lazy<IPSSEmailProvider>> emailProvider)
        {
            Logger = NullLogger.Instance;
            _roleService = roleService;
            _commandCategoryManager = commandCategoryManager;
            _handlerComposition = handlerComposition;
            _coreCommand = coreCommand;
            _corePSSAdminUser = corePSSAdminUser;
            _psServiceManager = psServiceManager;
            _pSServiceRequestFlowDefinitionLevelManager = pSServiceRequestFlowDefinitionLevelManager;
            _serviceRequestFlowApprover = serviceRequestFlowApprover;
            _approvalAccessListManager = approvalAccessListManager;
            _approvalAccesRoleManager = approvalAccesRoleManager;
            _orchardServices = orchardServices;
            _serviceRequestFlowDefinitionLevelManager = serviceRequestFlowDefinitionLevelManager;
            _externalDataOfficers = externalDataOfficers;
            _policeRankingManager = policeRankingManager;
            _commandManager = commandManager;
            _serviceWorkflowDifferentialManager = serviceWorkflowDifferentialManager;
            _pssAdminUsersManager = pssAdminUsersManager;
            _approvalAccessListStagingManager = approvalAccessListStagingManager;
            _serviceRequestFlowApproverStaging = serviceRequestFlowApproverStaging;
            _emailProvider = emailProvider;
        }

        /// <summary>
        /// Get create user VM
        /// </summary>
        /// <param name="commandCategoryId"></param>
        /// <returns>AdminUserCreationVM</returns>
        public AdminUserCreationVM GetCreateUserVM(int commandCategoryId = 0)
        {
            try
            {
                return new AdminUserCreationVM
                {
                    RoleTypes = _roleService.GetRoles().Select(x => new UserRoleVM { Id = x.Id, Name = x.Name }).ToList(),
                    CommandCategories = _commandCategoryManager.GetParentCategories(),
                    Commands = commandCategoryId != 0 ? _coreCommand.GetCommandsByCommandCategory(commandCategoryId) : null,
                    ServiceTypes = _psServiceManager.GetServices()
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get edit user VM
        /// </summary>
        /// <param name="commandCategoryId"></param>
        /// <returns>AdminUserCreationVM</returns>
        public AdminUserCreationVM GetEditUserVM(int adminUserId)
        {
            try
            {
                AdminUserVM adminUser = _pssAdminUsersManager.GetAdminUser(adminUserId);
                Core.HelperModels.CommandVM commandDetail = _coreCommand.GetCommandDetails(adminUser.CommandId);


                AdminUserCreationVM adminUserCreationVM = new AdminUserCreationVM
                {
                    AdminUserType = _approvalAccesRoleManager.Value.GetAccessRoleUserId(adminUser.UserPartRecordId, AdminUserType.Approver) > 0 ? (int)AdminUserType.Approver : (int)AdminUserType.Viewer,
                    Username = adminUser.Username,
                    PhoneNumber = adminUser.PhoneNumber,
                    Fullname = adminUser.Fullname,
                    Email = adminUser.Email,
                    AdminUserId = adminUserId,
                    RoleTypeId = adminUser.RoleTypeId,
                    CommandCategoryId = adminUser.CommandCategoryId,
                    CommandId = adminUser.CommandId,
                    RoleTypes = _roleService.GetRoles().Select(x => new UserRoleVM { Id = x.Id, Name = x.Name }).ToList(),
                    CommandCategories = _commandCategoryManager.GetParentCategories(),
                    Commands = adminUser.CommandCategoryId != 0 ? _coreCommand.GetCommandsByCommandCategory(adminUser.CommandCategoryId) : null,
                    ServiceTypes = _psServiceManager.GetServices()
                };

                string[] commandCode = commandDetail.Code.Split('-');

                if (commandCode.Length > 3)
                {
                    adminUserCreationVM.OfficerSubSectionId = commandDetail.Id;
                }
                if (commandCode.Length > 2)
                {
                    string code = string.Join("-", commandCode, 0, 3);
                    adminUserCreationVM.OfficerSectionId = _commandManager.GetCommandWithCode(code).Id;
                    adminUserCreationVM.OfficerSubSectionCommands = _coreCommand.GetCommandsByParentCode(code).ToList();
                }
                if (commandCode.Length > 1)
                {
                    string code = string.Join("-", commandCode, 0, 2);

                    adminUserCreationVM.CommandId = _commandManager.GetCommandWithCode(code).Id;
                    adminUserCreationVM.OfficerSectionCommands = _coreCommand.GetCommandsByParentCode(code).ToList();

                }
                if (commandCode.Length > 0)
                {
                    string code = string.Join("-", commandCode, 0, 1);
                    adminUserCreationVM.Commands = _coreCommand.GetCommandsByParentCode(code).ToList();
                }

                List<ApprovalAccessListVM> approvalAccessListVM = _approvalAccessListManager.GetApprovalAccessListByUserId(adminUser.UserPartRecordId);
                adminUserCreationVM.SelectedServiceTypes = approvalAccessListVM.GroupBy(x => x.ServiceId).Select(x => x.Key).ToList();
                adminUserCreationVM.SelectedCommands = new List<Core.HelperModels.CommandVM>();
                foreach (var selectedCommand in approvalAccessListVM.GroupBy(x => x.CommandId).Select(x => new { CommandId = x.Key, AccessType = x.OrderBy(s => s.AccessTypeId).FirstOrDefault().AccessTypeId }))
                {
                    CommandVM command = _commandManager.GetCommandDetails(selectedCommand.CommandId);
                    command.AccessType = selectedCommand.AccessType;
                    adminUserCreationVM.SelectedCommands.Add(command);
                }

                adminUserCreationVM.FlowDefinitions = new List<Core.DTO.PSServiceRequestFlowDefinitionDTO> { };
                adminUserCreationVM.FlowDefinitionLevels = new List<List<Core.DTO.PSServiceRequestFlowDefinitionLevelDTO>> { };

                if (adminUserCreationVM.AdminUserType == (int)AdminUserType.Approver)
                {
                    adminUserCreationVM.SelectedFlowDefinitions = _serviceRequestFlowApprover.Value.GetDistinctDefinitionIdByUserId(adminUser.UserPartRecordId);
                    adminUserCreationVM.SelectedFlowDefinitionLevels = _serviceRequestFlowApprover.Value.GetFlowDefintionLevelIdByUserId(adminUser.UserPartRecordId);
                }

                if (adminUserCreationVM.SelectedServiceTypes != null && adminUserCreationVM.SelectedServiceTypes.Any())
                {
                    foreach (var serviceType in adminUserCreationVM.SelectedServiceTypes)
                    {
                        adminUserCreationVM.FlowDefinitions.AddRange(_psServiceManager.GetFlowDefinitionForServiceType(serviceType));
                        adminUserCreationVM.FlowDefinitions.AddRange(_serviceWorkflowDifferentialManager.Value.GetFlowDefinitionForServiceType(serviceType));
                        foreach (Core.DTO.PSServiceRequestFlowDefinitionDTO definition in adminUserCreationVM.FlowDefinitions.Where(x => x.ServiceId == serviceType))
                        {

                            if (adminUserCreationVM.SelectedFlowDefinitions != null && adminUserCreationVM.SelectedFlowDefinitions.Contains(definition.Id))
                            {
                                var definitionLevels = _pSServiceRequestFlowDefinitionLevelManager.GetApprovalDefinitionLevelsForDefinitionWithId(definition.Id).ToList();
                                definitionLevels.ForEach(x => { x.ServiceId = serviceType; });
                                adminUserCreationVM.FlowDefinitionLevels.Add(definitionLevels);
                            }
                        }
                    }
                }

                return adminUserCreationVM;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canCreateAdminUser"></param>
        public void CheckForPermission(Orchard.Security.Permissions.Permission canCreateAdminUser)
        {
            _handlerComposition.IsAuthorized(canCreateAdminUser);
        }

        /// <summary>
        /// Create admin user
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="userInput"></param>
        /// <returns>bool</returns>
        public bool CreateAdminUser(ref List<ErrorModel> errors, AdminUserCreationVM userInput)
        {
            int tempCommandId = userInput.CommandId;
            List<ApprovalAccessRoleUser> approvalAccessUsers;
            try
            {
                #region Validations
                if (userInput.AdminUserType <= 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Kindly select the access type", FieldName = nameof(userInput.AdminUserType) });
                    throw new DirtyFormDataException();
                }

                if (userInput.SelectedServiceTypes == null || userInput.SelectedServiceTypes.Count <= 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Kindly select the service type", FieldName = nameof(userInput.SelectedServiceTypes) });
                    throw new DirtyFormDataException();
                }

                if (userInput.SelectedCommands == null || userInput.SelectedCommands.Count <= 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Kindly select the formation/command", FieldName = nameof(userInput.CommandId) });
                    throw new DirtyFormDataException();
                }

                if (userInput.RoleTypeId <= 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Kindly select the role type", FieldName = nameof(userInput.RoleTypeId) });
                    throw new DirtyFormDataException();
                }

                if (string.IsNullOrEmpty(userInput.Fullname) || string.IsNullOrEmpty(userInput.Fullname.Trim()))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"{nameof(userInput.Fullname)} is required", FieldName = nameof(userInput.Fullname) });
                    throw new DirtyFormDataException();
                }

                if (string.IsNullOrEmpty(userInput.Username) || string.IsNullOrEmpty(userInput.Username.Trim()))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"{nameof(userInput.Username)} is required", FieldName = nameof(userInput.Username) });
                    throw new DirtyFormDataException();
                }

                if (string.IsNullOrEmpty(userInput.PhoneNumber) || string.IsNullOrEmpty(userInput.PhoneNumber.Trim()))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"{nameof(userInput.PhoneNumber)} is required", FieldName = nameof(userInput.PhoneNumber) });
                    throw new DirtyFormDataException();
                }

                if (!Util.DoPhoneNumberValidation(userInput.PhoneNumber))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"{nameof(userInput.PhoneNumber)} is invalid", FieldName = nameof(userInput.PhoneNumber) });
                    throw new DirtyFormDataException();
                }

                if (string.IsNullOrEmpty(userInput.Email) || string.IsNullOrEmpty(userInput.Email.Trim()))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"{nameof(userInput.Email)} is required", FieldName = nameof(userInput.Email) });
                    throw new DirtyFormDataException();
                }

                if (userInput.AdminUserType == (int)AdminUserType.Approver && (userInput.SelectedFlowDefinitionLevels == null ||userInput.SelectedFlowDefinitionLevels.Count <= 0))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"flow definition level is required", FieldName = nameof(userInput.AdminUserType) });
                    throw new DirtyFormDataException();
                }

                //Set the lowest selected command 
                if (userInput.OfficerSubSectionId > 0)
                {
                    userInput.CommandId = userInput.OfficerSubSectionId;
                }
                else if (userInput.OfficerSectionId > 0)
                {
                    userInput.CommandId = userInput.OfficerSectionId;
                }

                if (!_coreCommand.CheckIfCommandExist(userInput.CommandId))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Kindly select valid the formation/command", FieldName = nameof(userInput.CommandId) });
                    throw new DirtyFormDataException();
                }

                Util.DoEmailValidation(userInput.Email.Trim(), ref errors, nameof(userInput.Email), true);
                if (errors.Count > 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"{nameof(userInput.Email)} is invalid", FieldName = nameof(userInput.Email) });
                    throw new DirtyFormDataException();
                } 
                #endregion

                PSSAdminUsers user = _corePSSAdminUser.TryCreateAdminUser(userInput, ref errors);

                if (userInput.AdminUserType == (int)AdminUserType.Approver)
                {
                    DataTable dataTable = new DataTable("Parkway_CBS_Police_Core" + typeof(PSServiceRequestFlowApprover).Name);

                    dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApprover.FlowDefinitionLevel) + "_Id", typeof(long)));
                    dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApprover.PSSAdminUser) + "_Id", typeof(long)));
                    dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApprover.AssignedApprover) + "_Id", typeof(long)));
                    dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApprover.CreatedAtUtc), typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApprover.UpdatedAtUtc), typeof(DateTime)));

                    foreach (var flowDefinitionLevelId in userInput.SelectedFlowDefinitionLevels)
                    {
                        if (!_serviceRequestFlowDefinitionLevelManager.Value.CheckIfDefinitionLevelExistAndIsApproval(flowDefinitionLevelId))
                        {
                            throw new NoRecordFoundException($"No definition level with Id {flowDefinitionLevelId} found");
                        }                      

                        DataRow row = dataTable.NewRow();
                        row[nameof(PSServiceRequestFlowApprover.FlowDefinitionLevel) + "_Id"] = flowDefinitionLevelId;
                        row[nameof(PSServiceRequestFlowApprover.PSSAdminUser) + "_Id"] = user.Id;
                        row[nameof(PSServiceRequestFlowApprover.AssignedApprover) + "_Id"] = user.User.Id;
                        row[nameof(PSServiceRequestFlowApprover.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                        row[nameof(PSServiceRequestFlowApprover.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    }

                    if(!_serviceRequestFlowApprover.Value.SaveBundle(dataTable, "Parkway_CBS_Police_Core_" + typeof(PSServiceRequestFlowApprover).Name))
                    {
                        throw new CouldNotSaveRecord();

                    }

                    approvalAccessUsers = new List<ApprovalAccessRoleUser>
                    {
                        new ApprovalAccessRoleUser
                        {
                            User = new Orchard.Users.Models.UserPartRecord { Id = user.User.Id },
                            AddedBy = new Orchard.Users.Models.UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id },
                            IsActive = true,
                            AccessType = (int)AdminUserType.Approver
                        },
                        new ApprovalAccessRoleUser
                        {
                            User = new Orchard.Users.Models.UserPartRecord { Id = user.User.Id },
                            AddedBy = new Orchard.Users.Models.UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id },
                            IsActive = true,
                            AccessType = (int)AdminUserType.Viewer
                        }
                    };

                    if (!_approvalAccesRoleManager.Value.SaveBundle(approvalAccessUsers))
                    {
                        throw new CouldNotSaveRecord();
                    }
                }
                else
                {
                    approvalAccessUsers = new List<ApprovalAccessRoleUser>
                    {
                        new ApprovalAccessRoleUser
                        {
                            User = new Orchard.Users.Models.UserPartRecord { Id = user.User.Id },
                            AddedBy = new Orchard.Users.Models.UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id },
                            IsActive = true,
                            AccessType = (int)AdminUserType.Viewer
                        }
                    };

                    if (!_approvalAccesRoleManager.Value.SaveBundle(approvalAccessUsers))
                    {
                        throw new CouldNotSaveRecord();
                    }
                }

                

                

                DataTable approvalAccessListdataTable = new DataTable("Parkway_CBS_Police_Core_" + typeof(ApprovalAccessList).Name);

                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessList.ApprovalAccessRoleUser) + "_Id", typeof(int)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessList.Command) + "_Id", typeof(int)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessList.State) + "_Id", typeof(int)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessList.LGA) + "_Id", typeof(int)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessList.Service) + "_Id", typeof(int)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessList.CommandCategory) + "_Id", typeof(int)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessList.IsDeleted), typeof(bool)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessList.CreatedAtUtc), typeof(DateTime)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessList.UpdatedAtUtc), typeof(DateTime)));

                foreach (var selectedServiceId in userInput.SelectedServiceTypes)
                {
                    if (!_psServiceManager.CheckIfServiceIdExist(selectedServiceId))
                    {
                        throw new NoRecordFoundException($"No service Type with Id {selectedServiceId} found");
                    }

                    foreach (var selectedCommand in userInput.SelectedCommands)
                    {
                        Core.HelperModels.CommandVM command = _coreCommand.GetCommandDetails(selectedCommand.Id);
                        string commandCode = command.Code;
                        int commandAccessType = selectedCommand.AccessType;
                        foreach(var accessRoleUserModel in approvalAccessUsers)
                        {
                            if(commandAccessType == (int)AdminUserType.Viewer && accessRoleUserModel.AccessType == (int)AdminUserType.Approver)
                            {
                                continue;
                            }

                            if (command == null)
                            {
                                throw new NoRecordFoundException($"No command with Id {selectedCommand.Id} found");
                            }

                            DataRow row = approvalAccessListdataTable.NewRow();
                            row[nameof(ApprovalAccessList.ApprovalAccessRoleUser) + "_Id"] = accessRoleUserModel.Id;
                            row[nameof(ApprovalAccessList.Command) + "_Id"] = command.Id;
                            row[nameof(ApprovalAccessList.State) + "_Id"] = command.StateId;
                            row[nameof(ApprovalAccessList.LGA) + "_Id"] = command.LGAId;
                            row[nameof(ApprovalAccessList.CommandCategory) + "_Id"] = command.CommandCategoryId;
                            row[nameof(ApprovalAccessList.Service) + "_Id"] = selectedServiceId;
                            row[nameof(ApprovalAccessList.IsDeleted)] = false;
                            row[nameof(ApprovalAccessList.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                            row[nameof(ApprovalAccessList.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                            approvalAccessListdataTable.Rows.Add(row);
                        }
                        

                        if (selectedCommand.SelectAllSections)
                        {
                            IEnumerable<Core.HelperModels.CommandVM> subCommands = _coreCommand.GetCommandsByParentCode(selectedCommand.Code);

                            if (subCommands.Any() && subCommands != null)
                            {
                                foreach (var subCommand in subCommands)
                                {
                                    foreach(var accessRoleUserModel in approvalAccessUsers)
                                    {
                                        if (commandAccessType == (int)AdminUserType.Viewer && accessRoleUserModel.AccessType == (int)AdminUserType.Approver)
                                        {
                                            continue;
                                        }

                                        if (_approvalAccessListManager.CheckIfCommandExistForUser(subCommand.Id, accessRoleUserModel.Id))
                                        {
                                            continue;
                                        }

                                        DataRow subCommandRow = approvalAccessListdataTable.NewRow();
                                        subCommandRow[nameof(ApprovalAccessList.ApprovalAccessRoleUser) + "_Id"] = accessRoleUserModel.Id;
                                        subCommandRow[nameof(ApprovalAccessList.Command) + "_Id"] = subCommand.Id;
                                        subCommandRow[nameof(ApprovalAccessList.State) + "_Id"] = subCommand.StateId;
                                        subCommandRow[nameof(ApprovalAccessList.LGA) + "_Id"] = subCommand.LGAId;
                                        subCommandRow[nameof(ApprovalAccessList.CommandCategory) + "_Id"] = command.CommandCategoryId;
                                        subCommandRow[nameof(ApprovalAccessList.Service) + "_Id"] = selectedServiceId;
                                        subCommandRow[nameof(ApprovalAccessList.IsDeleted)] = false;
                                        subCommandRow[nameof(ApprovalAccessList.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                                        subCommandRow[nameof(ApprovalAccessList.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                                        approvalAccessListdataTable.Rows.Add(subCommandRow);
                                    }

                                    if (selectedCommand.SelectAllSubSections)
                                    {
                                        IEnumerable<Core.HelperModels.CommandVM> subSubCommands = _coreCommand.GetCommandsByParentCode(subCommand.Code);

                                        if (subSubCommands.Any() && subSubCommands != null)
                                        {
                                            foreach (var subSubCommand in subSubCommands)
                                            {
                                                foreach(var accessRoleUserModel in approvalAccessUsers)
                                                {
                                                    if (commandAccessType == (int)AdminUserType.Viewer && accessRoleUserModel.AccessType == (int)AdminUserType.Approver)
                                                    {
                                                        continue;
                                                    }

                                                    if (_approvalAccessListManager.CheckIfCommandExistForUser(subSubCommand.Id, accessRoleUserModel.Id))
                                                    {
                                                        continue;
                                                    }
                                                    DataRow subSubCommandsRow = approvalAccessListdataTable.NewRow();

                                                    subSubCommandsRow[nameof(ApprovalAccessList.ApprovalAccessRoleUser) + "_Id"] = accessRoleUserModel.Id;
                                                    subSubCommandsRow[nameof(ApprovalAccessList.Command) + "_Id"] = subSubCommand.Id;
                                                    subSubCommandsRow[nameof(ApprovalAccessList.State) + "_Id"] = subSubCommand.StateId;
                                                    subSubCommandsRow[nameof(ApprovalAccessList.LGA) + "_Id"] = subSubCommand.LGAId;
                                                    subSubCommandsRow[nameof(ApprovalAccessList.CommandCategory) + "_Id"] = command.CommandCategoryId;
                                                    subSubCommandsRow[nameof(ApprovalAccessList.Service) + "_Id"] = selectedServiceId;
                                                    subSubCommandsRow[nameof(ApprovalAccessList.IsDeleted)] = false;
                                                    subSubCommandsRow[nameof(ApprovalAccessList.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                                                    subSubCommandsRow[nameof(ApprovalAccessList.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                                                    approvalAccessListdataTable.Rows.Add(subSubCommandsRow);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (selectedCommand.SelectAllSubSections)
                        {
                            if (commandCode.Split('-').Length > 2)
                            {
                                IEnumerable<Core.HelperModels.CommandVM> subSubCommands = _coreCommand.GetCommandsByParentCode(commandCode);

                                if (subSubCommands.Any() && subSubCommands != null)
                                {
                                    foreach (var subSubCommand in subSubCommands)
                                    {
                                        foreach (var accessRoleUserModel in approvalAccessUsers)
                                        {
                                            if (commandAccessType == (int)AdminUserType.Viewer && accessRoleUserModel.AccessType == (int)AdminUserType.Approver)
                                            {
                                                continue;
                                            }

                                            if (_approvalAccessListManager.CheckIfCommandExistForUser(subSubCommand.Id, accessRoleUserModel.Id))
                                            {
                                                continue;
                                            }
                                            DataRow subSubCommandsRow = approvalAccessListdataTable.NewRow();

                                            subSubCommandsRow[nameof(ApprovalAccessList.ApprovalAccessRoleUser) + "_Id"] = accessRoleUserModel.Id;
                                            subSubCommandsRow[nameof(ApprovalAccessList.Command) + "_Id"] = subSubCommand.Id;
                                            subSubCommandsRow[nameof(ApprovalAccessList.State) + "_Id"] = subSubCommand.StateId;
                                            subSubCommandsRow[nameof(ApprovalAccessList.LGA) + "_Id"] = subSubCommand.LGAId;
                                            subSubCommandsRow[nameof(ApprovalAccessList.CommandCategory) + "_Id"] = command.CommandCategoryId;
                                            subSubCommandsRow[nameof(ApprovalAccessList.Service) + "_Id"] = selectedServiceId;
                                            subSubCommandsRow[nameof(ApprovalAccessList.IsDeleted)] = false;
                                            subSubCommandsRow[nameof(ApprovalAccessList.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                                            subSubCommandsRow[nameof(ApprovalAccessList.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                                            approvalAccessListdataTable.Rows.Add(subSubCommandsRow);
                                        }
                                    }
                                }
                            }

                        }
                    }

                }

                if (!_approvalAccessListManager.SaveBundle(approvalAccessListdataTable, "Parkway_CBS_Police_Core_" + typeof(ApprovalAccessList).Name))
                {
                    throw new CouldNotSaveRecord();

                }
                //Send email notification
                SendRegistrationEmailNotification(userInput);
                return true;
            }
            catch (DirtyFormDataException)
            {
                userInput.CommandId = tempCommandId;
                throw;
            }
            catch (Exception exception)
            {
                _pSServiceRequestFlowDefinitionLevelManager.RollBackAllTransactions();
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Edit admin user
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="userInput"></param>
        /// <returns>bool</returns>
        public bool EditAdminUser(ref List<ErrorModel> errors, AdminUserCreationVM userInput)
        {
            int tempCommandId = userInput.CommandId;
            try
            {
                #region Validations
                if (userInput.SelectedCommands == null || userInput.SelectedCommands.Count <= 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Kindly select the formation/command", FieldName = nameof(userInput.CommandId) });
                    throw new DirtyFormDataException();
                }

                if (userInput.RoleTypeId <= 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Kindly select the role type", FieldName = nameof(userInput.RoleTypeId) });
                    throw new DirtyFormDataException();
                }

                if (string.IsNullOrEmpty(userInput.Fullname) || string.IsNullOrEmpty(userInput.Fullname.Trim()))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Full name is required", FieldName = nameof(userInput.Fullname) });
                    throw new DirtyFormDataException();
                }

                if (string.IsNullOrEmpty(userInput.Username) || string.IsNullOrEmpty(userInput.Username.Trim()))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Username is required", FieldName = nameof(userInput.Username) });
                    throw new DirtyFormDataException();
                }

                if (string.IsNullOrEmpty(userInput.PhoneNumber) || string.IsNullOrEmpty(userInput.PhoneNumber.Trim()))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Phone number is required", FieldName = nameof(userInput.PhoneNumber) });
                    throw new DirtyFormDataException();
                }

                if (!Util.DoPhoneNumberValidation(userInput.PhoneNumber))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Phone number is invalid", FieldName = nameof(userInput.PhoneNumber) });
                    throw new DirtyFormDataException();
                }

                if (string.IsNullOrEmpty(userInput.Email) || string.IsNullOrEmpty(userInput.Email.Trim()))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Email is required", FieldName = nameof(userInput.Email) });
                    throw new DirtyFormDataException();
                }

                //Set the lowest selected command 
                if (userInput.OfficerSubSectionId > 0)
                {
                    userInput.CommandId = userInput.OfficerSubSectionId;
                }
                else if (userInput.OfficerSectionId > 0)
                {
                    userInput.CommandId = userInput.OfficerSectionId;
                }

                if (!_coreCommand.CheckIfCommandExist(userInput.CommandId))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Kindly select valid the formation/command", FieldName = nameof(userInput.CommandId) });
                    throw new DirtyFormDataException();
                }

                Util.DoEmailValidation(userInput.Email.Trim(), ref errors, nameof(userInput.Email), true);
                if (errors.Count > 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Email is invalid", FieldName = nameof(userInput.Email) });
                    throw new DirtyFormDataException();
                }
                #endregion

                AdminUserVM adminUserVM = _corePSSAdminUser.TryEditAdminUser(userInput, ref errors);

                //Retrieve approval access role user
                IEnumerable<ApprovalAccessRoleUserDTO> approvalAccessRoleIds = _approvalAccesRoleManager.Value.GetApprovalAccessRoleUserId(adminUserVM.UserPartRecordId);
                string reference = string.Format("CMD-{0}-ACCID-{1}", DateTime.Now.Ticks, approvalAccessRoleIds.ElementAt(0).Id);

                DataTable dataTable = new DataTable("Parkway_CBS_Police_Core" + typeof(PSServiceRequestFlowApproverStaging).Name);

                dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApproverStaging.FlowDefinitionLevel) + "_Id", typeof(long)));
                dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApproverStaging.PSSAdminUser) + "_Id", typeof(long)));
                dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApproverStaging.AssignedApprover) + "_Id", typeof(long)));
                dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApproverStaging.IsDeleted), typeof(bool)));
                dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApproverStaging.Reference), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApproverStaging.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(PSServiceRequestFlowApproverStaging.UpdatedAtUtc), typeof(DateTime)));

                if (userInput.SelectedFlowDefinitionLevels != null && userInput.SelectedFlowDefinitionLevels.Any())
                {
                    foreach (int flowDefinitionLevelId in userInput.SelectedFlowDefinitionLevels)
                    {
                        if (!_serviceRequestFlowDefinitionLevelManager.Value.CheckIfDefinitionLevelExistAndIsApproval(flowDefinitionLevelId))
                        {
                            throw new NoRecordFoundException($"No approval definition level with Id {flowDefinitionLevelId} found");
                        }
                        DataRow row = dataTable.NewRow();
                        row[nameof(PSServiceRequestFlowApproverStaging.FlowDefinitionLevel) + "_Id"] = flowDefinitionLevelId;
                        row[nameof(PSServiceRequestFlowApproverStaging.PSSAdminUser) + "_Id"] = userInput.AdminUserId;
                        row[nameof(PSServiceRequestFlowApproverStaging.AssignedApprover) + "_Id"] = adminUserVM.UserPartRecordId;
                        row[nameof(PSServiceRequestFlowApproverStaging.Reference)] = reference;
                        row[nameof(PSServiceRequestFlowApproverStaging.IsDeleted)] = false;
                        row[nameof(PSServiceRequestFlowApproverStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                        row[nameof(PSServiceRequestFlowApproverStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    }
                }

                if (userInput.RemovedFlowDefinitionLevels != null && userInput.RemovedFlowDefinitionLevels.Any())
                {
                    foreach (int removedDefinitionLevelId in userInput.RemovedFlowDefinitionLevels)
                    {
                        DataRow row = dataTable.NewRow();
                        row[nameof(PSServiceRequestFlowApproverStaging.FlowDefinitionLevel) + "_Id"] = removedDefinitionLevelId;
                        row[nameof(PSServiceRequestFlowApproverStaging.PSSAdminUser) + "_Id"] = userInput.AdminUserId;
                        row[nameof(PSServiceRequestFlowApproverStaging.AssignedApprover) + "_Id"] = adminUserVM.UserPartRecordId;
                        row[nameof(PSServiceRequestFlowApproverStaging.IsDeleted)] = true;
                        row[nameof(PSServiceRequestFlowApproverStaging.Reference)] = reference;
                        row[nameof(PSServiceRequestFlowApproverStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                        row[nameof(PSServiceRequestFlowApproverStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    }

                }

                if (!_serviceRequestFlowApproverStaging.Value.SaveBundle(dataTable, "Parkway_CBS_Police_Core_" + typeof(PSServiceRequestFlowApproverStaging).Name))
                {
                    throw new CouldNotSaveRecord();

                }

                _serviceRequestFlowApproverStaging.Value.UpdateServiceRequestFlowApproverFromStaging(reference);

                DataTable approvalAccessListdataTable = new DataTable("Parkway_CBS_Police_Core_" + typeof(ApprovalAccessListStaging).Name);

                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessListStaging.ApprovalAccessRoleUser) + "_Id", typeof(int)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessListStaging.Command) + "_Id", typeof(int)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessListStaging.State) + "_Id", typeof(int)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessListStaging.LGA) + "_Id", typeof(int)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessListStaging.Service) + "_Id", typeof(int)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessListStaging.CommandCategory) + "_Id", typeof(int)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessListStaging.IsDeleted), typeof(bool)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessListStaging.Reference), typeof(string)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessListStaging.CreatedAtUtc), typeof(DateTime)));
                approvalAccessListdataTable.Columns.Add(new DataColumn(nameof(ApprovalAccessListStaging.UpdatedAtUtc), typeof(DateTime)));

                IEnumerable<int> previouslySelectedServices = _approvalAccessListManager.GetSelectedServicesFromAccessListByuserId(adminUserVM.UserPartRecordId);
                foreach (int selectedServiceId in userInput.SelectedServiceTypes)
                {
                    if (!_psServiceManager.CheckIfServiceIdExist(selectedServiceId))
                    {
                        throw new NoRecordFoundException($"No service Type with Id {selectedServiceId} found");
                    }

                    foreach (Core.HelperModels.CommandVM selectedCommand in userInput.SelectedCommands)
                    {
                        Core.HelperModels.CommandVM command = _coreCommand.GetCommandDetails(selectedCommand.Id);
                        string commandCode = command.Code;
                        int commandAccessType = selectedCommand.AccessType;

                        if (command == null)
                        {
                            throw new NoRecordFoundException($"No command with Id {selectedCommand.Id} found");
                        }

                        foreach(var accessRoleUserModel in approvalAccessRoleIds)
                        {
                            if (commandAccessType == (int)AdminUserType.Viewer && accessRoleUserModel.AccessType == (int)AdminUserType.Approver)
                            {
                                continue;
                            }

                            DataRow row = approvalAccessListdataTable.NewRow();
                            row[nameof(ApprovalAccessListStaging.ApprovalAccessRoleUser) + "_Id"] = accessRoleUserModel.Id;
                            row[nameof(ApprovalAccessListStaging.Command) + "_Id"] = command.Id;
                            row[nameof(ApprovalAccessListStaging.State) + "_Id"] = command.StateId;
                            row[nameof(ApprovalAccessListStaging.LGA) + "_Id"] = command.LGAId;
                            row[nameof(ApprovalAccessListStaging.CommandCategory) + "_Id"] = command.CommandCategoryId;
                            row[nameof(ApprovalAccessListStaging.Service) + "_Id"] = selectedServiceId;
                            row[nameof(ApprovalAccessListStaging.IsDeleted)] = false;
                            row[nameof(ApprovalAccessListStaging.Reference)] = reference;
                            row[nameof(ApprovalAccessListStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                            row[nameof(ApprovalAccessListStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                            approvalAccessListdataTable.Rows.Add(row);
                        }

                        if (selectedCommand.SelectAllSections)
                        {
                            IEnumerable<Core.HelperModels.CommandVM> subCommands = _coreCommand.GetCommandsByParentCode(selectedCommand.Code);

                            if (subCommands.Any() && subCommands != null)
                            {
                                foreach (Core.HelperModels.CommandVM subCommand in subCommands)
                                {
                                    foreach(var accessRoleUserModel in approvalAccessRoleIds)
                                    {
                                        if (commandAccessType == (int)AdminUserType.Viewer && accessRoleUserModel.AccessType == (int)AdminUserType.Approver)
                                        {
                                            continue;
                                        }

                                        DataRow subCommandRow = approvalAccessListdataTable.NewRow();
                                        subCommandRow[nameof(ApprovalAccessListStaging.ApprovalAccessRoleUser) + "_Id"] = accessRoleUserModel.Id;
                                        subCommandRow[nameof(ApprovalAccessListStaging.Command) + "_Id"] = subCommand.Id;
                                        subCommandRow[nameof(ApprovalAccessListStaging.State) + "_Id"] = subCommand.StateId;
                                        subCommandRow[nameof(ApprovalAccessListStaging.LGA) + "_Id"] = subCommand.LGAId;
                                        subCommandRow[nameof(ApprovalAccessListStaging.CommandCategory) + "_Id"] = subCommand.CommandCategoryId;
                                        subCommandRow[nameof(ApprovalAccessListStaging.Service) + "_Id"] = selectedServiceId;
                                        subCommandRow[nameof(ApprovalAccessListStaging.IsDeleted)] = false;
                                        subCommandRow[nameof(ApprovalAccessListStaging.Reference)] = reference;
                                        subCommandRow[nameof(ApprovalAccessListStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                                        subCommandRow[nameof(ApprovalAccessListStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                                        approvalAccessListdataTable.Rows.Add(subCommandRow);
                                    }

                                    

                                    if (selectedCommand.SelectAllSubSections)
                                    {
                                        IEnumerable<Core.HelperModels.CommandVM> subSubCommands = _coreCommand.GetCommandsByParentCode(subCommand.Code);

                                        if (subSubCommands.Any() && subSubCommands != null)
                                        {
                                            foreach (Core.HelperModels.CommandVM subSubCommand in subSubCommands)
                                            {
                                                foreach (var accessRoleUserModel in approvalAccessRoleIds)
                                                {
                                                    if (commandAccessType == (int)AdminUserType.Viewer && accessRoleUserModel.AccessType == (int)AdminUserType.Approver)
                                                    {
                                                        continue;
                                                    }

                                                    DataRow subSubCommandRow = approvalAccessListdataTable.NewRow();
                                                    subSubCommandRow[nameof(ApprovalAccessListStaging.ApprovalAccessRoleUser) + "_Id"] = accessRoleUserModel.Id;
                                                    subSubCommandRow[nameof(ApprovalAccessListStaging.Command) + "_Id"] = subSubCommand.Id;
                                                    subSubCommandRow[nameof(ApprovalAccessListStaging.State) + "_Id"] = subSubCommand.StateId;
                                                    subSubCommandRow[nameof(ApprovalAccessListStaging.LGA) + "_Id"] = subSubCommand.LGAId;
                                                    subSubCommandRow[nameof(ApprovalAccessListStaging.CommandCategory) + "_Id"] = subSubCommand.CommandCategoryId;
                                                    subSubCommandRow[nameof(ApprovalAccessListStaging.Service) + "_Id"] = selectedServiceId;
                                                    subSubCommandRow[nameof(ApprovalAccessListStaging.IsDeleted)] = false;
                                                    subSubCommandRow[nameof(ApprovalAccessListStaging.Reference)] = reference;
                                                    subSubCommandRow[nameof(ApprovalAccessListStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                                                    subSubCommandRow[nameof(ApprovalAccessListStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                                                    approvalAccessListdataTable.Rows.Add(subSubCommandRow);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (selectedCommand.SelectAllSubSections)
                        {
                            if (commandCode.Split('-').Length > 2)
                            {
                                IEnumerable<Core.HelperModels.CommandVM> subSubCommands = _coreCommand.GetCommandsByParentCode(commandCode);

                                if (subSubCommands.Any() && subSubCommands != null)
                                {
                                    foreach (Core.HelperModels.CommandVM subSubCommand in subSubCommands)
                                    {
                                        foreach (var accessRoleUserModel in approvalAccessRoleIds)
                                        {
                                            if (commandAccessType == (int)AdminUserType.Viewer && accessRoleUserModel.AccessType == (int)AdminUserType.Approver)
                                            {
                                                continue;
                                            }

                                            DataRow subSubCommandRow = approvalAccessListdataTable.NewRow();
                                            subSubCommandRow[nameof(ApprovalAccessListStaging.ApprovalAccessRoleUser) + "_Id"] = accessRoleUserModel.Id;
                                            subSubCommandRow[nameof(ApprovalAccessListStaging.Command) + "_Id"] = subSubCommand.Id;
                                            subSubCommandRow[nameof(ApprovalAccessListStaging.State) + "_Id"] = subSubCommand.StateId;
                                            subSubCommandRow[nameof(ApprovalAccessListStaging.LGA) + "_Id"] = subSubCommand.LGAId;
                                            subSubCommandRow[nameof(ApprovalAccessListStaging.CommandCategory) + "_Id"] = subSubCommand.CommandCategoryId;
                                            subSubCommandRow[nameof(ApprovalAccessListStaging.Service) + "_Id"] = selectedServiceId;
                                            subSubCommandRow[nameof(ApprovalAccessListStaging.IsDeleted)] = false;
                                            subSubCommandRow[nameof(ApprovalAccessListStaging.Reference)] = reference;
                                            subSubCommandRow[nameof(ApprovalAccessListStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                                            subSubCommandRow[nameof(ApprovalAccessListStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                                            approvalAccessListdataTable.Rows.Add(subSubCommandRow);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }


                foreach(int previouslySelectedService in previouslySelectedServices)
                {

                    if (userInput.RemovedCommands != null && userInput.RemovedCommands.Any())
                    {
                        foreach (Core.HelperModels.CommandVM removedCommand in userInput.RemovedCommands)
                        {
                            Core.HelperModels.CommandVM command = _coreCommand.GetCommandDetails(removedCommand.Id);
                            int commandAccessType = removedCommand.AccessType;

                            if (command == null)
                            {
                                throw new NoRecordFoundException($"No command with Id {removedCommand.Id} found");
                            }

                            foreach (var accessRoleUserModel in approvalAccessRoleIds)
                            {
                                if (commandAccessType == (int)AdminUserType.Viewer && accessRoleUserModel.AccessType == (int)AdminUserType.Approver)
                                {
                                    continue;
                                }

                                DataRow removedCommandRow = approvalAccessListdataTable.NewRow();

                                removedCommandRow[nameof(ApprovalAccessListStaging.ApprovalAccessRoleUser) + "_Id"] = accessRoleUserModel.Id;
                                removedCommandRow[nameof(ApprovalAccessListStaging.Command) + "_Id"] = command.Id;
                                removedCommandRow[nameof(ApprovalAccessListStaging.State) + "_Id"] = command.StateId;
                                removedCommandRow[nameof(ApprovalAccessListStaging.LGA) + "_Id"] = command.LGAId;
                                removedCommandRow[nameof(ApprovalAccessListStaging.CommandCategory) + "_Id"] = command.CommandCategoryId;
                                removedCommandRow[nameof(ApprovalAccessListStaging.Service) + "_Id"] = previouslySelectedService;
                                removedCommandRow[nameof(ApprovalAccessListStaging.IsDeleted)] = true;
                                removedCommandRow[nameof(ApprovalAccessListStaging.Reference)] = reference;
                                removedCommandRow[nameof(ApprovalAccessListStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                                removedCommandRow[nameof(ApprovalAccessListStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                                approvalAccessListdataTable.Rows.Add(removedCommandRow);
                            }
                        }

                    }
                }


                if (userInput.RemovedServiceTypes != null && userInput.RemovedServiceTypes.Any())
                {
                    foreach (int removedServiceId in userInput.RemovedServiceTypes)
                    {
                        if (!_psServiceManager.CheckIfServiceIdExist(removedServiceId))
                        {
                            throw new NoRecordFoundException($"No service Type with Id {removedServiceId} found");
                        }

                        _approvalAccessListManager.DeleteServiceInAccessList(adminUserVM.UserPartRecordId, removedServiceId);
                    }
                }

                if (!_approvalAccessListStagingManager.SaveBundle(approvalAccessListdataTable, "Parkway_CBS_Police_Core_" + typeof(ApprovalAccessListStaging).Name))
                {
                    throw new CouldNotSaveRecord();
                }

                _approvalAccessListStagingManager.UpdateApprovalAccessListFromStaging(reference);

                return true;
            }
            catch (DirtyFormDataException)
            {
                userInput.CommandId = tempCommandId;
                throw;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _pSServiceRequestFlowDefinitionLevelManager.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Gets flow definition for service type with specified id
        /// </summary>
        /// <param name="serviceTypeId"></param>
        /// <returns></returns>
        public APIResponse GetFlowDefinitionForServiceType(int serviceTypeId)
        {
            try
            {
                List<Core.DTO.PSServiceRequestFlowDefinitionDTO> flowDefinitions = _psServiceManager.GetFlowDefinitionForServiceType(serviceTypeId).ToList();
                if (flowDefinitions == null || !flowDefinitions.Any()) { return new APIResponse { Error = true, ResponseObject = "No flow definitions found" }; }

                IEnumerable<Core.DTO.PSServiceRequestFlowDefinitionDTO> serviceWorkflowDifferentials = _serviceWorkflowDifferentialManager.Value.GetFlowDefinitionForServiceType(serviceTypeId);

                flowDefinitions.AddRange(serviceWorkflowDifferentials);

                return new APIResponse { ResponseObject = flowDefinitions };
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Gets approval flow definition levels for flow definition with specified id
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        public APIResponse GetApprovalFlowDefinitionLevelsForDefinitionWithId(int definitionId)
        {
            try
            {
                IEnumerable<Core.DTO.PSServiceRequestFlowDefinitionLevelDTO> flowDefinitionLevels = _pSServiceRequestFlowDefinitionLevelManager.GetApprovalDefinitionLevelsForDefinitionWithId(definitionId);
                if (flowDefinitionLevels == null || !flowDefinitionLevels.Any()) { return new APIResponse { Error = true, ResponseObject = "No flow definition levels found" }; }
                return new APIResponse { ResponseObject = flowDefinitionLevels };
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Gets police officer with specified service number from HR system
        /// </summary>
        /// <param name="serviceNumber"></param>
        /// <returns></returns>
        public APIResponse GetPoliceOfficerDetails(string serviceNumber)
        {
            try
            {
                var personnel = _externalDataOfficers.Value.GetPoliceOfficer(new Core.ExternalSourceData.HRSystem.PersonnelRequestModel
                {
                    ServiceNumber = serviceNumber,
                    Page = "1",
                    PageSize = "1"
                });

                if (personnel.Error)
                {
                    List<PersonnelErrorResponseModel> response = JsonConvert.DeserializeObject<List<PersonnelErrorResponseModel>>(JsonConvert.SerializeObject(personnel.ResponseObject));
                    return new APIResponse { Error = true, ResponseObject = string.Join(",", response.Select(x => x.ErrorMessage)) };
                }
                else
                {
                    PersonnelResponseModel response = JsonConvert.DeserializeObject<PersonnelResponseModel>(JsonConvert.SerializeObject(personnel.ResponseObject));
                    PersonnelReportRecord personnelReportRecord = response.ReportRecords.FirstOrDefault();

                    if (personnelReportRecord == null)
                    {
                        return new APIResponse { Error = true, ResponseObject = "No records found for AP number, please try again." };
                    }
                
                    var command = _commandManager.GetCommandWithCode($"{personnelReportRecord.CommandLevelCode}-{personnelReportRecord.CommandCode}");

                    string commandCode = $"{personnelReportRecord.CommandLevelCode}-{personnelReportRecord.CommandCode}-{personnelReportRecord.SubCommandCode}-{personnelReportRecord.SubSubCommandCode}";

                    commandCode = commandCode.Replace("-0", "");

                    string[] commandCodes = commandCode.Split('-');
                    int[] commandIds = new int[commandCodes.Length -1];
                    string[] commandCodeArr = new string[commandCodes.Length -1];
                    string tempCommandCode = string.Empty;
                    for (int i = 0; i <= commandCodes.Length - 2; i++)
                    {
                        tempCommandCode = string.IsNullOrEmpty(tempCommandCode) ? $"{commandCodes[i]}-{commandCodes[i + 1]}" : tempCommandCode.Replace($"-{commandCodes[i]}", $"-{commandCodes[i]}-{commandCodes[i + 1]}");
                        commandCodeArr[i] = tempCommandCode;
                        var tempCommand = _commandManager.GetCommandWithCode(tempCommandCode);

                        if (tempCommand == null)
                        {
                            Logger.Error($"Unable to map command for personnel with command code {tempCommandCode}");
                            throw new Exception($"Unable to map personnel with command code {tempCommandCode}");
                        }

                        commandIds[i] = tempCommand.Id;
                    }

                    string name = string.Format("{0} {1}", personnelReportRecord.FirstName, personnelReportRecord.Surname).ToUpper();
                    int subCommandId = 0;
                    //Case when commandIds length is equal to 2, e.g 1-17-23, subCommandId will be at index 1 which is commandIds.Length - 1
                    //Case when commandIds length is equal to 3, e.g 1-17-24-56, subCommandId will be at index 1 which is commandIds.Length - 2   
                    if (commandIds.Length == 2) { subCommandId = commandIds[commandIds.Length - 1]; }
                    else if(commandIds.Length == 3) { subCommandId = commandIds[commandIds.Length - 2]; }

                    return new APIResponse { ResponseObject = new PoliceOfficerVM { Name = name, PhoneNumber = personnelReportRecord.PhoneNumber, IdNumber = personnelReportRecord.ServiceNumber.ToUpper(), CommandCategoryId = command.CommandCategoryId, CommandId = command.Id, SubCommandCode = commandCodeArr[1], SubSubCommandId = (commandIds.Length == 3) ? commandIds[commandIds.Length-1] : 0, SubCommandId = subCommandId, CommandCode = command.Code } };
                  
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Populates the vm for post back
        /// </summary>
        /// <param name="userInput"></param>
        public void PopulateAdminUserModelForPostback(AdminUserCreationVM userInput)
        {
            try
            {
                userInput.FlowDefinitions = new List<Core.DTO.PSServiceRequestFlowDefinitionDTO> { };
                userInput.FlowDefinitionLevels = new List<List<Core.DTO.PSServiceRequestFlowDefinitionLevelDTO>> { };

                if (userInput.CommandId > 0)
                {
                    Core.HelperModels.CommandVM commandDetail = _coreCommand.GetCommandDetails(userInput.CommandId);
                    userInput.OfficerSectionCommands = _coreCommand.GetCommandsByParentCode(commandDetail.Code).ToList();
                }
                if (userInput.OfficerSectionId > 0)
                {
                    Core.HelperModels.CommandVM commandDetail = _coreCommand.GetCommandDetails(userInput.OfficerSectionId);
                    userInput.OfficerSubSectionCommands = _coreCommand.GetCommandsByParentCode(commandDetail.Code).ToList();
                }
                if(userInput.SelectedServiceTypes != null && userInput.SelectedServiceTypes.Any())
                {
                    foreach(var serviceType in userInput.SelectedServiceTypes)
                    {
                        userInput.FlowDefinitions.AddRange(_psServiceManager.GetFlowDefinitionForServiceType(serviceType));
                        userInput.FlowDefinitions.AddRange(_serviceWorkflowDifferentialManager.Value.GetFlowDefinitionForServiceType(serviceType));
                        foreach(var definition in userInput.FlowDefinitions.Where(x => x.ServiceId == serviceType))
                        {
                            if (userInput.SelectedFlowDefinitions != null && userInput.SelectedFlowDefinitions.Contains(definition.Id))
                            {
                                var definitionLevels = _pSServiceRequestFlowDefinitionLevelManager.GetApprovalDefinitionLevelsForDefinitionWithId(definition.Id).ToList();
                                definitionLevels.ForEach(x => { x.ServiceId = serviceType; });
                                userInput.FlowDefinitionLevels.Add(definitionLevels);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Sends admin user registration email notification
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        private bool SendRegistrationEmailNotification(AdminUserCreationVM userInput)
        {
            dynamic emailDetails = new ExpandoObject();
            emailDetails.Email = userInput.Email;
            emailDetails.Name = userInput.Fullname.Trim();
            emailDetails.Password = userInput.PhoneNumber;
            emailDetails.Username = userInput.Username;
            emailDetails.Subject = "POSSAP Admin Account Creation Notification";

            if (PSSUtil.IsEmailEnabled(_orchardServices.WorkContext.CurrentSite.SiteName))
            {
                bool result = int.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.EmailProvider), out int providerId);

                if (!result)
                {
                    providerId = (int)EmailProvider.Pulse;
                }
                foreach (var impl in _emailProvider)
                {
                    if ((EmailProvider)providerId == impl.Value.GetEmailNotificationProvider)
                    {
                        return impl.Value.PSSAdminUserRegistrationNotification(emailDetails);
                    }
                }
            }
            return false;
        }
    }
}