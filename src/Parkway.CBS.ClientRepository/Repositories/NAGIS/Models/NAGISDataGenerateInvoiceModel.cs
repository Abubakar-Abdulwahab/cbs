using Parkway.Cashflow.Ng.Models.Enums;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.NAGIS.Models
{
    public class NAGISDataGenerateInvoiceModel
    {
        public string Recipient { get; set; }

        public decimal Amount { get; set; }

        public decimal AmountDue { get; set; }

        public string Address { get; set; }

        public string NagisInvoiceNumber { get; set; }

        public Int64 TaxProfileId { get; set; }

        public int TaxProfileCategoryId { get; set; }

        public Int64 CashflowCustomerId { get; set; }

        public List<NagisOldInvoices> InvoiceItems { get; set; }

        public long NAGISOldInvoiceSummaryId { get; set; }

        public int GroupId { get; set; }

        public CashFlowCustomerType Type { get; set; }
    }


}
