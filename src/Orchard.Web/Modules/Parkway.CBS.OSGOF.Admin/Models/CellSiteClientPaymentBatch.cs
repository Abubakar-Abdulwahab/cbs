using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.OSGOF.Admin.Models
{
    public class CellSiteClientPaymentBatch : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        //public virtual ICollection<CellSitesPayment> Records { get; set; }

        public virtual string FilePath { get; set; }

        /// <summary>
        /// For only reference/audit purposes. To get the latest use the billing model.
        /// </summary>
        public virtual string Template { get; set; }

        /// <summary>
        /// this value is a computed value
        /// batch ref is the id of the record padded by zeros
        /// </summary>
        public virtual string BatchRef { get; set; }

        public virtual bool ErrorOccurred { get; set; }

        public virtual string ErrorMessage { get; set; }

        //public virtual decimal Amount { get; set; }

        public virtual decimal PercentageProgress { get; set; }

        public virtual Int32 TotalNoOfRowsProcessed { get; set; }

        public virtual PayeAssessmentType Type { get; set; }

        /// <summary>
        /// Origin of this request. E.g Central billing, bank 3d
        /// </summary>
        public virtual string Origin { get; set; }

        public virtual CBSUser CBSUser { get; set; }

        public virtual string FileName { get; set; }

        public virtual bool InvoiceConfirmed { get; set; }

        public virtual bool PaymentStatus { get; set; }

        public virtual string ReceiptNumber { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual bool Processed { get; set; }
    }
}