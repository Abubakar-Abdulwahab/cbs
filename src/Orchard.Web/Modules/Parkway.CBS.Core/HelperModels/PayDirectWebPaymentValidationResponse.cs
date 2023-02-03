using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PayDirectWebPaymentValidationResponse
    {
        public bool PaymentWasProcessed { get; set; }

        public string CallBackURL { get; set; }

        public PaymentNotification PaymentNotification { get; set; }
    }
}