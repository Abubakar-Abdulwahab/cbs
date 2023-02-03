using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Payee.PayeeAdapters
{
    public class PayeeAmountAndBreakDown
    {
        public decimal TotalAmount { get; internal set; }

        public List<PayeeAssessmentLineRecordModel> Payees { get; internal set; }
    }


    public class PayeeBreakDown
    {
        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; }

        public decimal Tax { get; internal set; }

        public decimal Taxable { get; internal set; }

        public string TaxStringValue { get; internal set; }
    }
}
