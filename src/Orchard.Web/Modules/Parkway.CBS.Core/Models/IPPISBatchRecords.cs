using System;

namespace Parkway.CBS.Core.Models
{
    public class IPPISBatchRecords : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual IPPISBatch IPPISBatch { get; set; }

        public virtual string MinistryName { get; set; }

        public virtual string TaxPayerCode { get; set; }

        public virtual string EmployeeNumber { get; set; }

        public virtual string PayeeName { get; set; }

        public virtual string GradeLevel { get; set; }

        public virtual string Step { get; set; }

        public virtual string Address { get; set; }

        public virtual string Email { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string TaxStringValue { get; set; }

        public virtual decimal Tax { get; set; }

        public virtual bool HasErrors { get; set; }

        public virtual string ErrorMessages { get; set; }
    }
}