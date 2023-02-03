using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class UnreconciledPayePayments : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Receipt Receipt { get; set; }

        public virtual ExpertSystemSettings ExpertSystem { get; set; }

        public virtual string PaymentReference { get; set; }

        public virtual int Month { get; set; }

        public virtual int Year { get; set; }

        public virtual TaxEntity UnReconciledTaxEntity { get; set; }

        public virtual DirectAssessmentBatchRecord DirectAssessmentBatchRecord { get; set; }
    }
}