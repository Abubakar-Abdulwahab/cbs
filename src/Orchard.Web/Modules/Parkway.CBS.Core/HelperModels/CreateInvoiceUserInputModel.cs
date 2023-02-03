using Newtonsoft.Json;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Core.HelperModels
{
    public class CreateInvoiceUserInputModel
    {
        [Required(ErrorMessage = "InvoiceTitle field is required")]
        [StringLength(250, ErrorMessage = "The InvoiceTitle value can be 250 characters long or 2 characters short", MinimumLength = 2)]
        public string InvoiceTitle { get; set; }

        [Required(ErrorMessage = "InvoiceDescription field is required")]
        [StringLength(500, ErrorMessage = "The InvoiceDescription value can be 500 characters long or 2 characters short", MinimumLength = 2)]
        public string InvoiceDescription { get; set; }


        /// <summary>
        /// Revenue head group identifier
        /// <para>For multiple revenue heads, this field is mandatory</para>
        /// </summary>
        public int GroupId { get; set; }

        [Required(ErrorMessage = "TaxEntity field is required")]
        public TaxEntity TaxEntity { get; set; }


        [Required(ErrorMessage = "RevenueHeadModels field is required")]
        public List<RevenueHeadUserInputModel> RevenueHeadModels { get; set; }


        /// <summary>
        /// unix time stamp
        /// </summary>
        public long TimeStamp { get; set; }

        [Required(ErrorMessage = "RequestReference field is required")]
        [StringLength(100, ErrorMessage = "The RequestReference value can be 100 characters long or 5 characters short", MinimumLength = 5)]
        public string RequestReference { get; set; }

        [StringLength(200, ErrorMessage = "The CallBackURL value can only be 200 characters long")]
        public string CallBackURL { get; set; }

        [StringLength(200, ErrorMessage = "The ExternalRefNumber value can be 200 characters long")]
        public string ExternalRefNumber { get; set; }


        public int TaxEntityCategoryId { get; set; }

        /// <summary>
        /// Value Added Tax
        /// </summary>
        public decimal VAT { get; set; }


        /// <summary>
        /// This indicated that the form values has been validated
        /// </summary>
        [JsonIgnore]
        public bool DontValidateFormControls { get; set; }

        public bool AddSurcharge { get; set; }
    }
}