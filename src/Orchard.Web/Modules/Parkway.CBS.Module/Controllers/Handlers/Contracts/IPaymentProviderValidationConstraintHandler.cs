using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.Controllers.Handlers.Contracts
{
    public interface IPaymentProviderValidationConstraintHandler : IDependency
    {

        /// <summary>
        /// Get VM for Assign External Payment Provider
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns></returns>
        AssignExternalPaymentProviderVM GetAssignExternalPaymentProviderVM(int providerId);

        /// <summary>
        /// Get a list of revenue heads for the mdas with the specified Id
        /// </summary>
        /// <param name="mdaId"></param>
        /// <returns></returns>
        dynamic GetRevenueHeadsPerMda(string mdaIds);

        /// <summary>
        /// Assign payment provider to selected revenue heads
        /// </summary>
        /// <param name="userInput"></param>
        void AssignPaymentProviderToSelectedRevenueHeads(AssignExternalPaymentProviderVM userInput);


        /// <summary>
        /// Updates MDA & Revenue Heads access restrictions staging table with new records
        /// </summary>
        /// <param name="additions"></param>
        /// <param name="removals"></param>
        /// <param name="providerId"></param>
        /// <returns>APIResponse</returns>
        APIResponse UpdateStagingData(string additions, string removals, string providerId);


        /// <summary>
        /// Check for permission
        /// </summary>
        /// <param name="permission"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        void CheckForPermission(Permission permission);

    }
}
