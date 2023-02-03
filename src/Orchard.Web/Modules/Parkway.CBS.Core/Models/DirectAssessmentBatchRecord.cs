using System;
using System.Collections.Generic;
using Parkway.CBS.Core.Models.Enums;


namespace Parkway.CBS.Core.Models
{
    /// <summary>
    /// Use sees for raw sql query ref
    /// <see cref="Parkway.CBS.ClientRepository.Repositories.DirectAssessmentBatchRecordDAOManager"/>
    /// </summary>
    public class DirectAssessmentBatchRecord : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual BillingModel Billing { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual ICollection<DirectAssessmentPayeeRecord> Payees { get; set; }

        /// <summary>
        /// For only reference/audit purposes. To get the latest use the billing model.
        /// </summary>
        public virtual string RulesApplied { get; set; }

        public virtual string FilePath { get; set; }

        /// <summary>
        /// For only reference/audit purposes. To get the latest use the billing model.
        /// </summary>
        public virtual string AdapterValue { get; set; }

        public virtual string BatchRef { get; set; }

        public virtual bool ErrorOccurred { get; set; }

        public virtual string ErrorMessage { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual decimal PercentageProgress { get; set; }

        public virtual Int32 TotalNoOfRowsProcessed { get; set; }

        /// <summary>
        /// PayeAssessmentType on DB
        /// </summary>
        public virtual PayeAssessmentType Type { get; set; }

        /// <summary>
        /// So authorized user is here because we envisaged a time where multiple CBSUsers would have access to make payments
        /// or generate invoices for one tax entity.
        /// </summary>
        public virtual CBSUser CBSUser { get; set; }

        public virtual string FileName { get; set; }

        public virtual bool InvoiceConfirmed { get; set; }

        public virtual bool PaymentStatus { get; set; }

        public virtual string ReceiptNumber { get; set; }

        public virtual int Month { get; set; }

        public virtual int Year { get; set; }

        /// <summary>
        /// Direct assessment type 
        /// <see cref="PayeAssessmentType"/>
        /// </summary>
        public virtual int AssessmentType { get; set; }

        /// <summary>
        /// This is used in combination with the assessment type to determine the Id of
        /// the third party table used to process direct assessment
        /// <para>For example this property would contain the Id of the row that identifies the process 
        /// that has been done for reading the schedule file for IPPIS in the TaxPayer summary table
        /// </para>
        /// </summary>
        public virtual Int64 OriginIdentifier { get; set; }

        /// <summary>
        /// This field would help us check for duplicates.
        /// <para>For example direct assessments from IPPIS, we would not want duplicate batch id and tax payer summary Id, indicated by the 
        /// class prop OriginIdentifier to be store multiple times</para>
        /// </summary>
        public virtual string DuplicateComposite { get; set; }

        public virtual string TaxPayerCode { get; set; }

        public virtual InvoiceItems InvoiceItem { get; set; }


        /// <summary>
        /// Short code for Payment type description
        /// <see cref="PayeAssessmentType"/>
        /// </summary>
        public virtual string PaymentTypeCode
        {
            get { return this.Type.ToDescription(); }
            set { value = this.Type.ToDescription(); }
        }
    }
}

