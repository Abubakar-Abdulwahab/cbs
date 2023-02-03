using CBSPay.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Models
{
    public class AssessmentDetails
    {
        public virtual bool Success { get; set; }
        public virtual string Message { get; set; }
        public virtual AssessmentDetailsResult Result { get; set; }
    }

    public class AssessmentDetailsResult : BaseEntity<long>
    {
        public virtual long AssessmentID { get; set; }
        public virtual string AssessmentRefNo { get; set; }
        public virtual DateTime? AssessmentDate { get; set; }
        public virtual long TaxPayerTypeID { get; set; }
        public virtual string TaxPayerTypeName { get; set; }
        public virtual long TaxPayerID { get; set; }
        public virtual string TaxPayerName { get; set; }
        public virtual string TaxPayerRIN { get; set; }
        public virtual decimal AssessmentAmount { get; set; }
        public virtual decimal SetlementAmount { get; set; }
        public virtual long SettlementStatusID { get; set; }
        public virtual DateTime? SettlementDueDate { get; set; }
        public virtual string SettlementStatusName { get; set; }
        public virtual DateTime? SettlementDate { get; set; }
        public virtual string AssessmentNotes { get; set; }
        public virtual bool Active { get; set; }
        public virtual string ActiveText { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual IEnumerable<AssessmentRuleItem> AssessmentRuleItems { get; set; }
        public virtual IEnumerable<AssessmentRule> AssessmentRules { get; set; }

    }
}
