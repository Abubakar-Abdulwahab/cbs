using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PoliceofficerDeploymentAllowanceSettlementLog : CBSBaseModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Payment reference sent to settlement engine
        /// </summary>
        public virtual string ReferenceNumber { get; set; }

        /// <summary>
        /// Payment reference attached to the request to Bank3D
        /// </summary>
        public virtual string ItemReference { get; set; }

        /// <summary>
        /// <see cref="Enums.DeploymentAllowanceStatus"/>
        /// </summary>
        public virtual int TransactionStatus { get; set; }
    }
}