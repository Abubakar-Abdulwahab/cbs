using Orchard.Layouts.Framework.Elements;
using Parkway.CBS.Module.ViewModels.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Module.ViewModels
{
    public class DashboardViewModel
    {
        public decimal ActualIncomeOnInvoicesPaid { get; internal set; }
        public int NumberOfMDAs { get; internal set; }
        public int NumberOfRevenueHeads { get; internal set; }
        public string Tenant { get; set; }
        public decimal TotalExpectedIncome { get; set; }
        public decimal TotalIncomeDue { get; set; }
        public Int64 TotalInvoicePaid { get; set; }
        public Int64 TotalNumberOfInvoices { get; set; }
        public string Month { get; set; }
        //tab categories
        public IList<CategoryDescriptor> Categories { get; set; }

        public MainDashboardChartViewModel ChartViewModel { get; set; }

        public IEnumerable<MDADropDownListViewModel> ListOfMdas { get; set; }

        public string MDASelected { get; set; }

        public string FromRange { get; set; }
        public string EndRange { get; set; }
    }
}