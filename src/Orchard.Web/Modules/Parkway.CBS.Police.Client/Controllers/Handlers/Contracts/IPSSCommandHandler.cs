using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IPSSCommandHandler : IDependency
    {

        /// <summary>
        /// Get the list of commands for this LGA
        /// </summary>
        /// <param name="lgaid"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetCommands(int lgaid);

        /// <summary>
        /// Get the list of area and divisional commands for this LGA
        /// </summary>
        /// <param name="lgaid"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetAreaAndDivisionalCommandsByLGA(int lgaid);

        /// <summary>
        /// Get the list of area and divisional commands for this State
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetAreaAndDivisionalCommandsByStateId(int stateId);


        /// <summary>
        /// Get the commands that are available for this state and service Id
        /// </summary>
        /// <param name="parsedStateVal"></param>
        /// <param name="serviceId"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetCommandsByStateAndService(int stateId, int serviceId);

    }
}
