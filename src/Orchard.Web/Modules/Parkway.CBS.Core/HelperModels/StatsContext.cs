using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class StatsContext
    {
        public List<Invoice> Invoices { get; set; }

        public Invoice Invoice { get; set; }

        public RevenueHead RevenueHead { get; set; }

        public MDA MDA { get; set; }


        public TaxEntityCategory TaxEntityCategory { get; set; }

        public bool Paid { get; set; }


    }


    /// <summary>
    /// Stats context when invoice payment has been made
    /// </summary>
    public class StatsContextUpdate : StatsContext
    {
        public decimal AmountPaid { get; set; }

        /// <summary>
        /// set this value to true if this invoice has been part paid
        /// </summary>
        public bool PartPayment { get; set; }
         
    }
}