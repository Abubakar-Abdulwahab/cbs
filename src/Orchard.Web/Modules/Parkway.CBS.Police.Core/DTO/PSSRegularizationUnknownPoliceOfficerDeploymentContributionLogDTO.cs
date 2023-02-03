using System;

namespace Parkway.CBS.Police.Core.DTO
{
    public class PSSRegularizationUnknownPoliceOfficerDeploymentContributionLogDTO
    {
        public Int64 Id { get; set; }

        public int NumberOfDays { get; set; }

        public int NumberOfOfficers { get; set; }

        public decimal DeploymentRate { get; set; }

        public decimal DeploymentAllowanceAmount { get; set; }

        public decimal DeploymentAllowancePercentage { get; set; }

        public int CommandTypeId { get; set; }

        public int DayTypeId { get; set; }
    }
}