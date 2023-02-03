using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual GenerateRequestWithoutOfficersUploadBatchItemsStaging GenerateRequestWithoutOfficersUploadBatchItemsStaging { get; set; }

        /// <summary>
        /// Number of deployment days
        /// </summary>
        public virtual int NumberOfDays { get; set; }

        public virtual decimal DeploymentRate { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual decimal DeploymentAllowanceAmount { get; set; }

        public virtual decimal DeploymentAllowancePercentage { get; set; }

        public virtual bool IsActive { get; set; }
    }
}