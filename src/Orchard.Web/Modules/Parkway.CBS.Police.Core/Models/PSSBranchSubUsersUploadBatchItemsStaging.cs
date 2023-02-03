using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSBranchSubUsersUploadBatchItemsStaging : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSBranchSubUsersUploadBatchStaging PSSBranchSubUsersUploadBatchStaging { get; set; }

        public virtual LGA BranchLGA { get; set; }

        public virtual StateModel BranchState { get; set; }

        public virtual string BranchStateCode { get; set; }

        public virtual string BranchLGACode { get; set; }

        public virtual string BranchStateValue { get; set; }

        public virtual string BranchLGAValue { get; set; }

        public virtual string BranchAddress { get; set; }

        public virtual string BranchName { get; set; }

        /// <summary>
        /// TaxEntityProfileLocation Id for created branches
        /// </summary>
        public virtual TaxEntityProfileLocation TaxEntityProfileLocation { get; set; }

        public virtual string SubUserName { get; set; }

        public virtual string SubUserEmail { get; set; }

        public virtual string SubUserPhoneNumber { get; set; }

        /// <summary>
        /// User part record id for created sub users
        /// </summary>
        public virtual UserPartRecord User { get; set; }

        public virtual bool HasError { get; set; }

        public virtual string ErrorMessage { get; set; }
    }
}