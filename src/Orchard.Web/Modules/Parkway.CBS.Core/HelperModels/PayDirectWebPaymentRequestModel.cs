using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PayDirectWebPaymentRequestModel
    {
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Callback URL is the URL the third party wants us to call when payment response from pay direct is sent
        /// </summary>
        public string CallBackURL { get; set; }

        /// <summary>
        /// DateTime format dd/MM/yyyy hh:mm:ss
        /// </summary>
        public string TimeStamp { get; set; }

        /// <summary>
        /// Client Id of the system send this request
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// hash value is the concat hash value of the InvoiceNumber and the TimeStamp
        /// </summary>
        public string HashValue { get; set; }

        /// <summary>
        /// Reference sent by third party to track payment requests
        /// </summary>
        public string RequestReference { get; set; }
    }

    public class RedirectToWebPayModel
    {
        public string Message { get; set; }

        public string Token { get; set; }

        public HeaderObj HeaderObj { get; set; }
    }
}