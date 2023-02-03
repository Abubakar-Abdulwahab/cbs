using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class MDAReport
    {
        public string MDAName { get; set; }
        public Int64 NumberOfInvoices { get; set; }
        public Int64 AmountPaid { get; set; }
        public string MDASlug { get; set; }
    }
}