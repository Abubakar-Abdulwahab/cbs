using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class AccountPaymentRequestSettlementLog : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual string PaymentReference { get; set; }

        public virtual string Reference { get; set; }

        /// <summary>
        /// <see cref="Enums.PaymentRequestStatus"/>
        /// </summary>
        public virtual int TransactionStatus { get; set; }
    }
}