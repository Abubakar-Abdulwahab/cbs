using Newtonsoft.Json;

namespace Parkway.CBS.Core.HelperModels
{
    public class PaymentNotification
    {
        public string InvoiceNumber { get; set; }

        public string PaymentRef { get; set; }

        /// <summary>
        /// dd/MM/yyyy HH:mm:ss
        /// </summary>
        public string PaymentDate { get; set; }

        public string BankCode { get; set; }

        public string BankName { get; set; }

        public string BankBranch { get; set; }

        public decimal AmountPaid { get; set; }

        /// <summary>
        /// dd/MM/yyyy HH:mm:ss
        /// </summary>
        public string TransactionDate { get; set; }

        public string TransactionRef { get; set; }

        /// <summary>
        /// describe the channel this payment notification is coming from 
        /// <see cref="Core.Models.Enums.PaymentChannel"/>
        /// </summary>
        public string Channel { get; set; }

        public string PaymentProvider { get; set; }

        public string Mac { get; set; }

        /// <summary>
        /// Payment notification response code
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// If payment notification notifies of unsuccessful payment, add message detailing the reason..
        /// </summary>
        public string ResponseMessage { get; set; }

        /// <summary>
        /// Request reference if any
        /// </summary>
        public string RequestReference { get; set; }

        /// <summary>
        /// State whether this notification is for a reversal
        /// </summary>
        public bool IsReversal { get; set; }

        /// <summary>
        /// Payment method
        /// </summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// For paye assessment, we want to know the month this paye payment was made
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// For paye assessment, we want to know the year this paye payment was made for
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Agency code. For paye payment, the agency code identifies the tax profile the payment is for
        /// </summary>
        public string AgencyCode { get; set; }

        /// <summary>
        /// Assessment type is used to indicate what type of paye assessment this is
        /// <see cref="Models.Enums.PayeAssessmentType"/>
        /// </summary>
        public int PayeAssessmentType { get; set; }

        [JsonIgnore]
        /// <summary>
        /// Dump of the request
        /// </summary>
        public string RequestDump { get; set; }

        [JsonProperty]
        /// <summary>
        /// Header signature
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// this value will hold the agent fee 
        /// that was deducted from the amount the user has paid
        /// </summary>
        public decimal AgentFee { get; set; }

    }
}