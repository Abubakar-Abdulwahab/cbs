using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Models
{
    public class PAYEBatchItemsStaging : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string GrossAnnual { get; set; }

        public virtual decimal GrossAnnualValue { get; set; }

        public virtual string Exemptions { get; set; }

        public virtual decimal ExemptionsValue { get; set; }

        public virtual string PayerId { get; set; }

        public virtual string IncomeTaxPerMonth { get; set; }

        public virtual decimal IncomeTaxPerMonthValue { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual PAYEBatchRecordStaging PAYEBatchRecordStaging { get; set; }

        /// <summary>
        /// Unique identifier for each batch record items
        /// </summary>
        public virtual int SerialNumber { get; set; }

        public virtual string Month { get; set; }

        public virtual int MonthValue { get; set; }

        public virtual string Year { get; set; }

        public virtual int YearValue { get; set; }

        public virtual bool HasErrors { get; set; }

        public virtual string ErrorMessages { get; set; }

        /// <summary>
        /// this date is used for query purposes
        /// </summary>
        public virtual DateTime? AssessmentDate { get; set; }

        public virtual string ReceiptNumber { get; set; }

        public virtual string EmployeeNumber { get; set; }

        public virtual string GradeLevel { get; set; }

        public virtual string Step { get; set; }
    }
}