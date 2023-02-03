using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class TransactionLogViewModel
    {
        public IEnumerable<CustomerWalletViewModel> CustomerWallets { get; set; }

        public string FromRange { get; set; }
        public string EndRange { get; set; }

        public IEnumerable<MDA> Mdas { get; set; }
        public IEnumerable<RevenueHeadDropDownListViewModel> RevenueHeads { get; set; }

        public string SelectedMDA { get; set; }
        public string SelectedRevenueHead { get; set; }

        public dynamic Pager { get; set; }

        public decimal Credits { get; set; }
        public decimal Debits { get; set; }
        public decimal Balance { get; set; }
        public string Tin { get; set; }
    }
}