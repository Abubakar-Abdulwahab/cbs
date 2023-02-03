using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class AccountPaymentRequestApprovalLog : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual AccountPaymentRequest PaymentRequest { get; set; }

        /// <summary>
        /// <see cref="Enums.PaymentRequestStatus"/>
        /// </summary>
        public virtual int Status { get; set; }

        /// <summary>
        /// this indicates what stage this request was made for
        /// </summary>
        public virtual PSServiceRequestFlowDefinitionLevel FlowDefinitionLevel { get; set; }

        public virtual UserPartRecord AddedByAdminUser { get; set; }

        public virtual string Comment { get; set; }

    }
}