using System;

namespace PayeeProcessor.DAL.Model
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

        public virtual string Address { get; set; }

        public virtual string LGA { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string PayeeName { get; set; }

        public virtual bool HasErrors { get; set; }

        public virtual string ErrorMessages { get; set; }
    }


    public class Parkway_CBS_Core_DirectAssessmentPayeeRecord : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string GrossAnnual { get; set; }

        public virtual string Exemptions { get; set; }

        public virtual string TIN { get; set; }

        public virtual string IncomeTaxPerMonth { get; set; }

        public virtual decimal IncomeTaxPerMonthValue { get; set; }

        public virtual Int64 DirectAssessmentBatchRecord_Id { get; set; }

        public virtual string Month { get; set; }

        public virtual string Year { get; set; }

        public virtual string Email { get; set; }

        public virtual string Address { get; set; }

        public virtual string LGA { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string PayeeName { get; set; }

        public virtual bool HasErrors { get; set; }

        public virtual string ErrorMessages { get; set; }
    }

}
