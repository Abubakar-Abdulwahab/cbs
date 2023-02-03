using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSBranchOfficersUploadBatchItemsStaging : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSBranchOfficersUploadBatchStaging PSSBranchOfficersUploadBatchStaging { get; set; }

        public virtual string BranchCode { get; set; }

        public virtual string BranchCodeValue { get; set; }

        public virtual string APNumber { get; set; }

        public virtual string OfficerName { get; set; }

        public virtual Command OfficerCommand { get; set; }

        public virtual string OfficerCommandValue { get; set; }
        
        public virtual string OfficerCommandCode { get; set; }
        
        public virtual string RankCode { get; set; }
        
        public virtual string RankName { get; set; }
        
        public virtual PoliceRanking Rank { get; set; }

        public virtual string BankCode { get; set; }

        public virtual string BankName { get; set; }

        public virtual string IPPISNumber { get; set; }

        public virtual string Gender { get; set; }

        public virtual string AccountNumber { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual bool HasError { get; set; }
        
        public virtual string ErrorMessage { get; set; }
    }
}