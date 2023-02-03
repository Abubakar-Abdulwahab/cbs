using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.OSGOF.Admin.Models
{
    public class CellSitesPayment : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual CellSiteClientPaymentBatch CellSiteClientPaymentBatch { get; set; }

        public virtual CellSites CellSite { get; set; }

        public virtual string CellSiteId { get; set; }

        public virtual string Reference { get; set; }

        public virtual int Year { get; set; }

        public virtual string YearStringValue { get; set; }

        public virtual bool HasErrors { get; set; }

        public virtual string ErrorMessage { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual DateTime? AssessmentDate { get; set; }
    }
}