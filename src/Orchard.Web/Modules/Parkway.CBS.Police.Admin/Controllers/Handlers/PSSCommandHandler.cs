using System.Linq;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;


namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSCommandHandler : IPSSCommandHandler
    {
        public readonly ICoreCommand _coreCommand;
        private readonly ICorePSSAdminUserService _corePSSAdminUserService;

        public PSSCommandHandler(ICoreCommand coreCommand, ICorePSSAdminUserService corePSSAdminUserService)
        {
            _coreCommand = coreCommand;
            _corePSSAdminUserService = corePSSAdminUserService;
        }


        /// <summary>
        /// Get the list of commands for this LGA
        /// </summary>
        /// <param name="lgaid"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetCommands(int lgaid)
        {
            List<CommandVM> commands = _coreCommand.GetCommandsByLGAId(lgaid);
            if(commands == null || !commands.Any())
            {
                return new APIResponse { Error = true, ResponseObject = "No command was found in the selected local government area." };
            }
            
            return new APIResponse { ResponseObject = commands };
        }

        /// <summary>
        /// Get the list of commands for this state for logged in admin
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetCommandsForAdmin(int stateId)
        {
            try { 
            IEnumerable<CommandVM> commands = _coreCommand.GetNextLevelCommandsWithCodeForState(stateId, _corePSSAdminUserService.GetCommandForAdmin().Code);
            if (commands == null || !commands.Any())
            {
                return new APIResponse { Error = true, ResponseObject = "No command was found in the selected state." };
            }

            return new APIResponse { ResponseObject = commands };
            }
            catch (System.Exception) { throw; }
        }

        /// <summary>
        /// Get the list of area and divisional commands for this LGA for logged in admin
        /// </summary>
        /// <param name="lgaId"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetAreaAndDivisionalCommandsForAdmin(int lgaId)
        {
            try
            {
                IEnumerable<CommandVM> commands = _coreCommand.GetNextLevelAreaAndDivionalCommandsWithCodeForLGA(lgaId, _corePSSAdminUserService.GetCommandForAdmin().Code);
                if (commands == null || !commands.Any())
                {
                    return new APIResponse { Error = true, ResponseObject = "No command was found in the selected LGA." };
                }

                return new APIResponse { ResponseObject = commands };
            }
            catch (System.Exception) { throw; }
        }

        /// <summary>
        /// Get the list of commands for the specified command category
        /// </summary>
        /// <param name="commandCategoryId"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetCommandsByCommandCategory(int commandCategoryId)
        {
            List<CommandVM> commands = _coreCommand.GetCommandsByCommandCategory(commandCategoryId);
            return new APIResponse { ResponseObject = commands };
        }


        /// <summary>
        /// Gets commands with specified parent code
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        public APIResponse GetCommandsByParentCode(string parentCode)
        {
            IEnumerable<CommandVM> commands = _coreCommand.GetCommandsByParentCode(parentCode);
            return new APIResponse { ResponseObject = commands };
        }

    }
}