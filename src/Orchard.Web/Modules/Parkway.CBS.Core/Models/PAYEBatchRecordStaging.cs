using System;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Models
{
    public class PAYEBatchRecordStaging : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual BillingModel Billing { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        /// <summary>
        /// Indicates the current stage of the record batch items
        /// <see cref="PAYEBatchItemsProcessStages"/>
        /// </summary>
        public virtual int CurrentStage { get; set; }

        /// <summary>
        /// Indicates the next stage of the record batch items
        /// <see cref="PAYEBatchItemsProcessStages"/>
        /// </summary>
        public virtual int NextStage { get; set; }

        /// <summary>
        /// Indicates if the batch records items processing has been completed
        /// this flag is set to true
        /// </summary>
        public virtual bool IsProcessingCompleted { get; set; }

        public virtual string FilePath { get; set; }

        /// <summary>
        /// For only reference/audit purposes. To get the latest use the billing model.
        /// </summary>
        public virtual string AdapterValue { get; set; }

        public virtual string BatchRef { get; set; }

        public virtual bool ErrorOccurred { get; set; }

        public virtual string ErrorMessage { get; set; }

        public virtual decimal PercentageProgress { get; set; }

        public virtual Int32 TotalNoOfRowsProcessed { get; set; }

        /// <summary>
        /// Authorized user who generate the invoice
        /// </summary>
        public virtual CBSUser CBSUser { get; set; }

        public virtual string FileName { get; set; }

        public virtual bool InvoiceConfirmed { get; set; }

        public virtual bool Treated { get; set; }

        public virtual string ReceiptNumber { get; set; }

        public virtual int Month { get; set; }

        public virtual int Year { get; set; }

        /// <summary>
        /// Direct assessment type 
        /// <see cref="PayeAssessmentType"/>
        /// </summary>
        public virtual int AssessmentType { get; set; }


        public virtual Int64 OriginIdentifier { get; set; }


        public virtual string DuplicateComposite { get; set; }


        public virtual string TaxPayerCode { get; set; }


        public virtual InvoiceItems InvoiceItem { get; set; }

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