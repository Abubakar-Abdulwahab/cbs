using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class DeploymentAllowanceSettlementVM
    {
        public string RuleCode { get; set; }

        public string ReferenceNumber { get; set; }

        public string Narration { get; set; }

        public string SettlementDate { get; set; }

        public string PaymentType { get; set; }

        public string CallbackURL { get; set; }

        public List<DeploymentAllowanceSettlementItemsVM> Items { get; set; } = new List<DeploymentAllowanceSettlementItemsVM>();
    }
}