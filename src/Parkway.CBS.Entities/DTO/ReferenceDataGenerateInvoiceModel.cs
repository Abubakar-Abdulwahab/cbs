using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Entities.DTO
{
    public class ReferenceDataGenerateInvoiceModel
    {
        public string Recipient { get; set; }

        public decimal Amount { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        /// <summary>
        /// tax profile ID. This is used to uniquely identify the customer on cashflow
        /// </summary>
        public Int64 TaxProfileId { get; set; }

        public int TaxProfileCategoryId { get; set; }

        public Int64 CashflowCustomerId { get; set; }

        public long WithholdingTaxonRentId { get; set; }

        // This is use to determine if the Payer account details will be created on TaxEntityAccount table
        // or updated if already exist
        public int OperationType { get; set; }

    }
}
