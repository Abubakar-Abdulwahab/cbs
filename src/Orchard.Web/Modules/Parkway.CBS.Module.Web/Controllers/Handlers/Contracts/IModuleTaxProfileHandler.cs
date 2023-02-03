using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System.Collections.Generic;

namespace Parkway.CBS.Module.Web.Controllers.Handlers.Contracts
{
    public interface ITaxProfileHandler : IDependency, ICommonBaseHandler
    {

        ProceedWithInvoiceGenerationVM TrySaveTaxEntityProfile<C>(C callback, int revenueHeadId, int categoryId, TaxEntityViewModel model, TaxProfileFormVM persistedModel, ICollection<CollectionFormVM> additionalFormFields) where C : BaseTaxProfileController;

        /// <summary>
        /// Get view model for payer profile view
        /// </summary>
        /// <param name="processStage">GenerateInvoiceStepsModel</param>
        /// <returns>TaxProfileFormVM</returns>
        TaxProfileFormVM GetTaxPayerProfileVM(GenerateInvoiceStepsModel processStage);


        /// <summary>
        /// Get the route details for this billing type
        /// </summary>
        /// <param name="billingType"></param>
        /// <returns>ProfileRouteToVM</returns>
        /// <exception cref="NoBillingTypeSpecifiedException">No billing type found</exception>
        RouteToVM GetPageToRouteTo(BillingType billingType);
    }
}
