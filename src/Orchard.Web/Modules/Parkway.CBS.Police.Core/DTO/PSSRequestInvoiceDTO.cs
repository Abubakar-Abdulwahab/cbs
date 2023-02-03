using System;

namespace Parkway.CBS.Police.Core.DTO
{
    public class PSSRequestInvoiceDTO
    {
        public Int64 Id { get; set; }

        public Int64 InvoiceId { get; set; }

        public Int64 RequestId { get; set; }

        public int InvoiceStatus { get; set; }
    }
}