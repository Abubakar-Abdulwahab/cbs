using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class WebPaymentRequest : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        /// <summary>
        /// amount on the invoice
        /// </summary>
        public virtual decimal Amount { get; set; }

        public virtual string InvoiceNumber { get; set; }

        /// <summary>
        /// this is the transaction ref the application is sending to the payment processor
        /// </summary>
        public virtual string TransactionReference { get; set; }

        /// <summary>
        /// this is the call back URL for payment notification
        /// </summary>
        public virtual string CallBackURL { get; set; }

        /// <summary>
        /// the fee applied
        /// </summary>
        public virtual decimal FeeApplied { get; set; }

        /// <summary>
        /// this is the client Id of the expert system the request originates from
        /// </summary>
        public virtual string ClientId { get; set; }

        /// <summary>
        /// unique identifier sent by the payment request application to track payments
        /// </summary>
        public virtual string RequestIdentifier { get; set; }

        /// <summary>
        /// WebPaymentRequestSource
        /// </summary>
        public virtual WebPaymentRequestSource RequestSource { get; set; }

        /// <summary>
        /// PaymentChannel
        /// </summary>
        public virtual PaymentChannel WebPaymentChannel { get; set; }

        /// <summary>
        /// request dump
        /// </summary>
        public virtual string RequestDump { get; set; }

        public virtual string ResponseCode { get; set; }

        /// <summary>
        /// Pay Direct Web dump maps to PayDirectWebPaymentResponseModel
        /// </summary>
        public virtual string ResponseDump { get; set; }
    }
}