using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.ViewModels
{
    public class DemandNoticeViewModel
    {
       
            public dynamic Pager { get; set; }

            public IEnumerable<DetailReport> Report { get; set; }

            public string FromRange { get; set; }
            public string EndRange { get; set; }

            public string MDAName { get; set; }
            public string RevenueHeadName { get; set; }

            public string SelectedMDA { get; set; }
            public string SelectedRevenueHead { get; set; }


            public IEnumerable<MDA> Mdas { get; set; }
            public IEnumerable<RevenueHeadDropDownListViewModel> RevenueHeads { get; set; }

            public PaymentOptions Options { get; set; }

            public decimal TotalInvoiceAmount { get; set; }

            public Int64 TotalNumberOfInvoicesSent { get; set; }
    }
}