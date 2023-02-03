using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System.Collections.Generic;

namespace Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl.Contracts
{
    public interface IBillingImpl : IDependency
    {

        BillingType BillingType { get; }


        /// <summary>
        /// For invoice generation get  the view model for the billing type
        /// </summary>
        /// <param name="revenueHeadDetails"></param>
        /// <param name="entity"></param>
        /// <param name="controls"></param>
        /// <param name="category"></param>
        /// <param name="processStage"></param>
        /// <param name="tenant"></param>
        /// <returns>TaxProfileFormVM</returns>
        TaxProfileFormVM GetModelForFormInput(RevenueHeadDetails revenueHeadDetails, IEnumerable<FormControl> controls, TaxEntityCategory category, GenerateInvoiceStepsModel processStage, TenantCBSSettings tenant);


        /// <summary>
        /// After a new user profile has been created or one has been retrieved from storage, lets determine the route to get this user to
        /// <para>For example, direct assessment with file upload should redirect the user to a page where the user can add payes/upload a file</para>
        /// </summary>
        /// <returns>ProfileRouteToVM</returns>
        RouteToVM ConfirmingInvoiceRoute();


        /// <summary>
        /// Model called when you want to confirm an invoice
        /// <para>Dynamic model includes ViewToShow and ViewModel properties</para>
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="user"></param>
        /// <returns>dynamic</returns>
        dynamic ConfirmingInvoice(GenerateInvoiceStepsModel processStage, UserDetailsModel user);


        /// <summary>
        /// Validate model for invoice confirmation.
        /// <para>Validate the confirm invoice model for the specified billing type. 
        /// If an error occurs please check the error count for errors</para>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <param name="errors"></param>
        /// <returns>void</returns>
        void ValidateConfirmingInvoice(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model, ref List<ErrorModel> errors);


        /// <summary>
        /// Get the view model for when there is an error when this invoice confirmation has errors
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns>ValidateInvoiceConfirmModel</returns>
        ValidateInvoiceConfirmModel ConfirmingInvoiceFailed(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model, UserDetailsModel user, List<ErrorModel> errors);


        /// <summary>
        /// Get model invoice confirmation
        /// </summary>
        /// <param name="model"></param>
        /// <returns>InvoiceConfirmedModel</returns>
        InvoiceConfirmedModel ConfirmedInvoice(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model);


        /// <summary>
        /// Create model for invoice generation
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="user"></param>
        /// <returns>CreateInvoiceModel</returns>
        CreateInvoiceModel GenerateInvoice(GenerateInvoiceStepsModel processStage, TaxEntity user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="processStage"></param>
        /// <param name="siteName"></param>
        /// <returns></returns>
        ViewToShowVM InvoiceInputPage(TaxEntity entity, GenerateInvoiceStepsModel processStage, string siteName = null);
    }
}
