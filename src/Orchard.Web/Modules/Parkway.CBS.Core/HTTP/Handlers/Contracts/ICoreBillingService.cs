using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Web.Http;
using System.Collections.Generic;
using Parkway.CBS.Core.Models.Enums;
using System;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Payee;
using Parkway.CBS.FileUpload;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreBillingService : IDependency
    {
        #region Create billing

        /// <summary>
        /// Create a new billing for a given revenue head
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <param name="user"></param>
        /// <param name="errors"></param>
        /// <param name="billingHelperModel"></param>
        /// <returns></returns>
        BillingCreatedModel TryPostBillingForCollection(MDA mda, RevenueHead revenueHead, UserPartRecord user, ref List<ErrorModel> errors, BillingHelperModel billingHelperModel, bool isEdit, string requestRef = null, ExpertSystemSettings expertSystem = null);

        #endregion


        /// <summary>
        /// Convert BillingModel to BillingHelperModel
        /// </summary>
        /// <param name="billing"></param>
        /// <returns>BillingHelperModel</returns>
        /// <exception cref="HasNoBillingException"></exception>
        BillingHelperModel ConvertBillingToHelperModel(BillingModel billing);


        /// <summary>
        /// Get revenue head with the given details
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="revenueHeadSlug"></param>
        /// <returns>RevenueHead</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        RevenueHead GetRevenueHead(int revenueHeadId, string revenueHeadSlug);

        /// <summary>
        /// Checks if biling is allowed.
        /// <para>Checks if the revenue head & mda are active</para>
        /// </summary>
        /// <param name="billing"></param>
        /// <param name="revenueHead"></param>
        /// <param name="mda"></param>
        /// <returns>bool</returns>
        bool IsBillingAllowed(MDA mda, RevenueHead revenueHead);

        DurationModel GetDuration(BillingModel billing);

        /// <summary>
        /// Has sub revenue heads
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <returns>bool</returns>
        bool HasSubRevenueHeads(RevenueHead revenueHead);

        /// <summary>
        /// Has billing 
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <returns>bool</returns>
        bool HasBilling(RevenueHead revenueHead);

        /// <summary>
        /// Get Billing type
        /// </summary>
        /// <param name="billingType"></param>
        /// <returns>BillingType</returns>
        /// <exception cref="Exceptions.NoBillingTypeSpecifiedException">If billing type is not found</exception>
        BillingType GetBillingType(int billingType);

        /// <summary>
        /// Get due date model for this billing
        /// </summary>
        /// <param name="billing">BillingModel</param>
        /// <returns>DueDateModel</returns>
        DueDateModel GetDueDateModel(BillingModel billing);

        /// <summary>
        /// Get due date
        /// </summary>
        /// <param name="dueDateModel">DueDateModel</param>
        /// <param name="startTime">DateTime</param>
        /// <returns>DateTime</returns>
        /// <exception cref="NoDueDateTypeFoundException"></exception>
        DateTime GetDueDate(DueDateModel dueDateModel, DateTime startTime);


        #region Billing utils

        /// <summary>
        /// Get the amount for this billing
        /// </summary>
        /// <param name="billing"></param>
        /// <returns></returns>
        decimal GetBillingAmount(BillingModel billing, decimal amount);


        /// <summary>
        /// Footnotes in our case are a concat of discount and penalties that are to be applied
        /// </summary>
        /// <param name="billing"></param>
        /// <returns>string</returns>
        string GetFootNotes(BillingModel billing);


        /// <summary>
        /// Return the discount to be applied to a billing
        /// </summary>
        /// <param name="billing"></param>
        /// <param name="invoiceGenerationDate"></param>
        /// <returns>DiscountModel | null is no applicable discount is found</returns>
        DiscountModel GetApplicableDiscount(BillingModel billing, DateTime date);


        /// <summary>
        /// Get due date for this billing
        /// </summary>
        /// <param name="billing"></param>
        /// <param name="invoiceDate"></param>
        /// <param name="nextBillingDate"></param>
        /// <returns>DateTime</returns>
        DateTime GetDueDate(BillingModel billing, DateTime invoiceDate, DateTime nextBillingDate);


        /// <summary>
        /// Get a list of file upload templates
        /// </summary>
        /// <returns>List{Template}</returns>
        List<Template> GetListOfFileUploadTemplates();


        /// <summary>
        /// Get the Next billing date. Next date for the billing cycle
        /// </summary>
        /// <param name="billing"></param>
        /// <param name="billingType"></param>
        /// <returns>DateTime</returns>
        DateTime GetNextBillingDate(BillingModel billing, BillingType billingType, DateTime invoiceDate);


        List<AssessmentInterface> GetAssessmentAdapters();

        #endregion
    }
}
