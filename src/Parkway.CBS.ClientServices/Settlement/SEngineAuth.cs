using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientServices.Settlement
{
    public class SEngineAuth
    {
        public string ClientCode { get; set; }

        /// <summary>
        /// HMAC 256 of client code, hash with secret
        /// </summary>
        public string hmac { get; set; }
    }


    public class AuthToken
    {
        public string token { get; set; }

        public string expiration { get; set; }
    }

    public class ComputeRuleRequestModel
    {
        public string RuleCode { get; set; }

        public decimal Amount { get; set; }

        public string Narration { get; set; }

        public string SettlementDate { get; set; }

        public int NumberOfTransactions { get; set; }

        public string ReferenceNumber { get; set; }
    }


    public class UniSettlementDetails
    {
        public decimal TotalAmount { get; set; }

        public int TransactionCount { get; set; }
    }

    public class SettlementEngineResponse
    {
        /// <summary>
        /// Does the response have any errors.
        /// <para>If the response has any error the bool value is true.</para>
        /// </summary>
        public bool Error { get; set; }
        /// <summary>
        /// Error code
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// Error ErrorMessage
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// </summary>
        public dynamic ResponseObject { get; set; }
    }


    public class SettlementRequestModel
    {
        [JsonProperty("BatchRef")]
        public string BatchReference { get; set; }

        [JsonProperty("PaymentType")]
        public string PaymentType { get; set; }

        [JsonProperty("DebitMode")]
        public string DebitMode { get; set; }

        [JsonProperty("FromAccountNumber")]
        public string FromAccountNumber { get; set; }

        [JsonProperty("FromBankCode")]
        public string FromBankCode { get; set; }

        [JsonProperty("Narration")]
        public string Narration { get; set; }

        [JsonProperty("CorporateID")]
        public Int64 CorporateId { get; set; }

        [JsonProperty("ProcessDate")]
        public string ProcessDate { get; set; }

        [JsonProperty("Items")]
        public IEnumerable<SettlementParticipantRequestModel> Parties { get; set; } = new List<SettlementParticipantRequestModel>();
    }


    public class SettlementParticipantRequestModel
    {
        [JsonProperty("BeneficiaryName")]
        public string ParticipantName { get; set; }

        [JsonProperty("BeneficiaryRef")]
        public string ParticipantReference { get; set; }

        [JsonProperty("ItemRef")]
        public string Reference { get; set; }

        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        [JsonProperty("Narration")]
        public string Narration { get; set; }

        [JsonProperty("AccountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("BankCode")]
        public string BankCode { get; set; }
    }
}
