using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Helpers
{
    public class EIRSTaskConfigValues
    {
        public string EIRSBaseUrl { get; set; }
        public string LoginUrl { get; set; }
        public string EIRSAPILoginUsername { get; set; }
        public string EIRSAPILoginPassword { get; set; }
        public string EIRSAPILoginGrantType { get; set; }
        public string EIRSAPILoginContentType { get; set; }
        public string EIRSAPILoginAccept { get; set; }
        public string EIRSInsertPayOnAccountUrl { get; set; }
        public string EIRSAddSettlementUrl { get; set; }
        public string GetServiceBillItemsUrl { get; set; }
        public string GetAssessmentRuleItemsUrl { get; set; }
        public string GetServiceBillDetailsUrl { get; set; }
        public string GetAssessmentDetailUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string UnsyncedPaymentUrl { get; set; }
        public string AppBaseUrl { get; set; }
        public string UpdatePaymentUrl { get; set; }

    }
}
