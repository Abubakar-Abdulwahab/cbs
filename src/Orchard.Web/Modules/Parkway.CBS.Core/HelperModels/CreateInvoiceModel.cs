using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using Parkway.CBS.Core.Models;
using Orchard.Users.Models;
using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Core.HelperModels
{
    public class CreateInvoiceModelForPayeAssessment
    {
        public RevenueHead RevenueHead { get; set; }

        public MDA MDA { get; set; }

        public TaxEntityCategory TaxEntityCategory { get; set; }

        public CBSUser UserProfile { get; set; }

        public TaxEntity TaxEntity { get; set; }

        public BillingModel Billing { get; set; }

        public DirectAssessmentBatchRecord DirectAssessmentBatchRecord { get; set; }

        public decimal Amount { get; set; }

        public List<AdditionalDetails> AdditionalDetails { get; set; }
    }


    public class CreateInvoiceModel
    {
        public HeaderObj HeaderObj { get; set; }

        public int RevenueHeadId { get; set; }

        public TaxEntityInvoice TaxEntityInvoice { get; set; }

        /// <summary>
        /// unix time stamp
        /// </summary>
        public long TimeStamp { get; set; }

        /// <summary>
        /// Attach a batch token if direct assessment with file upload
        /// </summary>
        public FileProcessModel FileUploadModel { get; set; }

        public bool UserIsAuthorized { get; set; }

        public string ExternalRefNumber { get; set; }

        public bool GeneratedFromAdmin { get; set; }

        public UserPartRecord AdminUser { get; set; }

        [StringLength(100, ErrorMessage = "The Request Reference can only be 100 characters long or 1 character short", MinimumLength = 1)]
        public string RequestReference { get; set; }

        public string CallBackURL { get; set; }

        public ExternalRedirect ExternalRedirect { get; set; }

        public int Quantity { get; set; }

        public decimal VAT { get; set; }

        public bool ApplySurcharge { get; set; }

        public List<UserFormDetails> Forms { get; set; }

        public string MDAName { get; set; }

        public string RevenueHeadName { get; set; }

        public decimal Surcharge { get; set; }
    }
}