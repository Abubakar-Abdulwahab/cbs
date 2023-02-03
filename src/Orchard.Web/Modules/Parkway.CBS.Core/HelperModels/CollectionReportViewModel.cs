using Newtonsoft.Json;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CollectionReportViewModel
    {
        /// <summary>
        /// this value holds the encrypted object value
        /// </summary>
        public string Token { get; set; }

        public dynamic Pager { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "Enter a valid date range. Date format dd/MM/yyyy", MinimumLength = 10)]
        /// <summary>
        /// Format dd/MM/yyyy
        /// </summary>
        public string FromRange { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "Enter a valid date range. Date format dd/MM/yyyy", MinimumLength = 10)]
        /// <summary>
        /// Format dd/MM/yyyy
        /// </summary>
        public string EndRange { get; set; }

        public string MDAName { get; set; }

        public string RevenueHeadName { get; set; }

        public string SelectedMDA { get; set; }

        [JsonProperty("RevenueHeadId")]
        public string SelectedRevenueHead { get; set; }

        public IEnumerable<MDAVM> Mdas { get; set; }

        public IEnumerable<RevenueHeadDropDownListViewModel> RevenueHeads { get; set; }

        public IEnumerable<CollectionDetailReport> ReportRecords { get; set; }

        public int ReportCount { get; set; }

        public string TaxPayerName { get; set; }

        public string PaymentRef { get; set; }

        public string SelectedPaymentProvider { get; set; }

        public string SelectedPaymentChannel { get; set; }

        public string InvoiceNumber { get; set; }

        public string ReceiptNumber { get; set; }

        public string TaxPayerTIN { get; set; }

        //[JsonIgnore]
        public int TotalNumberOfPayment { get; set; }

        public decimal TotalAmountPaid { get; set; }

        public int MDAId { get; set; }

        //[Required(ErrorMessage = "Authorizing user email is required")]
        //[EmailAddress(ErrorMessage = "Add a valid Email address")]
        public string UserEmail { get; set; }

        [JsonProperty("BankCode")]
        public string SelectedBank { get; set; }

        public List<BankVM> Banks { get; set; }

        public CollectionPaymentDirection PaymentDirection { get; set; }

        public string LogoURL { get; set; }


        public string TenantName { get; set; }


        /// <summary>
        /// Used by api to determine the order for search
        /// <see cref="CollectionPaymentDirection"/>
        /// </summary>
        public int OrderBy { get; set; }


        /// <summary>
        /// Used by API to hold the selected payment provider
        /// </summary>
        public int PaymentProvider { get; set; }


        /// <summary>
        /// User Id of the party requesting this view
        /// Used by API for holding the user Id, that is authorized for to view this model
        /// </summary>
        public int UserId { get; set; }

        public string PayerId { get; set; }

        public List<PaymentProviderVM> PaymentProviders { get; set; }

    }

}