using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.HelperModels
{
    public class InvoiceGenerationDetailsModel
    {
        public string CategoryName { get; set; }

        public string RevenueHeadName { get; set; }

        public string MDAName { get; set; }

        public decimal Amount { get; set; }

        public decimal Surcharge { get; set; }

        public string ErrorMessage { get; set; }

        public bool HasErrors { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public BillingType BillingType { get; set; }
    }

    public class GenerateInvoiceStepsModel
    {
        /// <summary>
        /// this object would have the details of the PayerProfile stage of invoice generation
        /// </summary>
        public TaxProfileFormVM PayerProfileStageObj { get; set; }

        /// <summary>
        /// this object would contain details you might need to move from profile generation to comfirm invoice page
        /// or paye assessment page as the case might be
        /// </summary>
        public ProceedWithInvoiceGenerationVM ProceedWithInvoiceGenerationVM { get; set; }

        /// <summary>
        /// this object contains all you would need for redirect to report page
        /// </summary>
        public ProcessingReportVM ProcessingDirectAssessmentReportVM { get; set; }

        /// <summary>
        /// this object would contain details you would need when the invoice has been confirmed
        /// <para>For example when a self assessment invoice has been confirmed the amount field would contain the amount the customer wants to pay</para>
        /// </summary>
        public InvoiceConfirmedModel InvoiceConfirmedModel { get; set; }

        /// <summary>
        /// set this flag to true if the page you hit has a go back function
        /// </summary>
        public bool CanGoBack { get; set; }

        public int CategoryId { get; set; }

        public int RevenueHeadId { get; set; }

        public int AgencyId { get; set; }

        public InvoiceGenerationStage InvoiceGenerationStage { get; set; }

        public string ExternalRef { get; set; }

        public BillingType BillingType { get; set; }

        public List<UserFormDetails> UserFormDetails { get; set; }

        /// <summary>
        /// this indicates that the forms fields have been validated
        /// </summary>
        public bool ValidatedFormFields { get; set; }

        public int SelectedState { get; set; }

        public int SelectedStateLGA { get; set; }

        public int SelectedYear { get; set; }
    }


    


    public enum InvoiceGenerationStage
    {
        SelectIdentity,
        PayerProfile,
        InvoiceGenerated,
        
        InvoiceProceed,
        ShowInvoiceConfirm,
        GenerateInvoice,
        ExternalRedirect,
        PAYEProcess,
        PAYEProcessNoScheduleUpload,
        SelectPAYEOption
    }    
}