using System;
using Parkway.CBS.Core.HelperModels;


namespace Parkway.CBS.Police.Core.VM
{
    public class PSSRequestDetailsVM
    {
        public TaxEntityViewModel TaxEntity { get; set; }

        public TaxEntityCategorySettings CategorySettings { get; set; }

        public string Comment { get; set; }

        public string FileRefNumber { get; set; }

        public int Status { get; set; }

        public int ServiceTypeId { get; set; }

        public Int64 RequestId { get; set; }

        public string ViewName { get; set; }

        public int ApprovalStatus { get; set; }

        public int ApproverId { get; set; }

        public string ServiceName { get; set; }

        public string Reason { get; set; }

        public string StateName { get; set; }

        public string LGAName { get; set; }

        public string CommandName { get; set; }

        public string CommandAddress { get; set; }

        public dynamic ServiceVM { get; set; }

        public bool DisplayDetailsForApproval { get; set; }

        public DateTime RequestDate { get; set; }

        public DateTime ApprovalDate { get; set; }

        public string ApprovalNumber { get; set; }

        public string ApprovalButtonName { get; set; }

        /// <summary>
        /// Tracks if the admin can invite an applicant for biometric capture
        /// </summary>
        public bool CanInviteApplicant { get; set; }

        public bool IsApplicantInvitedForCapture { get; set; }

        public string ApprovalPartialName { get; set; }

        public CBSUserVM CbsUser { get; set; }

        public string LocationName { get; set; }
    }
}