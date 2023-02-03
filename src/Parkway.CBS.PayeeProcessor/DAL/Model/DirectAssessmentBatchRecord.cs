using System;
using System.Collections.Generic;

namespace Parkway.CBS.PayeeProcessor.DAL.Model
{
    public class DirectAssessmentBatchRecord : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string TIN { get; set; }

        public virtual ICollection<DirectAssessmentPayeeRecord> Payees { get; set; }

        public virtual string Rules { get; set; }

        public virtual string FilePath { get; set; }

        public virtual string AdapterValue { get; set; }

        public virtual string BatchRef { get; set; }

        public virtual decimal Amount { get; set; }
    }

    public class Parkway_CBS_Core_DirectAssessmentBatchRecord : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }
        public virtual string RulesApplied { get; set; }
        public virtual string FilePath { get; set; }
        public virtual string AdapterValue { get; set; }
        public virtual string BatchRef { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual decimal PercentageProgress { get; set; }
        public virtual Int32 TotalNoOfRowsProcessed { get; set; }
        public virtual bool ErrorOccurred { get; set; }
        public virtual string ErrorMessage { get; set; }
        public virtual string Origin { get; set; }
        public virtual string FileName { get; set; }
    }
}
