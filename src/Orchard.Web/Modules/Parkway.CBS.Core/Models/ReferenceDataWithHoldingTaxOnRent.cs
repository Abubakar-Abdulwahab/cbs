using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class ReferenceDataWithHoldingTaxOnRent : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual ReferenceDataTaxEntityStaging ReferenceDataTaxEntityStaging { get; set; }

        public virtual ReferenceDataBatch ReferenceDataBatch { get; set; }

        public virtual decimal PropertyRentAmount { get; set; }
    }
}