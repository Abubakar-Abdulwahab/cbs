using Parkway.CBS.Core.Models;
using Parkway.CBS.Payee.PayeeAdapters;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class InvoiceProceedVMForPayeAssessment
    {

        public InvoiceProceedVM InvoiceProceedVM { get; set; }
        public IEnumerable<PAYEExemptionTypeVM> PAYEExemptionTypes { get; set; }
        public List<LGA> LGAs { get; set; }

        public Dictionary<string, string> StateLGAs { get; set; }
    }
}