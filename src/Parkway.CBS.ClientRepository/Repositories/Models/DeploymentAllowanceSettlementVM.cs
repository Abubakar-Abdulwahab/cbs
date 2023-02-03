using System;

namespace Parkway.CBS.ClientRepository.Repositories.Models
{
    public class DeploymentAllowanceSettlementVM
    {
        public long SettlementAllowanceRequestId { get; set; }

        public string SettlementEngineResponseJSON { get; set; }

        public string SettlementEngineRequestJSON { get; set; }

        public bool Error { get; set; }

        public int RetryCount { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime TimeFired { get; set; }
    }
}
