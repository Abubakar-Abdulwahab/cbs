using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class DirectAssessmentPayeeRecord : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string GrossAnnual { get; set; }

        public virtual string Exemptions { get; set; }

        public virtual string TIN { get; set; }

        public virtual string IncomeTaxPerMonth { get; set; }

        public virtual decimal IncomeTaxPerMonthValue { get; set; }

        public virtual DirectAssessmentBatchRecord DirectAssessmentBatchRecord { get; set; }

        public virtual string Month { get; set; }

        public virtual string Year { get; set; }

        public virtual string Email { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string PayeeName { get; set; }

        public virtual bool HasErrors { get; set; }

        public virtual string ErrorMessages { get; set; }

        public virtual string Address { get; set; }

        public virtual string LGA { get; set; }

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