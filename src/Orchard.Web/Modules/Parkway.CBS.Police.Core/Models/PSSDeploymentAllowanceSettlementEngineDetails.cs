using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSDeploymentAllowanceSettlementEngineDetails : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PoliceofficerDeploymentAllowance PoliceofficerDeploymentAllowance { get; set; }

        public virtual int RetryCount { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual DateTime TimeFired { get; set; }

        public virtual bool Error { get; set; }

        public virtual string SettlementEngineResponseJSON { get; set; }

        public virtual string SettlementEngineRequestJSON { get; set; }

        public virtual string RequestReference { get; set; }

        public virtual string ErrorMessage { get; set; }
    }
}