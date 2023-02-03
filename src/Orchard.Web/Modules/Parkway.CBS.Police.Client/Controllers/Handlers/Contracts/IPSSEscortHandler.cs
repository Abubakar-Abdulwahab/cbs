using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IPSSEscortHandler : IDependency
    {

        /// <summary>
        /// Get view model for police escort
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        EscortRequestVM GetVMForPoliceEscort(int serviceId);


        /// <summary>
        /// Get state command for 
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>CommandVM</returns>
        CommandVM GetStateCommand(int stateId);


        /// <summary>
        /// validates selected team, tactical squad if any and next level commands if any
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        CommandVM ValidateSelectedCommand(EscortRequestVM userInput, ref List<ErrorModel> errors);


        /// <summary>
        /// Get the direction for request confirmation
        /// </summary>
        /// <returns>dynamic</returns>
        dynamic GetNextDirectionForConfirmation();

        /// <summary>
        /// Gets category types for service category with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<PSSEscortServiceCategoryVM> GetCategoryTypesForServiceCategoryWithId(int id);

        /// <summary>
        /// Gets escort service category with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PSSEscortServiceCategoryVM GetEscortServiceCategoryWithId(int id);

        /// <summary>
        /// Gets commands for command type with specified id
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetCommandsForCommandTypeWithId(int commandTypeId);

        /// <summary>
        /// Gets commands for state with specified id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetCommandsByState(int stateId);

        /// <summary>
        /// Gets area and divisional commands using stateId and optionally specified lgaId
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetAreaAndDivisionalCommandsByStateAndLGA(int stateId, int lgaId = 0);

        /// <summary>
        /// Gets next level commands using specified code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        IEnumerable<CommandVM> GetNextLevelCommandsWithCode(string code);

        /// <summary>
        /// Checks if selected category type exists for specified service category
        /// </summary>
        /// <param name="serviceCategoryId"></param>
        /// <param name="categoryTypeId"></param>
        /// <returns></returns>
        bool ValidateCategoryTypeForServiceCategory(int serviceCategoryId, int categoryTypeId);

        /// <summary>
        /// Gets LGA with specified id
        /// </summary>
        /// <returns></returns>
        LGAVM GetLGAWithId(int id);

        /// <summary>
        /// Gets command details
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns></returns>
        CommandVM GetCommandDetails(int commandId);
    }
}
