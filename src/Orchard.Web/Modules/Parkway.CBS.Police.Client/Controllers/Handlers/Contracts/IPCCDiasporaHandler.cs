using Orchard;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IPCCDiasporaHandler : IDependency
    {

        /// <summary>
        /// Validate user input for diaspora
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        void ValidateCharacterCertificateRequest(PCCDiasporaUserInputVM userInput, ref List<ErrorModel> errors);


        /// <summary>
        /// Get character certificate request VM
        /// for diaspora
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>PCCDiasporaUserInputVM</returns>
        PCCDiasporaUserInputVM GetVMForCharacterCertificate(int serviceId, long taxEntityId, int taxCategoryId);


        /// <summary>
        /// Validate uploaded files
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="uploads"></param>
        /// <param name="errors"></param>
        void ValidateCharacterCertificateFileInput(PCCDiasporaUserInputVM userInput, ICollection<UploadedFileAndName> uploads, ref List<ErrorModel> errors);


        /// <summary>
        /// Validate identity
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        /// <param name="taxEntityId"></param>
        /// <param name="taxCategoryId"></param>
        void ValidateIdentity(PCCDiasporaUserInputVM userInput, ref List<ErrorModel> errors, long taxEntityId, int taxCategoryId);


        /// <summary>
        /// Get the CPCCR command that would treat
        /// the requests for diaspora 
        /// </summary>
        /// <returns>CommandVM</returns>
        CommandVM GetCPCCRCommand();

    }
}
