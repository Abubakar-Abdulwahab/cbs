using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IPSSCharacterCertificateHandler : IDependency
    {
        /// <summary>
        /// Get character certificate request VM
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        CharacterCertificateRequestVM GetVMForCharacterCertificate(int serviceId);

        /// <summary>
        /// Validate  and get the command details
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="selectedState"></param>
        /// <param name="selectedStateLGA"></param>
        /// <param name="selectedCommand"></param>
        /// <returns>CommandVM</returns>
        CommandVM ValidateSelectedCommand(PSSCharacterCertificateController callback, int selectedState, int selectedStateLGA, int selectedCommand, ref List<ErrorModel> errors);


        /// <summary>
        /// Validate that the selected state, selected command and service Id match
        /// that would provide the capture location
        /// </summary>
        /// <param name="selectedState"></param>
        /// <param name="selectedCommand"></param>
        /// <param name="serviceId"></param>
        /// <param name="errors"></param>
        /// <returns>CommandVM</returns>
        CommandVM GetSelectedCommand(int selectedState, int selectedCommand, int serviceId, ref List<ErrorModel> errors);


        /// <summary>
        /// Validates character certificate request vm
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        void ValidateCharacterCertificateRequest(CharacterCertificateRequestVM userInput, out List<ErrorModel> errors);


        /// <summary>
        /// Get next action direction for character certificate
        /// </summary>
        /// <returns></returns>
        dynamic GetNextDirectionForConfirmation();


        /// <summary>
        /// Get the list of active commands for this state and service
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="serviceId"></param>
        /// <returns>IEnumerable{CommandVM}</returns>
        IEnumerable<CommandVM> GetListOfCommandsForState(int stateId, int serviceId);

    }
}
