using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface ICommandManager<Command> : IDependency, IBaseManager<Command>
    {
        /// <summary>
        /// Checks if a command exists
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns>CommandVM</returns>
        bool CheckIfCommandExist(int commandId);

        /// <summary>
        /// Get commands by state Id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetCommandsByState(int stateId);

        /// <summary>
        /// Get area and divisional commands by LGA Id
        /// </summary>
        /// <param name="lgaid"></param>
        /// <returns>List{CommandVM}</returns>
        List<CommandVM> GetAreaAndDivisionalCommandsByLGA(int lgaid);

        /// <summary>
        /// Get area and divisional commands by State Id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>List{CommandVM}</returns>
        List<CommandVM> GetAreaAndDivisionalCommandsByStateId(int stateId);

        /// <summary>
        /// Gets area and divisional commands for state with specified id and optional LGA with specified id
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="lgaId"></param>
        /// <returns>returns commands for specified state with no LGA is specified</returns>
        IEnumerable<CommandVM> GetAreaAndDivisionalCommandsByStateAndLGA(int stateId, int lgaId);

        /// <summary>
        /// Get commands by LGA Id
        /// </summary>
        /// <param name="lgaid"></param>
        /// <returns>List{CommandVM}</returns>
        List<CommandVM> GetCommandsByLGA(int lgaid);

        /// <summary>
        /// Get the list of commands for the specified command category
        /// </summary>
        /// <param name="commandCategoryId"></param>
        /// <returns>List{CommandVM}</returns>
        List<CommandVM> GetCommandsByCommandCategory(int commandCategoryId);


        /// <summary>
        /// Get command
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns>CommandVM</returns>
        CommandVM GetCommandDetails(int commandId);


        /// <summary>
        /// Get the state command
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>CommandVM</returns>
        CommandVM GetStateCommandDetails(int stateId);


        /// <summary>
        /// Get the federal level command
        /// </summary>
        /// <returns>CommandVM</returns>
        CommandVM GetFederalLevelCommand();

        /// <summary>
        /// Gets the inspector general of the police command
        /// </summary>
        /// <returns></returns>
        CommandVM GetIGPOfficeCommand();

        /// <summary>
        /// Get list of commands
        /// </summary>
        /// <returns>List{CommandVM}</returns>
        List<CommandVM> GetCommands();

        /// <summary>
        /// Get command with the specified code
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns>CommandVM</returns>
        CommandVM GetCommandWithCode(string code);


        /// <summary>
        /// Gets Criminal Investigation Department for specified state
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        CommandVM GetStateCID(int stateId);


        /// <summary>
        /// Gets commands for command type with specified id
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetCommandsForCommandTypeWithId(int commandTypeId);


        /// <summary>
        /// Gets command with specified id for command type with specified id
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        CommandVM GetCommandForCommandTypeWithId(int commandTypeId, int commandId);


        /// <summary>
        /// Gets next level commands using specified code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetNextLevelCommandsWithCode(string code);

        /// <summary>
        /// Gets next level commands for state with specified id using specified code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetNextLevelCommandsWithCodeForState(int stateId, string code);

        /// <summary>
        /// Gets next level area and divisional commands for LGA with specified id using specified code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetNextLevelAreaAndDivisionalCommandsWithCodeForLGA(int lgaId, string code);

        /// <summary>
        /// Gets commands with the specified parent code
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetCommandsByParentCode(string parentCode);

    }
}
