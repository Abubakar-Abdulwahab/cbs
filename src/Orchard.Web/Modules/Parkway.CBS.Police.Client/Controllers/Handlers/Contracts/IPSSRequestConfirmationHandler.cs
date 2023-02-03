using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IPSSRequestConfirmationHandler : IDependency
    {

        /// <summary>
        /// Get VM confirmation for service type
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="sRequestFormDump"></param>
        /// <returns>RequestConfirmationVM</returns>
        RequestConfirmationVM GetVMForRequestConfirmationPage(int serviceId, int serviceTypeId, string sRequestFormDump);


        /// <summary>
        /// Save details for this request
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="sRequestFormDump"></param>
        InvoiceGenerationResponse SaveRequestDetails(PSSRequestStageModel processStage, string sRequestFormDump, TaxEntityViewModel taxPayerProfileVM);


        /// <summary>
        /// Gets branch location for cbs user with specified id
        /// </summary>
        /// <param name="cbsUserId"></param>
        /// <returns></returns>
        TaxEntityProfileLocationVM GetCBSUserLocation(long cbsUserId);
    }
}
