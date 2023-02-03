using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Models
{
    public class AccountPaymentRequest : CBSBaseModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Source Account Number
        /// </summary>
        public virtual string AccountNumber { get; set; }

        /// <summary>
        /// Source Account Name
        /// </summary>
        public virtual string AccountName { get; set; }

        /// <summary>
        /// Source Account Bank 
        /// </summary>
        public virtual Bank Bank { get; set; }

        public virtual AccountWalletConfiguration AccountWalletConfiguration { get; set; }

        /// <summary>
        /// <see cref="Enums.PaymentRequestStatus"/>
        /// </summary>
        public virtual int PaymentRequestStatus { get; set; }

        public virtual PSServiceRequestFlowDefinitionLevel FlowDefinitionLevel { get; set; }

        public virtual UserPartRecord InitiatedBy { get; set; }

        public virtual IEnumerable<AccountPaymentRequestItem> AccountPaymentRequestItems { get; set; }

        public virtual bool IsDeleted { get; set; }

        /// <summary>
        /// Payment identifier for each batch of request items
        /// Automatically generated (READONLY)
        /// </summary>
        public virtual string PaymentReference { get; set; }

    }
}