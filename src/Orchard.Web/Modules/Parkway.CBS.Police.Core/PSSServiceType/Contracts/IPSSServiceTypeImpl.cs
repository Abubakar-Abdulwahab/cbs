using Orchard;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;


namespace Parkway.CBS.Police.Core.PSSServiceType.Contracts
{
    public interface IPSSServiceTypeImpl : IDependency
    {
        PSSServiceTypeDefinition GetServiceTypeDefinition { get; }

        /// <summary>
        /// This method gets the model for displaying the confirmation details of a request 
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="objStringValue"></param>
        /// <returns>RequestConfirmationVM</returns>
        RequestConfirmationVM GetModelForRequestConfirmation(int serviceId, string objStringValue);


        /// <summary>
        /// Save request details for this request
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="sRequestFormDump"></param>
        /// <param name="taxPayerProfileVM"></param>
        /// <returns>InvoiceGenerationResponse</returns>
        InvoiceGenerationResponse SaveRequestDetailsAfterConfirmation(PSSRequestStageModel processStage, string sRequestFormDump, TaxEntityViewModel taxPayerProfileVM);

    }
}
