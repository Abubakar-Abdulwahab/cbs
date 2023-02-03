using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class DashboardStats
    {
        //Expectations
        public decimal TotalExpectedIncome { get; set; }

        public Int64 NumberOfInvoicesExpectedToBePaid { get; set; }
        //

        //sum of invoice amounts and their count
        public virtual decimal TotalAmountOnInvoicesGenerated { get; set; }
        public virtual Int64 NumberOfInvoicesSent { get; set; }
        //

        // Received income 
        public virtual decimal ReceivedIncome { get; set; }
        //

        public virtual decimal TotalBillGeneratedForMonth { get; set; }


        public virtual decimal ReceivedIncomeForMonth { get; set; }

        public virtual decimal TotalAmountDue { get; set; }

        public virtual decimal AmountReceivedForMonth { get; set; }

        public virtual decimal AmountDueForMonth { get; set; }


        public virtual Int64 NumberOfInvoicesPaid { get; set; }

        public virtual MDA Mda { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

    }
}