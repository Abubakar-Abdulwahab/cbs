using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CollectionSearchParams
    {
        public DateTime? FromRange { get; set; }

        public DateTime? EndRange { get; set; }

        public string PaymentRef { get; set; }

        public int RevenueHeadId { get; set; }

        public string SRevenueHeadId { get; set; }

        public string SelectedMDA { get; set; }

        /// <summary>
        /// This is the selected payment Channel
        /// </summary>
        //public PaymentChannelp SelectedPaymentMethod { get; set; }

            
        public string SelectedPaymentProvider { get; set; }

        public int PaymentProviderId { get; set; }

        public string InvoiceNumber { get; set; }

        public string ReceiptNumber { get; set; }

        public string SelectedBankCode { get; set; }

        public CollectionPaymentDirection PaymentDirection { get; set; }

        public long TaxEntityId { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public int AdminUserId { get; set; }

        public int MDAId { get; set; }

        public bool DontPageData { get; set; }

        /// <summary>
        /// Selected payment channel
        /// </summary>
        public PaymentChannel SelectedPaymentChannel { get; set; }
    }
}