using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSAdminUserAssignEscortProcessFlowHandler : IPSSAdminUserAssignEscortProcessFlowHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly ICommandTypeManager<CommandType> _commandTypeManager;
        private readonly IEscortProcessStageDefinitionManager<EscortProcessStageDefinition> _escortProcessStageDefinitionManager;
        private readonly IPSSAdminUsersManager<PSSAdminUsers> _adminUsersManager;
        private readonly IEscortProcessFlowManager<EscortProcessFlow> _escortProcessFlowManager;
        ILogger Logger { get; set; }
        public PSSAdminUserAssignEscortProcessFlowHandler(ICommandTypeManager<CommandType> commandTypeManager, IHandlerComposition handlerComposition, IEscortProcessStageDefinitionManager<EscortProcessStageDefinition> escortProcessStageDefinitionManager, IPSSAdminUsersManager<PSSAdminUsers> adminUsersManager, IEscortProcessFlowManager<EscortProcessFlow> escortProcessFlowManager)
        {
            _commandTypeManager = commandTypeManager;
            _handlerComposition = handlerComposition;
            _escortProcessStageDefinitionManager = escortProcessStageDefinitionManager;
            _adminUsersManager = adminUsersManager;
            _escortProcessFlowManager = escortProcessFlowManager;
            Logger = NullLogger.Instance;
        }



        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="permission"></param>
        public void CheckForPermission(Orchard.Security.Permissions.Permission permission)
        {
            _handlerComposition.IsAuthorized(permission);
        }

        /// <summary>
        /// Gets Assign Escort Process Flow VM
        /// </summary>
        /// <returns></returns>
        public AssignEscortProcessFlowVM GetAssignEscortProcessFlowVM()
        {
            return new AssignEscortProcessFlowVM
            {
                CommandTypes = _commandTypeManager.GetCommandTypes()
            };
        }


        /// <summary>
        /// Assigns selected users to specified escort process stage definitions
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="userId"></param>
        /// <param name="errors"></param>
        public void AssignProcessFlowsToUsers(AssignEscortProcessFlowVM userInput, int userId, ref List<ErrorModel> errors)
        {
            try
            {
                if (userInput.EscortProcessFlows == null || !userInput.EscortProcessFlows.Any()) 
                {
                    errors.Add(new ErrorModel { ErrorMessage = "No process stage assigned", FieldName = nameof(AssignEscortProcessFlowVM.EscortProcessFlows) });
                    return;
                }

                List<EscortProcessFlow> processFlows = new List<EscortProcessFlow> { };

                foreach(EscortProcessFlowVM processStageAssignment in userInput.EscortProcessFlows)
                {
                    AdminUserVM adminUser = _adminUsersManager.GetAdminUser(processStageAssignment.AdminUserId);
                    if(adminUser == null) 
                    { errors.Add(new ErrorModel { ErrorMessage = "Admin user selected not found", FieldName = nameof(AssignEscortProcessFlowVM.EscortProcessFlows) }); return; }

                    if(_escortProcessFlowManager.Count(x => x.AdminUser.Id == adminUser.Id) > 0) 
                    { errors.Add(new ErrorModel { ErrorMessage = $"Admin user with username {processStageAssignment.AdminUsername} has already been assigned", FieldName = nameof(AssignEscortProcessFlowVM.EscortProcessFlows) }); return; }

                    EscortProcessStageDefinitionDTO escortProcessStage = _escortProcessStageDefinitionManager.GetProcessStageWithCommandTypeAndId(processStageAssignment.LevelId, processStageAssignment.CommandTypeId);

                    if(escortProcessStage == null) 
                    { errors.Add(new ErrorModel { ErrorMessage = $"Selected process stage {processStageAssignment.LevelName} not valid", FieldName = nameof(AssignEscortProcessFlowVM.EscortProcessFlows) }); return; }

                    processFlows.Add(new EscortProcessFlow
                    {
                        Name = $"DESK OFFICER {escortProcessStage.Name}-{adminUser.Username}",
                        Level = new EscortProcessStageDefinition { Id = escortProcessStage.Id },
                        AdminUser = new PSSAdminUsers { Id = adminUser.Id },
                        CommandType = new CommandType { Id = processStageAssignment.CommandTypeId },
                        AddedBy = new Orchard.Users.Models.UserPartRecord { Id = userId },
                        LastUpdatedBy = new Orchard.Users.Models.UserPartRecord { Id = userId },
                        IsActive = true
                    });
                }

                if (!_escortProcessFlowManager.SaveBundle(processFlows))
                {
                    _escortProcessFlowManager.RollBackAllTransactions();
                    throw new Exception("Could not assign process stage definitions");
                }
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}