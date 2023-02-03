using Orchard.Layouts.Framework.Elements;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Module.ViewModels.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class RevenueHeadDashboardViewModel
    {
        public decimal ActualIncomeOnInvoicesPaid { get; internal set; }
        public Int64 PendingInvoices { get; internal set; }
        public decimal PendingAmount { get; internal set; }
        public string RevenueHeadName { get; set; }
        public string MDAName { get; set; }
        public decimal TotalExpectedIncome { get; set; }
        public Int64 TotalInvoicePaid { get; set; }
        public Int64 TotalNumberOfInvoices { get; set; }
        public string Month { get; set; }

        public RevenueHead RevenueHead { get; set; }


        public dynamic AdminBreadCrumb { get; set; }

        //tab categories
        public IList<CategoryDescriptor> Categories { get; set; }



        public MainDashboardChartViewModel ChartViewModel { get; set; }
    }
}