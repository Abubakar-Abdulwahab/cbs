using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSRegularizationUnknownPoliceOfficerDeploymentLog : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual GenerateRequestWithoutOfficersUploadBatchItemsStaging GenerateRequestWithoutOfficersUploadBatchItemsStaging { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime EndDate { get; set; }

        public virtual decimal DeploymentRate { get; set; }

        public virtual StateModel State { get; set; }

        public virtual LGA LGA { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual bool IsActive { get; set; }
    }
}