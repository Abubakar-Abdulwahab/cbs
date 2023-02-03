using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class AccountPaymentRequestItem : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual string AccountName { get; set; }

        public virtual string AccountNumber { get; set; }

        public virtual AccountPaymentRequest AccountPaymentRequest { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual Bank Bank { get; set; }

        public virtual string BeneficiaryName { get; set; }

        public virtual PSSExpenditureHead PSSExpenditureHead { get; set; }

        /// <summary>
        /// <see cref="Enums.PaymentRequestStatus"/>
        /// </summary>
        public virtual int TransactionStatus { get; set; }

        public virtual string PaymentReference { get; set; }

    }
}