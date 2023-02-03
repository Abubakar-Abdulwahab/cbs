using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.ClientServices.Invoicing.Contracts;
using Parkway.CBS.Entities.DTO;
using System;

namespace Parkway.CBS.ClientServices.Invoicing
{
    public class DirectAssessmentInvoiceGeneration : BaseInvoiceGeneration, IInvoiceGenerationType
    {
        public BillingType InvoiceGenerationType => BillingType.DirectAssessment;



        public CreateInvoiceHelper GetInvoiceHelperModel(RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails)
        {
            DateTime nextBillingDate = DateTime.Now.ToLocalTime();
            return GetInvoiceFillables(revenueHeadDetails, nextBillingDate);
        }


        /// <summary>
        /// Get value of fields for invoice generation
        /// </summary>
        /// <param name="helperModel"></param>
        /// <param name="nextBillingDate"></param>
        /// <param name="additionalDetails"></param>
        /// <param name="fileUploadModel"></param>
        /// <returns>CreateInvoiceHelper</returns>
        private CreateInvoiceHelper GetInvoiceFillables(RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails, DateTime nextBillingDate)
        {
            CreateInvoiceHelper model = new CreateInvoiceHelper { };
            ////we need an amount for that invoice
            //get due date
            //Logger.Information("Getting due date");
            model.DueDate = GetDueDate(revenueHeadDetails.JSONDueDate, revenueHeadDetails.InvoiceDate, nextBillingDate);
            //invoice description
            model.InvoiceDescription = revenueHeadDetails.InvoiceDescription;
            //get title
            model.Title = revenueHeadDetails.RevenueHeadNameAndCode;
            //get type
            model.Type = "Single";
            //get items
            //Logger.Information("setting invoice items");
            model.Items = GetListOfValidPayees(revenueHeadDetails.Amount, revenueHeadDetails.CashFlowProductId, revenueHeadDetails.InvoiceDescription);
            //get footnotes for discounts and penalties that are to be applied
            //Logger.Information("Getting footnotes");
            model.FootNotes = GetFootNotes(revenueHeadDetails.JSONBillingDiscounts, revenueHeadDetails.JSONBillingPenalties);
            //invoice date
            model.InvoiceDate = revenueHeadDetails.InvoiceDate;
            model.UniqueInvoiceIdentifier = DateTime.Now.ToLocalTime().Ticks.ToString() + Guid.NewGuid().ToString("N");
            //get additional info
            model.ExternalRef = revenueHeadDetails.ExternalRefNumber;
            return model;
        }        
    }
}