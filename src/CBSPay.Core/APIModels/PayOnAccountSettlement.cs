using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.APIModels
{
    public class PayOnAccountSettlement
    {
        public long TaxPayerTypeID { get; set; }
        public long TaxPayerID { get; set; }
        public string TransactionRefNo { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethodID { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Notes { get; set; }
    }
}
