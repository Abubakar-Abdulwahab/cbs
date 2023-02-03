using System;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.DTO
{
    public class PSServiceRequestInvoiceValidationDTO
    {
        public decimal AmountDue { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; }

        public PSSRequest Request { get; set; }

        public DateTime? PaymentDate { get; set; }

        public DateTime? CancellationDate { get; set; }

        public int ServiceId { get; set; }

        public PSSServiceTypeDefinition ServiceType { get; set; }

        public string InvoiceNumber { get; set; }

        public Int64 TaxEntityId { get; set; }

        public int TaxEntityCategoryId { get; set; }

        public int ExpertSystemId { get; set; }

        public int DefinitionId { get; set; }

        public int DefinitionLevelId { get; set; }

        public int DefinitionLevelIdPosition { get; set; }

        public long InvoiceId { get; set; }

        public int ServiceRequestStatus { get; set; }

        public string PhoneNumber { get; set; }

        public string Recipient { get; set; }

        public string Email { get; set; }

        public string ServiceName { get; set; }

        public string CommandName { get; set; }

        public string CommandAddress { get; set; }

        public int CommandId { get; set; }

        public int RevenueHeadId { get; set; }
    }
}