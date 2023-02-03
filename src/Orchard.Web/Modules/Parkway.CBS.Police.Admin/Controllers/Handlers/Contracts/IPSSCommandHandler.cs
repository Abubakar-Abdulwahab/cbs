using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
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
        /// Get the list of commands for the specified command category
        /// </summary>
        /// <param name="commandCategoryId"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetCommandsByCommandCategory(int commandCategoryId);

        /// <summary>
        /// Get the list of area and divisional commands for this LGA for logged in admin
        /// </summary>
        /// <param name="lgaId"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetAreaAndDivisionalCommandsForAdmin(int lgaId);

        /// <summary>
        /// Get the list of commands for this state for logged in admin
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetCommandsForAdmin(int stateId);


        /// <summary>
        /// Gets commands with specified parent code
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        APIResponse GetCommandsByParentCode(string parentCode);
    }
}
