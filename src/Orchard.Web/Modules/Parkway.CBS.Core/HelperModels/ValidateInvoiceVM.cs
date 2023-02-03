using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.HelperModels
{
    public class ValidateInvoiceVM
    {
        public string InvoiceNumber { get; set; }

        public string ApplicantName { get; set; }

        public string InvoiceAmount { get; set; }

        public string AmountPaid { get; set; }

        public InvoiceStatus Status { get; set; }
    }
}