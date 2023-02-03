using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class ProposedEscortOffficerVM
    {
        public int OfficerId { get; set; }

        public long OfficerRankId { get; set; }

        public int OfficerCommandId { get; set; }

        public long PoliceOfficerLogId { get; set; }

        public string OfficerName { get; set; }

        public string OfficerRankName { get; set; }

        public string OfficerRankCode { get; set; }

        public decimal DeploymentRate { get; set; }

        public string OfficerCommandName { get; set; }

        public string OfficerCommandAddress { get; set; }

        public string OfficerCommandStateName { get; set; }

        public string OfficerCommandLGAName { get; set; }

        public string OfficerIdentificationNumber { get; set; }

        public string OfficerIPPISNumber { get; set; }

        public string OfficerAccountNumber { get; set; }

        public decimal EscortRankRate { get; set; }

        public string DateCreated { get; set; }
    }
}