using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IGenerateEscortRequestWithoutOfficersHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="permission"></param>
        void CheckForPermission(Orchard.Security.Permissions.Permission permission);


        /// <summary>
        /// Gets GenerateEscortRequestForWithoutOfficersVM
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        GenerateEscortRequestForWithoutOfficersVM GetGenerateEscortRequestVM(long batchId);


        /// <summary>
        /// Validates and Generate escort request
        /// </summary>
        /// <param name="userInputModel"></param>
        /// <param name="errors"></param>
        InvoiceGenerationResponse GenerateEscortRequestForDefaultBranch(GenerateEscortRequestForWithoutOfficersVM userInputModel, ref List<ErrorModel> errors);


        /// <summary>
        /// Validates and Generate escort request for branch
        /// </summary>
        /// <param name="userInputModel"></param>
        /// <param name="errors"></param>
        InvoiceGenerationResponse GenerateEscortRequestForBranch(GenerateEscortRequestForWithoutOfficersVM userInputModel, ref List<ErrorModel> errors);


        /// <summary>
        /// populates <paramref name="vm"/> for postback
        /// </summary>
        /// <param name="vm"></param>
        void PopulateGenerateEscortRequestVMForPostback(GenerateEscortRequestForWithoutOfficersVM vm);
    }
}
