using Parkway.Cashflow.Ng.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Entities.VMs
{
    public class NAGISGenerateInvoiceResult
    {
        public IntegrationResponseModel IntegrationResponseModel { get; set; }

        public string NAGISOldInvoiceNumber { get; set; }

        public Int64 TaxProfileId { get; set; }

        public int TaxProfileCategoryId { get; set; }

        public int RevenueHeadId { get; set; }

        public int MDAId { get; set; }

        public int ExpertSystemId { get; set; }

        public string InvoiceDescription { get; set; }

        public DateTime DueDate { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal AmountDue { get; set; }
    }
}
