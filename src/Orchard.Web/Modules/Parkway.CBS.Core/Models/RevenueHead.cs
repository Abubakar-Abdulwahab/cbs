using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Models
{
    public class RevenueHead : MDARevenueHead
    {
        public virtual BillingModel BillingModel { get; set; }

        public virtual RevenueHead Revenuehead { get; set; }

        public virtual MDA Mda { get; set; }

        public virtual IEnumerable<RevenueHead> RevenueHeads { get; set; }

        //public virtual IEnumerable<BillingSchedule> BillingSchedules { get; set; }

        public virtual IEnumerable<FormControlRevenueHead> FormControls { get; set; }

        public virtual IEnumerable<Invoice> Invoices { get; set; }

        public virtual string CashFlowProductCode { get; set; }

        public virtual Int64 CashFlowProductId { get; set; }

        public virtual string RefDataURL { get; set; }

        public virtual bool IsPayeAssessment { get; set; }

        /// <summary>
        /// this field holds the URL we want tax payer to be redirected to if do not want invoice generation to happen on Central Billing System
        /// </summary>
        public virtual string InvoiceGenerationRedirectURL { get; set; }


        public virtual RefDataDescription RefDataDescription { get; set; }


        public virtual bool IsGroup { get; set; }

        public virtual string CallBackURL { get; set; }

        /// <summary>
        /// groups RH belongs to
        /// </summary>
        public virtual IEnumerable<RevenueHeadGroup> Groups { get; set; }

        /// <summary>
        /// Group RH owns
        /// </summary>
        public virtual IEnumerable<RevenueHeadGroup> GroupParent { get; set; }

        public virtual string ServiceId { get; set; }

    }
}