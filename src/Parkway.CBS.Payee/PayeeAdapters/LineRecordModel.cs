using System;

namespace Parkway.CBS.Payee.PayeeAdapters
{
    public abstract class LineRecordModel
    {
        public PayeeIntValue Month { get; internal set; }

        public PayeeIntValue Year { get; internal set; }

        public PayeeBreakDown PayeeBreakDown { get; set; }

        public bool HasError { get; internal set; }

        public string ErrorMessages { get; internal set; }

        public DateTime? AssessmentDate { get; set; }
    }
}
