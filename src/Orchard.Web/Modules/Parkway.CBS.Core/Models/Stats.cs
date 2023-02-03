using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class Stats : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }
        public virtual MDA Mda { get; set; }
        public virtual RevenueHead RevenueHead { get; set; }
        public virtual Int64 NumberOfInvoicesSent { get; set; }
        public virtual decimal AmountExpected { get; set; }
        public virtual decimal AmountPaid { get; set; }
        public virtual Int64 NumberOfInvoicesPaid { get; set; }
        public virtual DateTime DueDate { get; set; }
        public virtual TaxEntityCategory TaxEntityCategory { get; set; }

        /// <summary>
        /// this is a concat of the due date, which is the month period for invoices generated
        /// This concat is made of off the revenue head id, due date and the category
        /// why do we have this? for indexing purposes. having one value indexed would greatly help
        /// instead of having the revenue head, due date and category
        /// <para>string.Format("{0}|{1}|{2}", DueDate.ToString("dd'/'MM'/'yyyy"), RevenueHead.Id, TaxEntityCategory.Id)</para>
        /// </summary>
        public virtual string StatsQueryConcat { get; set; }
    }
}