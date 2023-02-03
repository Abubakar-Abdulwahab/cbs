using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.ViewModels
{
    public class ReferenceItem
    {
        public long ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal ItemAmount { get; set; }
        public decimal SettlementAmount { get; set; }
        public string ItemDescription { get; set; }
    }

    public class EIRSReferenceModel
    {
        public string Status { get; set; }
        public string TaxPayerName { get; set; }
        public string ReferenceNumber { get; set; }
        public long ReferenceID { get; set; }
        public decimal Amount { get; set; }
        public List<ReferenceItem> ReferenceItems { get; set; }
    }
}
