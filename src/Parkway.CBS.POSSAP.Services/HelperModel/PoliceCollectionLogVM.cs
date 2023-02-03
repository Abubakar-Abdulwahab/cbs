using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class PoliceCollectionLogVM
    {
        public decimal AmountPaid { get; set; }

        public string RevenueHeadName { get; set; }

        public Int64 RequestId { get; set; }

        public Int64 InvoiceId { get; set; }

        public bool IsDeduction { get; set; }
    }
}
