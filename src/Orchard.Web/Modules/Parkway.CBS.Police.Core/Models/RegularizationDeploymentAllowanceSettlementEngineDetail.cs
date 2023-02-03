using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class RegularizationDeploymentAllowanceSettlementEngineDetail : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual string PaymentReference { get; set; }

        public virtual string SettlementEngineResponseJSON { get; set; }

        public virtual string SettlementEngineRequestJSON { get; set; }
    }
}