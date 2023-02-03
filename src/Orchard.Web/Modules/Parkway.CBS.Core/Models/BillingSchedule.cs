using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class BillingSchedule : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual DateTime? LastRunDate { get; set; }

        public virtual MDA MDA { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual BillingModel BillingModel { get; set; }

        /// <summary>
        /// <see cref="ScheduleStatus"/>
        /// </summary>
        public virtual int ScheduleStatus { get; set; }

        public virtual int Rounds { get; set; }

        /// <summary>
        /// TaxEntity TIN
        /// </summary>
        public virtual string TaxPayerNumber { get; set; }

        public virtual TaxEntity TaxPayer { get; set; }

    }
}