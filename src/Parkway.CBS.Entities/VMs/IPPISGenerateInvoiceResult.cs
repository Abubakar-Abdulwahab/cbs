using Parkway.Cashflow.Ng.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Entities.VMs
{
    public class IPPISGenerateInvoiceResult
    {
        public IntegrationResponseModel IntegrationResponseModel { get; set; }

        public Int64 IPPISTaxPayerSummaryId { get; set; }

        public Int64 TaxProfileId { get; set; }

        public int TaxProfileCategoryId { get; set; }

        public string InvoiceDescription { get; set; }

        public DateTime DueDate { get; set; }
    }
}
