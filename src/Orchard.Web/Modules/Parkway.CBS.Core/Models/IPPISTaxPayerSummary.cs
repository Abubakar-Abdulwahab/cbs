using System;

namespace Parkway.CBS.Core.Models
{
    public class IPPISTaxPayerSummary : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual IPPISBatch IPPISBatch { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        /// <summary>
        /// I don't know why I even have this here, but I am guessing it is to prevent too many inner joins
        /// when the process is at invoice generation stage
        /// </summary>
        public virtual TaxEntityCategory TaxEntityCategory { get; set; }

        public virtual string TaxPayerCode { get; set; }        

        public virtual int NumberofEmployees { get; set; }

        public virtual decimal TotalTaxAmount { get; set; }
    }
}