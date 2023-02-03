using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.Models
{
    public class CellSitesScheduleStaging : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual bool AddedByAdmin { get; set; }

        public virtual UserPartRecord AdminUser { get; set; }

        public virtual CBSUser OperatorUser { get; set; }

        public virtual TaxEntity TaxProfile { get; set; }

        public virtual OperatorCategory OperatorCategory { get; set; }

        public virtual ICollection<CellSitesStaging> CellSites { get; set; }

        public virtual string FilePath { get; set; }

        public virtual string FileName { get; set; }

        public virtual string BatchRef { get; set; }

        public virtual bool ErrorOccurred { get; set; }

        public virtual string ErrorMessage { get; set; }

        public virtual decimal Amount { get; set; }
        
        /// <summary>
        /// indicates whether this schedule is upload for invoice generation
        /// </summary>
        public virtual bool TiedToTransaction { get; set; }

        /// <summary>
        /// This flag indicates whether this staging process has been completed. 
        /// i.e whether the records under this schedule have been moved to the main table 
        /// </summary>
        public virtual bool Treated { get; set; }

        public virtual decimal PercentageProgress { get; set; }

        public virtual Int32 TotalNoOfRowsProcessed { get; set; }
    }
}