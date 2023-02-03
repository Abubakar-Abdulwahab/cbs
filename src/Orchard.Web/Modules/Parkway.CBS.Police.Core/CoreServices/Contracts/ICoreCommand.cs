using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreCommand : IDependency
    {

        /// <summary>
        /// Checks if a command exists
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns>CommandVM</returns>
        bool CheckIfCommandExist(int commandId);

        /// <summary>
        /// Get commands for state with specified id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetCommandsByState(int stateId);

        /// <summary>
        /// Gets area and divisional commands for state with specified id and optional LGA with specified id
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetAreaAndDivisionalCommandsByStateAndLGA(int stateId, int lgaId);

        /// <summary>
        /// Get area and divisional commands by State Id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetAreaAndDivisionalCommandsByStateId(int stateId);

        /// <summary>
        /// Get area and divisional commands by LGA Id
        /// </summary>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetAreaAndDivisionalCommandsByLGA(int lgaId);

        /// <summary>
        /// Get the list of commands within this LGA
        /// </summary>
        /// <param name="lgaid"></param>
        List<CommandVM> GetCommandsByLGAId(int lgaid);

        /// <summary>
        /// Get the list of commands for the specified command category
        /// </summary>
        /// <param name="commandCategoryId"></param>
        /// <returns>List{CommandVM}</returns>
        List<CommandVM> GetCommandsByCommandCategory(int commandCategoryId);

        /// <summary>
        /// Get command details
        /// </summary>
        /// <param name="selectedCommand"></param>
        /// <returns>CommandVM</returns>
        CommandVM GetCommandDetails(int selectedCommand);


        /// <summary>
        /// Get state command
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>CommandVM</returns>
        CommandVM GetStateCommand(int stateId);


        /// <summary>
        /// Get Federal command
        /// </summary>
        /// <returns>CommandVM</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        /// <exception cref="Exception">If more than one force record is found</exception>
        CommandVM GetFederalCommand();

        /// <summary>
        /// Get Inspector general of police command
        /// </summary>
        /// <returns></returns>
        CommandVM GetIGPOfficeCommand();

        /// <summary>
        /// Get list of commands
        /// </summary>
        /// <returns>List{CommandVM}</returns>
        List<CommandVM> GetCommands();

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
        /// Checkes if command type with specified id has commands
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        bool CheckIfCommandTypeHasCommands(int commandTypeId);

        /// <summary>
        /// Gets next level commands for LGA with specified id using specified code
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetNextLevelCommandsWithCodeForState(int stateId, string code);

        /// <summary>
        /// Gets next level area and divisional commands for LGA with specified id using specified code
        /// </summary>
        /// <param name="lgaId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetNextLevelAreaAndDivionalCommandsWithCodeForLGA(int lgaId, string code);

        /// <summary>
        /// Gets commands with specified parent code
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetCommandsByParentCode(string parentCode);


        /// <summary>
        /// Get the Command for CPCCR
        /// </summary>
        /// <returns></returns>
        CommandVM CPCCRCommand();

    }
}
