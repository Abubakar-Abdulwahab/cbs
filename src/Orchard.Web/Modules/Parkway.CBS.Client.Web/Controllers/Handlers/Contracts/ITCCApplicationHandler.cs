using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.StateConfig;
using System.Web;
using System.Collections.Generic;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface ITCCApplicationHandler : IDependency
    {
        /// <summary>
        /// Validate the existence of a particular stateTIN (payerid)
        /// </summary>
        /// <param name="stateTIN"></param>
        /// <returns>TaxPayerWithDetails</returns>
        TaxPayerWithDetails ValidateStateTIN(string stateTIN);

        /// <summary>
        /// Validate the development levy invoice using the invoice number and development revenue head id 
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns>long</returns>
        long ValidateDevelopmentLevyInvoice(string invoiceNumber, int developmentRevenueHeadId);

        /// <summary>
        /// Validate if the development levy invoice has not been used
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>bool</returns>
        bool CheckDevelopmentLevyInvoiceUsage(string invoiceNumber);

        /// <summary>
        /// Save new tcc application request
        /// </summary>
        /// <param name="tccApplicationRequest"></param>
        /// <param name="accountStatement"></param>
        /// <param name="exemptionCertificate"></param>
        /// <param name="schoolCertificate"></param>
        /// <param name="stateConfig"></param>
        void SaveTCCRequest(TCCApplicationRequestVM tccApplicationRequest, HttpPostedFileBase accountStatement, HttpPostedFileBase exemptionCertificate, HttpPostedFileBase schoolCertificate, StateConfig stateConfig);

        /// <summary>
        /// Validate if user upload all the necessary documents during tcc request application
        /// </summary>
        /// <param name="accountStatement"></param>
        /// <param name="exemptionCertificate"></param>
        /// <param name="schoolCertificate"></param>
        /// <param name="exemptionTypeId"></param>
        /// <param name="validationErrors"></param>
        void DoFileUploadValidation(HttpPostedFileBase accountStatement, HttpPostedFileBase exemptionCertificate, HttpPostedFileBase schoolCertificate, int exemptionTypeId, List<ErrorModel> validationErrors);

        /// <summary>
        /// Check if development levy invoice with specified invoice number has been used.
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="developmentLevyRevenueHeadId"></param>
        /// <returns></returns>
        APIResponse CheckIfDevelopmentLevyInvoiceHasBeenUsed(string invoiceNumber, int developmentLevyRevenueHeadId);

        /// Checks if specified payer id is different from that of the logged in user with the specified tax entity id.
        /// </summary>
        /// <param name="payerId"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        APIResponse CheckIfPayerIdValid(string payerId, long taxEntityId);


        /// <summary>
        /// Do validation for form fields
        /// <para>validation errors fields will contain error models if any error occurs</para>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="validationErrors"></param>
        void DoValidationForFormFields(TCCApplicationRequestVM model, List<ErrorModel> validationErrors);

    }
}
