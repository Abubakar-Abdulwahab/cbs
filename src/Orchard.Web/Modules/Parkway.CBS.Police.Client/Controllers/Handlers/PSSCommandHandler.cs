using System.Linq;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;


namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class PSSCommandHandler : IPSSCommandHandler
    {
        public readonly ICoreCommand _coreCommand;
        public readonly ICoreServiceStateCommand _coreServiceStateCommand;

        public PSSCommandHandler(ICoreCommand coreCommand, ICoreServiceStateCommand coreServiceStateCommand)
        {
            _coreCommand = coreCommand;
            _coreServiceStateCommand = coreServiceStateCommand;
        }


        /// <summary>
        /// Get the list of commands for this LGA
        /// </summary>
        /// <param name="lgaid"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetCommands(int lgaid)
        {
            List<CommandVM> commands = _coreCommand.GetCommandsByLGAId(lgaid);
            if (commands == null || !commands.Any())
            {
                return new APIResponse { Error = true, ResponseObject = "No command was found in the selected local government area." };
            }

            return new APIResponse { ResponseObject = commands };
        }

        /// <summary>
        /// Get the list of area and divisional commands for this LGA
        /// </summary>
        /// <param name="lgaid"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetAreaAndDivisionalCommandsByLGA(int lgaid)
        {
            IEnumerable<CommandVM> commands = _coreCommand.GetAreaAndDivisionalCommandsByLGA(lgaid);
            if(commands == null || !commands.Any())
            {
                return new APIResponse { Error = true, ResponseObject = "No command was found in the selected local government area." };
            }
            
            return new APIResponse { ResponseObject = commands };
        }


        /// <summary>
        /// Get the list of area and divisional commands for this State
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetAreaAndDivisionalCommandsByStateId(int stateId)
        {
            IEnumerable<CommandVM> commands = _coreCommand.GetAreaAndDivisionalCommandsByStateId(stateId);
            if (commands == null || !commands.Any())
            {
                return new APIResponse { Error = true, ResponseObject = "No command was found in the selected state." };
            }

            return new APIResponse { ResponseObject = commands };
        }


        /// <summary>
        /// Get the commands that are available for this state and service Id
        /// </summary>
        /// <param name="parsedStateVal"></param>
        /// <param name="serviceId"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetCommandsByStateAndService(int stateId, int serviceId)
        {
            IEnumerable<CommandVM> commands = _coreServiceStateCommand.GetActiveCommands(stateId, serviceId);
            if (commands == null || !commands.Any())
            {
                return new APIResponse { Error = true, ResponseObject = "No command was found in the selected state." };
            }

            return new APIResponse { ResponseObject = commands };
        }

    }
}