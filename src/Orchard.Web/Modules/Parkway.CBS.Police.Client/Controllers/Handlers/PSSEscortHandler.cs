using Orchard;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class PSSEscortHandler : IPSSEscortHandler
    {
        private readonly ICoreStateAndLGA _coreStateLGAService;
        private readonly Lazy<ICoreCommand> _coreCommandService;
        private readonly IPSSReasonManager<PSSReason> _pssReasonRepo;
        private readonly ICorePSSEscortServiceCategory _corePSSEscortServiceCategory;
        private readonly ICommandTypeManager<CommandType> _commandTypeManager;
        private readonly IPSServiceCaveatManager<PSServiceCaveat> _caveatRepo;
        private readonly IOrchardServices _orchardServices;

        public PSSEscortHandler(ICoreStateAndLGA coreStateLGAService, Lazy<ICoreCommand> coreCommandService, IPSSReasonManager<PSSReason> pssReasonRepo, ICorePSSEscortServiceCategory corePSSEscortServiceCategory, ICommandTypeManager<CommandType> commandTypeManager, IPSServiceCaveatManager<PSServiceCaveat> caveatRepo, IOrchardServices orchardServices)
        {
            _coreStateLGAService = coreStateLGAService;
            _coreCommandService = coreCommandService;
            _pssReasonRepo = pssReasonRepo;
            _corePSSEscortServiceCategory = corePSSEscortServiceCategory;
            _commandTypeManager = commandTypeManager;
            _caveatRepo = caveatRepo;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Get view model for police escort
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>EscortRequestVM</returns>
        public EscortRequestVM GetVMForPoliceEscort(int serviceId)
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            PSServiceCaveatVM caveat = ObjectCacheProvider.GetCachedObject<PSServiceCaveatVM>(tenant, $"{nameof(POSSAPCachePrefix.Caveat)}-{serviceId}");

            if (caveat == null)
            {
                caveat = _caveatRepo.GetServiceCaveat(serviceId);

                if (caveat != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.Caveat)}-{serviceId}", caveat);
                }
            }

            return new EscortRequestVM
            {
                HeaderObj = new HeaderObj { },
                StateLGAs = _coreStateLGAService.GetStates(),
                Reasons = _pssReasonRepo.GetReasonsVM(),
                EscortServiceCategories = _corePSSEscortServiceCategory.GetEscortServiceCategories(),
                SelectedEscortServiceCategories = new List<int>(),
                CommandTypes = _commandTypeManager.GetCommandTypes(),
                Caveat = caveat
            };
        }


        /// <summary>
        /// Get state command for 
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>CommandVM</returns>
        public CommandVM GetStateCommand(int stateId)
        {
            return _coreCommandService.Value.GetStateCommand(stateId);
        }


        /// <summary>
        /// validates selected team, tactical squad if any and next level commands if any
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public CommandVM ValidateSelectedCommand(EscortRequestVM userInput, ref List<ErrorModel> errors)
        {
            CommandTypeVM commandType = _commandTypeManager.GetCommandType(userInput.SelectedCommandType);
            if (commandType != null)
            {
                //checks if the selected command type i.e Tactical, Conventional has commands
                if (_coreCommandService.Value.CheckIfCommandTypeHasCommands(commandType.Id))
                {
                    //if the command type has commands it will fetch the commands i.e for tactical the commands retrieved could be PMF, CTU, SPU
                    CommandVM command = _coreCommandService.Value.GetCommandForCommandTypeWithId(commandType.Id, userInput.SelectedTacticalSquad);
                    if (command != null)
                    {
                        if (userInput.SelectedCommand == 0) { return command; }
                        CommandVM nextLevelCommand = _coreCommandService.Value.GetCommandDetails(userInput.SelectedCommand);
                        if (nextLevelCommand != null)
                        {
                            if (!nextLevelCommand.Code.Contains(command.Code))
                            {
                                errors.Add(new ErrorModel { FieldName = nameof(EscortRequestVM.SelectedCommand), ErrorMessage = "Selected command/formation value is not valid for the selected tactical squad" });
                                return null;
                            }
                            return nextLevelCommand;
                        }
                        else
                        {
                            errors.Add(new ErrorModel { FieldName = nameof(EscortRequestVM.SelectedCommand), ErrorMessage = "Selected command/formation value is not valid" });
                            return null;
                        }
                    }
                    else
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(EscortRequestVM.SelectedTacticalSquad), ErrorMessage = "Selected tactical squad value is not valid" });
                        return null;
                    }
                }
                else
                {
                    CommandVM selectedCommand = (userInput.SelectedCommand > 0) ? _coreCommandService.Value.GetCommandDetails(userInput.SelectedCommand) : (userInput.SelectedOriginState > 0) ? _coreCommandService.Value.GetStateCommand(userInput.SelectedOriginState) : _coreCommandService.Value.GetStateCommand(userInput.SelectedState);
                    if (selectedCommand != null) { return selectedCommand; }
                    errors.Add(new ErrorModel { FieldName = nameof(EscortRequestVM.SelectedCommand), ErrorMessage = "Selected command not found." });
                    return null;
                }
            }
            else
            {
                errors.Add(new ErrorModel { FieldName = nameof(EscortRequestVM.SelectedCommandType), ErrorMessage = "Selected team value is not valid" });
                return null;
            }
        }


        /// <summary>
        /// Get the direction for request confirmation
        /// </summary>
        /// <returns>dynamic</returns>
        public dynamic GetNextDirectionForConfirmation()
        {
            return new { RouteName = "P.Request.Confirm", Stage = PSSUserRequestGenerationStage.PSSRequestConfirmation };
        }


        /// <summary>
        /// Gets category types for service category with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<PSSEscortServiceCategoryVM> GetCategoryTypesForServiceCategoryWithId(int id)
        {
            return _corePSSEscortServiceCategory.GetCategoryTypesForServiceCategoryWithId(id).ToList();
        }


        /// <summary>
        /// Gets commands for command type with specified id
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetCommandsForCommandTypeWithId(int commandTypeId)
        {
            return _coreCommandService.Value.GetCommandsForCommandTypeWithId(commandTypeId);
        }

        /// <summary>
        /// Gets commands for state with specified id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetCommandsByState(int stateId)
        {
            return _coreCommandService.Value.GetCommandsByState(stateId);
        }

        /// <summary>
        /// Gets area and divisional commands using stateId and optionally specified lgaId
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetAreaAndDivisionalCommandsByStateAndLGA(int stateId, int lgaId = 0)
        {
            return _coreCommandService.Value.GetAreaAndDivisionalCommandsByStateAndLGA(stateId, lgaId);
        }

        /// <summary>
        /// Gets next level commands using specified code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetNextLevelCommandsWithCode(string code)
        {
            return _coreCommandService.Value.GetNextLevelCommandsWithCode(code);
        }

        /// <summary>
        /// Gets escort service category with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PSSEscortServiceCategoryVM GetEscortServiceCategoryWithId(int id)
        {
            return _corePSSEscortServiceCategory.GetEscortServiceCategoryWithId(id);
        }

        /// <summary>
        /// Checks if selected category type exists for specified service category
        /// </summary>
        /// <param name="serviceCategoryId"></param>
        /// <param name="categoryTypeId"></param>
        /// <returns></returns>
        public bool ValidateCategoryTypeForServiceCategory(int serviceCategoryId, int categoryTypeId)
        {
            return _corePSSEscortServiceCategory.CheckIfCategoryTypeInServiceCategory(serviceCategoryId, categoryTypeId);
        }

        /// <summary>
        /// Gets LGA with specified id
        /// </summary>
        /// <returns></returns>
        public LGAVM GetLGAWithId(int id)
        {
            return _coreStateLGAService.GetLGAWithId(id);
        }

        /// <summary>
        /// Gets command details
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns></returns>
        public CommandVM GetCommandDetails(int commandId)
        {
            return _coreCommandService.Value.GetCommandDetails(commandId);
        }
    }
}