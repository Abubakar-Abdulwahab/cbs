using Orchard;
using Orchard.Logging;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreCommand : ICoreCommand
    {
        private readonly ICommandManager<Command> _commandManager;
        private readonly IOrchardServices _orchardServices;
        public ILogger Logger { get; set; }

        public CoreCommand(ICommandManager<Command> commandManager, IOrchardServices orchardServices)
        {
            _commandManager = commandManager;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get commands for state with specified id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetCommandsByState(int stateId)
        {
            return _commandManager.GetCommandsByState(stateId);
        }


        /// <summary>
        /// Gets area and divisional commands for state with specified id and optional LGA with specified id
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetAreaAndDivisionalCommandsByStateAndLGA(int stateId, int lgaId)
        {
            return _commandManager.GetAreaAndDivisionalCommandsByStateAndLGA(stateId, lgaId);
        }


        /// <summary>
        /// Get area and divisional commands by State Id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetAreaAndDivisionalCommandsByStateId(int stateId)
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            IEnumerable<CommandVM> result = ObjectCacheProvider.GetCachedObject<IEnumerable<CommandVM>>(tenant, $"{nameof(POSSAPCachePrefix.AreaDivisionCommand)}-{stateId}");

            if (result == null)
            {
                result = _commandManager.GetAreaAndDivisionalCommandsByStateId(stateId);

                if (result != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.AreaDivisionCommand)}-{stateId}", result);
                }
            }
            return result;
        }

        /// <summary>
        /// Get area and divisional commands by LGA Id
        /// </summary>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetAreaAndDivisionalCommandsByLGA(int lgaId)
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            IEnumerable<CommandVM> result = ObjectCacheProvider.GetCachedObject<IEnumerable<CommandVM>>(tenant, $"{nameof(POSSAPCachePrefix.AreaDivisionLGACommand)}-{lgaId}");

            if (result == null)
            {
                result = _commandManager.GetAreaAndDivisionalCommandsByLGA(lgaId);

                if (result != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.AreaDivisionLGACommand)}-{lgaId}", result);
                }
            }
            return result;
        }

        /// <summary>
        /// Get the list of commands within this LGA
        /// </summary>
        /// <param name="lgaid"></param>
        public List<CommandVM> GetCommandsByLGAId(int lgaid)
        {
            return _commandManager.GetCommandsByLGA(lgaid);
        }

        /// <summary>
        /// Get the list of commands for the specified command category
        /// </summary>
        /// <param name="commandCategoryId"></param>
        /// <returns>List{CommandVM}</returns>
        public List<CommandVM> GetCommandsByCommandCategory(int commandCategoryId)
        {
            return _commandManager.GetCommandsByCommandCategory(commandCategoryId);
        }

        /// <summary>
        /// Get command details
        /// </summary>
        /// <param name="selectedCommand"></param>
        /// <returns>CommandVM</returns>
        public CommandVM GetCommandDetails(int selectedCommand)
        {
            return _commandManager.GetCommandDetails(selectedCommand);
        }

        /// <summary>
        /// Checks if a command exists
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns>CommandVM</returns>
        public bool CheckIfCommandExist(int commandId)
        {
            return _commandManager.CheckIfCommandExist(commandId);
        }

        /// <summary>
        /// Get state command
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>CommandVM</returns>
        public CommandVM GetStateCommand(int stateId)
        {
            return _commandManager.GetStateCommandDetails(stateId);
        }


        /// <summary>
        /// Get Federal command
        /// </summary>
        /// <returns>CommandVM</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        /// <exception cref="Exception">If more than one force record is found</exception>
        public CommandVM GetFederalCommand()
        {
            CommandVM returnVal = _commandManager.GetFederalLevelCommand();
            if (returnVal == null) { throw new NoRecordFoundException("Could not find the force command details"); }
            return returnVal;
        }

        /// <summary>
        /// Get Inspector general of police command
        /// </summary>
        /// <returns></returns>
        public CommandVM GetIGPOfficeCommand()
        {
            CommandVM returnVal = _commandManager.GetIGPOfficeCommand();
            if (returnVal == null) { throw new NoRecordFoundException("Could not find the IGP command details"); }
            return returnVal;
        }

        /// <summary>
        /// Get list of commands
        /// </summary>
        /// <returns>List{CommandVM}</returns>

        public List<CommandVM> GetCommands()
        {
            return _commandManager.GetCommands();
        }


        /// <summary>
        /// Gets commands for command type with specified id
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetCommandsForCommandTypeWithId(int commandTypeId)
        {
            return _commandManager.GetCommandsForCommandTypeWithId(commandTypeId);
        }


        /// <summary>
        /// Gets command with specified id for command type with specified id
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        public CommandVM GetCommandForCommandTypeWithId(int commandTypeId, int commandId)
        {
            return _commandManager.GetCommandForCommandTypeWithId(commandTypeId, commandId);
        }


        /// <summary>
        /// Gets next level commands using specified code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetNextLevelCommandsWithCode(string code)
        {
            return _commandManager.GetNextLevelCommandsWithCode(code);
        }


        /// <summary>
        /// Checkes if command type with specified id has commands
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        public bool CheckIfCommandTypeHasCommands(int commandTypeId)
        {
            return _commandManager.Count(x => x.CommandType == new CommandType { Id = commandTypeId }) > 0;
        }


        /// <summary>
        /// Gets next level commands for state with specified id using specified code
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetNextLevelCommandsWithCodeForState(int stateId, string code)
        {
            return _commandManager.GetNextLevelCommandsWithCodeForState(stateId, code);
        }


        /// <summary>
        /// Gets next level area and divisional commands for LGA with specified id using specified code
        /// </summary>
        /// <param name="lgaId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetNextLevelAreaAndDivionalCommandsWithCodeForLGA(int lgaId, string code)
        {
            return _commandManager.GetNextLevelAreaAndDivisionalCommandsWithCodeForLGA(lgaId, code);
        }


        /// <summary>
        /// Gets commands with specified parent code
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetCommandsByParentCode(string parentCode)
        {
            return _commandManager.GetCommandsByParentCode(parentCode);
           
        }


        /// <summary>
        /// Get the Command for CPCCR
        /// </summary>
        /// <returns>CommandVM</returns>
        public CommandVM CPCCRCommand()
        {
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            Node node = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.CPCCR_DB_Command_Identifier.ToString()).FirstOrDefault();
            if (node == null || string.IsNullOrEmpty(node.Value))
            {
                Logger.Error(string.Format("Unable to get the command ID of CPCCR that handles diaspora requests"));
                throw new Exception();
            }
            return _commandManager.GetCommandDetails(int.Parse(node.Value));
        }


    }
}