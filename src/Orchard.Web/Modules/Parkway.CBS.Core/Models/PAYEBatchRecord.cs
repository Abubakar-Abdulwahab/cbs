using System;
using System.Collections.Generic;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Models
{
    public class PAYEBatchRecord : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual BillingModel Billing { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual ICollection<PAYEBatchItems> Payees { get; set; }

        public virtual string FilePath { get; set; }

        /// <summary>
        /// For only reference/audit purposes. To get the latest use the billing model.
        /// </summary>
        public virtual string AdapterValue { get; set; }

        public virtual string BatchRef { get; set; }

        /// <summary>
        /// Authorized user who generate the invoice
        /// </summary>
        public virtual CBSUser CBSUser { get; set; }

        public virtual string FileName { get; set; }

        /// <summary>
        /// Direct assessment type 
        /// <see cref="PayeAssessmentType"/>
        /// </summary>
        public virtual int AssessmentType { get; set; }

        public virtual Int64 OriginIdentifier { get; set; }

        public virtual string DuplicateComposite { get; set; }

        public virtual string TaxPayerCode { get; set; }

        public virtual decimal RevenueHeadSurCharge { get; set; }

        /// <summary>
        /// Once the payment for this record has been completed
        /// this flag is set to true
        /// </summary>
        public virtual bool PaymentCompleted { get; set; }

        /// <summary>
        /// indicate whether the PAYE batch record is active
        /// </summary>
        public virtual bool IsActive { get; set; }


        /// <summary>
        /// Short code for Payment type description
        /// <see cref="PayeAssessmentType"/>
        /// </summary>
        public virtual string PaymentTypeCode
        {
            get { return ((PayeAssessmentType)this.AssessmentType).ToDescription(); }
            set { value = ((PayeAssessmentType)this.AssessmentType).ToDescription(); }
        }

    }
}